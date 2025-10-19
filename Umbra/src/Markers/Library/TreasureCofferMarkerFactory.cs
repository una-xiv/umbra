using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using TreasureObject = FFXIVClientStructs.FFXIV.Client.Game.Object.Treasure;

namespace Umbra.Markers.Library;

[Service]
internal class TreasureCofferMarkerFactory(IObjectTable objectTable, IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "TreasureCoffers";
    public override string Name        { get; } = I18N.Translate("Markers.TreasureCoffer.Name");
    public override string Description { get; } = I18N.Translate("Markers.TreasureCoffer.Description");

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            ..DefaultFadeConfigVariables,
        ];
    }

    [OnTick(interval: 500)]
    private void OnTick()
    {
        if (!GetConfigValue<bool>("Enabled") || !zoneManager.HasCurrentZone) {
            RemoveAllMarkers();
            return;
        }

        var fadeDist       = GetConfigValue<int>("FadeDistance");
        var fadeAttn       = GetConfigValue<int>("FadeAttenuation");
        var showOnCompass  = GetConfigValue<bool>("ShowOnCompass");
        var maxVisDistance = GetConfigValue<int>("MaxVisibleDistance");

        List<string> usedIds = [];
        IZone        zone    = zoneManager.CurrentZone;

        foreach (IGameObject obj in objectTable) {
            if (!obj.IsValid() || (obj.ObjectKind != ObjectKind.Treasure && (obj.ObjectKind != ObjectKind.EventObj || obj.BaseId is not (2007357 or 2007358 or 2007543))) || !obj.IsTargetable) continue;
            unsafe {
                var treasureObject = (TreasureObject*)obj.Address;
                if (treasureObject->Flags.HasFlag(TreasureObject.TreasureFlags.FadedOut)) continue;
            }

            string key = $"TC_{zone.Id}_{(int)obj.Position.X}_{(int)obj.Position.Z}_{obj.BaseId}";
            usedIds.Add(key);

            SetMarker(
                new() {
                    Key                = key,
                    MapId              = zone.Id,
                    Label              = obj.Name.TextValue,
                    IconId             = 60356u,
                    Position           = obj.Position,
                    FadeDistance       = new(fadeDist, fadeDist + fadeAttn),
                    ShowOnCompass      = showOnCompass,
                    MaxVisibleDistance = maxVisDistance,
                }
            );
        }

        RemoveMarkersExcept(usedIds);
    }
}
