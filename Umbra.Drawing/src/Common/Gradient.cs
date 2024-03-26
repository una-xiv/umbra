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

namespace Umbra.Drawing;

/// <summary>
/// Represents the gradient configuration of a <see cref="RectNode"/>.
/// </summary>
public struct Gradient(uint topLeft = 0x00000000, uint topRight = 0x00000000, uint bottomLeft = 0x00000000, uint bottomRight = 0x00000000)
{
    public readonly uint TopLeft     = topLeft;
    public readonly uint TopRight    = topRight;
    public readonly uint BottomLeft  = bottomLeft;
    public readonly uint BottomRight = bottomRight;
}
