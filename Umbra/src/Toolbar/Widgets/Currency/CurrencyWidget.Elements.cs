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

using Dalamud.Interface;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Gearset;

internal partial class CurrencyWidget
{
    public Element Element { get; } = new(
        id: "CurrencyToolbarWidget",
        anchor: Anchor.MiddleLeft,
        sortIndex: int.MinValue + 1,
        flow: Flow.Horizontal,
        size: new(0, 28),
        padding: new(left: 6),
        gap: 6,
        style: new() { TextColor = Theme.Color(ThemeColor.Text) },
        children: [
            new(
                id: "Icon",
                anchor: Anchor.MiddleLeft,
                text: FontAwesomeIcon.Coins.ToIconString(),
                size: new(20, 20),
                style: new() {
                    Font         = Font.FontAwesome,
                    TextAlign    = Anchor.MiddleCenter,
                    OutlineColor = Theme.Color(ThemeColor.TextOutlineLight),
                    OutlineWidth = 1,
                    TextOffset   = new(0, -1)
                }
            ),
            new(
                id: "Text",
                anchor: Anchor.MiddleLeft,
                size: new(0, 28),
                text: "Currencies",
                style: new() {
                    Font         = Font.Axis,
                    TextAlign    = Anchor.MiddleLeft,
                    OutlineColor = Theme.Color(ThemeColor.TextOutlineLight),
                    OutlineWidth = 1,
                    TextOffset   = new(0, -1)
                }
            ),
        ]
    );
}
