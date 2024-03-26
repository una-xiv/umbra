/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Game is free software: you can          \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Game is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Numerics;

namespace Umbra.Game;

public readonly struct ZoneMarker(
    ZoneMarkerType type,
    string         name,
    Vector2        position,
    Vector3        worldPosition,
    uint           iconId,
    uint           dataId,
    float          radius = 0.0f
)
{
    public readonly ZoneMarkerType Type          = type;
    public readonly string         Name          = name;
    public readonly Vector2        Position      = position;
    public readonly Vector3        WorldPosition = worldPosition;
    public readonly uint           IconId        = iconId;
    public readonly uint           DataId        = dataId;
    public readonly float          Radius        = radius;
}
