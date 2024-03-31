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

namespace Umbra.Toolbar.Widgets.MainMenu;

internal sealed partial class BattleEffectsWidget
{
    private readonly DropdownElement _dropdownElement = new(
        id: "BattleEffectsPopupWidget",
        anchor: Anchor.MiddleRight,
        children: [
            new(
                id: "Header",
                text: "Battle effect visibility",
                size: new(0, 16),
                padding: new(top: 8),
                fit: true,
                style: new() {
                    Font         = Font.AxisLarge,
                    TextAlign    = Anchor.MiddleCenter,
                    TextColor    = 0xFFC0C0C0,
                    OutlineColor = 0xFF000000,
                    OutlineWidth = 1
                }
            ),
            new(
                id: "Content",
                padding: new(8, 0),
                flow: Flow.Vertical,
                children: [
                    CreateEffectControllerRow("Self",   "Self"),
                    CreateEffectControllerRow("Party",  "Party Members"),
                    CreateEffectControllerRow("Others", "Other Players"),
                    CreateEffectControllerRow("PvP",    "PvP Enemies"),
                ]
            ),
        ]
    );

    private static Element CreateEffectControllerRow(string id, string label)
    {
        return new(
            id: id,
            anchor: Anchor.TopLeft,
            size: new(0, 32),
            flow: Flow.Horizontal,
            gap: 16,
            children: [
                new(
                    id: "Label",
                    text: label,
                    size: new(110, 32),
                    anchor: Anchor.TopLeft,
                    style: new() {
                        Font         = Font.Axis,
                        TextAlign    = Anchor.MiddleRight,
                        TextColor    = 0xFFC0C0C0,
                        TextOffset   = new(0, -1),
                        OutlineColor = 0x80000000,
                        OutlineWidth = 1
                    }
                ),
                new(
                    id: "Buttons",
                    gap: 1,
                    flow: Flow.Horizontal,
                    size: new(0, 32),
                    padding: new(right: 8),
                    anchor: Anchor.TopLeft,
                    children: [
                        new ButtonElement("None",    "None",    isGhost: true) { Anchor = Anchor.MiddleLeft },
                        new ButtonElement("Limited", "Limited", isGhost: true) { Anchor = Anchor.MiddleLeft },
                        new ButtonElement("Full",    "Full",    isGhost: true) { Anchor = Anchor.MiddleLeft },
                    ]
                )
            ]
        );
    }
}
