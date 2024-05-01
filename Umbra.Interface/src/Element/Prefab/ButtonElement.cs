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

public class ButtonElement : Element
{
    /// <summary>
    /// Defines the text label of the button. Set to null to hide the label.
    /// </summary>
    public string? Label;

    /// <summary>
    /// Defines the icon of the button. The icon type is determined by the object type:
    ///
    /// <list type="bullet">
    /// <item>A <see cref="FontAwesomeIcon"/> will render a FontAwesome icon.</item>
    /// <item>A <see cref="uint"/> will render a game icon with the given ID.</item>
    /// <item>A <see cref="SeIconChar"/> will render a font icon character.</item>
    /// </list>
    /// </summary>
    public object? Icon;

    /// <summary>
    /// Whether to render a smaller button.
    /// </summary>
    public bool IsSmall;

    /// <summary>
    /// True to hide the outline and background of the button until hovered.
    /// </summary>
    public bool IsGhost;

    /// <summary>
    /// The horizontal padding of the button.
    /// </summary>
    public int HorizontalPadding;

    /// <summary>
    /// A custom overlay color for the button.
    /// </summary>
    public Color? Color;

    private readonly Style _fontAwesomeIconStyle = new() {
        Font       = Font.FontAwesome,
        TextAlign  = Anchor.MiddleCenter,
        TextOffset = new(0, -1),
    };

    private readonly Style _seIconCharStyle = new() {
        Font      = Font.AxisLarge,
        TextAlign = Anchor.MiddleCenter,
        TextColor = Theme.Color(ThemeColor.Text),
    };

    private readonly Element _bodyElement = new(
        id: "Body",
        gap: 6,
        size: new(0, 28),
        padding: new(0, 4),
        children: [
            new(
                "Icon",
                anchor: Anchor.MiddleLeft,
                size: new(20, 20)
            ),
            new(
                "Label",
                anchor: Anchor.MiddleCenter,
                size: new(0, 28),
                style: new() {
                    Font       = Font.Axis,
                    TextAlign  = Anchor.MiddleLeft,
                    TextOffset = new(0, -1),
                }
            )
        ]
    );

    public ButtonElement(
        string  id,
        string? label    = null,
        object? icon     = null,
        bool    isSmall  = false,
        bool    isGhost  = false,
        int     hPadding = 8,
        Color?  color    = null,
        Action? onClick  = null
    ) : base(id)
    {
        Label             = label;
        Icon              = icon;
        IsSmall           = isSmall;
        IsGhost           = isGhost;
        HorizontalPadding = hPadding;
        Color             = color;

        if (onClick is not null) OnClick += onClick;

        Flow = Flow.Horizontal;
        Size = new(0, isSmall ? 18 : 28);

        Style = new() {
            TextColor    = Theme.Color(ThemeColor.Text),
            OutlineColor = Theme.Color(ThemeColor.TextOutline),
            OutlineWidth = 1,
        };

        AddChild(
            new BackgroundElement(
                color: Theme.Color(ThemeColor.Background),
                edgeColor: Theme.Color(ThemeColor.BorderDark),
                edgeThickness: 1,
                rounding: 4
            )
        );

        AddChild(new BorderElement(rounding: 3, padding: new(1)));

        AddChild(
            new(
                id: "CustomColorOverlay",
                anchor: Anchor.None,
                padding: new(3, 3),
                children: [
                    new BackgroundElement()
                ]
            )
        );

        AddChild(_bodyElement);

        OnMouseEnter += HandleMouseEnter;
        OnMouseLeave += HandleMouseLeave;
        OnMouseDown  += HandleMouseDown;
        OnMouseUp    += HandleMouseUp;

        var iconElement  = _bodyElement.Get("Icon");
        var labelElement = _bodyElement.Get("Label");

        Size              = IsSmall ? new(0, 18) : new(0, 28);
        _bodyElement.Size = Size;
        iconElement.Size  = IsSmall ? new(18, 18) : new(20, 20);
        labelElement.Size = IsSmall ? new(0, 18) : new(0, 28);
    }

