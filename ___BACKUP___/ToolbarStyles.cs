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

internal class ToolbarStyles
{
    public static Stylesheet ToolbarStylesheet { get; } = new(
        [
            new(
                ".toolbar",
                new() {
                    Flow          = Flow.Horizontal,
                    Size          = new(0, 32),
                    Padding       = new(0, 6),
                    BorderColor   = new(new("Toolbar.Border")),
                    ShadowSize    = new(64),
                    ShadowInset   = 1,
                    IsAntialiased = false,
                }
            ),
            new(
                ".toolbar:top, .toolbar:aux",
                new() {
                    Anchor             = Anchor.TopCenter,
                    BackgroundGradient = GradientColor.Vertical(new("Toolbar.Background2"), new("Toolbar.Background1")),
                }
            ),
            new(
                ".toolbar:bottom",
                new() {
                    Anchor             = Anchor.BottomCenter,
                    BackgroundGradient = GradientColor.Vertical(new("Toolbar.Background1"), new("Toolbar.Background2")),
                }
            ),
            new(
                ".toolbar:top:blur",
                new() {
                    BorderColor = new(new("Toolbar.InactiveBorder")),
                    BackgroundGradient = GradientColor.Vertical(
                        new("Toolbar.InactiveBackground2"),
                        new("Toolbar.InactiveBackground1")
                    ),
                }
            ),
            new(
                ".toolbar:bottom:blur",
                new() {
                    BorderColor = new(new("Toolbar.InactiveBorder")),
                    BackgroundGradient = GradientColor.Vertical(
                        new("Toolbar.InactiveBackground1"),
                        new("Toolbar.InactiveBackground2")
                    ),
                }
            ),
            new(
                ".toolbar:top:stretched",
                new() {
                    BorderWidth = new() { Bottom = 1 },
                    BorderInset = new(0),
                }
            ),
            new(
                ".toolbar:bottom:stretched",
                new() {
                    BorderWidth = new() { Top = 1 },
                    BorderInset = new(0),
                }
            ),
            new(
                ".toolbar:top:floating",
                new() {
                    BorderWidth    = new() { Bottom = 2, Left = 2, Right = 2 },
                    BorderRadius   = 5,
                    RoundedCorners = RoundedCorners.BottomLeft | RoundedCorners.BottomRight,
                    IsAntialiased  = true,
                }
            ),
            new(
                ".toolbar:bottom:floating",
                new() {
                    BorderWidth    = new() { Top = 1, Left = 1, Right = 1 },
                    BorderRadius   = 5,
                    RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.TopRight,
                    IsAntialiased  = true,
                }
            ),
            new(
                ".toolbar:aux:floating",
                new() {
                    BorderWidth    = new(1),
                    BorderRadius   = 5,
                    RoundedCorners = RoundedCorners.All,
                    IsAntialiased  = true,
                }
            ),
            new(
                ".toolbar-panel",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 6,
                }
            ),
            new(
                "#AuxBar",
                new() {
                    Anchor         = Anchor.TopCenter,
                    RoundedCorners = RoundedCorners.All,
                }
            ),
            new("#AuxBar.left-aligned", new() { Anchor = Anchor.TopLeft }),
            new("#AuxBar.right-aligned", new() { Anchor = Anchor.TopRight }),
            new("#AuxBar.center-aligned", new() { Anchor = Anchor.TopCenter }),
        ]
    );
}
