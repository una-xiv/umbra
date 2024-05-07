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

using System.Numerics;
using Dalamud.Game.Text;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Clock2;

internal partial class StackedClockWidget
{
    public Element Element { get; } = new(
        "StackedClockWidget",
        flow: Flow.Vertical,
        anchor: Anchor.MiddleRight,
        sortIndex: 0,
        gap: 0,
        children: [
            CreateClock("ET", SeIconChar.EorzeaTimeEn, new(0, -2)),
            CreateClock("LT", SeIconChar.LocalTimeEn, new(0, -2)),
            CreateClock("ST", SeIconChar.ServerTimeEn, new(0, -2))
        ]
    );

    private static Element CreateClock(string id, SeIconChar prefix, Vector2 textOffset)
    {
        return new(
            id,
            anchor: Anchor.MiddleCenter,
            flow: Flow.Horizontal,
            size: new(0, 14),
            children: [
                new(
                    "Container",
                    anchor: Anchor.MiddleCenter,
                    flow: Flow.Horizontal,
                    gap: 4,
                    padding: new(left: 8, right: 2),
                    children: [
                        new(
                            "Prefix",
                            text: prefix.ToIconString(),
                            style: new() {
                                Font         = Font.AxisExtraSmall,
                                TextAlign    = Anchor.MiddleLeft,
                                TextOffset   = textOffset,
                                TextColor    = Theme.Color(ThemeColor.TextMuted),
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1,
                            }
                        ),
                        new(
                            "Time",
                            text: "00:00",
                            style: new() {
                                Font         = Font.MonospaceSmall,
                                TextColor    = Theme.Color(ThemeColor.Text),
                                TextAlign    = Anchor.MiddleLeft,
                                TextOffset   = textOffset,
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1,
                            }
                        ),
                        new(
                            "Suffix",
                            text: "AM",
                            style: new() {
                                Font         = Font.AxisExtraSmall,
                                TextColor    = Theme.Color(ThemeColor.Text),
                                TextAlign    = Anchor.MiddleLeft,
                                TextOffset   = textOffset,
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1,
                            }
                        )
                    ]
                )
            ]
        );
    }
}
