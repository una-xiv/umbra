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
        "CompanionToolbarWidget",
        anchor: Anchor.MiddleRight,
        sortIndex: -1000,
        size: new(0, 28),
        children: [
            new BackgroundElement(color: 0xFF1A1A1A, edgeColor: 0xFF101010, edgeThickness: 1, rounding: 4),
            new BorderElement(color: 0xFF3F3F3F, rounding: 3, padding: new(1)),
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
                                    TextColor    = 0xFFC0C0C0,
                                    TextAlign    = Anchor.BottomLeft,
                                    TextOffset   = new(0, 1),
                                    OutlineColor = 0x80000000,
                                    OutlineWidth = 1,
                                }
                            ),
                            new(
                                "Status",
                                text: "",
                                size: new(0, 13),
                                style: new() {
                                    Font         = Font.AxisExtraSmall,
                                    TextColor    = 0xFF909090,
                                    TextAlign    = Anchor.TopLeft,
                                    TextOffset   = new(0, -1),
                                    OutlineColor = 0x80000000,
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
