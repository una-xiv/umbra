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

public sealed partial class GearsetSwitcherPopup
{
    #region Header

    private static Stylesheet GearsetSwitcherHeaderStylesheet { get; } = new(
        [
            new(
                "#Header",
                new() {
                    Flow                    = Flow.Horizontal,
                    Padding                 = new(15),
                    Size                    = new(0, 90),
                    Gap                     = 15,
                    BackgroundGradient      = GradientColor.Vertical(new(0), new(0x80C0A070)),
                    BackgroundGradientInset = new(4) { Bottom = 0 },
                }
            ),
            new(
                "#HeaderIcon",
                new() {
                    Anchor          = Anchor.MiddleLeft,
                    Size            = new(60, 60),
                    IconId          = 62101,
                    ImageInset      = new(4),
                    BackgroundColor = new("Input.Background"),
                }
            ),
            new(
                "#HeaderBody",
                new() {
                    Anchor = Anchor.MiddleLeft,
                    Flow   = Flow.Vertical,
                    Size   = new(0, 50),
                }
            ),
            new(
                "#HeaderItemLevel",
                new() {
                    Anchor       = Anchor.TopRight,
                    Font         = 0,
                    FontSize     = 38,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.TopRight,
                    Margin       = new() { Right = -30 },
                    Opacity      = 0.75f,
                }
            ),
            new(
                "#HeaderGearsetName",
                new() {
                    Size         = new(GearsetNode.NodeWidth, 0),
                    Font         = 0,
                    FontSize     = 18,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                "#HeaderGearsetInfo",
                new() {
                    Size         = new(GearsetNode.NodeWidth, 0),
                    Font         = 0,
                    FontSize     = 11,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                "#HeaderControls",
                new() {
                    Anchor = Anchor.BottomLeft,
                    Flow   = Flow.Horizontal,
                    Gap    = 8,
                    Margin = new() { Bottom = -4 },
                }
            ),
        ]
    );

    #endregion

    #region Columns

    private static Stylesheet GearsetSwitcherColumnsStylesheet { get; } = new(
        [
            new(
                "#Columns",
                new() {
                    Flow    = Flow.Horizontal,
                    Gap     = 8,
                    Padding = new(0, 15) { Bottom = 15 },
                }
            ),
            new(
                ".column",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 4,
                }
            ),
            new(
                ".role-container",
                new() {
                    Flow = Flow.Vertical,
                    Size = new(GearsetNode.NodeWidth, 0),
                    Gap  = 4,
                }
            ),
            new(
                "#RoleHeader",
                new() {
                    Size         = new(GearsetNode.NodeWidth, GearsetNode.NodeHeight),
                    Font         = 0,
                    FontSize     = 16,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.MiddleCenter,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                "#RoleBody",
                new() {
                    Flow                = Flow.Vertical,
                    Size                = new(GearsetNode.NodeWidth + 8, 0),
                    ScrollbarTrackColor = new(0),
                    ScrollbarThumbColor = new("Input.Background"),
                }
            ),
            new(
                "#GearsetList",
                new() {
                    Flow = Flow.Vertical,
                    Size = new(GearsetNode.NodeWidth, 0), // TODO: Configurable overflow height.
                    Gap  = 4,
                }
            )
        ]
    );

    #endregion
}
