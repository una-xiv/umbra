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

namespace Umbra.Windows;

public static class WindowStyles
{
    public static readonly Stylesheet WindowStylesheet = new(
        [
            new(
                ".window",
                new() {
                    Anchor          = Anchor.TopLeft,
                    Flow            = Flow.Vertical,
                    BackgroundColor = new("Window.Background"),
                    StrokeColor     = new("Window.Border"),
                    StrokeWidth     = 1,
                    StrokeInset     = 1,
                    BorderRadius    = 6,
                    IsAntialiased   = false,
                    RoundedCorners  = RoundedCorners.All,
                    ShadowSize      = new(64),
                    ShadowInset     = 8,
                    Padding         = new(2),
                }
            ),
            new(
                ".window--titlebar",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 32),
                    Color           = new("Window.TitlebarText"),
                    BackgroundColor = new("Window.TitlebarBackground"),
                    BackgroundGradient = GradientColor.Vertical(
                        new("Window.TitlebarGradient1"),
                        new("Window.TitlebarGradient2")
                    ),
                    BackgroundGradientInset = new(0) { Bottom = 0 },
                    BorderColor             = new(new("Window.TitlebarBorder")),
                    BorderWidth             = new() { Bottom = 1 },
                    BorderRadius            = 4,
                    IsAntialiased           = false,
                    RoundedCorners          = RoundedCorners.TopLeft | RoundedCorners.TopRight,
                    Margin                  = new(1) { Right = -1, Bottom = -1 },
                }
            ),
            new(
                ".window--titlebar-text",
                new() {
                    FontSize     = 13,
                    Color        = new("Window.TitlebarText"),
                    OutlineColor = new("Window.TitlebarTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.MiddleLeft,
                    TextOffset   = new(0, -1),
                    TextOverflow = false,
                    WordWrap     = false,
                    Size         = new(0, 32),
                    Padding      = new(0, 6)
                }
            ),
            new(
                ".window--titlebar-button",
                new() {
                    Anchor          = Anchor.TopRight,
                    Size            = new(22, 22),
                    BackgroundColor = new("Window.TitlebarCloseButton"),
                    StrokeColor     = new("Window.TitlebarCloseButtonBorder"),
                    StrokeWidth     = 1,
                    StrokeInset     = 0,
                    BorderRadius    = 3,
                    TextAlign       = Anchor.MiddleCenter,
                    Font            = 2,
                    FontSize        = 12,
                    Color           = new("Window.TitlebarCloseButtonX"),
                    OutlineColor    = new("Window.TitlebarCloseButtonXOutline"),
                    TextOverflow    = true,
                    Margin          = new() { Top = 2, Right = 4 },
                    IsAntialiased   = false,
                }
            ),
            new(
                ".window--titlebar-button.close-button:hover",
                new() {
                    BackgroundColor = new("Window.TitlebarCloseButtonHover"),
                    Color           = new("Window.TitlebarCloseButtonXHover"),
                    StrokeWidth     = 2,
                }
            ),
            new(
                ".window--content",
                new() {
                    Anchor                    = Anchor.TopLeft,
                    Flow                      = Flow.Vertical,
                    BorderRadius              = 5,
                    RoundedCorners            = RoundedCorners.BottomLeft | RoundedCorners.BottomRight,
                    ScrollbarTrackColor       = new("Window.ScrollbarTrack"),
                    ScrollbarThumbColor       = new("Window.ScrollbarThumb"),
                    ScrollbarThumbHoverColor  = new("Window.ScrollbarThumbHover"),
                    ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
                }
            )
        ]
    );

    public static Stylesheet ItemPickerStylesheet { get; } = new(
        [
            new("#ItemPickerWindow", new() { Flow = Flow.Vertical }),
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
            new(
                "#SearchInputWrapper",
                new() {
                    Flow = Flow.Horizontal,
                    Size = new(0, 30),
                }
            ),
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
                "#ItemList",
                new() {
                    Flow = Flow.Vertical,
                }
            ),
            new(
                "#ItemListWrapper",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 10,
                    Padding = new(10),
                }
            ),
            new(
                ".item",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 42),
                    Padding         = new(5),
                    Gap             = 10,
                    IsAntialiased   = false,
                    BackgroundColor = new("Input.Background"),
                }
            ),
            new(
                ".item:hover",
                new() {
                    BackgroundColor = new("Input.BackgroundHover"),
                    StrokeColor     = new("Input.Border"),
                    StrokeWidth     = 1,
                }
            ),
            new(
                ".item-icon",
                new() {
                    Size = new(32, 32),
                }
            ),
            new(
                ".item-body",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 2,
                }
            ),
            new(
                ".item-name",
                new() {
                    FontSize     = 13,
                    Color        = new("Input.Text"),
                    OutlineColor = new("Input.TextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".item-name:hover",
                new() {
                    Color        = new("Input.TextHover"),
                    OutlineColor = new("Input.TextOutlineHover"),
                }
            ),
            new(
                ".item-command",
                new() {
                    Font         = 0,
                    FontSize     = 12,
                    Color        = new("Window.TextMuted"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            )
        ]
    );
}
