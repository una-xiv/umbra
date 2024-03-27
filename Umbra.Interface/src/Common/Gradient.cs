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

namespace Umbra.Interface;

public class Gradient(uint topLeft, uint topRight, uint bottomLeft, uint bottomRight)
{
    public uint TopLeft     { get; set; } = topLeft;
    public uint TopRight    { get; set; } = topRight;
    public uint BottomLeft  { get; set; } = bottomLeft;
    public uint BottomRight { get; set; } = bottomRight;

    public Gradient(uint color) : this(color, color, color, color) { }
    public Gradient(uint topColor, uint bottomColor) : this(topColor, topColor, bottomColor, bottomColor) { }

    public static Gradient VerticalGradient(uint top, uint bottom) => new Gradient(top, top, bottom, bottom);
    public static Gradient HorizontalGradient(uint left, uint right) => new Gradient(left, right, left, right);

    public bool HasValue => TopLeft != 0 || TopRight != 0 || BottomLeft != 0 || BottomRight != 0;
}
