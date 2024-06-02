/* Umbra.Interface | (c) 2024 by Una    ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Interface is free software: you can    \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Interface is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Numerics;
using ImGuiNET;
using Una.Drawing;

namespace Umbra.Windows.Clipping;

internal class Rect(int x1 = 0, int y1 = 0, int x2 = 0, int y2 = 0)
{
    public int X1 { get; set; } = x1;
    public int Y1 { get; set; } = y1;
    public int X2 { get; set; } = x2;
    public int Y2 { get; set; } = y2;

    /// <summary>
    /// Creates a new empty <see cref="Rect"/>.
    /// </summary>
    public static Rect Empty => new();

    /// <summary>
    /// A <see cref="Vector2"/> representing the minimum point of this rect.
    /// </summary>
    public Vector2 Min  => new(X1, Y1);

    /// <summary>
    /// A <see cref="Vector2"/> representing the maximum point of this rect.
    /// </summary>
    public Vector2 Max  => new(X2, Y2);

    /// <summary>
    /// A <see cref="Vector2"/> representing the size of this rect.
    /// </summary>
    public Size Size => new(Width, Height);

    /// <summary>
    /// The width of this rect.
    /// </summary>
    public int Width  => X2 - X1;

    /// <summary>
    /// The height of this rect.
    /// </summary>
    public int Height => Y2 - Y1;

    public Rect(Vector2 min, Vector2 max) : this((int)min.X, (int)min.Y, (int)max.X, (int)max.Y) { }

    /// <summary>
    /// Returns true if this rect intersects with the other one.
    /// </summary>
    public bool IntersectsWith(Rect other)
    {
        return !(
            other.Min.X >= Max.X ||
            other.Max.X <= Min.X ||
            other.Min.Y >= Max.Y ||
            other.Max.Y <= Min.Y
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
    public bool Overlaps(Rect other)
    {
        return Min.X <= other.Min.X && Min.Y <= other.Min.Y && Max.X >= other.Max.X && Max.Y >= other.Max.Y;
    }

    /// <summary>
    /// Returns a new Rect that is clamped to the bounds of the given rect.
    /// </summary>
    public Rect Clamp(Rect rect)
    {
        return new Rect(
            new Vector2(Math.Max(Min.X, rect.Min.X), Math.Max(Min.Y, rect.Min.Y)),
            new Vector2(Math.Min(Max.X, rect.Max.X), Math.Min(Max.Y, rect.Max.Y))
        );
    }

    /// <summary>
    /// Returns true if the rect is valid.
    /// </summary>
    public bool IsValid()
    {
        return Min.X < Max.X && Min.Y < Max.Y;
    }

    public void RenderToScreen()
    {
        ImGui.GetForegroundDrawList().AddRect(Min, Max, 0x80FFFF00);
    }

    public override string ToString() => $"(Min: {X1}, {Y1}) (Max: {X2}, {Y2})";
}
