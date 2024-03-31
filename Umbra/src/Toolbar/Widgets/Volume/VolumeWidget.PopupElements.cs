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
using Umbra.Game;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.MainMenu;

internal sealed partial class VolumeWidget
{
    private readonly DropdownElement _dropdownElement = new(
        id: "VolumePopup",
        flow: Flow.Vertical,
        children: [
            new(
                id: "Channels",
                flow: Flow.Horizontal,
                gap: 8,
                padding: new(0, 8),
                children: [
                    CreateChannelWidget("Master", "Master volume"),
                    CreateChannelWidget("BGM",    "Background music"),
                    CreateChannelWidget("SFX",    "Sound effects"),
                    CreateChannelWidget("VOC",    "Voice"),
                    CreateChannelWidget("AMB",    "Ambient sound effects"),
                    CreateChannelWidget("SYS",    "System sounds"),
                    CreateChannelWidget("PERF",   "Performance music"),
                ]
            )
        ]
    );

    private static Element CreateChannelWidget(string id, string tooltip)
    {
        return new(
            id: id,
            flow: Flow.Vertical,
            size: new(32, 250),
            children: [
                new(
                    id: "Label",
                    text: id,
                    size: new(32, 20),
                    anchor: Anchor.TopCenter,
                    tooltip: tooltip,
                    style: new() {
                        Font         = Font.AxisExtraSmall,
                        TextAlign    = Anchor.BottomCenter,
                        TextColor    = 0xFFC0C0C0,
                        OutlineColor = 0x80000000,
                        OutlineWidth = 1,
                        TextOffset   = new(0, -1)
                    }
                ),
                new(
                    id: "Value",
                    text: "100%",
                    size: new(32, 10),
                    anchor: Anchor.TopCenter,
                    tooltip: tooltip,
                    style: new() {
                        Font         = Font.AxisExtraSmall,
                        TextAlign    = Anchor.MiddleCenter,
                        TextColor    = 0xFF909090,
                        OutlineColor = 0x80000000,
                        OutlineWidth = 1,
                        TextOffset   = new(0, -1)
                    }
                ),
                new(
                    id: "Slider",
                    anchor: Anchor.TopCenter,
                    flow: Flow.Horizontal,
                    size: new(32, 220),
                    children: [
                        new VerticalSliderElement(id, new(16, 220), Anchor.TopCenter)
                            { Padding = new(top: 8, bottom: 4) },
                    ]
                ),
            ]
        );
    }
}
