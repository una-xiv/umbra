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

namespace Umbra.Widgets;

internal partial class CompanionPopup
{
    private static Stylesheet CompanionPopupStylesheet { get; } = new(
        [
            new(
                ".popup",
                new() {
                    Flow = Flow.Vertical,
                    Size = new(330, 0),
                }
            ),
            new(
                ".header",
                new() {
                    Flow                    = Flow.Horizontal,
                    Size                    = new(330, 0),
                    Padding                 = new(15),
                    Gap                     = 15,
                    BackgroundGradient      = GradientColor.Vertical(new(0x80ACD1F1), new(0x00ACD1F1)),
                    BackgroundGradientInset = new(4),
                    BorderRadius            = 3,
                    IsAntialiased           = false,
                }
            ),
            new(
                ".header-text",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 3,
                }
            ),
            new(
                ".header-text-name",
                new() {
                    Size         = new(200, 0),
                    Font         = 0,
                    FontSize     = 20,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextOffset   = new(0, 2),
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".header-text-info",
                new() {
                    Size         = new(200, 0),
                    Font         = 0,
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".header-time-left",
                new() {
                    Anchor       = Anchor.TopRight,
                    Font         = 0,
                    FontSize     = 28,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".body",
                new() {
                    Flow        = Flow.Horizontal,
                    Size        = new(329, 0),
                    Padding     = new(15) { Top = 0 },
                    Gap         = 15,
                    BorderColor = new() { Bottom = new("Widget.Border") },
                    BorderWidth = new() { Bottom = 1 },
                    BorderInset = new() { Bottom = 0 },
                }
            ),
            new(
                ".stance-button, .food-button",
                new() {
                    Size            = new(48, 48),
                    Padding         = new(4),
                    BackgroundColor = new("Widget.Background"),
                    BorderRadius    = 6,
                    IsAntialiased   = false,
                }
            ),
            new(
                ".stance-button:hover",
                new() {
                    StrokeColor    = new("Window.Border"),
                    StrokeInset    = 2,
                    StrokeWidth    = 1,
                    ImageGrayscale = false,
                }
            ),
            new(
                ".food-button.has-food:hover",
                new() {
                    StrokeColor    = new("Window.Border"),
                    StrokeInset    = 2,
                    StrokeWidth    = 1,
                    ImageGrayscale = false,
                }
            ),
            new(
                ".button--icon",
                new() {
                    Size            = new(40, 40),
                    Padding         = new(4),
                    BorderRadius    = 4,
                    BackgroundColor = new("Input.Background"),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".button--count",
                new() {
                    Anchor       = Anchor.BottomRight,
                    TextAlign    = Anchor.TopRight,
                    Font         = 0,
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    Padding      = new(2),
                }
            ),
            new(
                ".footer",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(325, 0),
                    Padding         = new(15),
                    Gap             = 15,
                    BackgroundColor = new("Toolbar.Background1"),
                    Margin          = new(2) { Top = 0 },
                }
            )
        ]
    );
}
