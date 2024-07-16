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
    private const int ExpansionWidth = 200;
    private const int ColumnWidth    = 300;

    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                "#Popup",
                new() {
                    Flow = Flow.Horizontal,
                }
            ),

            new(
                "#Title",
                new() {
                    Flow    = Flow.Horizontal,
                    Size    = new(ExpansionWidth, 40),
                    Gap     = 8,
                    Padding = new(4),
                }
            ),
            new(
                "#TitleIcon",
                new() {
                    Size   = new(32, 32),
                    IconId = 111, // Teleport icon.
                }
            ),
            new(
                "#TitleText",
                new() {
                    Size         = new(0, 32),
                    FontSize     = 16,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.MiddleLeft,
                }
            ),

            #region Expansions

            new(
                "#ExpansionList",
                new() {
                    Flow    = Flow.Vertical,
                    Size    = new(ExpansionWidth, 0),
                    Gap     = 6,
                }
            ),
            new(
                "#ExpansionList:left",
                new() {
                    Padding = new(4) { Right = 0 },
                }
            ),
            new(
                "#ExpansionList:right",
                new() {
                    Padding = new(4) { Left = 0 },
                }
            ),
            new(
                ".expansion",
                new() {
                    Size           = new(ExpansionWidth - 4, 30),
                    Padding        = new(8),
                    FontSize       = 14,
                    Color          = new("Widget.PopupMenuText"),
                    OutlineColor   = new("Widget.PopupMenuTextOutline"),
                    OutlineSize    = 1,
                    TextAlign      = Anchor.MiddleLeft,
                    BorderRadius   = 8,
                    RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
                    IsAntialiased  = false,
                }
            ),
            new(
                ".expansion:hover",
                new() {
                    Color        = new("Widget.PopupMenuTextHover"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineHover"),
                }
            ),
            new(
                ".expansion:selected",
                new() {
                    BackgroundColor = new(0x40000000),
                }
            ),

            #endregion

            #region Destinations

            new(
                "#DestinationList",
                new() {
                    Flow    = Flow.Horizontal,
                }
            ),
            new(
                "#DestinationList:right",
                new() {
                    Padding = new(4) { Left = 0 },
                }
            ),
            new(
                "#DestinationList:left",
                new() {
                    Padding = new(4) { Right = 0 },
                }
            ),
            new(
                ".region-container",
                new() {
                    Flow            = Flow.Horizontal,
                    Gap             = 15,
                    Padding         = new(8),
                    BackgroundColor = new(0x40000000),
                    BorderRadius    = 5,
                    IsAntialiased   = false,
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
                    Size          = new(ColumnWidth, 30),
                    Padding       = new(8),
                    FontSize      = 14,
                    Color         = new("Widget.PopupMenuText"),
                    OutlineColor  = new("Widget.PopupMenuTextOutline"),
                    OutlineSize   = 1,
                    TextAlign     = Anchor.MiddleLeft,
                    TextOffset    = new(0, 1),
                    IsAntialiased = false,
                    BorderWidth   = new() { Bottom = 1 },
                    BorderColor   = new() { Bottom = new("Window.AccentColor") },
                }
            ),
            new(
                ".region-spacer",
                new() {
                    Size          = new(ColumnWidth, 30),
                    Padding       = new(8),
                    FontSize      = 14,
                    Color         = new("Widget.PopupMenuText"),
                    TextAlign     = Anchor.MiddleLeft,
                    TextOffset    = new(0, 1),
                    IsAntialiased = false,
                }
            ),
            new(
                ".region-destinations",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 4,
                }
            ),

            #endregion

            #region Map

            new(
                ".map",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 4,
                }
            ),
            new(
                ".map-header",
                new() {
                    Size         = new(ColumnWidth, 0),
                    Padding      = new(0, 8),
                    FontSize     = 11,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.MiddleLeft,
                }
            ),
            new(
                ".map-destinations",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 4,
                    Padding = new() { Left = 8, Bottom = 15 },
                }
            ),

            #endregion

            #region Destination

            new(
                ".destination",
                new() {
                    Flow    = Flow.Horizontal,
                    Gap     = 0,
                    Size    = new(ColumnWidth - 16, 20),
                    Padding = new() { Left = 4, Right = 8 },
                }
            ),
            new(
                ".destination:hover",
                new() {
                    BackgroundColor = new(0x40FFFFFF),
                }
            ),
            new(
                ".destination-icon",
                new() {
                    Size   = new(20, 20),
                    UldPartId = 3,
                    UldPartsId = 0,
                    UldResource = "ui/uld/Teleport",
                }
            ),
            new(
                ".destination-name",
                new() {
                    Size         = new(ColumnWidth - 142, 20),
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.MiddleLeft,
                    TextOverflow = false,
                    WordWrap     = false,
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
                    Flow         = Flow.Vertical,
                    Gap          = 4,
                    Size         = new(100, 20),
                    FontSize     = 11,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.MiddleRight,
                    TextOffset   = new(0, 1),
                }
            ),

            #endregion
        ]
    );
}
