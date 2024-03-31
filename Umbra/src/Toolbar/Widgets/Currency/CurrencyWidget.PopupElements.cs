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
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Gearset;

internal partial class CurrencyWidget
{
    private readonly DropdownElement _dropdownElement = new(
        id: "CurrencyPopupWidget",
        anchor: Anchor.MiddleLeft,
        children: [
            new(
                id: "Content",
                padding: new(2, 6),
                flow: Flow.Vertical
            )
        ]
    );

    private int _sortIndex;

    private Element CreateCurrencyRow(uint itemId)
    {
        _sortIndex++;

        var item = _dataManager.GetExcelSheet<Item>()!.GetRow(itemId);

        if (null == item) {
            throw new KeyNotFoundException($"Item {itemId} does not exist.");
        }

        Element button = new(
            id: $"Currency_{itemId}",
            size: new(0, 32),
            gap: 6,
            sortIndex: _sortIndex,
            children: [
                new(
                    id: "Icon",
                    anchor: Anchor.MiddleLeft,
                    size: new(20, 20),
                    style: new() {
                        Image = (uint)item.Icon,
                    }
                ),
                new(
                    id: "Name",
                    anchor: Anchor.MiddleLeft,
                    text: item.Name.ToString(),
                    padding: new(right: 64),
                    size: new(0, 20),
                    style: new() {
                        Font         = Font.Axis,
                        TextColor    = 0xFFC0C0C0,
                        OutlineColor = 0xAA000000,
                        OutlineWidth = 1,
                    }
                ),
                new(
                    id: "Value",
                    text: "99.999.999 / 99.999",
                    anchor: Anchor.MiddleRight,
                    size: new(100, 20),
                    style: new() {
                        Font         = Font.Axis,
                        TextAlign    = Anchor.MiddleRight,
                        TextColor    = 0xFFA0A0A0,
                        OutlineColor = 0x80000000,
                        OutlineWidth = 1,
                    }
                )
            ]
        );

        button.OnMouseEnter += () => button.Get("Name").Style.TextColor = 0xFFFFFFFF;
        button.OnMouseLeave += () => button.Get("Name").Style.TextColor = 0xFFC0C0C0;
        button.OnClick += () => {
            if (TrackedCurrencyId == itemId) {
                ConfigManager.Set("Toolbar.Widget.Currencies.TrackedCurrencyId", 0);
                return;
            }

            ConfigManager.Set("Toolbar.Widget.Currencies.TrackedCurrencyId", itemId);
        };

        return button;
    }
}
