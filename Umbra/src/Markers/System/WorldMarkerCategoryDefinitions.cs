using Umbra.Markers;

namespace Umbra.Markers.System;

internal static class WorldMarkerCategoryDefinitions
{
    internal const string CategoryGeneral = "General";
    internal const string CategoryOpenWorld = "OpenWorld";
    internal const string CategoryEureka = "Eureka";
    internal const string CategoryOccultCrescent = "OccultCrescent";

    internal static readonly IReadOnlyList<string> CategoryOrder = [
        CategoryGeneral,
        CategoryOpenWorld,
        CategoryEureka,
        CategoryOccultCrescent
    ];

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> CategoryAssignments =
        new Dictionary<string, IReadOnlyList<string>> {
            [CategoryGeneral] = [
                "MapLinkMarkers",
                "FlagMarker",
                "PartyMembers",
                "QuestMarkers",
                "RelicMarkers",
                "TreasureCoffers",
                "WaymarkWorldMarker"
            ],
            [CategoryOpenWorld] = [
                "AetherCurrents",
                "FateMarkers",
                "GatheringNodeMarkers",
                "HuntMarkers",
                "TripleTriadMarkers",
                "Vista"
            ],
            [CategoryEureka] = [
                "EurekaCoffers"
            ],
            [CategoryOccultCrescent] = [
                "OccultCarrots",
                "OccultCoffers",
                "OccultSurveyPointMarkers"
            ]
        };

    internal static IEnumerable<(string CategoryId, IReadOnlyList<WorldMarkerFactory> Factories)> GetCategorizedFactories(
        WorldMarkerFactoryRegistry registry
    )
    {
        foreach (string categoryId in CategoryOrder) {
            if (!CategoryAssignments.TryGetValue(categoryId, out var ids)) continue;

            List<WorldMarkerFactory> factories = [];

            foreach (string id in ids) {
                factories.Add(registry.GetFactory(id));
            }

            yield return (categoryId, factories);
        }
    }

    internal static string GetCategoryLabel(string categoryId)
    {
        return I18N.Translate($"Markers.Category.{categoryId}");
    }
}
