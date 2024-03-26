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

namespace Umbra.Drawing.Prefabs;

public sealed class DropdownSeparatorElement : Element
{
    public DropdownSeparatorElement(string id = "") : base(id)
    {
        Direction = Direction.Horizontal;
        Anchor    = Anchor.Top | Anchor.Left;
        Fit       = true;
        Padding   = new (2, 4);

        AddNode(new LineNode(
            id    : "Line2",
            color : 0x80FFFFFF,
            margin: new(1, 0, 0, 0)
        ));
        AddNode(new LineNode(
            id    : "Line1",
            color : 0xFF000000,
            margin: new(0, 0, 0, 0)
        ));
    }
}
