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

internal partial class BattleEffectsPopup
{
    private static Stylesheet BattleEffectsPopupStylesheet { get; } = new(
        [
            new(
                ".popup",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 8,
                    Padding = new(15),
                    Size    = new(350, 0),
                }
            ),
            new(
                ".header",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                    Size = new(320, 0),
                }
            ),
            new(
                ".header-label",
                new() {
                    Size         = new(320, 0),
                    TextAlign    = Anchor.MiddleCenter,
                    AutoSize     = (AutoSize.Fit, AutoSize.Grow),
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".row",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 15,
                    Size = new(320, 28),
                }
            ),
            new(
                ".row-label",
                new() {
                    Size         = new(80, 28),
                    TextAlign    = Anchor.MiddleRight,
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".slider",
                new() {
                    Size            = new(150, 28),
                    BackgroundColor = new(0x40000000),
                    BorderRadius    = 8,
                    IsAntialiased   = false,
                    Padding         = new(0, 4),
                }
            ),
            new(
                ".row-value",
                new() {
                    Flow         = Flow.Horizontal,
                    Size         = new(70, 28),
                    TextAlign    = Anchor.MiddleLeft,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                    Padding      = new() { Left = 15 },
                }
            ),
        ]
    );
}
