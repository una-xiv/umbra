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

internal sealed partial class BattleEffectsWidget
{
    private readonly DropdownElement _dropdownElement = new(
        id: "BattleEffectsPopupWidget",
        anchor: Anchor.MiddleRight,
        children: [
            new(
                id: "Header",
                text: I18N.Translate("BattleEffectsWidget.Title"),
                size: new(0, 16),
                padding: new(top: 8),
                fit: true,
                style: new() {
                    Font         = Font.AxisLarge,
                    TextAlign    = Anchor.MiddleCenter,
                    TextColor    = Theme.Color(ThemeColor.Text),
                    OutlineColor = Theme.Color(ThemeColor.TextOutlineLight),
                    OutlineWidth = 1
                }
            ),
            new(
                id: "Content",
                padding: new(8, 0),
                flow: Flow.Vertical,
                children: [
                    CreateEffectControllerRow("Self",   I18N.Translate("BattleEffectsWidget.Self")),
                    CreateEffectControllerRow("Party",  I18N.Translate("BattleEffectsWidget.Party")),
                    CreateEffectControllerRow("Others", I18N.Translate("BattleEffectsWidget.Others")),
                    CreateEffectControllerRow("PvP",    I18N.Translate("BattleEffectsWidget.PvP")),
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
                        TextColor    = Theme.Color(ThemeColor.Text),
                        TextOffset   = new(0, -1),
                        OutlineColor = Theme.Color(ThemeColor.TextOutline),
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
                        new ButtonElement("None", I18N.Translate("BattleEffectsWidget.None"), isGhost: true)
                            { Anchor = Anchor.MiddleLeft },
                        new ButtonElement("Limited", I18N.Translate("BattleEffectsWidget.Limited"), isGhost: true)
                            { Anchor = Anchor.MiddleLeft },
                        new ButtonElement("Full", I18N.Translate("BattleEffectsWidget.Full"), isGhost: true)
                            { Anchor = Anchor.MiddleLeft },
                    ]
                )
            ]
        );
    }
}
