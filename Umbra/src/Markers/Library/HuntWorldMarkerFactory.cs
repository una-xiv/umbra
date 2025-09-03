using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.Sheets;

namespace Umbra.Markers.Library;

[Service]
internal class HuntWorldMarkerFactory(IDataManager dataManager, IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "HuntMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.Hunt.Name");
    public override string Description { get; } = I18N.Translate("Markers.Hunt.Description");

    private readonly Dictionary<uint, NotoriousMonster?> _notoriousMonstersCache = [];

    private readonly Dictionary<byte, string> _rankPrefixes = new() {
        { 1, "[B]" },
        { 2, "[A]" },
        { 3, "[S]" },
        { 4, "[SS]" }
    };

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            new BooleanMarkerConfigVariable("ShowB",  I18N.Translate("Markers.Hunt.ShowB"),  null, false),
            new BooleanMarkerConfigVariable("ShowA",  I18N.Translate("Markers.Hunt.ShowA"),  null, true),
            new BooleanMarkerConfigVariable("ShowS",  I18N.Translate("Markers.Hunt.ShowS"),  null, true),
            new BooleanMarkerConfigVariable("ShowSS", I18N.Translate("Markers.Hunt.ShowSS"), null, true),
            ..DefaultFadeConfigVariables,
        ];
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

        var fadeDist       = GetConfigValue<int>("FadeDistance");
        var fadeAttn       = GetConfigValue<int>("FadeAttenuation");
        var maxVisDistance = GetConfigValue<int>("MaxVisibleDistance");

        List<string> activeIds = [];

        foreach (var chara in cm->BattleCharas) {
            BattleChara* bc = chara.Value;

            if (bc == null || 0 == bc->BaseId) continue;

            var nm = GetNotoriousMonster(bc->BaseId);
            if (nm == null) continue;

            string name = bc->NameString;
            string rank = _rankPrefixes[nm.Value.Rank];

            switch (nm.Value.Rank) {
                case 0:
                case 1 when !GetConfigValue<bool>("ShowB"):
                case 2 when !GetConfigValue<bool>("ShowA"):
                case 3 when !GetConfigValue<bool>("ShowS"):
                case 4 when !GetConfigValue<bool>("ShowSS"):
                    continue;
            }

            var p  = bc->Position;
            var id = $"NM_{bc->BaseId}";

            activeIds.Add(id);

            SetMarker(
                new() {
                    Key                = id,
                    MapId              = zoneManager.CurrentZone.Id,
                    Position           = new(p.X, p.Y + bc->Effects.CurrentFloatHeight, p.Z),
                    IconId             = GetMarkerIcon(nm.Value.Rank, bc->Character.IsHostile),
                    Label              = $"{rank} {name}",
                    SubLabel           = bc->Character.InCombat ? " (In Combat)" : null,
                    FadeDistance       = new(fadeDist, fadeDist + fadeAttn),
                    ShowOnCompass      = GetConfigValue<bool>("ShowOnCompass"),
                    MaxVisibleDistance = maxVisDistance,
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }

    private NotoriousMonster? GetNotoriousMonster(uint dataId)
    {
        if (_notoriousMonstersCache.TryGetValue(dataId, out var nm)) return nm;

        NotoriousMonster? monster = dataManager
            .GetExcelSheet<NotoriousMonster>()
            .FirstOrDefault(n => n.BNpcBase.RowId == dataId);

        if (monster is { RowId: 0 }) {
            monster = null;
        }

        _notoriousMonstersCache[dataId] = monster;

        return monster;
    }

    private static uint GetMarkerIcon(byte rank, bool isHostile)
    {
        uint startOffset = (uint)(isHostile ? 61706 : 61700);
        uint iconOffset  = (uint)Math.Min(rank + 3, 4);

        return startOffset + iconOffset;
    }
}
