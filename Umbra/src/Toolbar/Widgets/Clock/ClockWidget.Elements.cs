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

namespace Umbra.Toolbar.Widgets.Clock;

internal partial class ClockWidget
{
    public Element Element { get; } = new(
        "Clock",
        flow: Flow.Vertical,
        anchor: Anchor.MiddleRight,
        sortIndex: int.MinValue,
        gap: 2,
        children: [
            CreateClock("ET", SeIconChar.EorzeaTimeEn, new(0, -1)),
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
            size: new(0, 28),
            children: [
                new BackgroundElement(color: 0xFF1A1A1A, edgeColor: 0xFF101010, edgeThickness: 1, rounding: 4),
                new BorderElement(color: 0xFF3F3F3F, rounding: 3, padding: new(1)),
                new(
                    "Container",
                    anchor: Anchor.MiddleCenter,
                    flow: Flow.Horizontal,
                    gap: 4,
                    padding: new(0, 8),
                    children: [
                        new(
                            "Prefix",
                            text: prefix.ToIconString(),
                            style: new() {
                                Font         = Font.AxisSmall,
                                TextAlign    = Anchor.MiddleLeft,
                                TextOffset   = textOffset,
                                TextColor    = 0xFF707070,
                                OutlineColor = 0x90000000,
                                OutlineWidth = 1,
                            }
                        ),
                        new(
                            "Time",
                            text: "00:00",
                            style: new() {
                                Font         = Font.AxisSmall,
                                TextColor    = 0xFFC0C0C0,
                                TextAlign    = Anchor.MiddleLeft,
                                TextOffset   = textOffset,
                                OutlineColor = 0x90000000,
                                OutlineWidth = 1,
                            }
                        )
                    ]
                )
            ]
        );
    }
}
