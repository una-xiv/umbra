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

using System.Numerics;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.System;

[Service]
internal sealed class WorldMarkerRaycaster
{
    private readonly IPlayer _player;

    public WorldMarkerRaycaster(IPlayer player, IZoneManager zoneManager)
    {
        _player = player;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public Vector3 Raycast(in WorldMarker marker)
    {
        var position = marker.Position;

        if (position.Y != 0) {
            return position;
        }

        // First we go up...
        if (BGCollisionModule.RaycastMaterialFilter(position, new(0, 1, 0), out var hitInfo)) {
            position.Y = hitInfo.Point.Y + 1.8f;
        } else {
            // Can't hit anything, let's go high up and cast down.
            position.Y = _player.Position.Y + 250;
        }

        // Then we go down to "ground" the marker if possible.
        if (BGCollisionModule.RaycastMaterialFilter(position, new(0, -1, 0), out var hitInfo2)) {
            position.Y = hitInfo2.Point.Y + 1f;
        } else {
            // Can't hit anything. Let's move the marker up and cast down by a larger amount.
            position.Y += 500;

            if (BGCollisionModule.RaycastMaterialFilter(position, new(0, -1, 0), out var hitInfo3)) {
                position.Y = hitInfo3.Point.Y + 1f;
            } else {
                // Can't hit anything. Let's just set the marker to the player's Y position.
                position.Y = _player.Position.Y;
            }
        }

        return position;
    }
}
