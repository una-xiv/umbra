namespace Umbra.Markers.Library;

[Service]
public sealed class MapLinkMarkerFactory(IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "MapLinkMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.MapLink.Name");
    public override string Description { get; } = I18N.Translate("Markers.MapLink.Description");

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanMarkerConfigVariable(
                "ShowAreaBoundaries",
                I18N.Translate("Markers.MapLink.ShowAreaBoundaries.Name"),
                I18N.Translate("Markers.MapLink.ShowAreaBoundaries.Description"),
                true
            ),
            new BooleanMarkerConfigVariable(
                "ShowInstanceEntries",
                I18N.Translate("Markers.MapLink.ShowInstanceEntries.Name"),
                I18N.Translate("Markers.MapLink.ShowInstanceEntries.Description"),
                false
            ),
            new BooleanMarkerConfigVariable(
                "ShowAetherytes",
                I18N.Translate("Markers.MapLink.ShowAetherytes.Name"),
                I18N.Translate("Markers.MapLink.ShowAetherytes.Description"),
                false
            ),
            new BooleanMarkerConfigVariable(
                "ShowAethernetShards",
                I18N.Translate("Markers.MapLink.ShowAethernetShards.Name"),
                I18N.Translate("Markers.MapLink.ShowAethernetShards.Description"),
                false
            ),
            new BooleanMarkerConfigVariable(
                "ShowFerryDocks",
                I18N.Translate("Markers.MapLink.ShowFerryDocks.Name"),
                I18N.Translate("Markers.MapLink.ShowFerryDocks.Description"),
                false
            ),
            new BooleanMarkerConfigVariable(
                "ShowChocoboPorters",
                I18N.Translate("Markers.MapLink.ShowChocoboPorters.Name"),
                I18N.Translate("Markers.MapLink.ShowChocoboPorters.Description"),
                false
            ),

            ..DefaultStateConfigVariables,
            ..DefaultFadeConfigVariables,
        ];
    }

    [OnTick(interval: 1000)]
    private void OnUpdate()
    {
        if (!GetConfigValue<bool>("Enabled") || !zoneManager.HasCurrentZone) {
            RemoveAllMarkers();
            return;
        }

        uint mapId           = zoneManager.CurrentZone.Id;
        var  showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var  fadeDistance    = GetConfigValue<int>("FadeDistance");
        var  fadeAttenuation = GetConfigValue<int>("FadeAttenuation");
        var  maxVisDistance  = GetConfigValue<int>("MaxVisibleDistance");

        List<ZoneMarkerType> types = [];

        if (GetConfigValue<bool>("ShowAreaBoundaries")) {
            types.Add(ZoneMarkerType.MapLink);
        }

        if (GetConfigValue<bool>("ShowInstanceEntries")) {
            types.Add(ZoneMarkerType.InstanceEntry);
        }

        if (GetConfigValue<bool>("ShowAetherytes")) {
            types.Add(ZoneMarkerType.Aetheryte);
        }

        if (GetConfigValue<bool>("ShowAethernetShards")) {
            types.Add(ZoneMarkerType.Aethernet);
        }

        if (GetConfigValue<bool>("ShowFerryDocks")) {
            types.Add(ZoneMarkerType.Ferry);
        }

        if (GetConfigValue<bool>("ShowChocoboPorters")) {
            types.Add(ZoneMarkerType.Taxi);
        }

        List<string> usedIds = [];

        List<ZoneMarker> markers = zoneManager
            .CurrentZone.StaticMarkers
            .Where(marker => types.Contains(marker.Type))
            .ToList();

        foreach (ZoneMarker marker in markers) {
            string id = $"{marker.Type}_{marker.IconId}_{marker.Position.X}_{marker.Position.Y}";
            usedIds.Add(id);

            SetMarker(
                new() {
                    Key                = id,
                    Position           = marker.WorldPosition,
                    IconId             = marker.IconId,
                    Label              = marker.Name,
                    MapId              = mapId,
                    FadeDistance       = new(fadeDistance, fadeDistance + fadeAttenuation),
                    MaxVisibleDistance = maxVisDistance,
                    ShowOnCompass      = showDirection,
                }
            );
        }

        RemoveMarkersExcept(usedIds);
    }
}
