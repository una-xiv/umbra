using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.Library;

[Service]
public class WaymarkWorldMarkerFactory(IPlayer player, IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "WaymarkWorldMarker";
    public override string Name        { get; } = I18N.Translate("Markers.Waymark.Name");
    public override string Description { get; } = I18N.Translate("Markers.Waymark.Description");

    private List<uint> Icons { get; } = [
        // A,  B,     C,     D,     1,     2,     3,     4
        61341, 61342, 61343, 61347, 61344, 61345, 61346, 61348
    ];

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            ..DefaultFadeConfigVariables,
        ];
    }

    [OnTick(interval: 1000)]
    private unsafe void OnTick()
    {
        if (!GetConfigValue<bool>("Enabled") || !zoneManager.HasCurrentZone || player.IsBetweenAreas) {
            RemoveAllMarkers();
            return;
        }

        MarkingController* mc = MarkingController.Instance();

        if (mc == null) {
            RemoveAllMarkers();
            return;
        }

        IZone        zone    = zoneManager.CurrentZone;
        uint         mapId   = zone.Id;
        List<string> usedIds = [];

        var showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var fadeDistance    = GetConfigValue<int>("FadeDistance");
        var fadeAttenuation = GetConfigValue<int>("FadeAttenuation");

        for (var i = 0; i < 8; i++) {
            var marker = mc->FieldMarkers[i];
            if (!marker.Active) continue;

            var key = $"Waymark_{mapId}_{i}";
            usedIds.Add(key);

            SetMarker(
                new() {
                    Key           = key,
                    MapId         = mapId,
                    IconId        = Icons[i],
                    Position      = new(marker.X / 1000f, marker.Y / 1000f, marker.Z / 1000f),
                    ShowOnCompass = showDirection,
                    FadeDistance  = new(fadeDistance, fadeDistance + fadeAttenuation),
                }
            );
        }

        RemoveMarkersExcept(usedIds);
    }
}
