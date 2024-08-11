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

namespace Umbra.Windows.Settings.Modules;

internal partial class WidgetsModule
{
    internal static Stylesheet WidgetsModuleStylesheet { get; } = new(
        [
            new(
                ".widgets-module",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new(15),
                    Gap     = 10,
                }
            ),
            new(
                ".module-header",
                new() {
                    FontSize     = 24,
                    TextAlign    = Anchor.MiddleLeft,
                    Anchor       = Anchor.TopLeft,
                    Padding      = new() { Bottom = 15 },
                    Margin       = new() { Bottom = 5 },
                    BorderColor  = new() { Bottom = new("Window.AccentColor") },
                    BorderWidth  = new() { Bottom = 1 },
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    Gap          = 8,
                }
            ),
            new(
                ".module-header--profile-name",
                new() {
                    Anchor       = Anchor.BottomRight,
                    FontSize     = 13,
                    Color        = new("Window.TextMuted"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    Margin       = new() { Bottom = -4, Left = -2 },
                }
            ),
            new(
                ".widgets-column-wrapper",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 15,
                }
            ),
            new(
                ".widgets-column",
                new() {
                    Flow            = Flow.Vertical,
                    Gap             = 15,
                    Padding         = new(15),
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderRadius    = 7,
                    IsAntialiased   = false,
                }
            ),
            new(
                ".widgets-column--header",
                new() {
                    FontSize  = 16,
                    Color     = new("Window.Text"),
                    TextAlign = Anchor.TopCenter,
                    Size      = new(0, 30),
                }
            ),
            new(
                ".widgets-column--list-wrapper",
                new() {
                    Flow                      = Flow.Vertical,
                    ScrollbarTrackColor       = new(0),
                    ScrollbarThumbColor       = new("Window.ScrollbarThumb"),
                    ScrollbarThumbHoverColor  = new("Window.ScrollbarThumbHover"),
                    ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
                }
            ),
            new(
                ".widgets-column--list",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 15,
                }
            ),
            new(
                ".widgets-column--add-new",
                new() {
                    Flow          = Flow.Vertical,
                    Gap           = 15,
                    Size          = new(0, 30),
                    BorderColor   = new() { Top = new("Window.Border") },
                    BorderWidth   = new() { Top = 1 },
                    IsAntialiased = false,
                }
            ),
            new(
                ".widgets-column--add-new--label",
                new() {
                    Anchor       = Anchor.MiddleCenter,
                    TextAlign    = Anchor.TopCenter,
                    Margin       = new(8, 0),
                    Stretch      = true,
                    FontSize     = 12,
                    Color        = new("Window.TextMuted"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".widgets-column--add-new--label:hover",
                new() {
                    Color = new("Window.TextLight"),
                }
            ),
            new(
                ".widget-instance",
                new() {
                    Flow            = Flow.Vertical,
                    Gap             = 6,
                    BackgroundColor = new("Window.Background"),
                    IsAntialiased   = false,
                    BorderRadius    = 6,
                    Padding         = new(8, 0),
                }
            ),
            new(
                ".widget-instance--name",
                new() {
                    Anchor       = Anchor.TopCenter,
                    FontSize     = 13,
                    Color        = new("Window.Text"),
                    TextAlign    = Anchor.TopCenter,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".widget-instance--name:hover",
                new() {
                    Color = new("Widget.TextHover"),
                }
            ),
            new(
                ".widget-instance--controls",
                new() {
                    Anchor = Anchor.TopCenter,
                }
            ),
            new(
                ".widget-instance--controls--buttons",
                new() {
                    Anchor = Anchor.TopCenter,
                    Gap    = 3,
                }
            )
        ]
    );
}
