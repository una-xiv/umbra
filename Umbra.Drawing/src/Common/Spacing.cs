/* Umbra.Drawing | (c) 2024 by Una      ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Drawing is free software: you can       \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Numerics;

namespace Umbra.Drawing;

/// <summary>
/// Represents spacing used as padding or margin for elements and nodes.
/// </summary>
public readonly struct Spacing(float top = 0, float right = 0, float bottom = 0, float left = 0)
{
    public readonly float Top    = top;
    public readonly float Right  = right;
    public readonly float Bottom = bottom;
    public readonly float Left   = left;

    public Vector2 TopLeft     => new(Left, Top);
    public Vector2 BottomRight => new(Right, Bottom);

    public Spacing(float all = 0) : this(all, all, all, all) { }
    public Spacing(float vertical = 0, float horizontal = 0) : this(vertical, horizontal, vertical, horizontal) { }
}
