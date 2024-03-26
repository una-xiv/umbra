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

public sealed class TextElement : Element
{
    public TextElement(string text, string id = "", Font font = Font.Default, uint color = 0xFFFFFFFF, uint outlineColor = 0xFF000000, int outlineThickness = 0, Align align = Align.TopLeft, Anchor anchor = Anchor.Top | Anchor.Left) : base(id: id, anchor: anchor)
    {
        Direction = Direction.Horizontal;
        Fit       = true;

        AddNode(new TextNode(
            id               : "Text",
            text             : text,
            font             : font,
            color            : color,
            align            : align,
            outlineColor     : outlineColor,
            outlineSize : outlineThickness
        ));
    }
}
