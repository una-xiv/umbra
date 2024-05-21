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

public partial class CvarModule
{
    private static Stylesheet CvarModuleStylesheet { get; } = new(
        [
            new(
                ".cvar-header",
                new() {
                    FontSize    = 24,
                    TextAlign   = Anchor.MiddleLeft,
                    Anchor      = Anchor.TopLeft,
                    Padding     = new() { Bottom = 15 },
                    Margin      = new() { Bottom = 5 },
                    BorderColor = new() { Bottom = new("Window.AccentColor") },
                    BorderWidth = new() { Bottom = 1 },
                }
            ),
            new(
                ".cvar-list",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 15,
                }
            ),
            new(
                ".cvar-subcategory",
                new() {
                    Flow            = Flow.Vertical,
                    Gap             = 10,
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderRadius    = 7,
                    Padding         = new(10),
                }
            ),
            new(
                ".cvar-subcategory-header",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                ".cvar-subcategory--chevron",
                new() {
                    Font       = 2,
                    FontSize   = 16,
                    Anchor     = Anchor.MiddleLeft,
                    TextAlign  = Anchor.MiddleLeft,
                    TextOffset = new(0, -1),
                }
            ),
            new(
                ".cvar-subcategory--label",
                new() {
                    Color     = new("Window.Text"),
                    FontSize  = 16,
                    Anchor    = Anchor.MiddleLeft,
                    TextAlign = Anchor.MiddleLeft,
                    Padding   = new() { Right = 100 },
                }
            ),
            new(
                ".cvar-subcategory--label:hover",
                new() {
                    Color = new("Window.TextLight"),
                }
            ),
            new(
                ".cvar-list.in-subcategory",
                new() {
                    Padding = new() { Top = 15 }
                }
            )
        ]
    );
}
