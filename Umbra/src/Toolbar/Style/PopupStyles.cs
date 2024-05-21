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

internal class PopupStyles
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
                }
            ),
            new(
                ".widget-popup:top",
                new() {
                    ShadowSize              = new() { Top = 0, Left = 64, Bottom = 64, Right = 64 },
                    BorderWidth             = new() { Top = 0, Left = 1, Bottom  = 1, Right  = 1 },
                    RoundedCorners          = RoundedCorners.BottomLeft | RoundedCorners.BottomRight,
                    BackgroundGradientInset = new(2) { Top = 0, Right = 3 },
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
                    BackgroundGradientInset = new(2) { Bottom = 0, Right = 3 },
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
                    BackgroundGradientInset = new(3) { Right = 4 },
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
                ".button--icon",
                new() {
                    Anchor        = Anchor.MiddleLeft,
                    Size          = new(24, 24),
                    ImageInset     = new(2),
                    ImageRounding  = 4,
                    ImageOffset    = new(0, -1),
                    ImageGrayscale = true,
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
                }
            ),
        ]
    );
}
