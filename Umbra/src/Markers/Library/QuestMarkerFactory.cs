using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using NotImplementedException = System.NotImplementedException;

namespace Umbra.Markers.Library;

[Service]
internal class QuestMarkerFactory(IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "QuestMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.Quest.Name");
    public override string Description { get; } = I18N.Translate("Markers.Quest.Description");

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

    [OnTick(interval: 1000)]
    private void OnUpdate()
    {
        if (!GetConfigValue<bool>("Enabled") || !zoneManager.HasCurrentZone) {
            RemoveAllMarkers();
            return;
        }

        List<ZoneMarker> markers = zoneManager
            .CurrentZone.DynamicMarkers.Where(
                marker => marker.Type is ZoneMarkerType.ObjectiveArea or ZoneMarkerType.QuestObjective
            )
            .ToList();

        List<string> activeIds = [];

        var showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var fadeDistance    = GetConfigValue<int>("FadeDistance");
        var fadeAttenuation = GetConfigValue<int>("FadeAttenuation");

        foreach (ZoneMarker marker in markers) {
            string id =
                $"QM_{zoneManager.CurrentZone.Id}_{marker.Type}_{marker.IconId}_{marker.WorldPosition.X:N0}_{marker.WorldPosition.Y:N0}_{marker.WorldPosition.Z:N0}";

            activeIds.Add(id);

            SetMarker(
                new() {
                    Key           = id,
                    MapId         = zoneManager.CurrentZone.Id,
                    IconId        = marker.IconId,
                    Position      = marker.WorldPosition,
                    Label         = marker.Name,
                    ShowOnCompass = showDirection,
                    FadeDistance  = new(fadeDistance, fadeDistance + fadeAttenuation),
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }
}
