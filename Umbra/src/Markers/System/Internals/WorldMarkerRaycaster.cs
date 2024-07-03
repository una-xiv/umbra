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

        // If Y is not zero we assume that position was already computed, so we early return.
        if (position.Y != 0)
        {
            return position;
        }

        var castDirection = new Vector3(0, -1, 0); // Downward direction vector for raycast.

        // Initially, set the height just a bit higher than the current marker's position.
        position.Y += 1.8f;

        // Cast a ray downwards. If it doesn't hit anything,
        if (!BGCollisionModule.RaycastMaterialFilter(position, castDirection, out var hitInfo))
        {
            // Move the marker far up and cast another ray downwards.
            position.Y = _player.Position.Y + 250f;

            // If it still doesn't hit anything,
            if (!BGCollisionModule.RaycastMaterialFilter(position, castDirection, out hitInfo))
            {
                // Move marker very high up, fallback to player's height if it still doesn't hit anything.
                position.Y += 500f;

                if (!BGCollisionModule.RaycastMaterialFilter(position, castDirection, out hitInfo))
                {
                    position.Y = _player.Position.Y;
                    return position;
                }
            }
        }

        // If a ray intersected with anything, place the marker 1 unit above the hit point.
        position.Y = hitInfo.Point.Y + 1f;
        return position;
    }
}
