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

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                "#Popup",
                new() {
                    Flow = Flow.Horizontal,
                }
            ),
            new(
                "#ExpansionList",
                new() {
                    Flow = Flow.Horizontal,
                }
            ),
            new(
                ".expansion",
                new() {
                    Padding = new(8),
                    Size    = new(ColumnWidth + 16, 0),
                    Flow    = Flow.Vertical,
                }
            ),
            new(
                ".expansion-header",
                new() {
                    Size          = new(ColumnWidth, 28),
                    Flow          = Flow.Horizontal,
                    FontSize      = 18,
                    TextAlign     = Anchor.TopCenter,
                    BorderWidth   = new() { Bottom = 1 },
                    BorderColor   = new() { Bottom = new("Window.AccentColor") },
                    IsAntialiased = false,
                    Color         = new("Widget.PopupMenuText"),
                    OutlineColor  = new("Widget.PopupMenuTextOutline"),
                    OutlineSize   = 1,
                }
            ),
            new(
                ".expansion-regions",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 10,
                    Size = new(ColumnWidth, 0),
                }
            ),
            new(
                ".region",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 10,
                }
            ),
            new(
                ".region-header",
                new() {
                    Flow            = Flow.Horizontal,
                    Padding         = new(4),
                    Margin          = new() { Top = 4 },
                    Size            = new(ColumnWidth, 24),
                    TextAlign       = Anchor.MiddleLeft,
                    FontSize        = 15,
                    Color           = new("Widget.PopupMenuText"),
                    OutlineColor    = new("Widget.PopupMenuTextOutline"),
                    OutlineSize     = 1,
                    BackgroundColor = new(0x50FFFFFF),
                    BorderRadius    = 6,
                    TextOffset      = new(0, 1),
                }
            ),
            new(
                ".region-destinations",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 4,
                }
            ),
            new(
                ".destination",
                new() {
                    Flow = Flow.Horizontal,
                    Size = new(ColumnWidth, 20),
                }
            ),
            new(
                ".destination-name",
                new() {
                    Flow         = Flow.Horizontal,
                    Size         = new(ColumnWidth - 100, 20),
                    Padding      = new(0, 8),
                    TextAlign    = Anchor.MiddleLeft,
                    TextOverflow = false,
                    WordWrap     = false,
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".destination-name:hover",
                new() {
                    Color        = new("Widget.PopupMenuTextHover"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineHover"),
                }
            ),
            new(
                ".destination-cost",
                new() {
                    Flow         = Flow.Horizontal,
                    Size         = new(100, 20),
                    Padding      = new(0, 8),
                    TextAlign    = Anchor.MiddleRight,
                    TextOverflow = false,
                    WordWrap     = false,
                    FontSize     = 11,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineDisabled"),
                    OutlineSize  = 1,
                }
            ),
        ]
    );
}
