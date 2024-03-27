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

public readonly struct Spacing(int top = 0, int right = 0, int bottom = 0, int left = 0)
{
    public int Top    { get; } = top;
    public int Right  { get; } = right;
    public int Bottom { get; } = bottom;
    public int Left   { get; } = left;

    public int Horizontal => Left + Right;
    public int Vertical   => Top  + Bottom;

    public Vector2 TopLeft     => new(Left, Top);
    public Vector2 BottomRight => new(Right, Bottom);

    public Spacing(int spacing) : this(spacing, spacing, spacing, spacing) { }

    public Spacing(int verticalSpacing, int horizontalSpacing) : this(
        verticalSpacing,
        horizontalSpacing,
        verticalSpacing,
        horizontalSpacing
    ) { }

    public static Spacing operator +(Spacing a, Spacing b) =>
        new(a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom, a.Left + b.Left);

    public static Spacing operator -(Spacing a, Spacing b) =>
        new(a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom, a.Left - b.Left);

    public static bool operator ==(Spacing a, Spacing b) =>
        a.Top == b.Top && a.Right == b.Right && a.Bottom == b.Bottom && a.Left == b.Left;

    public static bool operator !=(Spacing a, Spacing b) =>
        a.Top != b.Top || a.Right != b.Right || a.Bottom != b.Bottom || a.Left != b.Left;

    public override bool Equals(object? obj) => obj is Spacing size && this == size;
    public override int  GetHashCode()       => HashCode.Combine(Top, Right, Bottom, Left);
}
