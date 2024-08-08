/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using Una.Drawing;

namespace Umbra.Style;

public class PopupStyles
{
    public static Stylesheet WidgetPopupStylesheet { get; } = new(
        [
            new(
                ".widget-popup",
                new() {
                    Anchor          = Anchor.TopLeft,
                    Flow            = Flow.Vertical,
                    BackgroundColor = new("Widget.PopupBackground"),
                    BorderColor     = new(new("Widget.PopupBorder")),
                    BorderRadius    = 7,
                    ShadowInset     = 8,
                    IsAntialiased   = false,
                }
            ),
            new(
                ".widget-popup:top",
                new() {
                    ShadowSize              = new() { Top = 0, Left = 64, Bottom = 64, Right = 64 },
                    BorderWidth             = new() { Top = 0, Left = 1, Bottom  = 1, Right  = 1 },
                    RoundedCorners          = RoundedCorners.BottomLeft | RoundedCorners.BottomRight,
                    BackgroundGradientInset = new(2) { Top = 0 },
                    BackgroundGradient = GradientColor.Vertical(
                        new("Widget.PopupBackground.Gradient1"),
                        new("Widget.PopupBackground.Gradient2")
                    ),
                }
            ),
            new(
                ".widget-popup:bottom",
                new() {
                    ShadowSize              = new() { Top = 64, Left = 64, Bottom = 0, Right = 64 },
                    BorderWidth             = new() { Top = 1, Left  = 1, Bottom  = 0, Right = 1 },
                    RoundedCorners          = RoundedCorners.TopLeft | RoundedCorners.TopRight,
                    BackgroundGradientInset = new(2) { Bottom = 0 },
                    BackgroundGradient = GradientColor.Vertical(
                        new("Widget.PopupBackground.Gradient2"),
                        new("Widget.PopupBackground.Gradient1")
                    ),
                }
            ),
            new(
                ".widget-popup:floating",
                new() {
                    ShadowSize              = new(64),
                    BorderWidth             = new(1),
                    BackgroundGradientInset = new(3),
                    RoundedCorners          = RoundedCorners.All,
                    Margin                  = new(8),
                }
            ),
        ]
    );

