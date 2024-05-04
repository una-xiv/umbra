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

using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Companion;

internal partial class CompanionWidget
{
    public Element Element { get; } = new(
        "CompanionWidget",
        anchor: Anchor.MiddleRight,
        sortIndex: 6,
        size: new(0, 28),
        children: [
            new BackgroundElement(color: Theme.Color(ThemeColor.BackgroundDark), edgeColor: Theme.Color(ThemeColor.BorderDark), edgeThickness: 1, rounding: 4),
            new BorderElement(color: Theme.Color(ThemeColor.Border), rounding: 3, padding: new(1)),
            new(
                "Container",
                size: new(0, 28),
                padding: new(left: 3, right: 3),
                gap: 6,
                children: [
                    new(
                        "Icon",
                        size: new(24, 22),
                        anchor: Anchor.MiddleLeft,
                        style: new() {
                            Image = 14u,
                        }
                    ),
                    new(
                        "Text",
                        flow: Flow.Vertical,
                        anchor: Anchor.MiddleLeft,
                        size: new(0, 28),
                        margin: new(right: 5),
                        children: [
                            new(
                                "Name",
                                text: "Companion",
                                size: new(0, 13),
                                style: new() {
                                    Font         = Font.AxisExtraSmall,
                                    TextColor    = Theme.Color(ThemeColor.Text),
                                    TextAlign    = Anchor.BottomLeft,
                                    TextOffset   = new(0, 1),
                                    OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                    OutlineWidth = 1,
                                }
                            ),
                            new(
                                "Status",
                                text: "",
                                size: new(0, 13),
                                style: new() {
                                    Font         = Font.AxisExtraSmall,
                                    TextColor    = Theme.Color(ThemeColor.TextMuted),
                                    TextAlign    = Anchor.TopLeft,
                                    TextOffset   = new(0, -1),
                                    OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                    OutlineWidth = 1,
                                }
                            )
                        ]
                    )
                ]
            )
        ]
    );
}
