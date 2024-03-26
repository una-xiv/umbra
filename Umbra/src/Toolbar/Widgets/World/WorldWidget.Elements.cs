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

internal sealed partial class WorldWidget
{
    public Element Element { get; } = new(
        direction: Direction.Horizontal,
        anchor: Anchor.Middle | Anchor.Right,
        size: new(0, Height),
        sortIndex: int.MinValue + 2,
        children: [
            new(
                id: "Inner",
                anchor: Anchor.Middle | Anchor.Left,
                direction: Direction.Horizontal,
                size: new(0, Height - 9),
                gap: 8,
                children: [
                    new(
                        id: "Border",
                        size: new(1, Height),
                        anchor: Anchor.Middle | Anchor.Left,
                        nodes: [
                            new RectNode(
                                color: 0xFF454545,
                                overflow: true,
                                margin: new(-1, 0, 1)
                            )
                        ]
                    ),
                    new(
                        id: "Text",
                        direction: Direction.Vertical,
                        anchor: Anchor.Top | Anchor.Left,
                        size: new(0, 20),
                        gap: 0,
                        children: [
                            new(
                                id: "WorldName",
                                size: new(0, 10),
                                anchor: Anchor.Bottom | Anchor.Left,
                                fit: true,
                                nodes: [
                                    new TextNode(
                                        text: "",
                                        color: 0xFFD0D0D0,
                                        outlineColor: 0xFF000000,
                                        outlineSize: 1,
                                        font: Font.AxisSmall,
                                        align: Align.MiddleRight,
                                        margin: new(0, 0, 0),
                                        offset: new(0, 0)
                                    )
                                ]
                            ),
                            new(
                                size: new(0, 10),
                                anchor: Anchor.Top | Anchor.Left,
                                fit: true,
                                nodes: [
                                    new TextNode(
                                        text: "Visiting",
                                        color: 0xFFA0A0A0,
                                        outlineColor: 0xAA000000,
                                        outlineSize: 1,
                                        font: Font.AxisExtraSmall,
                                        align: Align.MiddleRight,
                                        margin: new(0, 0, 0),
                                        offset: new(0, -1)
                                    )
                                ]
                            )
                        ]
                    ),
                ]
            )
        ]
    );
}