    public static Stylesheet MenuPopupStylesheet { get; } = new(
        [
            new(
                ".button",
                new() {
                    Flow         = Flow.Horizontal,
                    Size         = new(0, 24),
                    BorderRadius = 3,
                }
            ),
            new(
                ".button:hover",
                new() {
                    BackgroundColor = new("Widget.PopupMenuBackgroundHover")
                }
            ),
            new(
                ".button:disabled",
                new() {
                    BackgroundColor = new(0)
                }
            ),
            new(
                ".button--icon",
                new() {
                    Anchor         = Anchor.MiddleLeft,
                    Size           = new(24, 24),
                    ImageInset     = new(2),
                    ImageRounding  = 4,
                    ImageOffset    = new(0, -1),
                    ImageGrayscale = true,
                    Font           = 2,
                    TextAlign      = Anchor.MiddleCenter,
                    TextOffset     = new(0, -1),
                }
            ),
            new(
                ".button--icon:disabled",
                new() {
                    Opacity        = 0.5f,
                    ImageGrayscale = true,
                }
            ),
            new(
                ".button--icon--glyph",
                new() {
                    Anchor       = Anchor.MiddleLeft,
                    Size         = new(24, 24),
                    Font         = 4,
                    FontSize     = 14,
                    TextAlign    = Anchor.MiddleCenter,
                    TextOffset   = new(0, 1),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".button--label",
                new() {
                    Anchor       = Anchor.MiddleLeft,
                    Padding      = new(0, 4),
                    FontSize     = 13,
                    TextAlign    = Anchor.MiddleLeft,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".button--label:hover",
                new() {
                    Color        = new("Widget.PopupMenuTextHover"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineHover"),
                }
            ),
            new(
                ".button--label:disabled",
                new() {
                    Color        = new("Widget.PopupMenuTextDisabled"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineDisabled"),
                }
            ),
            new(
                ".button--altText",
                new() {
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleRight,
                    Padding      = new(0, 4),
                    Margin       = new() { Left = 24 },
                    Color        = new("Widget.PopupMenuTextMuted"),
                    FontSize     = 11,
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".button--altText:hover",
                new() {
                    Color        = new("Widget.PopupMenuTextHover"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineHover"),
                }
            ),
            new(
                ".button--altText:disabled",
                new() {
                    Color        = new("Widget.PopupMenuTextDisabled"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineDisabled"),
                }
            ),
            new(
                ".button-group",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new(4, 0),
                    Gap     = 6,
                }
            ),
            new(
                ".button-group--header",
                new() {
                    Flow = Flow.Horizontal,
                }
            ),
            new(
                ".button-group--line",
                new() {
                    Anchor          = Anchor.MiddleCenter,
                    Size            = new(2, 1),
                    BackgroundColor = new("Widget.Border"),
                }
            ),
            new(
                ".button-group--label",
                new() {
                    Anchor    = Anchor.MiddleCenter,
                    FontSize  = 12,
                    Padding   = new(0, 4),
                    TextAlign = Anchor.MiddleCenter,
                    Color     = new("Widget.PopupMenuTextMuted"),
                }
            ),
            new(
                ".button-group--items",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 6,
                }
            ),
        ]
    );

    public static Stylesheet ButtonGridStylesheet { get; } = new(
        [
            new(
                "#Popup",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new(8),
                    Gap     = 8,
                }
            ),
            new(
                "#CategoryBar",
                new() {
                    Flow          = Flow.Horizontal,
                    Size          = new(424, 0),
                    BorderColor   = new() { Bottom = new("Widget.PopupBorder") },
                    BorderWidth   = new() { Bottom = 1 },
                    IsAntialiased = false,
                }
            ),
            new(
                ".category-button",
                new() {
                    Size         = new(102, 24),
                    Padding      = new(0, 4),
                    TextAlign    = Anchor.MiddleLeft,
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineDisabled"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".category-button:hover",
                new() {
                    Color           = new("Widget.PopupMenuText"),
                    OutlineColor    = new("Widget.PopupMenuTextOutline"),
                    BackgroundColor = new(0x40FFFFFF),
                }
            ),
            new(
                ".category-button:selected",
                new() {
                    Color         = new("Widget.PopupMenuText"),
                    OutlineColor  = new("Widget.PopupMenuTextOutline"),
                    BorderColor   = new() { Bottom = new("Window.AccentColor") },
                    BorderWidth   = new() { Bottom = 3 },
                    IsAntialiased = false,
                }
            ),
            new(
                "#Footer",
                new() {
                    Flow          = Flow.Vertical,
                    Size          = new(424, 0),
                    Padding       = new(8, 0),
                    BorderColor   = new() { Top = new("Widget.PopupBorder") },
                    BorderWidth   = new() { Top = 1 },
                    IsAntialiased = false,
                    Gap           = 4,
                }
            ),
            new(
                ".slot-container",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 8,
                }
            ),
            new(
                ".slot-row",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                ".slot-button",
                new() {
                    Size            = new(46, 46),
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.Border"),
                    StrokeWidth     = 1,
                    BorderRadius    = 6,
                    Padding         = new(2),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".slot-button:hover",
                new() {
                    BackgroundColor = new("Input.BackgroundHover"),
                    StrokeColor     = new("Input.BorderHover"),
                }
            ),
            new(
                ".slot-button:empty-hidden",
                new() {
                    Opacity         = 0.15f,
                    BackgroundColor = new(0),
                    StrokeColor     = new(0),
                }
            ),
            new(
                ".slot-button:empty-visible",
                new() {
                    Opacity         = 0.15f,
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.Border"),
                }
            ),
            new(
                ".slot-button:empty-visible:hover",
                new() {
                    Opacity         = 0.35f,
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.Border"),
                }
            ),
            new(
                ".slot-button--icon",
                new() {
                    Size          = new(42, 42),
                    BorderRadius  = 5,
                    IsAntialiased = false,
                    FontSize      = 10,
                }
            ),
            new(
                ".slot-button--sub-icon",
                new() {
                    Anchor = Anchor.TopRight,
                    Size   = new(20, 20),
                    Margin = new() { Bottom = -1, Right = -1 },
                }
            ),
            new(
                ".slot-button--count",
                new() {
                    Anchor       = Anchor.BottomRight,
                    TextAlign    = Anchor.TopRight,
                    FontSize     = 12,
                    Padding      = new(2),
                    Color        = new("Window.Text"),
                    OutlineColor = new(0xDA000000),
                    OutlineSize  = 2,
                }
            ),
            new(".slot-button:disabled, .slot-button:blocked", new() {
                Opacity         = 0.65f,
                BackgroundColor = new("Input.Background"),
                StrokeColor     = new("Input.Border"),
            }),
            new(".slot-button--icon:disabled, .slot-button--icon:blocked", new() { ImageGrayscale = true })
        ]
    );
}
