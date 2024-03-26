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

public sealed class DropdownCheckboxElement : Element
{
    public string Label { get; set; }
    public bool IsChecked { get; set; }

    public event Action<bool>? OnChange;

    public DropdownCheckboxElement(string? id = null, string label = "", bool isChecked = false) : base(id ?? Guid.NewGuid().ToString())
    {
        Label     = label;
        IsChecked = isChecked;
        Direction = Direction.Horizontal;
        Anchor    = Anchor.Top | Anchor.Left;
        Fit       = true;
        Gap       = 4;
        Padding   = new(0, 0, 0, 4);

        AddChild(new Element(
            id        : "Checkbox",
            anchor    : Anchor.Middle | Anchor.Left,
            size      : new(16, 20),
            nodes     : [
                new RectNode(
                    id          : "Background",
                    color       : IsChecked ? 0xA0704520 : 0xFF2F2F2F,
                    rounding    : 3,
                    margin      : new(2, 0, 2),
                    borderColor : 0xFF000000,
                    borderSize  : 1
                ),
                new TextNode(
                    id       : "Checkmark",
                    color    : 0xFFFFFFFF,
                    font     : Font.AxisLarge,
                    text     : "✓",
                    align    : Align.MiddleCenter,
                    autoSize : false
                )
            ]
        ));

        AddChild(new Element(
            id        : "Label",
            anchor    : Anchor.Middle | Anchor.Left,
            isVisible : true,
            size      : new(0, 28),
            nodes     : [
                new TextNode(
                    id       : "Text",
                    text     : Label,
                    color    : 0xFFD0D0D0,
                    font     : Font.Axis,
                    align    : Align.MiddleLeft,
                    autoSize : false,
                    offset   : new(0, -1),
                    margin   : new(0, 0, 0, 4)
                )
            ]
        ));

        OnMouseEnter    += () => Get("Label").GetNode<TextNode>().Color = 0xFFFFFFFF;
        OnMouseLeave    += () => Get("Label").GetNode<TextNode>().Color = 0xFFD0D0D0;

        OnBeforeCompute += () => {
            Get("Checkbox").GetNode<TextNode>().IsVisible = IsChecked;
            Get("Checkbox").GetNode<RectNode>().Color = IsChecked ? 0xFF705020 : 0xFF2F2F2F;
            Opacity = IsDisabled ? 0.5f : 1f;
        };

        OnClick         += () => {
            IsChecked = !IsChecked;
            OnChange?.Invoke(IsChecked);
        };
    }
}
