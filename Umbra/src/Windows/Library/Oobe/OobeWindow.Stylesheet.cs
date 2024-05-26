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

namespace Umbra.Windows.Oobe;

internal partial class OobeWindow
{
    private static Stylesheet OobeWindowStylesheet { get; } = new(
        [
            new(
                ".oobe",
                new() {
                    Flow = Flow.Vertical,
                    Size = new(712, 458),
                }
            ),
            new(
                ".oobe-header",
                new() {
                    Flow               = Flow.Horizontal,
                    Size               = new(712, 94),
                    BackgroundColor    = new("Window.BackgroundLight"),
                    BorderColor        = new(new("Window.AccentColor")),
                    BorderWidth        = new() { Bottom = 1 },
                    BackgroundGradient = GradientColor.Vertical(new(0xFF3C3C3C), new(0xFF2E2E2E))
                }
            ),
            new(
                ".oobe-header-logo",
                new() {
                    Anchor          = Anchor.TopLeft,
                    Size            = new(93, 92),
                    BackgroundColor = new("Input.Background"),
                }
            ),
            new(
                ".oobe-header-text",
                new() {
                    Anchor = Anchor.TopLeft,
                    Flow   = Flow.Vertical,
                    Size   = new(0, 48),
                }
            ),
            new(
                ".oobe-header-text--title",
                new() {
                    FontSize        = 26,
                    Color           = new("Window.Text"),
                    OutlineColor    = new("Window.TextOutline"),
                    OutlineSize     = 1,
                    TextShadowSize  = 4,
                    TextShadowColor = new(0xA0000000),
                    Padding         = new(15) { Top = 24, Bottom = 4 },
                }
            ),
            new(
                ".oobe-header-text--subtitle",
                new() {
                    FontSize     = 14,
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    Opacity      = 0.75f,
                    Padding      = new(15) { Top = 0 },
                }
            ),
            new(
                ".oobe-body",
                new() {
                    Flow = Flow.Vertical,
                    Size = new(712, 306),
                }
            ),
            new(
                ".oobe-footer",
                new() {
                    Flow               = Flow.Horizontal,
                    Size               = new(712, 60),
                    BackgroundColor    = new("Window.BackgroundLight"),
                    BorderColor        = new(new("Window.Border")),
                    BorderWidth        = new() { Top = 1 },
                    Padding            = new(15),
                    Gap                = 15,
                    BackgroundGradient = GradientColor.Vertical(new(0xFF3C3C3C), new(0xFF2E2E2E))
                }
            ),
            new(
                ".oobe-footer--left-group",
                new() {
                    Anchor = Anchor.MiddleLeft,
                    Flow   = Flow.Horizontal,
                    Gap    = 15,
                }
            ),
            new(
                ".oobe-footer--right-group",
                new() {
                    Anchor = Anchor.MiddleRight,
                    Flow   = Flow.Horizontal,
                    Gap    = 15,
                }
            ),
            new(
                ".oobe-step-left-image",
                new() {
                    Anchor = Anchor.MiddleCenter,
                    Flow   = Flow.Horizontal,
                    Gap    = 32,
                }
            ),
            new(
                ".oobe-step",
                new() {
                    Flow = Flow.Horizontal,
                    Size = new(712, 306),
                    Gap  = 15,
                }
            ),
            new(
                ".oobe-step-left-image",
                new() {
                    Anchor       = Anchor.MiddleLeft,
                    Flow         = Flow.Horizontal,
                    Font         = 2,
                    FontSize     = 100,
                    Color        = new("Window.AccentColor"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 4,
                    TextAlign    = Anchor.MiddleCenter,
                    Size         = new(178, 306),
                }
            ),
            new(
                ".oobe-step-right-content",
                new() {
                    Anchor = Anchor.MiddleLeft,
                    Flow   = Flow.Vertical,
                    Gap    = 15,
                    Size   = new(478, 0),
                }
            ),
            new(
                ".oobe-step-right-content--text-title",
                new() {
                    Anchor       = Anchor.TopLeft,
                    TextAlign    = Anchor.MiddleLeft,
                    Flow         = Flow.Vertical,
                    Gap          = 10,
                    Size         = new(478, 0),
                    TextOverflow = false,
                    TextOffset   = new(0, -1),
                    WordWrap     = false,
                    FontSize     = 18,
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".oobe-step-right-content--text-content",
                new() {
                    Anchor       = Anchor.TopLeft,
                    TextAlign    = Anchor.MiddleLeft,
                    Flow         = Flow.Vertical,
                    Gap          = 10,
                    Size         = new(478, 0),
                    TextOverflow = false,
                    WordWrap     = true,
                    FontSize     = 13,
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    LineHeight   = 1.5f,
                    Padding      = new(4, 0),
                }
            ),
        ]
    );
}
