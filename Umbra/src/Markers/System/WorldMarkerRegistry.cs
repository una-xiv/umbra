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
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.System;

[Service]
internal sealed class WorldMarkerRegistry : IDisposable
{
    private Dictionary<uint, Dictionary<string, WorldMarker>>      WorldMarkers { get; } = [];
    private Dictionary<uint, Dictionary<string, ResolvedPosition>> Positions    { get; } = [];

    private readonly IZoneManager         _zoneManager;
    private readonly WorldMarkerRaycaster _raycaster;
    private readonly UmbraVisibility      _visibility;

    public WorldMarkerRegistry(
        IDataManager dataManager,
        IZoneManager zoneManager,
        WorldMarkerRaycaster raycaster,
        UmbraVisibility visibility
    )
    {
        _zoneManager = zoneManager;
        _raycaster   = raycaster;
        _visibility  = visibility;

        // Pre-cache a list of markers based on map ids.
        foreach (Map map in dataManager.GetExcelSheet<Map>()!) {
            WorldMarkers[map.RowId] = [];
            Positions[map.RowId]    = [];
        }
    }

    public void Dispose()
    {
        Positions.Clear();
        WorldMarkers.Clear();
    }

    public List<WorldMarker> GetMarkers()
    {
        return WorldMarkers[_zoneManager.CurrentZone.Id].Values.ToList();
    }

    public Vector3 GetResolvedPosition(WorldMarker marker)
    {
        return !Positions[marker.MapId].TryGetValue(marker.Key, out ResolvedPosition? value)
            ? Vector3.Zero
            : value.Resolved;
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

        if (markers.TryGetValue(marker.Key, out WorldMarker? value) && ReferenceEquals(marker, value)) {
            UpdateResolvedPositionOf(marker);
            return marker.Key;
        }

        marker.OnDisposed += () => {
            WorldMarkers[marker.MapId].Remove(marker.Key);
            Positions[marker.MapId].Remove(marker.Key);
        };

        marker.OnKeyChanged += (newKey, oldKey) => {
            markers.Remove(oldKey);
            markers[newKey] = marker;
        };

        marker.OnMapIdChanged += (newMapId, oldMapId) => {
            WorldMarkers[oldMapId].Remove(marker.Key);
            WorldMarkers[newMapId].Add(marker.Key, marker);
            Positions[newMapId].Add(marker.Key, Positions[oldMapId][marker.Key]);
            Positions[oldMapId].Remove(marker.Key);
        };

        markers[marker.Key] = marker;
        UpdateResolvedPositionOf(marker);

        return marker.Key;
    }

    /// <summary>
    /// In some situations, the raycaster may not be able to resolve the
    /// correct position of a marker because the collision data hasn't been
    /// fully loaded yet. Since we can't know when the collision data is
    /// actually loaded, we can periodically clear the cache of resolved
    /// positions to force the raycaster to re-calculate the positions of all
    /// cached markers in a particular zone.
    /// </summary>
    [OnTick(interval: 2000)]
    private void PeriodicallyClearResolvedPositionCache()
    {
        if (!_zoneManager.HasCurrentZone) return;
        Positions[_zoneManager.CurrentZone.Id].Clear();
    }

    [OnDraw(executionOrder: int.MinValue + 1)]
    private void UpdateResolvedPositions()
    {
        if (!_zoneManager.HasCurrentZone || !_visibility.AreMarkersVisible()) return;

        foreach (WorldMarker marker in WorldMarkers[_zoneManager.CurrentZone.Id].Values) {
            UpdateResolvedPositionOf(marker);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void UpdateResolvedPositionOf(WorldMarker marker)
    {
        if (_zoneManager.CurrentZone.Id != marker.MapId) return;

        if (Positions[_zoneManager.CurrentZone.Id].TryGetValue(marker.Key, out ResolvedPosition? resolved)) {
            if (marker.Position.Equals(resolved.Position)) {
                return;
            }
        }

        if (marker.Position.Y != 0) {
            Positions[_zoneManager.CurrentZone.Id][marker.Key] = new() {
                Position = marker.Position,
                Resolved = marker.Position
            };

            return;
        }

        Positions[_zoneManager.CurrentZone.Id][marker.Key] = new() {
            Position = marker.Position,
            Resolved = _raycaster.Raycast(marker)
        };
    }

    private record ResolvedPosition
    {
        public Vector3 Position { get; set; }
        public Vector3 Resolved { get; init; }
    }
}
