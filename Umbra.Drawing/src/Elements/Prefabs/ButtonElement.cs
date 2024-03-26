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

namespace Umbra.Drawing.Prefabs;

public sealed class ButtonElement : Element
{
    public string Text { get; set; }
    public uint? IconId { get; set; }
    public bool IsGhost { get; set; }
    public bool IsSmall { get; set; }

    public ButtonElement(
        string? id = null,
        string text = "",
        uint? iconId = null,
        bool isGhost = false,
        bool isSmall = false,
        Anchor anchor = Anchor.Top | Anchor.Left,
        Spacing padding = new(),
        Spacing margin = new()
    )
    {
        Id        = id ?? Guid.NewGuid().ToString();
        Text      = text;
        IconId    = iconId;
        IsGhost   = isGhost;
        IsSmall   = isSmall;
        Anchor    = anchor;
        Direction = Direction.Horizontal;
        Padding   = padding;
        Margin    = margin;
        Size      = new(0, IsSmall ? 18 : 28);
        Gap       = 0;

        AddNode(new RectNode(id: "Background",  color: 0xFF2F2F2F, borderColor: 0xFF000000, borderSize: 1, rounding: 5));
        AddNode(new RectNode(id: "GradientTop", color: 0, gradients: new(bottomLeft: 0xFF3F3F3F, bottomRight: 0xFF3F3F3F), margin: new(2, 2, 14, 2)));
        AddNode(new RectNode(id: "GradientBottom", color: 0, gradients: new(topLeft: 0xFF3F3F3F, topRight: 0xFF3F3F3F), margin: new(14, 2, 2, 2)));

        AddChild(new Element(
            id        : "Icon",
            anchor    : Anchor.Middle | Anchor.Left,
            size      : new(IsSmall ? 17 : 27, IsSmall ? 17 : 27),
            isVisible : iconId != null,
            nodes     : [
                new IconNode(
                    id       : "Icon",
                    iconId   : iconId ?? 0,
                    margin   : new(3, 3, 2, 3),
                    rounding : 5
                )
            ]
        ));

        AddNode(new RectNode(id: "InsetBorder", color: 0, borderColor: 0xFF686868, borderSize: 1, rounding: 4, margin: new(1, 1, 1, 1)));

        AddChild(new Element(
            id        : "Label",
            anchor    : Anchor.Middle | Anchor.Left,
            isVisible : text != "",
            nodes     : [
                new TextNode(
                    id               : "Label",
                    text             : text,
                    font             : IsSmall ? Font.AxisSmall : Font.Axis,
                    align            : iconId != null ? Align.MiddleLeft : Align.MiddleCenter,
                    color            : 0xFFD0D0D0,
                    outlineColor     : 0xA0000000,
                    outlineSize : 1,
                    margin           : new(left: IsSmall ? 6 : 12, right: IsSmall ? 6 : 12),
                    offset           : new(0, -1)
                )
            ]
        ));

        OnMouseEnter += () => {
            GetNode<RectNode>("Background").Color = 0xFF3F3F3F;
            Get("Label").GetNode<TextNode>().Color = 0xFFFFFFFF;
        };

        OnMouseLeave += () => {
            GetNode<RectNode>("Background").Color = 0xFF2F2F2F;
            Get("Label").GetNode<TextNode>().Color = 0xFFD0D0D0;
        };

        OnMouseDown += () => {
            GetNode<RectNode>("Background").BorderColor = 0x80FFFFFF;
        };

        OnMouseUp += () => {
            GetNode<RectNode>("Background").BorderColor = 0xFF000000;
        };

        OnBeforeCompute += () => {
            Get("Label").IsVisible = Text != "";
            Get("Icon").IsVisible  = IconId != null;

            Get("Label").GetNode<TextNode>().Text     = Text;
            Get("Icon").GetNode<IconNode>().IconId    = IconId ?? 0;
            Get("Icon").GetNode<IconNode>().Grayscale = IsDisabled ? 0.75f : 0f;

            GetNode<RectNode>("GradientTop").IsVisible    = !IsGhost && !IsDisabled;
            GetNode<RectNode>("GradientBottom").IsVisible = !IsGhost && !IsDisabled;
            GetNode<RectNode>("Background").IsVisible     = !IsGhost;
            GetNode<RectNode>("InsetBorder").IsVisible    = !IsGhost;

            Opacity = IsDisabled ? 0.65f : 1f;
        };
    }
}
