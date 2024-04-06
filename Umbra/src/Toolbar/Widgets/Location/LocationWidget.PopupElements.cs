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

using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Location;

internal partial class LocationWidget
{
    private readonly DropdownElement _dropdownElement = new(
        id: "LocationWidget",
        flow: Flow.Vertical,
        children: [
            new GradientElement(
                id: "BG",
                gradient: Gradient.Vertical(0x40FFFAAA, 0),
                padding: new(left: 2, right: 2, bottom: 0, top: 46)
            ),
            new(
                id: "VerticalLine",
                anchor: Anchor.None,
                margin: new(top: 42, left: 42),
                padding: new(bottom: 80),
                size: new(1, 0),
                style: new() {
                    BorderWidth = new(left: 1, right: 0, bottom: 0, top: 0),
                    BorderColor = new(left: 0xFF707070, right: 0, top: 0, bottom: 0),
                }
            ),
            new(
                id: "Header",
                flow: Flow.Vertical,
                size: new(0, 80),
                fit: true,
                children: [
                    new GradientElement(
                        id: "BG",
                        gradient: Gradient.Vertical(0, 0x20FFFAAA),
                        padding: new(top: 1, left: 2, right: 2, bottom: 42)
                    ),
                    new(
                        id: "Content",
                        flow: Flow.Horizontal,
                        padding: new(12),
                        gap: 8,
                        children: [
                            new(
                                id: "Icon",
                                anchor: Anchor.MiddleLeft,
                                size: new(64, 64),
                                style: new() {
                                    Image = 60277u,
                                }
                            ),
                            new(
                                id: "Text",
                                flow: Flow.Vertical,
                                anchor: Anchor.MiddleLeft,
                                size: new(0, 64),
                                children: [
                                    new(
                                        id: "Name",
                                        size: new(0, 32),
                                        anchor: Anchor.TopLeft,
                                        margin: new(right: 12),
                                        text: "Zone Name Here",
                                        style: new() {
                                            Font         = Font.AxisExtraLarge,
                                            TextAlign    = Anchor.BottomLeft,
                                            TextColor    = 0xFFC0C0C0,
                                            TextOffset   = new(0, 1),
                                            OutlineColor = 0xAA000000,
                                            OutlineWidth = 1,
                                        }
                                    ),
                                    new(
                                        id: "Info",
                                        size: new(0, 32),
                                        anchor: Anchor.TopLeft,
                                        text: "District name, Weather name here",
                                        margin: new(right: 12),
                                        style: new() {
                                            Font         = Font.Axis,
                                            TextAlign    = Anchor.TopLeft,
                                            TextColor    = 0xFFA0A0A0,
                                            TextOffset   = new(0, 3),
                                            OutlineColor = 0x80000000,
                                            OutlineWidth = 1,
                                        }
                                    ),
                                ]
                            ),
                        ]
                    ),
                ]
            ),
            new(
                id: "ForecastList",
                flow: Flow.Vertical,
                gap: 8,
                children: [
                    CreateForecastEntry(0),
                    CreateForecastEntry(1),
                    CreateForecastEntry(2),
                    CreateForecastEntry(3),
                    CreateForecastEntry(4),
                    CreateForecastEntry(5),
                ]
            )
        ]
    );

    private static Element CreateForecastEntry(int id)
    {
        return new(
            id: $"Forecast{id}",
            flow: Flow.Horizontal,
            size: new(0, 48),
            padding: new(0, 12),
            gap: 8,
            children: [
                new(
                    id: "Icon",
                    size: new(32, 32),
                    margin: new(left: 14),
                    style: new() {
                        Image = 60277u,
                    }
                ),
                new(
                    id: "Text",
                    flow: Flow.Vertical,
                    size: new(0, 32),
                    children: [
                        new(
                            id: "Name",
                            size: new(0, 13),
                            text: "Weather Name Here",
                            style: new() {
                                Font         = Font.Axis,
                                TextAlign    = Anchor.BottomLeft,
                                TextColor    = 0xFFC0C0C0,
                                TextOffset   = new(0, 4),
                                OutlineColor = 0xAA000000,
                                OutlineWidth = 1,
                            }
                        ),
                        new(
                            id: "Info",
                            size: new(0, 16),
                            text: "42 minutes",
                            style: new() {
                                Font         = Font.AxisSmall,
                                TextAlign    = Anchor.TopLeft,
                                TextColor    = 0xFFA0A0A0,
                                TextOffset   = new(0, 4),
                                OutlineColor = 0x80000000,
                                OutlineWidth = 1,
                            }
                        ),
                    ]
                ),
            ]
        );
    }
}
