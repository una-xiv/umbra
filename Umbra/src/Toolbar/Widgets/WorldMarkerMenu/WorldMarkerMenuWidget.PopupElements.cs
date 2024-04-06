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

using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.MainMenu;

internal sealed partial class WorldMarkerMenuWidget
{
    private readonly DropdownElement _dropdownElement = new(
        id: "WorldMarkerMenuToolbarWidget",
        anchor: Anchor.MiddleRight,
        children: [
            new(
                id: "Header",
                padding: new(8, bottom: 0),
                text: "World Markers",
                fit: true,
                style: new() {
                    Font         = Font.AxisLarge,
                    TextAlign    = Anchor.MiddleCenter,
                    TextOffset   = new(0, -1),
                    TextColor    = 0xFFC0C0C0,
                    OutlineColor = 0x80000000,
                    OutlineWidth = 1,
                }
            ),
            new(
                id: "Container",
                flow: Flow.Vertical,
                anchor: Anchor.TopLeft,
                padding: new(8),
                children: []
            )
        ]
    );

    private static Element CreateMarkerButton(Cvar cvar)
    {
        Element button = new(
            id: cvar.Id.Replace('.', '_'),
            flow: Flow.Horizontal,
            anchor: Anchor.TopLeft,
            size: new(0, 32),
            gap: 8,
            children: [
                new(
                    id: "Checkbox",
                    anchor: Anchor.MiddleLeft,
                    size: new(24, 24),
                    children: [
                        new BackgroundElement(color: 0xFF2F2F2F, edgeColor: 0xFF2F2F2F, edgeThickness: 1, rounding: 4),
                        new BorderElement(color: 0xFF101010, padding: new(1), rounding: 3),
                        new(
                            id: "Check",
                            size: new(24, 24),
                            anchor: Anchor.MiddleCenter,
                            text: "✔",
                            style: new() {
                                Font         = Font.FontAwesome,
                                TextAlign    = Anchor.MiddleCenter,
                                TextOffset   = new(0, -1),
                                TextColor    = 0xFFC0C0C0,
                                OutlineColor = 0x80000000,
                                OutlineWidth = 1,
                            }
                        )
                    ]
                ),
                new(
                    id: "Label",
                    anchor: Anchor.MiddleLeft,
                    size: new(0, 24),
                    text: I18N.Translate($"CVAR.{cvar.Id}.Name"),
                    padding: new(right: 8),
                    style: new() {
                        Font         = Font.Axis,
                        TextAlign    = Anchor.MiddleLeft,
                        TextOffset   = new(0, -1),
                        TextColor    = 0xFFC0C0C0,
                        OutlineColor = 0x80000000,
                        OutlineWidth = 1,
                    }
                )
            ]
        );

        button.OnBeforeCompute += () => {
            var enabled = (bool)cvar.Value!;
            button.Get("Checkbox.Check").IsVisible = enabled;
        };

        button.OnClick      += () => ConfigManager.Set(cvar.Id, !(bool)cvar.Value!);
        button.OnMouseEnter += () => button.Get("Label").Style.TextColor = 0xFFFFFFFF;
        button.OnMouseLeave += () => button.Get("Label").Style.TextColor = 0xFFC0C0C0;

        return button;
    }
}
