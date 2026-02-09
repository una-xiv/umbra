using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;

namespace Umbra.Markers.Library;

[Service]
internal class OccultCarrotMarkerFactory(IObjectTable objectTable, IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "OccultCarrots";
    public override string Name        { get; } = I18N.Translate("Markers.OccultCarrot.Name");
    public override string Description { get; } = I18N.Translate("Markers.OccultCarrot.Description");

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
        if (!GetConfigValue<bool>("Enabled") || 
            !zoneManager.HasCurrentZone ||
            zoneManager.CurrentZone.TerritoryId != 1252) {
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
            if (!obj.IsValid() || (obj.ObjectKind != ObjectKind.EventObj || obj.BaseId != 2010139)) continue;

            string key = $"OC_{zone.Id}_{(int)obj.Position.X}_{(int)obj.Position.Z}_{obj.BaseId}";
            usedIds.Add(key);

            SetMarker(
                new() {
                    Key                = key,
                    MapId              = zone.Id,
                    Label              = I18N.Translate("Markers.OccultCarrot.ItemName"),
                    IconId             = 25207u,
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
