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

using Umbra.Drawing;

namespace Umbra;

internal sealed partial class CompanionWidget
{
    public Element Element { get; } = new(
        id: "CompanionToolbarWidget",
        direction: Direction.Horizontal,
        anchor: Anchor.Middle | Anchor.Right,
        sortIndex: int.MinValue + 11,
        size: new(0, Height - 6),
        gap: 6,
        children: [
            new(
                id: "Info",
                size: new(0, Height - 6),
                direction: Direction.Vertical,
                children: [
                    new(
                        id: "Name",
                        size: new(0, Height / 2 - 3),
                        fit: true,
                        nodes: [
                            new TextNode(
                                text: "Nugget Mc'Crispy",
                                font: Font.AxisSmall,
                                align: Align.BottomRight,
                                color: 0xFFC0C0C0,
                                outlineColor: 0xFF000000,
                                outlineSize: 1,
                                offset: new(0, 0),
                                margin: new(left: 8)
                            )
                        ]
                    ),
                    new(
                        id: "Sub",
                        size: new(0, Height / 2 - 3),
                        fit: true,
                        nodes: [
                            new TextNode(
                                text: "00:00",
                                font: Font.AxisExtraSmall,
                                align: Align.TopRight,
                                color: 0xFFAAAAAA,
                                outlineColor: 0xAA000000,
                                outlineSize: 1,
                                offset: new(0, -1),
                                margin: new(left: 8)
                            )
                        ]
                    )
                ]
            ),
            new(
                id: "Icon",
                size: new(Height - 6, Height - 6),
                anchor: Anchor.Middle | Anchor.Left,
                nodes: [
                    new RectNode(
                        color: 0xFF1A1A1A,
                        borderColor: 0xFF101010,
                        borderSize: 1,
                        rounding: 4
                    ),
                    new RectNode(
                        color: 0,
                        borderColor: 0xFF3F3F3F,
                        borderSize: 1,
                        rounding: 3,
                        margin: new(1, 1, 1, 1)
                    ),
                    new IconNode(
                        iconId: 25218,
                        rounding: 7,
                        margin: new(3, 3, 3, 3),
                        uv1: new(0.05f, 0.05f),
                        uv2: new(0.95f, 0.95f)
                    )
                ]
            )
        ]
    );
}
