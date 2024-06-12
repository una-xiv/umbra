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

using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal sealed partial class MarkersModule
{
    private static Stylesheet MarkersModuleStylesheet { get; } = new(
        [
            new(
                ".markers",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 15,
                    Padding = new(15),
                }
            ),
            new(
                ".markers-header",
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
                }
            ),
            new(
                ".markers-list",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 15,
                }
            ),
            new(
                ".marker",
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
                ".marker-header",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                ".marker-header-chevron",
                new() {
                    Font       = 2,
                    FontSize   = 16,
                    Size       = new(20, 20),
                    Color      = new("Window.Text"),
                    TextAlign  = Anchor.MiddleLeft,
                    TextOffset = new(0, -1),
                    Opacity    = 0.65f,
                }
            ),
            new(
                ".marker-header-chevron:hover",
                new() {
                    Color   = new("Window.TextLight"),
                    Opacity = 0.75f,
                }
            ),
            new(
                ".marker-header-text",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 2,
                }
            ),
            new(
                ".marker-header-text-name",
                new() {
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    FontSize     = 16,
                    TextAlign    = Anchor.MiddleLeft,
                    Padding      = new() { Bottom = 4 },
                }
            ),
            new(
                ".marker-header-text-name:hover",
                new() {
                    Color        = new("Window.TextLight"),
                    OutlineColor = new("Window.TextOutline"),
                }
            ),
            new(
                ".marker-header-text-desc",
                new() {
                    Color        = new("Window.TextMuted"),
                    FontSize     = 12,
                    TextAlign    = Anchor.MiddleLeft,
                    TextOverflow = false,
                    WordWrap     = true,
                }
            ),
            new(
                ".marker-body",
                new() {
                    Flow      = Flow.Vertical,
                    Padding   = new() { Left = 26 },
                    Gap       = 10,
                    IsVisible = false,
                }
            ),
        ]
    );
}
