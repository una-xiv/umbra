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
        new() {
            {
                ".toolbar-widget-default",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 28),
                    Anchor          = Anchor.MiddleLeft,
                    Padding         = new(0, 6),
                    BackgroundColor = new("Widget.Background"),
                    StrokeColor     = new("Widget.Border"),
                    StrokeWidth     = 1,
                    StrokeInset     = 2,
                    BorderRadius    = 5,
                    StrokeRadius    = 4,
                    Gap             = 3,
                }
            }, {
                ".toolbar-widget-default:ghost",
                new() {
                    BackgroundColor = new(0x00000000),
                    BorderColor     = new(new(0x00000000)),
                    BorderWidth     = new(0),
                    BorderInset     = new(0),
                    StrokeWidth     = 0,
                }
            }, {
                ".toolbar-widget-default:hover",
                new() {
                    Color           = new("Widget.TextHover"),
                    BackgroundColor = new("Widget.BackgroundHover"),
                    StrokeColor     = new("Widget.BorderHover"),
                }
            }, {
                ".toolbar-widget-default:ghost:hover",
                new() {
                    BackgroundColor = new(0x00000000),
                    BorderColor     = new(new(0x00000000)),
                }
            }, {
                "LeftIcon",
                new() {
                    Size               = new(20, 20),
                    Anchor             = Anchor.MiddleLeft,
                    IconId             = 62101,
                    IconRounding       = 3,
                    IconRoundedCorners = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
                    Margin             = new() { Left = -2 },
                    IsVisible          = false,
                }
            }, {
                "RightIcon",
                new() {
                    Size               = new(20, 20),
                    Anchor             = Anchor.MiddleLeft,
                    IconId             = 62101,
                    IconRounding       = 3,
                    IconRoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
                    Margin             = new() { Right = -2 },
                    IsVisible          = false,
                }
            }, {
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
                }
            }, {
                "TopLabel",
                new() {
                    Size      = new(0, 12),
                    Margin    = new() { Top = 1 },
                    Anchor    = Anchor.MiddleCenter,
                    TextAlign = Anchor.MiddleRight,
                    Color     = new("Widget.Text"),
                    IsVisible = false,
                    FontSize  = 11,
                    Stretch   = true,
                }
            }, {
                "BottomLabel",
                new() {
                    Size       = new(0, 12),
                    Margin     = new() { Top = -3 },
                    Anchor     = Anchor.MiddleCenter,
                    TextAlign  = Anchor.MiddleRight,
                    TextOffset = new(0, -1),
                    Color      = new("Widget.TextMuted"),
                    IsVisible  = false,
                    FontSize   = 10,
                    Stretch    = true,
                }
            }
        }
    );
}
