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

namespace Umbra.Interface;

public readonly record struct Size(int Width = 0, int Height = 0) : IComparable
{
    public Size(int size) : this(size, size) { }

    public static Size Auto => new();

    public bool    IsEmpty                => Width == 0 && Height == 0;
    public bool    IsFixed                => Width != 0 && Height != 0;
    public bool    ShouldSpanHorizontally => Width  == 0;
    public bool    ShouldSpanVertically   => Height == 0;
    public Vector2 ToVector2()            => new(Width, Height);

    public static Size operator +(Size  a, Size b) => new Size(a.Width + b.Width, a.Height + b.Height);
    public static Size operator -(Size  a, Size b) => new Size(a.Width - b.Width, a.Height - b.Height);
    public static bool operator <(Size  a, Size b) => a.Width < b.Width && a.Height  < b.Height;
    public static bool operator >(Size  a, Size b) => a.Width > b.Width && a.Height  > b.Height;
    public static Size operator *(double multiplier, Size size) => new((int)Math.Ceiling(size.Width * multiplier), (int)Math.Ceiling(size.Height * multiplier));
    public static Size operator /(double multiplier, Size size) => new((int)Math.Floor(size.Width / multiplier), (int)Math.Floor(size.Height / multiplier));

    public int CompareTo(object? obj)
    {
        if (obj is Size size) {
            if (this < size) return -1;
            if (this > size) return  1;
        }

        return 0;
    }

    public static Size Max(Size a, Size b)
    {
        int width  = Math.Max(a.Width,  b.Width);
        int height = Math.Max(a.Height, b.Height);

        return new Size(width, height);
    }

    public override string ToString() => $"({Width}, {Height})";
}
