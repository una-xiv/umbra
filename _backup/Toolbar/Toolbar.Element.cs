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

internal sealed partial class Toolbar
{
    private readonly Element _toolbarBackgroundElement = new(
        direction: Direction.Horizontal,
        anchor: Anchor.Top | Anchor.Left,
        size: new(0, Height),
        padding: new(0, 0),
        nodes: [
            new ShadowNode(),
            new RectNode(
                color: 0xFF1A1A1A,
                gradients: new(
                    topLeft: 0xFF2F2F2F,
                    topRight: 0xFF2F2F2F,
                    bottomLeft: 0xFF1A1A1A,
                    bottomRight: 0xFF1A1A1A
                )
            ),
            new TextNode()
        ],
        children: [
            new(
                id: "Border",
                size: new(0, 1),
                anchor: Anchor.Top | Anchor.Left,
                nodes: [
                    new RectNode(
                        color: 0xFF454545,
                        overflow: true,
                        margin: new(-1, 0, 1)
                    )
                ]
            )
        ]
    );

    private readonly Element _toolbarLeftWidgetContainer = new(
        id: "ToolbarLeftWidgetContainer",
        direction: Direction.Horizontal,
        anchor: Anchor.Top | Anchor.Left,
        size: new(0, Height),
        padding: new(left: 6),
        gap: 8
    );

    private readonly Element _toolbarCenterWidgetContainer = new(
        id: "ToolbarCenterWidgetContainer",
        direction: Direction.Horizontal,
        anchor: Anchor.Top | Anchor.Left,
        size: new(0, Height),
        gap: 8
    );

    private readonly Element _toolbarRightWidgetContainer = new(
        id: "ToolbarRightWidgetContainer",
        direction: Direction.Horizontal,
        anchor: Anchor.Top | Anchor.Left,
        size: new(0, Height),
        padding: new(left: -6),
        gap: 8
    );
}
