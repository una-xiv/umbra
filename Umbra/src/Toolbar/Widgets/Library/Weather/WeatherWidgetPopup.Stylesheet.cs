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

public partial class WeatherWidgetPopup
{
    private static Stylesheet PopupStylesheet { get; } = new(
        [
            new(
                ".popup",
                new() {
                    Flow = Flow.Vertical,
                }
            ),
            new(
                ".popup-gradient",
                new() {
                    Anchor                  = Anchor.TopRight,
                    BackgroundGradient      = GradientColor.Vertical(new(0xA0FF7070), new(0)),
                    BackgroundGradientInset = new(4) { Top = 0 },
                    BorderRadius            = 5,
                    RoundedCorners          = RoundedCorners.BottomLeft | RoundedCorners.BottomRight,
                    Margin                  = new() { Top = 52 },
                }
            ),
            new(
                ".header",
                new() {
                    Flow                    = Flow.Horizontal,
                    Size                    = new(0, 70),
                    Padding                 = new(15) { Left = 10, Top = -15 },
                    Gap                     = 15,
                    BackgroundGradient      = GradientColor.Vertical(new(0), new(0xA0FF7070)),
                    BackgroundGradientInset = new(4) { Bottom = 18 },
                    BorderRadius            = 5,
                    RoundedCorners          = RoundedCorners.TopLeft | RoundedCorners.TopRight,
                    Stretch                 = true,
                }
            ),
            new(
                ".header-icon",
                new() {
                    Anchor = Anchor.MiddleLeft,
                    Size   = new(64),
                    IconId = 60271u,
                }
            ),
            new(
                ".header-text",
                new() {
                    Anchor  = Anchor.MiddleLeft,
                    Flow    = Flow.Vertical,
                    Gap     = 4,
                    Padding = new() { Right = 15 }
                }
            ),
            new(
                ".header-text--title",
                new() {
                    Anchor       = Anchor.TopLeft,
                    Font         = 0,
                    FontSize     = 18,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.TopLeft,
                }
            ),
            new(
                ".header-text--subtitle",
                new() {
                    Anchor       = Anchor.TopLeft,
                    Font         = 0,
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.TopLeft,
                }
            ),
            new(
                ".body",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new(0, 15) { Bottom = 10 },
                    Gap     = 10,
                }
            ),
            new(
                ".forecast-item",
                new() {
                    Anchor         = Anchor.TopLeft,
                    Flow           = Flow.Horizontal,
                    Size           = new(0, 40),
                    Padding        = new(10),
                    Gap            = 10,
                    BorderRadius   = 5,
                    RoundedCorners = RoundedCorners.All,
                }
            ),
            new(
                ".forecast-item--icon",
                new() {
                    Anchor = Anchor.MiddleLeft,
                    Size   = new(32),
                    IconId = 60277u,
                }
            ),
            new(
                ".forecast-item--text",
                new() {
                    Anchor = Anchor.MiddleLeft,
                    Flow   = Flow.Vertical,
                    Size   = new(0, 32),
                }
            ),
            new(
                ".forecast-item--text--name",
                new() {
                    Anchor       = Anchor.TopLeft,
                    Font         = 0,
                    FontSize     = 14,
                    Size         = new(0, 16),
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.TopLeft,
                    Margin       = new() { Top = 6 },
                }
            ),
            new(
                ".forecast-item--text--time",
                new() {
                    Anchor       = Anchor.TopLeft,
                    Font         = 0,
                    FontSize     = 12,
                    Size         = new(0, 16),
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.TopLeft,
                    Margin       = new() { Top = -3 },
                }
            )
        ]
    );
}
