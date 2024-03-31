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

    private readonly Style _fontAwesomeIconStyle = new() {
        Font       = Font.FontAwesome,
        TextAlign  = Anchor.MiddleCenter,
        TextOffset = new(0, -1),
    };

    private readonly Style _seIconCharStyle = new() {
        Font      = Font.AxisLarge,
        TextAlign = Anchor.MiddleCenter,
        TextColor = 0xFFFFFFFF,
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
        string id, string? label = null, object? icon = null, bool isSmall = false, bool isGhost = false
    ) : base(id)
    {
        Label   = label;
        Icon    = icon;
        IsSmall = isSmall;
        IsGhost = isGhost;

        Flow = Flow.Horizontal;
        Size = new(0, isSmall ? 18 : 28);

        Style = new() {
            TextColor             = 0xFFC0C0C0,
            OutlineColor          = 0xFF000000,
            OutlineWidth          = 1,
        };

        AddChild(new BackgroundElement(color: 0xFF212021, edgeColor: 0xFF101010, edgeThickness: 1, rounding: 4));
        AddChild(new BorderElement(rounding: 3, padding: new(1)));
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

        if (Icon is null) {
            iconElement.IsVisible = false;
            labelElement.Anchor   = Anchor.MiddleCenter;
        } else {
            iconElement.IsVisible = true;
            labelElement.Anchor   = Anchor.MiddleLeft;
        }

        if (Label is not null) {
            _bodyElement.Padding   = new(0, 8, 0, 4);
            labelElement.IsVisible = true;
            labelElement.Text      = Label;

            labelElement.Style.Font         = IsSmall ? Font.AxisSmall : Font.Axis;
            labelElement.Style.TextColor    = IsDisabled ? 0xFF6F6F6F : 0xFFC0C0C0;
            labelElement.Style.OutlineColor = IsDisabled ? 0x25000000 : 0xA0000000;
        } else {
            labelElement.IsVisible = false;
            _bodyElement.Padding   = new(0, 4);
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
        Get<BorderElement>().Color                 = 0xFF6F6F6F;
        Get<BorderElement>().Style.BackgroundColor = 0xFF313131;

        if (IsGhost) _bodyElement.Get("Label").Style.TextColor = 0xFFFFFFFF;
    }

    private void HandleMouseLeave()
    {
        Get<BorderElement>().Color                 = 0xFF3F3F3F;
        Get<BorderElement>().Style.BackgroundColor = 0;

        if (IsGhost) _bodyElement.Get("Label").Style.TextColor = 0xFFC0C0C0;
    }

    private void HandleMouseDown()
    {
        Get<BorderElement>().Color                 = 0xFF9F9F9F;
        Get<BorderElement>().Style.BackgroundColor = 0xFF3A3A3A;
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
