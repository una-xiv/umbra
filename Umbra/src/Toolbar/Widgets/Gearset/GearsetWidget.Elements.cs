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

using Umbra.Drawing;
using Umbra.Drawing.Prefabs;
using Umbra.Game;

namespace Umbra;

internal sealed partial class GearsetWidget
{
    private const int GearsetWidth  = 250;
    private const int GearsetHeight = 32;

    public Element Element { get; } = new(
        id: "GearsetToolbarWidget",
        direction: Direction.Horizontal,
        anchor: Anchor.Middle | Anchor.Right,
        sortIndex: int.MinValue + 10,
        size: new(0, Height - 6),
        gap: 6,
        children: [
            new(
                id: "Icon",
                direction: Direction.Horizontal,
                size: new(Height - 6, Height - 6),
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
                    new IconNode(
                        iconId: 62031,
                        rounding: 7,
                        margin: new(3, 3, 3, 3),
                        uv1: new(0.15f, 0.15f),
                        uv2: new(0.85f, 0.85f)
                    )
                ]
            ),
            new(
                id: "Info",
                direction: Direction.Vertical,
                size: new(0, Height - 6),
                children: [
                    new(
                        id: "Name",
                        size: new(0, Height / 2 - 3),
                        nodes: [
                            new TextNode(
                                text: "Gearset Name",
                                font: Font.AxisSmall,
                                align: Align.BottomLeft,
                                color: 0xFFC0C0C0,
                                outlineColor: 0xFF000000,
                                outlineSize: 1,
                                offset: new(0, 0)
                            )
                        ]
                    ),
                    new(
                        id: "Sub",
                        size: new(0, Height / 2 - 3),
                        nodes: [
                            new TextNode(
                                text: "ILvl. 999",
                                font: Font.AxisExtraSmall,
                                align: Align.TopLeft,
                                color: 0xFFAAAAAA,
                                outlineColor: 0xAA000000,
                                outlineSize: 1,
                                offset: new(0, -1)
                            )
                        ]
                    )
                ]
            )
        ]
    );

    private readonly static Element HeaderElement = new(
        id: "Header",
        direction: Direction.Vertical,
        size: new(0, 80),
        fit: true,
        children: [
            new(
                id: "Body",
                direction: Direction.Horizontal,
                size: new(0, 79),
                fit: true,
                gap: 8,
                padding: new(left: 8, right: 8),
                nodes: [
                    new RectNode(
                        color: 0x00000000,
                        margin: new(left: -10, right: -10, bottom: GearsetHeight),
                        overflow: true,
                        gradients: new(
                            bottomLeft: 0x803247AC,
                            bottomRight: 0x803247AC
                        )
                    ),
                ],
                children: [
                    new(
                        id: "Icon",
                        anchor: Anchor.Middle | Anchor.Left,
                        size: new(64, 64),
                        nodes: [
                            new RectNode(
                                color: 0xFF1A1A1A,
                                borderColor: 0xFF101010,
                                borderSize: 1,
                                rounding: 4
                            ),
                            new IconNode(
                                iconId: 62131,
                                margin: new(1, 1, 1, 1)
                            ),
                            new RectNode(
                                color: 0,
                                borderColor: 0xFF3F3F3F,
                                borderSize: 1,
                                rounding: 3,
                                margin: new(1, 1, 1, 1)
                            ),
                        ]
                    ),
                    new(
                        id: "Info",
                        anchor: Anchor.Middle | Anchor.Left,
                        direction: Direction.Vertical,
                        size: new(0, 80),
                        children: [
                            new(
                                id: "Name",
                                size: new(0, 24),
                                padding: new(top: 10),
                                nodes: [
                                    new TextNode(
                                        text: "Gearset Name",
                                        font: Font.MiedingerLarge,
                                        align: Align.TopLeft,
                                        color: 0xFFC0C0C0,
                                        outlineColor: 0xFF000000,
                                        outlineSize: 1
                                    )
                                ]
                            ),
                            new(
                                id: "Job",
                                size: new(0, 20),
                                nodes: [
                                    new TextNode(
                                        text: "90 JobNameHere",
                                        font: Font.Axis,
                                        align: Align.TopLeft,
                                        color: 0xFF909090,
                                        outlineColor: 0xAA000000,
                                        outlineSize: 1,
                                        offset: new(0, -1)
                                    )
                                ]
                            ),
                            new(
                                id: "Buttons",
                                direction: Direction.Horizontal,
                                size: new(0, 32),
                                gap: 8,
                                children: [
                                    new ButtonElement("Update",    "Update",    isSmall: true),
                                    new ButtonElement("Duplicate", "Duplicate", isSmall: true),
                                    new ButtonElement("MoveUp",    "Move Up",   isSmall: true),
                                    new ButtonElement("MoveDown",  "Move Down", isSmall: true),
                                    new ButtonElement("Delete", "Delete", isSmall: true, isGhost: true)
                                        { Tooltip = "Right click to delete this gearset." }
                                ]
                            )
                        ]
                    ),
                    new(
                        id: "ItemLevel",
                        anchor: Anchor.Top | Anchor.Right,
                        size: new(0, 32),
                        padding: new(top: 10),
                        nodes: [
                            new TextNode(
                                text: "iLvl 999",
                                font: Font.MiedingerLarge,
                                align: Align.TopRight,
                                color: 0xFFC0C0C0,
                                outlineColor: 0xFF000000,
                                outlineSize: 1,
                                margin: new(right: 8)
                            )
                        ]
                    )
                ]
            ),
            new(
                id: "Border",
                direction: Direction.Horizontal,
                size: new(0, 1),
                fit: true,
                nodes: [
                    new RectNode(
                        color: 0xFF3F3F3F,
                        rounding: 0,
                        overflow: true
                    )
                ]
            )
        ]
    );

    private static readonly Element ColumnsElement = new(
        id: "Columns",
        direction: Direction.Horizontal,
        size: new(0, 0),
        gap: 4,
        padding: new(left: 4, right: 4),
        children: [
            new(
                id: "Left",
                direction: Direction.Vertical,
                size: new(GearsetWidth, 0),
                gap: 6,
                children: [
                    CreateGearsetCategoryElement(GearsetCategory.Tank),
                    CreateGearsetCategoryElement(GearsetCategory.Healer),
                    CreateGearsetCategoryElement(GearsetCategory.Melee),
                ]
            ),
            new(
                id: "Center",
                direction: Direction.Vertical,
                size: new(GearsetWidth, 0),
                gap: 6,
                children: [
                    CreateGearsetCategoryElement(GearsetCategory.Ranged),
                    CreateGearsetCategoryElement(GearsetCategory.Caster),
                ]
            ),
            new(
                id: "Right",
                direction: Direction.Vertical,
                size: new(GearsetWidth, 0),
                gap: 6,
                children: [
                    CreateGearsetCategoryElement(GearsetCategory.Crafter),
                    CreateGearsetCategoryElement(GearsetCategory.Gatherer),
                ]
            )
        ]
    );

    private readonly static DropdownElement DropdownElement = new(
        children: [
            HeaderElement,
            ColumnsElement
        ]
    );

    private static Element CreateGearsetCategoryElement(GearsetCategory category)
    {
        string name = category switch {
            GearsetCategory.Tank     => "Tanks",
            GearsetCategory.Healer   => "Healers",
            GearsetCategory.Melee    => "Melee",
            GearsetCategory.Ranged   => "Physical Ranged",
            GearsetCategory.Caster   => "Magical Ranged",
            GearsetCategory.Crafter  => "Crafters",
            GearsetCategory.Gatherer => "Gatherers",
            _                        => "Unknown"
        };

        return new(
            id: $"Category{category}",
            direction: Direction.Vertical,
            size: new(GearsetWidth, 0),
            gap: 6,
            children: [
                new(
                    id: "Title",
                    size: new(0, GearsetHeight),
                    nodes: [
                        new TextNode(
                            text: name,
                            font: Font.Jupiter,
                            align: Align.BottomCenter,
                            margin: new(left: 8),
                            color: 0xFFC0C0C0,
                            outlineColor: 0xFF000000,
                            outlineSize: 1
                        )
                    ]
                ),
                new(
                    id: "Gearsets",
                    direction: Direction.Vertical,
                    size: new(GearsetWidth, 0),
                    gap: 6
                )
            ]
        );
    }

    private Element CreateGearsetElement(Gearset gearset)
    {
        Element el = new(
            id: $"Gearset{gearset.Id}",
            size: new(GearsetWidth, GearsetHeight),
            anchor: Anchor.Top | Anchor.Left,
            direction: Direction.Horizontal,
            sortIndex: gearset.Id,
            gap: 8,
            nodes: [
                new RectNode(
                    color: 0x701A1A1A,
                    borderColor: 0xFF101010,
                    borderSize: 1,
                    rounding: 4
                ),
                new RectNode(
                    id: "CategoryColor",
                    color: GearsetCategoryRepository.GetCategoryColor(gearset.Category),
                    opacity: gearset.IsCurrent ? 0.45f : 0.15f,
                    rounding: 3,
                    margin: new(3)
                ),
                new RectNode(
                    id: "Border",
                    color: 0,
                    borderColor: 0xFF3F3F3F,
                    borderSize: 1,
                    rounding: 3,
                    margin: new(1, 1, 1, 1)
                ),
            ],
            children: [
                new(
                    id: "Icon",
                    anchor: Anchor.Top | Anchor.Left,
                    size: new(GearsetHeight, GearsetHeight),
                    nodes: [
                        new IconNode(
                            iconId: gearset.JobId + 62000u,
                            margin: new(6, 5, 3, 4)
                        ),
                        new LineNode(
                            direction: Direction.Vertical,
                            margin: new(left: GearsetHeight / 2 + 8, top: 1, bottom: 1),
                            color: 0xFF3F3F3F
                        )
                    ]
                ),
                new(
                    id: "Info",
                    anchor: Anchor.Top | Anchor.Left,
                    size: new(0, GearsetHeight),
                    direction: Direction.Vertical,
                    children: [
                        new(
                            id: "Name",
                            size: new(0, GearsetHeight / 2),
                            direction: Direction.Horizontal,
                            nodes: [
                                new TextNode(
                                    text: gearset.Name,
                                    font: Font.AxisSmall,
                                    align: Align.BottomLeft,
                                    color: gearset.IsCurrent ? 0xFFFFFFFF : 0xFFC0C0C0,
                                    outlineColor: 0xBB000000,
                                    outlineSize: 1,
                                    offset: new(0, 0)
                                )
                            ]
                        ),
                        new(
                            id: "Sub",
                            size: new(0, GearsetHeight / 2),
                            direction: Direction.Horizontal,
                            nodes: [
                                new TextNode(
                                    text: $"Level {gearset.JobLevel} {gearset.JobName}",
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
                ),
                new(
                    id: "ItemLevel",
                    anchor: Anchor.Top | Anchor.Right,
                    size: new(0, GearsetHeight),
                    direction: Direction.Horizontal,
                    nodes: [
                        new TextNode(
                            text: gearset.ItemLevel.ToString(),
                            font: Font.Miedinger,
                            align: Align.MiddleRight,
                            color: 0xFFC0C0C0,
                            outlineColor: 0xBB000000,
                            outlineSize: 1,
                            margin: new(right: 8),
                            offset: new(0, -1)
                        )
                    ]
                )
            ]
        );

        el.OnMouseEnter += () => el.GetNode<RectNode>("Border").Color = 0x25FFFFFF;
        el.OnMouseLeave += () => el.GetNode<RectNode>("Border").Color = 0;
        el.OnClick      += () => _repository.EquipGearset(gearset.Id);

        return el;
    }
}
