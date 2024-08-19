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

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow
{
    private static Stylesheet SettingsStylesheet { get; } = new(
        [
            new(
                "SettingsWindow",
                new() {
                    Anchor = Anchor.TopLeft,
                    Flow   = Flow.Vertical,
                }
            ),
            new(
                "NavigationPanel",
                new() {
                    Flow            = Flow.Vertical,
                    Anchor          = Anchor.TopLeft,
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderColor     = new() { Right = new("Window.Border") },
                    BorderWidth     = new() { Right = 1 },
                    BorderInset     = new() { Right = 0 },
                }
            ),
            new(
                "ContentPanel",
                new() {
                    Flow                      = Flow.Vertical,
                    Anchor                    = Anchor.TopRight,
                    BackgroundColor           = new("Window.Background"),
                    ScrollbarTrackColor       = new("Window.ScrollbarTrack"),
                    ScrollbarThumbColor       = new("Window.ScrollbarThumb"),
                    ScrollbarThumbHoverColor  = new("Window.ScrollbarThumbHover"),
                    ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
                }
            ),
            new(
                "FooterPanel",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 40),
                    Stretch         = true,
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderColor     = new() { Top = new("Window.Border") },
                    BorderWidth     = new() { Top = 1 },
                    BorderInset     = new() { Top = 0 },
                }
            ),
            new(
                "NavigationList",
                new() {
                    Anchor                    = Anchor.TopLeft,
                    Flow                      = Flow.Vertical,
                    Size                      = new(150, 0),
                    ScrollbarTrackColor       = new(0),
                    ScrollbarThumbColor       = new("Window.ScrollbarThumb"),
                    ScrollbarThumbHoverColor  = new("Window.ScrollbarThumbHover"),
                    ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
                }
            ),
            new(
                "ModuleButtons",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 8,
                    Padding = new(15),
                }
            ),
            new(
                "Logo",
                new() {
                    Anchor          = Anchor.TopLeft,
                    Size            = new(100, 100),
                    Margin          = new(15),
                    BackgroundColor = new("Window.Background"),
                    ImageBytes      = LogoTexture,
                    BorderRadius    = 8,
                }
            ),
            new(
                "FooterText",
                new() {
                    Anchor    = Anchor.MiddleLeft,
                    Size      = new(0, 24),
                    TextAlign = Anchor.MiddleCenter,
                    Color     = new("Window.TextMuted"),
                    FontSize  = 11,
                    Padding   = new(0, 15),
                }
            ),
            new(
                "Buttons",
                new() {
                    Anchor  = Anchor.MiddleRight,
                    Padding = new(0, 15),
                    Gap     = 10,
                }
            ),
            new(
                ".module-button",
                new() {
                    Size            = new(0, 28),
                    Padding         = new() { Right = 15 },
                    Color           = new("Window.Text"),
                    BackgroundColor = new("Window.BackgroundLight"),
                    IsAntialiased   = false,
                    Stretch         = true,
                    TextAlign       = Anchor.MiddleRight,
                    FontSize        = 13,
                    TextOverflow    = false,
                    OutlineColor    = new("Window.TextOutline"),
                    OutlineSize     = 1,
                    WordWrap        = false,
                    BorderRadius    = 7,
                }
            ),
            new(
                ".module-button.active",
                new() {
                    BackgroundColor = new("Window.Background"),
                    Color           = new("Window.TextLight"),
                }
            ),
            new(
                ".module-button:hover",
                new() {
                    Color = new("Window.TextLight")
                }
            )
        ]
    );
}
