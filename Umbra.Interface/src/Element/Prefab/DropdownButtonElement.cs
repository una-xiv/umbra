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
using Dalamud.Game.Text;
using Dalamud.Interface;

namespace Umbra.Interface;

public class DropdownButtonElement : Element
{
    public string  Label;
    public object? Icon;
    public uint?   IconColor;
    public string? KeyBind;

    private readonly BackgroundElement _highlightElement = new(color: 0x305FCFFF, rounding: 4, padding: new(-2, 1));
    private readonly Element _iconElement = new("Icon", size: new(18), margin: new(0, 4), anchor: Anchor.MiddleLeft);
    private readonly Element _labelElement = new("Label", size: new(0, 18), anchor: Anchor.MiddleLeft);

    private readonly Element _keyBindElement = new(
        "KeyBind",
        size: new(0, 18),
        margin: new(right: 8),
        anchor: Anchor.MiddleRight
    );

    public DropdownButtonElement(
        string  id,
        string  label,
        object? icon       = null,
        string? keyBind    = null,
        uint?   iconColor  = null,
        bool    isDisabled = false
    ) : base(id)
    {
        Label      = label;
        Icon       = icon;
        IconColor  = iconColor;
        KeyBind    = keyBind;
        IsDisabled = isDisabled;

        Anchor = Anchor.TopLeft;
        Flow   = Flow.Horizontal;
        Size   = new(0, 20);
        Gap    = 4;

        AddChild(_highlightElement);
        AddChild(_iconElement);
        AddChild(_labelElement);
        AddChild(_keyBindElement);

        _labelElement.Style = new() {
            Font         = Font.Axis,
            TextAlign    = Anchor.MiddleLeft,
            TextColor    = 0xFFC0C0C0,
            TextOffset   = new(0, -1),
            OutlineColor = 0xFF000000,
            OutlineWidth = 1,
        };

        _keyBindElement.Style = new() {
            Font         = Font.AxisSmall,
            TextAlign    = Anchor.MiddleRight,
            TextColor    = 0xFF909090,
            OutlineColor = 0x80000000,
            OutlineWidth = 1,
        };

        Style.BackgroundColor           = 0;
        _highlightElement.Style.Opacity = 0;

        OnMouseEnter += () => {
            _highlightElement.Style.Opacity = 1;
            _labelElement.Style.TextColor   = 0xFFFFFFFF;
        };

        OnMouseLeave += () => {
            _highlightElement.Style.Opacity = 0;
            _labelElement.Style.TextColor   = 0xFFC0C0C0;
        };
    }

    protected override void BeforeCompute()
    {
        UpdateSize();
        UpdateIcon();

        Style.Opacity = IsDisabled ? 0.5f : 1;

        _labelElement.Text   = Label;
        _keyBindElement.Text = !string.IsNullOrEmpty(KeyBind) ? $"[{KeyBind}]" : "";
    }

    private void UpdateSize()
    {
        // Find all siblings and determine the max width.
        var maxWidth = 0;

        foreach (var sibling in Parent?.Children ?? Array.Empty<Element>()) {
            if (sibling is not DropdownButtonElement button) continue;

            maxWidth = Math.Max(maxWidth, button._labelElement.GetTextSize().Width);
        }

        // Set the width of the label element to the max width.
        _labelElement.Size = new(maxWidth, 20);
        Size               = new(maxWidth + 20 + 120, 20);
    }

    private void UpdateIcon()
    {
        switch (Icon) {
            case uint iconId:
                _iconElement.Size        = new(18);
                _iconElement.Text        = null;
                _iconElement.Style.Image = iconId;
                return;
            case string iconPath:
                _iconElement.Size        = new(18);
                _iconElement.Text        = "";
                _iconElement.Style.Image = iconPath;
                return;
            case SeIconChar iconChar:
                _iconElement.Style.Font       = Font.AxisLarge;
                _iconElement.Style.TextOffset = new(2, 0);
                _iconElement.Style.TextAlign  = Anchor.MiddleCenter;
                _iconElement.Style.TextColor  = IconColor;
                _iconElement.Text             = iconChar.ToIconString();
                return;
            case FontAwesomeIcon faIcon:
                _iconElement.Style.Font       = Font.FontAwesome;
                _iconElement.Style.TextOffset = new(0, -1);
                _iconElement.Style.TextAlign  = Anchor.MiddleCenter;
                _iconElement.Style.TextColor  = IconColor;
                _iconElement.Text             = faIcon.ToIconString();
                return;
            default:
                _iconElement.Style.Image = null;
                _iconElement.Text        = null;
                break;
        }
    }
}
