using NotImplementedException = System.NotImplementedException;

namespace Umbra.Markers.Library;

[Service]
internal class QuestMarkerFactory(IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "QuestMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.Quest.Name");
    public override string Description { get; } = I18N.Translate("Markers.Quest.Description");

    private static List<uint> UnavailableQuestIconIds { get; } = [71151, 71152, 71153, 71154, 71155];

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanMarkerConfigVariable(
                "ShowBlueQuests",
                I18N.Translate("Markers.Quest.ShowBluePendingQuests.Name"),
                I18N.Translate("Markers.Quest.ShowBluePendingQuests.Description"),
                false
            ),
            new BooleanMarkerConfigVariable(
                "ShowUnavailableBlueQuests",
                I18N.Translate("Markers.Quest.ShowUnavailableBlueQuests.Name"),
                I18N.Translate("Markers.Quest.ShowUnavailableBlueQuests.Description"),
                false
            ),
            new BooleanMarkerConfigVariable(
                "ShowMapLinkMarkers",
                I18N.Translate("Markers.Quest.ShowMapLinkMarkers.Name"),
                I18N.Translate("Markers.Quest.ShowMapLinkMarkers.Description"),
                true // Default true since this was the behavior prior to 2.3.3
            ),
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

        List<ZoneMarkerType> types = [
            ZoneMarkerType.ObjectiveArea,
            ZoneMarkerType.QuestObjective
        ];

        if (GetConfigValue<bool>("ShowBlueQuests")) {
            types.Add(ZoneMarkerType.FeatureQuest);
        }

        if (GetConfigValue<bool>("ShowMapLinkMarkers")) {
            types.Add(ZoneMarkerType.QuestLink);
        }

        List<ZoneMarker> markers = zoneManager
            .CurrentZone.DynamicMarkers.Where(
                marker => types.Contains(marker.Type) &&
                          (!UnavailableQuestIconIds.Contains(marker.IconId) || GetConfigValue<bool>("ShowUnavailableBlueQuests"))
            )
            .ToList();

        List<string> activeIds = [];

        var showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var fadeDistance    = GetConfigValue<int>("FadeDistance");
        var fadeAttenuation = GetConfigValue<int>("FadeAttenuation");
        var maxVisDistance  = GetConfigValue<int>("MaxVisibleDistance");

        foreach (ZoneMarker marker in markers) {
            string id =
                $"QM_{zoneManager.CurrentZone.Id}_{marker.Type}_{marker.IconId}_{marker.WorldPosition.X:N0}_{marker.WorldPosition.Y:N0}_{marker.WorldPosition.Z:N0}";

            activeIds.Add(id);

            SetMarker(
                new() {
                    Key                = id,
                    MapId              = zoneManager.CurrentZone.Id,
                    IconId             = marker.IconId,
                    Position           = marker.WorldPosition,
                    Label              = marker.Name,
                    ShowOnCompass      = showDirection,
                    FadeDistance       = new(fadeDistance, fadeDistance + fadeAttenuation),
                    MaxVisibleDistance = maxVisDistance,
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }
}
