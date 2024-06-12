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

internal partial class AppearanceModule
{
    private static Stylesheet AppearanceModuleStylesheet { get; } = new(
        [
            new(
                ".appearance-header",
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
                ".appearance-subcategory",
                new() {
                    Flow            = Flow.Vertical,
                    Gap             = 10,
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderRadius    = 7,
                    Padding         = new(15),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".appearance-subcategory-header",
                new() {
                    Color     = new("Window.Text"),
                    FontSize  = 16,
                    Anchor    = Anchor.TopLeft,
                    TextAlign = Anchor.TopLeft,
                }
            ),
            new(
                ".appearance-subcategory-description",
                new() {
                    Color        = new("Window.TextMuted"),
                    FontSize     = 12,
                    Anchor       = Anchor.TopLeft,
                    TextAlign    = Anchor.TopLeft,
                    TextOverflow = false,
                    WordWrap     = true,
                    LineHeight   = 1.5f,
                    Padding      = new() { Bottom = 15 },
                }
            ),
            new(
                ".appearance-subcategory-body",
                new() {
                    Anchor = Anchor.TopLeft,
                    Flow   = Flow.Vertical,
                    Gap    = 15,
                }
            ),
            new(
                ".appearance-button-row",
                new() {
                    Flow    = Flow.Horizontal,
                    Gap     = 10,
                    Padding = new() { Bottom = 15 },
                }
            )
        ]
    );
}
