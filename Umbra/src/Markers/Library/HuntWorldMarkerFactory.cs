using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
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
            new BooleanMarkerConfigVariable("ShowBillMobs",  I18N.Translate("Markers.Hunt.ShowBillMobs"),  null, false),
            new BooleanMarkerConfigVariable("ShowB",  I18N.Translate("Markers.Hunt.ShowB"),  null, false),
            new BooleanMarkerConfigVariable("ShowA",  I18N.Translate("Markers.Hunt.ShowA"),  null, true),
            new BooleanMarkerConfigVariable("ShowS",  I18N.Translate("Markers.Hunt.ShowS"),  null, true),
            new BooleanMarkerConfigVariable("ShowSS", I18N.Translate("Markers.Hunt.ShowSS"), null, true),
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
                    SubLabel           = bc->Character.InCombat ? I18N.Translate("Markers.SubLabel.InCombat") : null,
                    FadeDistance       = new(fadeDist, fadeDist + fadeAttn),
                    ShowOnCompass      = GetConfigValue<bool>("ShowOnCompass"),
                    MaxVisibleDistance = maxVisDistance,
                }
            );
        }

        if (GetConfigValue<bool>("ShowBillMobs")) {
            ref var mobHunt = ref UIState.Instance()->MobHunt;

            foreach (var chara in cm->BattleCharas) {
                Character* c = (Character*)chara.Value;

                if (c == null || 0 == c->BaseId) continue;
                if (!mobHunt.IsHuntTarget(c)) continue;

                bool inCombat = c->InCombat;
                var (current, needed) = GetMarkBillKills(c);
                string name = (needed > 0 ? $"[{current}/{needed}] " : string.Empty) + c->NameString;

                var p = c->Position;
                var id = $"MHT_{c->EntityId}";

                activeIds.Add(id);

                SetMarker(
                    new() {
                        Key                = id,
                        MapId              = zoneManager.CurrentZone.Id,
                        Position           = new(p.X, p.Y + c->Effects.CurrentFloatHeight, p.Z),
                        IconId             = 60004,
                        Label              = name,
                        SubLabel           = c->InCombat ? I18N.Translate("Markers.SubLabel.InCombat") : null,
                        FadeDistance       = new(fadeDist, fadeDist + fadeAttn),
                        ShowOnCompass      = GetConfigValue<bool>("ShowOnCompass"),
                        MaxVisibleDistance = maxVisDistance,
                    }
                );
            }
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

    private unsafe (int current, int needed) GetMarkBillKills(Character* c)
    {
        ref var mobHunt = ref UIState.Instance()->MobHunt;
        var nameId = c->NameId;

        for (byte markIndex = 0; markIndex < mobHunt.AvailableMarkId.Length; markIndex++) {
            var huntOrderRowId = mobHunt.GetObtainedHuntOrderRowId(markIndex);

            if (!dataManager.GetSubrowExcelSheet<MobHuntOrder>().TryGetRow((uint)huntOrderRowId, out var orders))
                continue;
            
            foreach (var (mobIndex, order) in orders.Index()) {
                if (order.Target.Value.Name.RowId != nameId)
                    continue;

                var current = mobHunt.GetKillCount(markIndex, (byte)mobIndex);
                var needed = order.NeededKills;

                return (current, order.NeededKills);
            }
        }

        return (0, 0);
    }
}
