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

namespace Umbra.Windows.Config;

internal sealed partial class ConfigWindow
{
    private readonly Element _root = new(
        direction: Direction.Horizontal,
        anchor: Anchor.Top | Anchor.Left,
        fit: true,
        gap: 0,
        children: [
            new(
                id: "Menu",
                direction: Direction.Vertical,
                anchor: Anchor.Top | Anchor.Left,
                gap: 4,
                size: new(150, 0),
                nodes: [
                    new RectNode(
                        gradients: new(
                            topLeft: 0xFF292929,
                            topRight: 0xFF3C3C3C,
                            bottomLeft: 0xFF292929,
                            bottomRight: 0xFF3C3C3C
                        )
                    )
                ],
                children: [
                    new(
                        id: "Items",
                        direction: Direction.Vertical,
                        anchor: Anchor.Top | Anchor.Left,
                        gap: 6,
                        fit: true,
                        padding: new(6, 6, 0, 6),
                        children: []
                    ),
                    new(
                        id: "Logo",
                        anchor: Anchor.Bottom | Anchor.Left,
                        size: new(150, 150),
                        nodes: [
                            new RectNode(
                                gradients: new(
                                    topLeft: 0xFF1F1F1F,
                                    topRight: 0xFF2E2E2E,
                                    bottomLeft: 0xFF1F1F1F,
                                    bottomRight: 0xFF2E2E2E
                                )
                            ),
                            new ImageNode(
                                path: "images/icon.png",
                                margin: new(bottom: 8)
                            )
                        ]
                    ),
                ]
            ),
            new(
                id: "Separator",
                size: new(2, 0),
                nodes: [
                    new RectNode(color: 0xFF3F3F3F, margin: new(left: 1)),
                    new RectNode(color: 0xFF191919, margin: new(right: 1))
                ]
            ),
            new(
                id: "Panels",
                direction: Direction.Vertical,
                anchor: Anchor.Top | Anchor.Left,
                fit: true,
                padding: new(8, 8)
            )
        ]
    );
}
