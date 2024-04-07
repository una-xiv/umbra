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

public class Gradient(Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
{
    public Color TopLeft     { get; set; } = topLeft;
    public Color TopRight    { get; set; } = topRight;
    public Color BottomLeft  { get; set; } = bottomLeft;
    public Color BottomRight { get; set; } = bottomRight;

    public Gradient(Color color) : this(color, color, color, color) { }
    public Gradient(Color topColor, Color bottomColor) : this(topColor, topColor, bottomColor, bottomColor) { }

    public static Gradient Vertical(Color   top,  Color bottom) => new(top, top, bottom, bottom);
    public static Gradient Horizontal(Color left, Color right)  => new(left, right, left, right);

    public bool HasValue => TopLeft != 0 || TopRight != 0 || BottomLeft != 0 || BottomRight != 0;
}
