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

using Dalamud.Interface;
using Umbra.Drawing;
using Umbra.Drawing.Prefabs;

namespace Umbra;

internal sealed partial class VolumeWidget
{
    public Element Element { get; } = new(
        id: "VolumeToolbarWidget",
        anchor: Anchor.Middle | Anchor.Right,
        direction: Direction.Horizontal,
        sortIndex: int.MinValue,
        size: new(Height - 6, Height - 6),
        gap: 6,
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
            new TextNode(
                text: FontAwesomeIcon.VolumeUp.ToIconString(),
                align: Align.MiddleCenter,
                font: Font.FontAwesomeSmall,
                color: 0xFFC0C0C0,
                outlineColor: 0xFF000000,
                outlineSize: 1,
                offset: new(1, -1)
            )
        ]
    );

    private readonly DropdownElement _dropdownElement = new(
        id: "VolumeDropdown",
        anchor: Anchor.Middle | Anchor.Right,
        gap: 6,
        children: [
            CreateSliderElement("Master", "Master"),
            new DropdownSeparatorElement(),
            CreateSliderElement("Effects",     "Effects"),
            CreateSliderElement("Music",       "Music"),
            CreateSliderElement("Voice",       "Voice"),
            CreateSliderElement("Ambient",     "Ambient"),
            CreateSliderElement("System",      "System"),
            CreateSliderElement("Performance", "Performance")
        ]
    );

    private static Element CreateSliderElement(string id, string label)
    {
        return new(
            id: id,
            size: new(0, 32),
            direction: Direction.Horizontal,
            gap: 6,
            children: [
                new(
                    id: "Label",
                    size: new(100, 0),
                    fit: true,
                    anchor: Anchor.Middle | Anchor.Left,
                    nodes: [
                        new TextNode(
                            text: label,
                            font: Font.Axis,
                            align: Align.MiddleLeft,
                            color: 0xFFC0C0C0,
                            outlineColor: 0xFF000000,
                            outlineSize: 1,
                            offset: new(0, 0),
                            margin: new(left: 8)
                        )
                    ]
                ),
                new(
                    id: "Slider",
                    size: new(200, 0),
                    direction: Direction.Horizontal,
                    anchor: Anchor.Middle | Anchor.Left,
                    nodes: [
                        new SliderNode(
                            id: $"{id}_Slider",
                            value: 0.5f,
                            size: new(200, 22)
                        )
                    ]
                ),
                new(
                    id: "Value",
                    size: new(48, 0),
                    fit: true,
                    anchor: Anchor.Middle | Anchor.Left,
                    nodes: [
                        new TextNode(
                            font: Font.Axis,
                            align: Align.MiddleRight,
                            color: 0xFFC0C0C0,
                            outlineColor: 0xFF000000,
                            outlineSize: 1,
                            offset: new(0, 0),
                            margin: new(right: 8)
                        )
                    ]
                ),
            ]
        );
    }
}
