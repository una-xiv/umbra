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

namespace Umbra.Windows.Library.WidgetConfig;

internal partial class WidgetConfigWindow
{
    private static Stylesheet WidgetConfigWindowStylesheet { get; } = new(
        [
            new(
                ".widget-config-window",
                new() {
                    Flow = Flow.Vertical,
                }
            ),
            new(
                "#SearchPanel",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 45),
                    FontSize        = 16,
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderColor     = new() { Bottom = new("Window.Border") },
                    BorderWidth     = new() { Bottom = 1 },
                    IsAntialiased   = false,
                    Padding         = new(10, 15),
                    Gap             = 5,
                }
            ),
            new("#SearchInputWrapper", new() {
                Flow = Flow.Horizontal,
                Size = new(0, 30),
            }),
            new(
                "#SearchIcon",
                new() {
                    Size         = new(26, 26),
                    Font         = 2,
                    FontSize     = 18,
                    Color        = new("Window.TextMuted"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.MiddleLeft,
                    TextOffset   = new(0, -1),
                }
            ),
            new(
                ".widget-config-list--wrapper",
                new() {
                    Flow                      = Flow.Vertical,
                    ScrollbarTrackColor       = new(0),
                    ScrollbarThumbColor       = new("Window.ScrollbarThumb"),
                    ScrollbarThumbHoverColor  = new("Window.ScrollbarThumbHover"),
                    ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
                }
            ),
            new(
                ".widget-config-list",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 15,
                    Padding = new(15),
                }
            ),
            new(
                ".widget-config-footer",
                new() {
                    Flow            = Flow.Horizontal,
                    Gap             = 15,
                    Padding         = new(0, 15),
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderWidth     = new() { Top = 1 },
                    BorderColor     = new() { Top = new("Window.Border") },
                    IsAntialiased   = false,
                }
            ),
            new(
                ".widget-config-footer--buttons",
                new() {
                    Anchor = Anchor.MiddleRight,
                    Gap    = 15,
                }
            ),
            new(
                ".widget-config-footer--buttons.left-side",
                new() {
                    Anchor = Anchor.MiddleLeft,
                }
            ),
            new(
                ".widget-config-category",
                new() {
                    Flow            = Flow.Vertical,
                    Gap             = 10,
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderRadius    = 7,
                    Padding         = new(10),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".widget-config-category-header",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                ".widget-config-category--chevron",
                new() {
                    Font       = 2,
                    FontSize   = 16,
                    Padding    = new(2, 0),
                    Color      = new("Window.Text"),
                    Anchor     = Anchor.MiddleLeft,
                    TextAlign  = Anchor.MiddleLeft,
                    TextOffset = new(0, -1),
                    Opacity    = 0.65f,
                }
            ),
            new(
                ".widget-config-category--chevron:hover",
                new() {
                    Color   = new("Window.TextLight"),
                    Opacity = 0.75f,
                }
            ),
            new(
                ".widget-config-category--label",
                new() {
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    FontSize     = 16,
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleLeft,
                    Padding      = new() { Right = 100 },
                }
            ),
            new(
                ".widget-config-category--label:hover",
                new() {
                    Color = new("Window.TextLight"),
                }
            ),
            new(
                ".widget-config-category--content",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 15,
                    Padding = new() { Top = 15, Right = 15 }
                }
            )
        ]
    );
}
