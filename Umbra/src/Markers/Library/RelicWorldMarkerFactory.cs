using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using RelicNoteSheet = Lumina.Excel.Sheets.RelicNote;

namespace Umbra.Markers.Library;

[Service]
internal class RelicWorldMarkerFactory : WorldMarkerFactory
{
    private readonly IDataManager dataManager;
    private readonly IZoneManager zoneManager;

    public RelicWorldMarkerFactory(IDataManager dataManager, IZoneManager zoneManager, IGameInteropProvider gameInteropProvider)
    {
        this.dataManager = dataManager;
        this.zoneManager = zoneManager;

        gameInteropProvider.InitializeFromAttributes(this);
    }

    public override string Id          { get; } = "RelicMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.Relic.Name");
    public override string Description { get; } = I18N.Translate("Markers.Relic.Description");

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            ..DefaultFadeConfigVariables,
        ];
    }

    protected override void OnZoneChanged(IZone zone)
    {
        RemoveAllMarkers();
    }

    [OnTick(interval: 100)]
    private unsafe void OnUpdate()
    {
        if (!GetConfigValue<bool>("Enabled") || !zoneManager.HasCurrentZone) {
            RemoveAllMarkers();
            return;
        }

        var cm = CharacterManager.Instance();
        if (cm == null) return;

        var relicNote = &UIState.Instance()->RelicNote;

        var fadeDist       = GetConfigValue<int>("FadeDistance");
        var fadeAttn       = GetConfigValue<int>("FadeAttenuation");
        var maxVisDistance = GetConfigValue<int>("MaxVisibleDistance");

        List<string> activeIds = [];

        foreach (var chara in cm->BattleCharas) {
            Character* c = (Character*)chara.Value;

            if (c == null || 0 == c->BaseId || c->IsDead()) continue;
            if (!relicNote->IsMonsterNoteTarget(c)) continue;

            bool inCombat = c->InCombat;
            var (current, needed) = GetTargetKills(c);
            string name = (needed > 0 ? $"[{current}/{needed}] " : string.Empty) + c->NameString;

            var p = c->Position;
            var id = $"RMT_{c->EntityId}";

            activeIds.Add(id);

            SetMarker(
                new() {
                    Key                = id,
                    MapId              = zoneManager.CurrentZone.Id,
                    Position           = new(p.X, p.Y + c->Effects.CurrentFloatHeight, p.Z),
                    IconId             = 92178,
                    Label              = name,
                    SubLabel           = c->InCombat ? I18N.Translate("Markers.SubLabel.InCombat") : null,
                    FadeDistance       = new(fadeDist, fadeDist + fadeAttn),
                    ShowOnCompass      = GetConfigValue<bool>("ShowOnCompass"),
                    MaxVisibleDistance = maxVisDistance,
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }

    private unsafe (object current, int needed) GetTargetKills(Character* c)
    {
        ref var relicNote = ref UIState.Instance()->RelicNote;
        var nameId = c->NameId;

        if (!dataManager.GetExcelSheet<RelicNoteSheet>().TryGetRow(relicNote.RelicNoteId, out var relicNoteRow))
            return (0, 0);

        foreach (var (index, commonTarget) in relicNoteRow.MonsterNoteTargetCommon.Index()) {
            if (commonTarget.ValueNullable?.BNpcName.RowId != nameId)
                continue;

            var current = relicNote.GetMonsterProgress(index);
            var max = relicNoteRow.MonsterCount[index];

            return (current, max);
        }

        return (0, 0);
    }
}
