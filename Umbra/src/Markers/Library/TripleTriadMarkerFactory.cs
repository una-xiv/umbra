/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
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

        dataManager.GetExcelSheet<TripleTriadCardResident>()!
            .ToList()
            .ForEach(
                (resident) => {
                    var card = dataManager.GetExcelSheet<TripleTriadCard>()!.GetRow(resident.RowId);

                    if (card == null || card.Name.ToString() == string.Empty) return;
                    if (ui->IsTripleTriadCardUnlocked((ushort)card.RowId)) return;

                    // Only show cards that we can infer a valid location from.
                    if (resident.Location < 1
                        || dataManager.GetExcelSheet<Level>()!.GetRow(resident.Location) is not { } level)
                        return;

                    var position = new Vector3(level.X, level.Y + 1.25f, level.Z);
                    var mapId    = level.Map.Row;
                    var name     = card.Name.ToString();

                    var starCount           = resident.TripleTriadCardRarity.Value?.Stars ?? 0;
                    if (starCount > 0) name = $"{(char)(SeIconChar.BoxedNumber0.ToIconChar() + starCount)} {name}";

                    if (!_cardLocations.ContainsKey(mapId)) _cardLocations[mapId] = [];

                    _cardLocations[mapId]
                        .Add(
                            new() {
                                Id            = card.RowId,
                                Name          = name,
                                Position      = position,
                                UnlockQuestId = resident.Quest.Row,
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
                    + (dataManager.GetExcelSheet<Quest>()!.GetRow(card.UnlockQuestId)?.Name.ToString()
                        ?? "Unknown Quest");
            }

            name = string.Join("\n", name.Split('\n').Distinct());

            activeIds.Add(id);

            SetMarker(
                new() {
                    Key           = id,
                    MapId         = zoneManager.CurrentZone.Id,
                    IconId        = 71102,
                    Label         = name,
                    SubLabel      = subLabel,
                    Position      = card.Position,
                    FadeDistance  = new(fadeDistance, fadeDistance + fadeAttenuation),
                    ShowOnCompass = showDirection,
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
