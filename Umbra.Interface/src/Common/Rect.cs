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

using System.Numerics;

namespace Umbra.Interface;

public readonly struct Rect(int x1 = 0, int y1 = 0, int x2 = 0, int y2 = 0)
{
    public int X1 { get; } = x1;
    public int Y1 { get; } = y1;
    public int X2 { get; } = x2;
    public int Y2 { get; } = y2;

    public static Rect Empty => new Rect();

    public Vector2 Min  => new(X1, Y1);
    public Vector2 Max  => new(X2, Y2);
    public Vector2 Size => new(Width, Height);

    public int Width  => X2 - X1;
    public int Height => Y2 - Y1;

    public Rect(Vector2 min, Vector2 max) : this((int)min.X, (int)min.Y, (int)max.X, (int)max.Y) { }

    public bool Contains(Vector2 v) => v.X >= X1 && v.X <= X2 && v.Y >= Y1 && v.Y <= Y2;

    public override string ToString() => $"(Min: {X1}, {Y1}) (Max: {X2}, {Y2})";
}
