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
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Location;

internal partial class LocationWidget
{
    public Element Element { get; } = new(
        id: "LocationWidget",
        anchor: Anchor.MiddleCenter,
        flow: Flow.Horizontal,
        size: new(0, 28),
        gap: 6,
        children: [
            new(
                id: "Location",
                flow: Flow.Vertical,
                anchor: Anchor.MiddleCenter,
                size: new(225, 28),
                margin: new(right: ItemSpacing),
                children: [
                    new(
                        id: "Name",
                        flow: Flow.Horizontal,
                        size: new(225, 14),
                        gap: 6,
                        children: [
                            new(
                                id: "Label",
                                size: new(0, 14),
                                text: "Zone Name Here",
                                anchor: Anchor.MiddleRight,
                                style: new() {
                                    Font         = Font.AxisSmall,
                                    TextAlign    = Anchor.BottomRight,
                                    TextColor    = Theme.Color(ThemeColor.Text),
                                    TextOffset   = new(0, 0),
                                    OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                    OutlineWidth = 1,
                                }
                            ),
                            new(
                                id: "SanctuaryIcon",
                                anchor: Anchor.MiddleRight,
                                text: FontAwesomeIcon.Moon.ToIconString(),
                                style: new() {
                                    Font       = Font.FontAwesomeSmall,
                                    TextOffset = new(2, 0),
                                    TextColor  = Theme.Color(ThemeColor.TextMuted)
                                }
                            ),
                        ]
                    ),
                    new(
                        id: "Info",
                        size: new(225, 14),
                        text: "District Name Here",
                        style: new() {
                            Font         = Font.AxisExtraSmall,
                            TextAlign    = Anchor.TopRight,
                            TextColor    = Theme.Color(ThemeColor.TextMuted),
                            TextOffset   = new(0, -1),
                            OutlineColor = Theme.Color(ThemeColor.TextOutline),
                            OutlineWidth = 1,
                        }
                    ),
                ]
            ),
            new(
                id: "Icon",
                anchor: Anchor.MiddleCenter,
                size: new(28, 28),
                style: new() {
                    Image = 60277u,
                }
            ),
            new(
                id: "Weather",
                anchor: Anchor.MiddleCenter,
                flow: Flow.Vertical,
                size: new(225, 28),
                children: [
                    new(
                        id: "Name",
                        size: new(225, 14),
                        text: "Weather Name Here",
                        style: new() {
                            Font         = Font.AxisSmall,
                            TextAlign    = Anchor.BottomLeft,
                            TextColor    = Theme.Color(ThemeColor.Text),
                            TextOffset   = new(0, 0),
                            OutlineColor = Theme.Color(ThemeColor.TextOutline),
                            OutlineWidth = 1,
                        }
                    ),
                    new(
                        id: "Info",
                        size: new(225, 14),
                        text: "42 minutes",
                        style: new() {
                            Font         = Font.AxisExtraSmall,
                            TextAlign    = Anchor.TopLeft,
                            TextColor    = Theme.Color(ThemeColor.TextMuted),
                            TextOffset   = new(0, -1),
                            OutlineColor = Theme.Color(ThemeColor.TextOutline),
                            OutlineWidth = 1,
                        }
                    ),
                ]
            ),
        ]
    );
}
