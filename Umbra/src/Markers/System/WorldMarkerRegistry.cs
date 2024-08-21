/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Game;

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
        foreach (Map map in dataManager.GetExcelSheet<Map>()!) {
            WorldMarkers[map.RowId] = [];
        }
    }

    public void Dispose()
    {
        WorldMarkers.Clear();
    }

    public List<WorldMarker> GetMarkers()
    {
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
