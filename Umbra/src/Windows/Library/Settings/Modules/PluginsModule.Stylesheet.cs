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

internal partial class PluginsModule
{
    private static Stylesheet PluginsModuleStylesheet { get; } = new(
        [
            new(
                ".plugins",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 15,
                    Padding = new(15),
                }
            ),
            new(
                ".plugins-header",
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
                ".plugins-description",
                new() {
                    TextOverflow = false,
                    WordWrap     = true,
                    FontSize     = 13,
                    Color        = new("Window.Text"),
                    LineHeight   = 1.5f,
                }
            ),
            new(
                ".plugins-list",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 10,
                }
            ),
            new(
                ".plugins-footer",
                new() {
                    Flow        = Flow.Vertical,
                    Padding     = new(10, 0),
                    BorderColor = new() { Top = new("Window.Border") },
                    BorderWidth = new() { Top = 2 },
                }
            ),
            new(
                ".plugins-changed",
                new() {
                    Flow            = Flow.Vertical,
                    Padding         = new(10),
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderRadius    = 7,
                    IsAntialiased   = false,
                    Margin          = new() { Top = 10 },
                    Gap             = 10,
                    IsVisible       = false,
                }
            ),
            new(
                ".plugins-changed-text",
                new() {
                    FontSize     = 13,
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".plugin",
                new() {
                    BackgroundColor = new("Window.BackgroundLight"),
                    Padding         = new(10),
                    BorderRadius    = 7,
                    IsAntialiased   = false,
                    Flow            = Flow.Vertical,
                    Gap             = 10,
                }
            ),
            new(
                ".plugin-name",
                new() {
                    FontSize     = 16,
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".plugin-load-error",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 5,
                }
            ),
            new(
                ".plugin-load-error-icon",
                new() {
                    Font         = 2,
                    FontSize     = 16,
                    Size         = new(22, 22),
                    TextAlign    = Anchor.TopLeft,
                    TextOffset   = new(0, -3),
                    Color        = new("Window.AccentColor"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".plugin-load-error-message",
                new() {
                    FontSize     = 12,
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = true,
                }
            ),
            new(
                ".plugin-buttons",
                new() {
                    Anchor = Anchor.TopRight,
                    Flow   = Flow.Horizontal,
                    Gap    = 10,
                }
            ),
            new(
                ".plugins-changed",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 10,
                }
            )
        ]
    );
}
