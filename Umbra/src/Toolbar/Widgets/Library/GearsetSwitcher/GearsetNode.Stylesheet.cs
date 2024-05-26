using Una.Drawing;

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

namespace Umbra.Widgets;

public partial class GearsetNode
{
    private static Stylesheet GearsetSwitcherItemStylesheet { get; } = new(
        [
            new(
                ".gearset",
                new() {
                    Size                    = new(NodeWidth, NodeHeight),
                    IsAntialiased           = false,
                    BackgroundColor         = new("Input.Background"),
                    BorderRadius            = 7,
                    Padding                 = new(5),
                    StrokeWidth             = 1,
                    BackgroundGradient      = GradientColor.Vertical(new(0), new(0)),
                    BackgroundGradientInset = new(2),
                }
            ),
            new(
                ".gearset:current",
                new() {
                    StrokeColor = new("Input.TextMuted"),
                    StrokeWidth = 1,
                }
            ),
            new(
                ".gearset:hover",
                new() {
                    BackgroundColor = new("Input.BackgroundHover"),
                    StrokeColor     = new("Input.BorderHover")
                }
            ),
            new(
                ".gearset--icon",
                new() {
                    Size            = new(30, 30),
                    ImageInset      = new(2),
                    BorderRadius    = 6,
                }
            ),
            new(
                ".gearset--body",
                new() {
                    Flow    = Flow.Vertical,
                    Size    = new(NodeWidth - 30 - 60),
                    Padding = new() { Left = 6 },
                    Gap     = 4,
                }
            ),
            new(
                ".gearset--body--name",
                new() {
                    Font         = 0,
                    FontSize     = 13,
                    Color        = new("Input.Text"),
                    Size         = new(NodeWidth - 30 - 60, 0),
                    TextOverflow = false,
                    WordWrap     = false,
                    Padding      = new() { Bottom = 2 },
                }
            ),
            new(
                ".gearset--body--info",
                new() {
                    Font         = 0,
                    FontSize     = 11,
                    Color        = new("Input.Text"),
                    Size         = new(NodeWidth - 30 - 60, 0),
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".gearset--ilvl",
                new() {
                    Size      = new(50, NodeHeight - 10),
                    TextAlign = Anchor.MiddleRight,
                    Color     = new("Widget.PopupMenuTextMuted"),
                    FontSize  = 20,
                }
            )
        ]
    );
}
