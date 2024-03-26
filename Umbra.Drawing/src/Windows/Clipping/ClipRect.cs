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

using System;
using System.Numerics;

namespace Umbra.Drawing;

public class ClipRect(Vector2 min, Vector2 max)
{
    public Vector2 Min = min;
    public Vector2 Max = max;

    /// <summary>
    /// Returns true if this rect intersects with the other one.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IntersectsWith(ClipRect other)
    {
        return !(
            other.Min.X > Max.X ||
            other.Max.X < Min.X ||
            other.Min.Y > Max.Y ||
            other.Max.Y < Min.Y
        );
    }

    /// <summary>
    /// Returns true if the given point is inside this rect.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool Contains(Vector2 point)
    {
        return point.X >= Min.X && point.X <= Max.X && point.Y >= Min.Y && point.Y <= Max.Y;
    }

    /// <summary>
    /// Returns true if this rect completely overlaps the other one.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Overlaps(ClipRect other)
    {
        return Min.X <= other.Min.X && Min.Y <= other.Min.Y && Max.X >= other.Max.X && Max.Y >= other.Max.Y;
    }

    /// <summary>
    /// Returns a new ClipRect that is clamped to the bounds of the given rect.
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public ClipRect Clamp(ClipRect rect)
    {
        return new ClipRect(
            new Vector2(Math.Max(Min.X, rect.Min.X), Math.Max(Min.Y, rect.Min.Y)),
            new Vector2(Math.Min(Max.X, rect.Max.X), Math.Min(Max.Y, rect.Max.Y))
        );
    }

    /// <summary>
    /// Returns true if the rect is valid.
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        return Min.X < Max.X && Min.Y < Max.Y;
    }
}
