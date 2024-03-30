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

using Lumina.Excel.GeneratedSheets;
using Umbra.Drawing;
using Umbra.Drawing.Prefabs;

namespace Umbra;

internal sealed partial class CurrencyWidget
{
    public Element Element { get; } = new(
        id: "CurrencyToolbarWidget",
        anchor: Anchor.Middle | Anchor.Left,
        direction: Direction.Horizontal,
        sortIndex: int.MaxValue,
        size: new(0, Height - 6),
        gap: 6,
        children: [
            new(
                id: "Icon",
                size: new(Height - 6, Height - 6),
                direction: Direction.Horizontal,
                nodes: [
                    new IconNode(
                        iconId: 65002,
                        margin: new(2)
                    )
                ]
            ),
            new(
                id: "Info",
                size: new(0, Height - 6),
                direction: Direction.Vertical,
                children: [
                    new(
                        id: "Name",
                        size: new(0, Height / 2 - 3),
                        fit: true,
                        nodes: [
                            new TextNode(
                                text: "Currency",
                                font: Font.AxisSmall,
                                align: Align.BottomLeft,
                                color: 0xFFD0D0D0,
                                outlineColor: 0xAA000000,
                                outlineSize: 1,
                                offset: new(0, 0)
                            )
                        ]
                    ),
                    new(
                        id: "Sub",
                        size: new(0, Height / 2 - 3),
                        fit: true,
                        nodes: [
                            new TextNode(
                                text: "99.999,999",
                                font: Font.AxisExtraSmall,
                                align: Align.TopLeft,
                                color: 0xFFA0A0A0,
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

    private readonly DropdownElement _dropdownElement = new(
        id: "CurrencyDropdown",
        anchor: Anchor.Middle | Anchor.Left,
        children: [
            new(
                id: "CurrencyList",
                direction: Direction.Vertical,
                children: []
            )
        ]
    );

    private Element CreateCurrencyRow(uint itemId)
    {
        var item = _dataManager.GetExcelSheet<Item>()!.GetRow(itemId);

        return new(
            id: $"CurrencyRow_{itemId}",
            direction: Direction.Horizontal,
            size: new(0, 28),
            gap: 6,
            sortIndex: 0,
            children: [
                new(
                    id: "Icon",
                    size: new(28, 28),
                    nodes: [
                        new IconNode(
                            iconId: item!.Icon,
                            margin: new(2)
                        )
                    ]
                ),
                new(
                    id: "Name",
                    size: new(200, 28),
                    direction: Direction.Horizontal,
                    nodes: [
                        new TextNode(
                            text: item.Name.ToString(),
                            font: Font.Axis,
                            align: Align.MiddleLeft,
                            color: 0xFFD0D0D0,
                            outlineColor: 0xAA000000,
                            outlineSize: 1,
                            autoSize: false,
                            offset: new(0, -1)
                        )
                    ]
                ),
                new(
                    id: "Amount",
                    size: new(100, 28),
                    nodes: [
                        new TextNode(
                            text: "99.999,999",
                            font: Font.Axis,
                            align: Align.MiddleRight,
                            color: 0xFFA0A0A0,
                            outlineColor: 0x80000000,
                            outlineSize: 1,
                            autoSize: false,
                            offset: new(0, -1),
                            margin: new(right: 6)
                        )
                    ]
                )
            ]
        );
    }
}
