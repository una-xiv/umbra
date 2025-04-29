using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.Library;

[Service]
internal class TripleTriadMarkerFactory(IDataManager dataManager, IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "TripleTriadMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.TripleTriad.Name");
    public override string Description { get; } = I18N.Translate("Markers.TripleTriad.Description");

    private readonly Dictionary<uint, List<Card>> _cardLocations = [];

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            new BooleanMarkerConfigVariable(
                "ShowLockedCards",
                I18N.Translate("Markers.TripleTriad.ShowLockedCards.Name"),
                I18N.Translate("Markers.TripleTriad.ShowLockedCards.Description"),
                false
            ),
            ..DefaultFadeConfigVariables,
        ];
    }

    protected override unsafe void OnInitialized()
    {
        UIState* ui = UIState.Instance();
        if (ui == null) return;

        dataManager
            .GetExcelSheet<TripleTriadCardResident>()
            .ToList()
            .ForEach(
                (resident) => {
                    var card = dataManager.GetExcelSheet<TripleTriadCard>().FindRow(resident.RowId);

                    if (card == null || card.Value.Name.ToString() == string.Empty) return;
                    if (ui->IsTripleTriadCardUnlocked((ushort)card.Value.RowId)) return;

                    // Only show cards that we can infer a valid location from.
                    if (resident.Location.RowId < 1
                        || dataManager.GetExcelSheet<Level>().FindRow(resident.Location.RowId) is not { } level)
                        return;

                    var position = new Vector3(level.X, level.Y + 1.25f, level.Z);
                    var mapId    = level.Map.RowId;
                    var name     = card.Value.Name.ToString();

                    var starCount           = resident.TripleTriadCardRarity.ValueNullable?.Stars ?? 0;
                    if (starCount > 0) name = $"{(char)(SeIconChar.BoxedNumber0.ToIconChar() + starCount)} {name}";

                    if (!_cardLocations.ContainsKey(mapId)) _cardLocations[mapId] = [];

                    _cardLocations[mapId]
                        .Add(
                            new() {
                                Id            = card.Value.RowId,
                                Name          = name,
                                Position      = position,
                                UnlockQuestId = resident.Quest.RowId,
                            }
                        );
                }
            );
    }

    protected override void OnZoneChanged(IZone zone)
    {
        RemoveAllMarkers();
    }

    [OnTick(interval: 1000)]
    private unsafe void OnUpdate()
    {
        if (false == GetConfigValue<bool>("Enabled")
            || !zoneManager.HasCurrentZone
            || !_cardLocations.TryGetValue(zoneManager.CurrentZone.Id, out List<Card>? cards)
           ) {
            RemoveAllMarkers();
            return;
        }

        var showLockedCards = GetConfigValue<bool>("ShowLockedCards");
        var showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var fadeDistance    = GetConfigValue<int>("FadeDistance");
        var fadeAttenuation = GetConfigValue<int>("FadeAttenuation");
        var maxVisDistance  = GetConfigValue<int>("MaxVisibleDistance");

        UIState*     uiState   = UIState.Instance();
        List<string> activeIds = [];

        foreach (var card in cards) {
            if (uiState->IsTripleTriadCardUnlocked((ushort)card.Id)) continue;

            bool isLocked = !QuestManager.IsQuestComplete(card.UnlockQuestId);
            if (isLocked && !showLockedCards) continue;

            var     id       = $"TT_{card.Id}";
            var     name     = $"{card.Name}";
            string? subLabel = null;

            if (isLocked) {
                subLabel = "Quest Required: "
                    + (dataManager.GetExcelSheet<Quest>().FindRow(card.UnlockQuestId)?.Name.ToString()
                        ?? "Unknown Quest");
            }

            name = string.Join("\n", name.Split('\n').Distinct());

            activeIds.Add(id);

            SetMarker(
                new() {
                    Key                = id,
                    MapId              = zoneManager.CurrentZone.Id,
                    IconId             = 71102,
                    Label              = name,
                    SubLabel           = subLabel,
                    Position           = card.Position,
                    FadeDistance       = new(fadeDistance, fadeDistance + fadeAttenuation),
                    ShowOnCompass      = showDirection,
                    MaxVisibleDistance = maxVisDistance,
                }
            );
        }

        if (activeIds.Count == 0) {
            RemoveAllMarkers();
            return;
        }

        RemoveMarkersExcept(activeIds);
    }

    private struct Card
    {
        public uint    Id;
        public string  Name;
        public Vector3 Position;
        public uint    UnlockQuestId;
    }
}