    protected override void BeforeCompute()
    {
        var iconElement  = _bodyElement.Get("Icon");
        var labelElement = _bodyElement.Get("Label");

        Get<BorderElement>().Style.Opacity = IsDisabled ? 0.33f : 1.0f;
        Get<BorderElement>().IsVisible     = !IsGhost || IsMouseOver;
        Get<BackgroundElement>().IsVisible = !IsGhost || IsMouseOver;

        Get("CustomColorOverlay").Get<BackgroundElement>().Style.Gradient = Color is not null
            ? Gradient.Vertical(Color, 0)
            : null;

        if (Icon is null) {
            iconElement.IsVisible = false;
            labelElement.Anchor   = Anchor.MiddleCenter;
        } else {
            iconElement.IsVisible = true;
            labelElement.Anchor   = Anchor.MiddleLeft;
        }

        if (Label is not null) {
            _bodyElement.Padding   = new(0, HorizontalPadding, 0, HorizontalPadding / 2);
            labelElement.IsVisible = true;
            labelElement.Text      = Label;

            labelElement.Style.Font = IsSmall ? Font.AxisSmall : Font.Axis;

            labelElement.Style.TextColor =
                IsDisabled ? Theme.Color(ThemeColor.TextMuted) : Theme.Color(ThemeColor.Text);

            labelElement.Style.OutlineColor =
                IsDisabled ? Theme.Color(ThemeColor.TextOutline) : Theme.Color(ThemeColor.TextOutlineLight);
        } else {
            labelElement.IsVisible = false;
            _bodyElement.Padding   = new(0, HorizontalPadding / 2);
        }

        switch (Icon) {
            case FontAwesomeIcon fontAwesomeIcon:
                iconElement.Size  = new(18, 20);
                iconElement.Style = _fontAwesomeIconStyle;
                iconElement.Text  = fontAwesomeIcon.ToIconString();
                break;
            case uint iconId:
                iconElement.Size = new(20, 20);

                iconElement.Style = new() {
                    Image          = iconId,
                    ImageGrayscale = IsDisabled ? 1.0f : 0,
                    Opacity        = IsDisabled ? 0.66f : 1.0f,
                };

                break;
            case string iconStr:
                iconElement.Size = new(20, 20);

                iconElement.Style = new() {
                    Image          = iconStr,
                    ImageGrayscale = IsDisabled ? 1.0f : 0,
                    Opacity        = IsDisabled ? 0.66f : 1.0f,
                };

                break;
            case SeIconChar iconChar:
                iconElement.Size  = new(18, 20);
                iconElement.Text  = iconChar.ToIconString();
                iconElement.Style = _seIconCharStyle;
                break;
            default:
                iconElement.Style = new();
                break;
        }
    }

    private void HandleMouseEnter()
    {
        Get<BorderElement>().Color                 = Theme.Color(ThemeColor.BorderLight);
        Get<BorderElement>().Style.BackgroundColor = Theme.Color(ThemeColor.BackgroundLight);

        if (IsGhost) _bodyElement.Get("Label").Style.TextColor = Theme.Color(ThemeColor.TextLight);
    }

    private void HandleMouseLeave()
    {
        Get<BorderElement>().Color                 = Theme.Color(ThemeColor.Border);
        Get<BorderElement>().Style.BackgroundColor = 0;

        if (IsGhost) _bodyElement.Get("Label").Style.TextColor = Theme.Color(ThemeColor.Text);
    }

    private void HandleMouseDown()
    {
        Get<BorderElement>().Color                 = Theme.Color(ThemeColor.BorderLight);
        Get<BorderElement>().Style.BackgroundColor = Theme.Color(ThemeColor.BackgroundActive);
    }

    private void HandleMouseUp()
    {
        if (IsMouseOver) {
            HandleMouseEnter();
        } else {
            HandleMouseLeave();
        }
    }
}
