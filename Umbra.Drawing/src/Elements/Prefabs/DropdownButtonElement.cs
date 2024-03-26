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
using Dalamud.Game.Text;

namespace Umbra.Drawing.Prefabs;

public sealed class DropdownButtonElement : Element
{
    public string Label { get; set; }
    public string? HotKey { get; set; }
    public SeIconChar? IconChar { get; set; }
    public uint IconColor { get; set; }

    public DropdownButtonElement(string? id = null, string label = "", string? hotKey = null, SeIconChar? iconChar = null, uint iconColor = 0xFFAA6E92, int sortIndex = 0)
    {
        Id        = id ?? Guid.NewGuid().ToString();
        Label     = label;
        HotKey    = hotKey;
        IconChar  = iconChar;
        IconColor = iconColor;
        Direction = Direction.Horizontal;
        Anchor    = Anchor.Top | Anchor.Left;
        SortIndex = sortIndex;
        Size      = new(0, 24);
        Fit       = true;

        AddNode(new RectNode(
            id       : "Background",
            color    : 0xA0704520,
            rounding : 5
            // margin   : new(2, 0, 2, 0)
        ) { IsVisible = false });

        AddChild(new Element(
            id        : "Icon",
            anchor    : Anchor.Middle | Anchor.Left,
            size      : new(24, 24),
            isVisible : IconChar != null,
            nodes     : [
                new TextNode(
                    id       : "Text",
                    text     : IconChar?.ToIconString() ?? "",
                    color    : IconColor,
                    font     : Font.AxisLarge,
                    align    : Align.MiddleLeft,
                    autoSize : false,
                    offset   : new(0, -1),
                    margin   : new(0, 0, 0, 4)
                )
            ]
        ));

        AddChild(new Element(
            id        : "Label",
            anchor    : Anchor.Middle | Anchor.Left,
            isVisible : true,
            size      : new(0, 24),
            margin    : new(0, 0, 0, IconChar == null ? 6 : 0),
            nodes     : [
                new TextNode(
                    id               : "Text",
                    text             : Label,
                    color            : 0xFFD0D0D0,
                    outlineColor     : 0xA0000000,
                    outlineSize : 1,
                    font             : Font.Axis,
                    align            : Align.MiddleLeft,
                    offset           : new(0, -1)
                )
            ]
        ));

        AddChild(new Element(
            id        : "HotKey",
            anchor    : Anchor.Middle | Anchor.Right,
            isVisible : HotKey != null,
            size      : new(0, 24),
            nodes     : [
                new TextNode(
                    id               : "Text",
                    text             : HotKey ?? "",
                    color            : 0xFF909090,
                    outlineColor     : 0xA0000000,
                    outlineSize : 1,
                    font             : Font.AxisSmall,
                    align            : Align.MiddleRight,
                    offset           : new(0, -1),
                    margin           : new(0, 8, 0, 64)
                )
            ]
        ));

        OnMouseEnter += () => {
            GetNode<RectNode>("Background").IsVisible = true;
            Get("Label").GetNode<TextNode>().Color = 0xFFFFFFFF;
        };

        OnMouseLeave += () => {
            GetNode<RectNode>("Background").IsVisible = false;
            Get("Label").GetNode<TextNode>().Color = 0xFFD0D0D0;
        };

        OnBeforeCompute += () => {
            Get("Label").IsVisible  = Label != "";
            Get("HotKey").IsVisible = HotKey != null && HotKey != "";
            Get("Icon").IsVisible   = IconChar != null;

            Get("Label").GetNode<TextNode>().Text  = Label;
            Get("HotKey").GetNode<TextNode>().Text = HotKey != null && HotKey != "" ? $"[{HotKey}]" : "";
            Get("Icon").GetNode<TextNode>().Text   = IconChar?.ToIconString() ?? "";
            Get("Icon").GetNode<TextNode>().Color  = IconColor;

            Opacity = IsDisabled ? 0.5f : 1f;
        };
    }
}