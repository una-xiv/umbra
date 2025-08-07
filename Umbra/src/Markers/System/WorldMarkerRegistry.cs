using Lumina.Excel.Sheets;

namespace Umbra.Markers.System;

[Service]
internal sealed class WorldMarkerRegistry : IDisposable
{
    private Dictionary<uint, Dictionary<string, WorldMarker>> WorldMarkers { get; } = [];

    private readonly IZoneManager         _zoneManager;
    private readonly UmbraVisibility      _visibility;

    public WorldMarkerRegistry(
        IDataManager         dataManager,
        IZoneManager         zoneManager,
        UmbraVisibility      visibility
    )
    {
        _zoneManager = zoneManager;
        _visibility  = visibility;

        // Pre-cache a list of markers based on map ids.
        foreach (Map map in dataManager.GetExcelSheet<Map>()) {
            WorldMarkers[map.RowId] = [];
        }
    }

    public void Dispose()
    {
        WorldMarkers.Clear();
    }

    public List<WorldMarker> GetMarkers()
    {
        if (! _zoneManager.HasCurrentZone) return [];

        return _visibility.AreMarkersVisible()
            ? WorldMarkers[_zoneManager.CurrentZone.Id].Values.ToList()
            : [];
    }

    public void RemoveMarker(WorldMarker marker)
    {
        if (WorldMarkers.TryGetValue(marker.MapId, out Dictionary<string, WorldMarker>? markers)) {
            markers.Remove(marker.Key);
        }
    }

    /// <summary>
    /// <para>
    /// Adds or updates a marker in the registry and returns its unique ID as
    /// a string.
    /// </para>
    /// <para>
    /// If the marker has no key set, a random GUID will be generated and
    /// returned as the key. Use this key to manipulate the marker later on if
    /// needed.
    /// </para>
    /// </summary>
    /// <param name="marker"></param>
    /// <returns></returns>
    public string SetMarker(WorldMarker marker)
    {
        if (marker.MapId == 0 || !WorldMarkers.TryGetValue(marker.MapId, out Dictionary<string, WorldMarker>? markers))
            throw new ArgumentException("The marker must have a valid map ID set.");

        markers[marker.Key] = marker;

        return marker.Key;
    }
}
