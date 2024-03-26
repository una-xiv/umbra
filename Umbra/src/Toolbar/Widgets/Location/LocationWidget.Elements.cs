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

using System.Collections.Generic;
using Umbra.Drawing;
using Umbra.Drawing.Prefabs;

namespace Umbra;

internal sealed partial class LocationWidget
{
    public Element Element { get; } = new(
        id: "LocationToolbarWidget",
        direction: Direction.Horizontal,
        anchor: Anchor.Middle | Anchor.Center,
        sortIndex: 0,
        size: new(512, Height - 4),
        gap: 6,
        padding: new(left: Height - 12),
        children: [
            new(
                id: "Location",
                size: new(240 - (Height / 2 - 3), Height - 6),
                direction: Direction.Vertical,
                children: [
                    new(
                        id: "Name",
                        size: new(0, Height / 2 - 3),
                        fit: true,
                        nodes: [
                            new TextNode(
                                text: "The Crystarium",
                                font: Font.AxisSmall,
                                align: Align.BottomRight,
                                color: 0xFFD0D0D0,
                                outlineColor: 0xBB000000,
                                outlineSize: 1,
                                offset: new(0, 0)
                            )
                        ]
                    ),
                    new(
                        id: "District",
                        size: new(0, Height / 2 - 3),
                        fit: true,
                        nodes: [
                            new TextNode(
                                text: "Some very cool place",
                                font: Font.AxisExtraSmall,
                                align: Align.TopRight,
                                color: 0xFFC0C0C0,
                                outlineColor: 0x80000000,
                                outlineSize: 1,
                                offset: new(0, -1)
                            )
                        ]
                    )
                ]
            ),
            new(
                id: "Icon",
                size: new(Height - 6, Height - 6),
                nodes: [
                    new IconNode(
                        iconId: 24
                    )
                ]
            ),
            new(
                id: "Weather",
                size: new(240 - (Height / 2 - 3), Height - 6),
                direction: Direction.Vertical,
                children: [
                    new(
                        id: "Name",
                        size: new(0, Height / 2 - 3),
                        fit: true,
                        nodes: [
                            new TextNode(
                                text: "Hot as fuck",
                                font: Font.AxisSmall,
                                align: Align.BottomLeft,
                                color: 0xFFD0D0D0,
                                outlineColor: 0xBB000000,
                                outlineSize: 1,
                                offset: new(0, 0)
                            )
                        ]
                    ),
                    new(
                        id: "Time",
                        size: new(0, Height / 2 - 3),
                        fit: true,
                        nodes: [
                            new TextNode(
                                text: "24 minutes",
                                font: Font.AxisExtraSmall,
                                align: Align.TopLeft,
                                color: 0xFFC0C0C0,
                                outlineColor: 0x80000000,
                                outlineSize: 1,
                                offset: new(0, -1)
                            )
                        ]
                    )
                ]
            )
        ]
    );

    public DropdownElement DropdownElement { get; } = new(
        id: "LocationDropdown",
        anchor: Anchor.Bottom | Anchor.Center,
        nodes: [
            new RectNode(
                id: "GradientBackground",
                color: 0,
                margin: new(top: 40, bottom: 16, left: -4, right: -4),
                gradients: new(topLeft: 0xFF906010, topRight: 0xFF906010),
                overflow: true
            )
        ],
        children: [
            new(
                id: "Header",
                direction: Direction.Horizontal,
                anchor: Anchor.Top | Anchor.Left,
                fit: true,
                gap: 4,
                nodes: [
                    new RectNode(
                        id: "GradTop",
                        color: 0,
                        gradients: new(bottomLeft: 0xFF906010, bottomRight: 0xFF906010),
                        margin: new(bottom: 40, left: -4, right: -4),
                        overflow: true
                    ),
                ],
                children: [
                    new(
                        id: "Icon",
                        size: new(80, 80),
                        direction: Direction.Horizontal,
                        anchor: Anchor.Top | Anchor.Left,
                        nodes: [
                            new IconNode(
                                iconId: 60206,
                                margin: new(8, 8, 8, 8)
                            )
                        ]
                    ),
                    new(
                        id: "Text",
                        size: new(0, 80),
                        direction: Direction.Vertical,
                        anchor: Anchor.Top | Anchor.Left,
                        children: [
                            new(
                                id: "Location",
                                size: new(0, 42),
                                direction: Direction.Horizontal,
                                anchor: Anchor.Top | Anchor.Left,
                                nodes: [
                                    new TextNode(
                                        text: "Location Name",
                                        font: Font.Jupiter,
                                        align: Align.BottomLeft,
                                        color: 0xFFE0E0E0,
                                        margin: new(0, 16, 0),
                                        outlineColor: 0xFF000000,
                                        outlineSize: 1
                                    )
                                ]
                            ),
                            new(
                                id: "District",
                                size: new(0, 38),
                                direction: Direction.Horizontal,
                                anchor: Anchor.Top | Anchor.Left,
                                nodes: [
                                    new TextNode(
                                        text: "District Name, Sunny",
                                        font: Font.AxisSmall,
                                        align: Align.TopLeft,
                                        color: 0xFFC0C0C0,
                                        margin: new(0, 16, 0),
                                        outlineColor: 0xAA000000,
                                        outlineSize: 1,
                                        offset: new(0, -2)
                                    )
                                ]
                            ),
                        ]
                    ),
                ]
            ),
            new(
                id: "WeatherList",
                direction: Direction.Vertical,
                anchor: Anchor.Top | Anchor.Left,
                gap: 4,
                padding: new(left: 12, bottom: 4),
                children: BuildWeatherList()
            )
        ]
    );

    private static List<Element> BuildWeatherList()
    {
        List<Element> list = [];

        for (int i = 0; i < 6; i++) {
            list.Add(
                new(
                    id: $"WeatherRow_{i}",
                    size: new(0, 32),
                    direction: Direction.Horizontal,
                    anchor: Anchor.Top | Anchor.Left,
                    padding: new(left: 10),
                    gap: 6,
                    children: [
                        new(
                            id: "Icon",
                            size: new(32, 32),
                            direction: Direction.Horizontal,
                            anchor: Anchor.Top | Anchor.Left,
                            nodes: [
                                new IconNode(
                                    iconId: 60208,
                                    margin: new(2, 2, 2, 2)
                                )
                            ]
                        ),
                        new(
                            id: "Text",
                            size: new(0, 32),
                            direction: Direction.Vertical,
                            anchor: Anchor.Top | Anchor.Left,
                            children: [
                                new(
                                    id: "Name",
                                    size: new(0, 16),
                                    direction: Direction.Horizontal,
                                    fit: true,
                                    nodes: [
                                        new TextNode(
                                            text: "Sunny",
                                            color: 0xFFD0D0D0,
                                            font: Font.AxisSmall,
                                            align: Align.BottomLeft,
                                            outlineColor: 0xBB000000,
                                            outlineSize: 1
                                        )
                                    ]
                                ),
                                new(
                                    id: "Time",
                                    size: new(0, 16),
                                    direction: Direction.Horizontal,
                                    fit: true,
                                    nodes: [
                                        new TextNode(
                                            text: "1 hour and 43 minutes.",
                                            color: 0xFFC0C0C0,
                                            font: Font.AxisExtraSmall,
                                            align: Align.TopLeft,
                                            outlineColor: 0xBB000000,
                                            outlineSize: 1
                                        )
                                    ]
                                )
                            ]
                        )
                    ]
                )
            );
        }

        return list;
    }
}
