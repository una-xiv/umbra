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
using System.Numerics;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers;

[Service]
public sealed class WorldMarkerRaycaster
{
    private readonly Dictionary<string, CachedMarkerPosition> _cachedMarkerPositions = [];

    private readonly IZoneManager _zoneManager;
    private readonly Player       _player;

    public WorldMarkerRaycaster(Player player, IZoneManager zoneManager)
    {
        _player      = player;
        _zoneManager = zoneManager;

        zoneManager.ZoneChanged += OnZoneChanged;
    }

    public void Dispose()
    {
        _zoneManager.ZoneChanged -= OnZoneChanged;

        _cachedMarkerPositions.Clear();
    }

    [OnTick(interval: 1000)]
    public void OnTick()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        List<string> toRemove = [];

        foreach (var pair in _cachedMarkerPositions) {
            if (now - pair.Value.CreatedAt > 2) {
                toRemove.Add(pair.Key);
            }
        }

        toRemove.ForEach((marker) => _cachedMarkerPositions.Remove(marker));
    }

    public Vector3 Raycast(in WorldMarker marker)
    {
        var cacheKey = GetCacheKeyOf(marker);

        if (_cachedMarkerPositions.TryGetValue(cacheKey, out var cachedPosition)) {
            return cachedPosition.Position;
        }

        var position = marker.Position;

        if (position.Y != 0) {
            return position;
        }

        // First we go up...
        if (BGCollisionModule.Raycast2(position, new(0, 1, 0), out var hitInfo)) {
            position.Y = hitInfo.Point.Y + 1.8f;
        } else {
            // Can't hit anything, let's go high up and cast down.
            position.Y = _player.Position.Y + 250;
        }

        // Then we go down to "ground" the marker if possible.
        if (BGCollisionModule.Raycast2(position, new(0, -1, 0), out var hitInfo2)) {
            position.Y = hitInfo2.Point.Y + 1f;
        } else {
            // Can't hit anything. Let's move the marker up and cast down by a larger amount.
            position.Y += 500;

            if (BGCollisionModule.Raycast2(position, new(0, -1, 0), out var hitInfo3)) {
                position.Y = hitInfo3.Point.Y + 1f;
            } else {
                // Can't hit anything. Let's just set the marker to the player's Y position.
                position.Y = _player.Position.Y;
            }
        }

        _cachedMarkerPositions[cacheKey] = new() {
            Position  = position,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        return position;
    }

    private void OnZoneChanged(Zone _)
    {
        _cachedMarkerPositions.Clear();
    }

    private static string GetCacheKeyOf(in WorldMarker marker)
    {
        return $"{marker.IconId}:{marker.Position.X},{marker.Position.Y},{marker.Position.Z}";
    }

    private struct CachedMarkerPosition
    {
        public Vector3 Position;
        public double  CreatedAt;
    }

    [OnTick]
    public unsafe void OverrideCollisionStreamSphere()
    {
        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
        if (fw == null) return;

        var bc = fw->BGCollisionModule;
        if (bc == null) return;

        bc->ForcedStreamingSphere.W = 2000;
    }
}
