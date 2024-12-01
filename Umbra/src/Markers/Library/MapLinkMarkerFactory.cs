using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;

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
                "ShowInstanceEntries",
                I18N.Translate("Markers.MapLink.ShowInstanceEntries.Name"),
                I18N.Translate("Markers.MapLink.ShowInstanceEntries.Description"),
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

        if (zoneManager.CurrentZone.Offset.X != 0 || zoneManager.CurrentZone.Offset.Y != 0) {
            // Temporarily disable static markers from maps with offsets until we figure out how to handle them.
            RemoveAllMarkers();
            return;
        }

        uint mapId           = zoneManager.CurrentZone.Id;
        var  showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var  fadeDistance    = GetConfigValue<int>("FadeDistance");
        var  fadeAttenuation = GetConfigValue<int>("FadeAttenuation");

        List<ZoneMarkerType> types = [
            ZoneMarkerType.MapLink, ZoneMarkerType.Ferry,
            ZoneMarkerType.Aetheryte, ZoneMarkerType.SummoningBell, ZoneMarkerType.Mailbox, ZoneMarkerType.Aethernet
        ];

        if (GetConfigValue<bool>("ShowInstanceEntries")) {
            types.Add(ZoneMarkerType.InstanceEntry);
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
                    Key           = id,
                    Position      = marker.WorldPosition,
                    IconId        = marker.IconId,
                    Label         = marker.Name,
                    MapId         = mapId,
                    FadeDistance  = new(fadeDistance, fadeDistance + fadeAttenuation),
                    ShowOnCompass = showDirection,
                }
            );
        }

        RemoveMarkersExcept(usedIds);
    }
    //
    // [OnDraw]
    // private void OnDraw()
    // {
    //     ImGui.Begin("MapLinkWindowThing", ImGuiWindowFlags.None | ImGuiWindowFlags.NoSavedSettings);
    //     ImGui.TextUnformatted($"MapScale: {zoneManager.CurrentZone.SizeFactor}");
    //     ImGui.TextUnformatted($"MapOffset: {zoneManager.CurrentZone.Offset}");
    //     ImGui.End();
    // }
}
