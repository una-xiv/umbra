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

using Dalamud.Game.Text;
using Umbra.Drawing;

namespace Umbra;

internal sealed partial class ClockWidget
{
    public Element Element { get; } = new(
        direction: Direction.Horizontal,
        anchor: Anchor.Middle | Anchor.Right,
        size: new(0, Height),
        sortIndex: int.MinValue + 1,
        gap: 6,
        children: []
    );

    private Element BuildClockElement(string id, SeIconChar prefixIcon)
    {
        return new(
            id: id,
            direction: Direction.Horizontal,
            anchor: Anchor.Middle | Anchor.Left,
            size: new(0, Height - 6),
            gap: 6,
            nodes: [
                new RectNode(
                    color: 0xFF1A1A1A,
                    borderColor: 0xFF3F3F3F,
                    borderSize: 1,
                    rounding: 4,
                    margin: new(1, 1, 1, 1)
                ),
                new RectNode(
                    color: 0x00000000,
                    borderColor: 0xFF101010,
                    borderSize: 1,
                    rounding: 5
                ),
            ],
            children: [
                new(
                    id: "Icon",
                    fit: true,
                    anchor: Anchor.Middle | Anchor.Left,
                    nodes: [
                        new TextNode(
                            text: prefixIcon.ToIconString(),
                            font: Font.Axis,
                            align: Align.MiddleLeft,
                            color: 0xFF606060,
                            margin: new(0, 0, 0, 6),
                            offset: new(0, -1)
                        )
                    ]
                ),
                new(
                    id: "Text",
                    fit: true,
                    anchor: Anchor.Middle | Anchor.Left,
                    nodes: [
                        new TextNode(
                            text: "00:00",
                            font: Font.Monospace,
                            align: Align.MiddleLeft,
                            color: 0xFFCFCFCF,
                            outlineColor: 0xFF000000,
                            outlineSize: 1,
                            margin: new(0, 8, 0),
                            offset: new(0, -1)
                        )
                    ]
                )
            ]
        );
    }
}
