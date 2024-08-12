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

internal class WidgetStyles
{
    public static Stylesheet DefaultWidgetStylesheet { get; } = new(
        [
            new(
                ".toolbar-widget-default",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 28),
                    Anchor          = Anchor.MiddleLeft,
                    Padding         = new(0, 6),
                    BackgroundColor = new("Widget.Background"),
                    StrokeColor     = new("Widget.Border"),
                    StrokeWidth     = 1,
                    StrokeInset     = 1,
                    BorderRadius    = 5,
                    StrokeRadius    = 4,
                    Gap             = 3,
                }
            ),
            new(
                ".toolbar-widget-default:ghost",
                new() {
                    BackgroundColor = new(0x00000000),
                    BorderColor     = new(new(0x00000000)),
                    StrokeColor     = null,
                    BorderWidth     = new(0),
                    BorderInset     = new(0),
                    StrokeWidth     = 0,
                    Padding         = new(0),
                }
            ),
            new(
                ".toolbar-widget-default:hover",
                new() {
                    Color           = new("Widget.TextHover"),
                    BackgroundColor = new("Widget.BackgroundHover"),
                    StrokeColor     = new("Widget.BorderHover"),
                }
            ),
            new(
                ".toolbar-widget-default:disabled",
                new() {
                    Color           = new("Widget.TextDisabled"),
                    BackgroundColor = new("Widget.BackgroundDisabled"),
                    StrokeColor     = new("Widget.BorderDisabled"),
                }
            ),
            new(
                ".toolbar-widget-default:ghost:hover",
                new() {
                    BackgroundColor = new(0x00000000),
                    BorderColor     = new(new(0x00000000)),
                    StrokeColor     = null,
                }
            ),
            new(
                ".toolbar-widget-default:ghost:disabled",
                new() {
                    BackgroundColor = new(0x00000000),
                    BorderColor     = new(new(0x00000000)),
                    Color           = new("Widget.TextDisabled"),
                }
            ),
            new(
                "LeftIcon",
                new() {
                    Anchor              = Anchor.MiddleLeft,
                    IconId              = 62101,
                    ImageRounding       = 3,
                    ImageRoundedCorners = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
                    Margin              = new() { Left = -2 },
                    IsVisible           = false,
                }
            ),
            new("LeftIcon:ghost", new() { ImageRounding = 0, Margin = new(0) }),
            new(
                "RightIcon",
                new() {
                    Anchor              = Anchor.MiddleLeft,
                    IconId              = 62101,
                    ImageRounding       = 3,
                    ImageRoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
                    Margin              = new() { Right = -2 },
                    IsVisible           = false,
                }
            ),
            new("RightIcon:ghost", new() { ImageRounding = 0, Margin = new(0) }),
            new(
                "Label",
                new() {
                    Flow         = Flow.Vertical,
                    Size         = new(0, 28),
                    Padding      = new(0, 2),
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleCenter,
                    Font         = 0,
                    FontSize     = 13,
                    Color        = new("Widget.Text"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    TextOffset   = new(0, 1),
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                "Label:hover",
                new() {
                    Color = new("Widget.TextHover"),
                }
            ),

            new(
                "Label:disabled",
                new() {
                    Color = new("Widget.TextDisabled"),
                }
            ),
            new(
                "TopLabel",
                new() {
                    Size         = new(0, 14),
                    Margin       = new() { Top = 3 },
                    Anchor       = Anchor.MiddleCenter,
                    TextAlign    = Anchor.BottomRight,
                    Color        = new("Widget.Text"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    IsVisible    = false,
                    FontSize     = 12,
                    Stretch      = true,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                "TopLabel:disabled",
                new() {
                    Color = new("Widget.TextDisabled"),
                }
            ),
            new(
                "BottomLabel",
                new() {
                    Size         = new(0, 12),
                    Margin       = new() { Bottom = 2 },
                    Anchor       = Anchor.MiddleCenter,
                    TextAlign    = Anchor.MiddleRight,
                    Color        = new("Widget.TextMuted"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    IsVisible    = false,
                    FontSize     = 9,
                    Stretch      = true,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                "BottomLabel:disabled",
                new() {
                    Color = new("Widget.TextDisabled"),
                }
            ),
        ]
    );

    public static Stylesheet IconButtonStylesheet = new(
        [
            new(
                ".toolbar-widget-icon",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(28, 28),
                    Anchor          = Anchor.MiddleLeft,
                    Padding         = new(0, 6),
                    BackgroundColor = new("Widget.Background"),
                    StrokeColor     = new("Widget.Border"),
                    StrokeWidth     = 1,
                    StrokeInset     = 1,
                    BorderRadius    = 5,
                    StrokeRadius    = 4,
                    Gap             = 3,
                }
            ),
            new(
                ".toolbar-widget-icon:disabled",
                new() {
                    BackgroundColor = new("Widget.BackgroundDisabled"),
                    StrokeColor     = new("Widget.BorderDisabled"),
                }
            ),
            new(
                ".toolbar-widget-icon:ghost",
                new() {
                    BackgroundColor = new(0x00000000),
                    BorderColor     = new(new(0x00000000)),
                    BorderWidth     = new(0),
                    BorderInset     = new(0),
                    StrokeWidth     = 0,
                    Padding         = new(0),
                }
            ),
            new(
                ".toolbar-widget-icon:ghost:disabled",
                new() {
                    BackgroundColor = new(0x00000000),
                    BorderColor     = new(new(0x00000000)),
                    BorderWidth     = new(0),
                    BorderInset     = new(0),
                    StrokeWidth     = 0,
                    Padding         = new(0),
                    Color           = new("Widget.TextDisabled"),
                    Opacity         = 0.7f,
                }
            ),
            new(
                ".toolbar-widget-icon:hover",
                new() {
                    BackgroundColor = new("Widget.BackgroundHover"),
                    StrokeColor     = new("Widget.BorderHover"),
                }
            ),
            new(
                ".toolbar-widget-icon:ghost:hover",
                new() {
                    BackgroundColor = new(0x00000000),
                    BorderColor     = new(new(0x00000000)),
                }
            ),
            new(
                "Icon",
                new() {
                    Size         = new(26, 26),
                    Anchor       = Anchor.MiddleCenter,
                    TextAlign    = Anchor.MiddleCenter,
                    Font         = 2,
                    FontSize     = 13,
                    TextOffset   = new(0, -1),
                    Color        = new("Widget.Text"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                }
            ),
        ]
    );

    public static Stylesheet ClockWidgetStylesheet { get; } = new(
        [
            new(
                ".clock-widget",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 28),
                    Padding         = new(0, 4),
                    Anchor          = Anchor.MiddleLeft,
                    BackgroundColor = new("Widget.Background"),
                    StrokeColor     = new("Widget.Border"),
                    StrokeWidth     = 1,
                    StrokeInset     = 1,
                    BorderRadius    = 5,
                    StrokeRadius    = 4,
                    Gap             = 6,
                }
            ),
            new(
                ".clock-widget:ghost",
                new() {
                    BackgroundColor = new(0x00000000),
                    BorderColor     = new(new(0x00000000)),
                    BorderWidth     = new(0),
                    BorderInset     = new(0),
                    StrokeWidth     = 0,
                    Padding         = new(0),
                }
            ),
            new(
                ".clock-widget--prefix",
                new() {
                    Size            = new(0, 20),
                    Padding         = new(0, 2),
                    Anchor          = Anchor.MiddleLeft,
                    TextAlign       = Anchor.MiddleCenter,
                    Font            = 4,
                    FontSize        = 12,
                    Color           = new("Widget.TextMuted"),
                    BackgroundColor = new(0x60FFFFFF),
                    BorderRadius    = 2,
                    RoundedCorners  = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
                    Opacity         = 0.66f,
                    TextOffset      = new(0, 1),
                }
            ),
            new(
                ".clock-widget--prefix:native",
                new() {
                    BackgroundColor = new(0),
                    Size            = new(28, 18),
                    Margin          = new() { Left = 2 },
                    Opacity         = 0.75f,
                    Font            = 4,
                    FontSize        = 14,
                    Color           = new("Widget.TextMuted"),
                    OutlineColor    = new("Widget.TextOutline"),
                    OutlineSize     = 1,
                }
            ),
            new(
                ".clock-widget--prefix:ghost",
                new() {
                    BackgroundColor = new(0),
                    RoundedCorners  = RoundedCorners.All,
                }
            ),
            new(
                ".clock-widget--time",
                new() {
                    Size         = new(0, 28),
                    Padding      = new() { Right = 6 },
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleLeft,
                    Font         = 1,
                    FontSize     = 13,
                    Color        = new("Widget.Text"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    TextOffset   = new(0, 0),
                    TextOverflow = false,
                    WordWrap     = false,
                }
            )
        ]
    );
}
