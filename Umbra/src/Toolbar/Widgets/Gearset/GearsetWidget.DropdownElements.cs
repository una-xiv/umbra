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
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Gearset;

internal partial class GearsetWidget
{
    private const int CellWidth  = 250;
    private const int CellHeight = 36;

    private readonly DropdownElement _dropdownElement = new(
        id: "GearsetDropdownWidget",
        anchor: Anchor.MiddleRight,
        flow: Flow.Vertical,
        children: [
            new(
                id: "Header",
                padding: new(top: 1, left: 1, right: 1, bottom: 0),
                flow: Flow.Horizontal,
                size: new(0, 80),
                fit: true,
                gap: 6,
                children: [
                    new GradientElement(gradient: Gradient.Vertical(0, 0x203599FF)),
                    new(
                        id: "Icon",
                        size: new(64, 64),
                        margin: new(left: 12),
                        anchor: Anchor.MiddleLeft,
                        children: [
                            new BackgroundElement(color: 0xFF101010, rounding: 6),
                            new BorderElement(color: 0xFF3F3F3F, rounding: 5, padding: new(1)),
                            new("Image", size: new(60, 60), anchor: Anchor.MiddleCenter, margin: new(left: 12)),
                        ]
                    ),
                    new(
                        id: "Info",
                        flow: Flow.Vertical,
                        anchor: Anchor.MiddleLeft,
                        stretch: true,
                        children: [
                            new(
                                id: "Name",
                                text: "",
                                size: new(0, 25),
                                style: new() {
                                    Font         = Font.Axis,
                                    TextAlign    = Anchor.TopLeft,
                                    TextColor    = 0xFFC0C0C0,
                                    TextOffset   = new(0, -2),
                                    OutlineColor = 0x80000000,
                                    OutlineWidth = 1
                                }
                            ),
                            new(
                                id: "Job",
                                text: "",
                                size: new(0, 20),
                                padding: new(left: 2),
                                style: new() {
                                    Font         = Font.Axis,
                                    TextAlign    = Anchor.TopLeft,
                                    TextColor    = 0xFF909090,
                                    TextOffset   = new(0, -1),
                                    OutlineColor = 0x80000000,
                                    OutlineWidth = 1
                                }
                            ),
                            new(
                                id: "Buttons",
                                size: new(0, 18),
                                gap: 4,
                                children: [
                                    new ButtonElement("Update",    I18N.Translate("GearsetWidget.Update"),    isSmall: true),
                                    new ButtonElement("Duplicate", I18N.Translate("GearsetWidget.Duplicate"), isSmall: true),
                                    new ButtonElement("MoveUp",    I18N.Translate("GearsetWidget.MoveUp"),   isSmall: true),
                                    new ButtonElement("MoveDown",  I18N.Translate("GearsetWidget.MoveDown"), isSmall: true),
                                    new ButtonElement("Delete", I18N.Translate("GearsetWidget.Delete"), isSmall: true, isGhost: true)
                                        { Tooltip = I18N.Translate("GearsetWidget.Delete.Tooltip") },
                                ]
                            )
                        ]
                    ),
                    new(
                        id: "ItemLevel",
                        text: "999",
                        anchor: Anchor.MiddleRight,
                        margin: new(right: 12),
                        style: new() {
                            Font         = Font.MiedingerLarge,
                            TextAlign    = Anchor.MiddleRight,
                            TextColor    = 0xFFC0C0C0,
                            OutlineColor = 0xFF000000,
                            OutlineWidth = 1
                        }
                    )
                ]
            ),
            new(
                id: "Columns",
                padding: new(left: 6, right: 6, bottom: 6),
                flow: Flow.Horizontal,
                gap: 6,
                fit: true,
                children: [
                    new GradientElement(gradient: Gradient.Vertical(0x303599FF, 0), padding: new(0, -6)),
                    BuildColumn(
                        "Left",
                        [
                            BuildGroup("Tank",   I18N.Translate("GearsetWidget.Tank")),
                            BuildGroup("Healer", I18N.Translate("GearsetWidget.Healer")),
                            BuildGroup("Melee",  I18N.Translate("GearsetWidget.Melee")),
                        ]
                    ),
                    BuildColumn(
                        "Middle",
                        [
                            BuildGroup("Ranged", I18N.Translate("GearsetWidget.PhysicalRanged")),
                            BuildGroup("Caster", I18N.Translate("GearsetWidget.MagicalRanged")),
                        ]
                    ),
                    BuildColumn(
                        "Right",
                        [
                            BuildGroup("Crafter",  I18N.Translate("GearsetWidget.Crafter")),
                            BuildGroup("Gatherer", I18N.Translate("GearsetWidget.Gatherer")),
                        ]
                    ),
                ]
            )
        ]
    );

    private Element LeftColumn => _dropdownElement.Get("Columns.Left");
    private Element MiddleColumn => _dropdownElement.Get("Columns.Middle");
    private Element RightColumn => _dropdownElement.Get("Columns.Right");
    private Element TankGroup => _dropdownElement.Get("Columns.Left.Tank");
    private Element HealerGroup => _dropdownElement.Get("Columns.Left.Healer");
    private Element MeleeGroup => _dropdownElement.Get("Columns.Left.Melee");
    private Element RangedGroup => _dropdownElement.Get("Columns.Middle.Ranged");
    private Element CasterGroup => _dropdownElement.Get("Columns.Middle.Caster");
    private Element CrafterGroup => _dropdownElement.Get("Columns.Right.Crafter");
    private Element GathererGroup => _dropdownElement.Get("Columns.Right.Gatherer");

    private static Element BuildColumn(string id, List<Element> groups)
    {
        return new(
            id: id,
            padding: new(1),
            size: new(CellWidth, 0),
            flow: Flow.Vertical,
            gap: 6,
            children: groups
        ) { IsVisible = false };
    }

    private static Element BuildGroup(string id, string title)
    {
        return new(
            id: id,
            flow: Flow.Vertical,
            gap: 6,
            size: new(CellWidth, 0),
            children: [
                new(
                    id: "Title",
                    text: title,
                    size: new(CellWidth, CellHeight),
                    style: new() {
                        Font         = Font.AxisLarge,
                        TextAlign    = Anchor.MiddleCenter,
                        TextColor    = 0xFFC0C0C0,
                        TextOffset   = new(0, 1),
                        OutlineColor = 0x80000000,
                        OutlineWidth = 1
                    }
                ),
                new(
                    id: "List",
                    flow: Flow.Vertical,
                    size: new(CellWidth, 0),
                    gap: 6,
                    children: []
                )
            ]
        );
    }

    private Element BuildGearset(Game.Gearset gearset)
    {
        var el = new Element(
            id: $"Gearset_{gearset.Id}",
            size: new(CellWidth, CellHeight),
            anchor: Anchor.TopLeft,
            flow: Flow.Horizontal,
            gap: 5,
            children: [
                new BackgroundElement(color: 0x10FFFFFF, rounding: 6, edgeColor: 0xFF000000, edgeThickness: 1),
                new BorderElement(color: 0xFF3F3F3F, rounding: 5, padding: new(1)),
                new(
                    id: "Icon",
                    size: new(CellHeight - 8, CellHeight - 8),
                    margin: new(left: 4),
                    anchor: Anchor.MiddleLeft,
                    children: [
                        new BackgroundElement(color: 0xFF101010, edgeColor: 0xFF101010, edgeThickness: 1, rounding: 4),
                        new(
                            "Image",
                            size: new(CellHeight - 12, CellHeight - 12),
                            anchor: Anchor.MiddleCenter,
                            margin: new(left: 4),
                            style: new() {
                                ImageRounding  = 4,
                                ImageUVs       = new(.1f, .1f, .9f, .9f),
                                ImageGrayscale = 1f,
                                ImageOffset    = new(0, 1),
                            }
                        ),
                    ]
                ),
                new(
                    id: "Info",
                    flow: Flow.Vertical,
                    anchor: Anchor.MiddleLeft,
                    stretch: true,
                    children: [
                        new(
                            id: "Name",
                            text: gearset.Name,
                            size: new(0, CellHeight / 2),
                            style: new() {
                                Font         = Font.Axis,
                                TextAlign    = Anchor.BottomLeft,
                                TextColor    = 0xFFC0C0C0,
                                TextOffset   = new(0, 2),
                                OutlineColor = 0x80000000,
                                OutlineWidth = 1
                            }
                        ),
                        new(
                            id: "Job",
                            text: $"Level {gearset.JobLevel} {gearset.JobName}",
                            size: new(0, CellHeight / 2),
                            style: new() {
                                Font         = Font.AxisSmall,
                                TextAlign    = Anchor.TopLeft,
                                TextColor    = 0xFF909090,
                                TextOffset   = new(1, 0),
                                OutlineColor = 0x80000000,
                                OutlineWidth = 1
                            }
                        )
                    ]
                ),
                new(
                    id: "ItemLevel",
                    text: gearset.ItemLevel.ToString(),
                    anchor: Anchor.TopRight,
                    margin: new(top: 6, right: 6),
                    style: new() {
                        Font         = Font.Miedinger,
                        TextAlign    = Anchor.TopRight,
                        TextColor    = 0xFFC0C0C0,
                        OutlineColor = 0xFF000000,
                        OutlineWidth = 1
                    }
                ),
                new(
                    id: "XpBarWrapper",
                    size: new(CellWidth - 80, 5),
                    anchor: Anchor.BottomRight,
                    padding: new(bottom: 6, right: 8),
                    children: [
                        new(
                            id: "XpBar",
                            size: new(CellWidth - 80, 5),
                            anchor: Anchor.BottomRight,
                            style: new() {
                                BackgroundColor = 0xFF151515,
                            },
                            children: [
                                new(
                                    id: "Bar",
                                    size: new(100, 5),
                                    anchor: Anchor.TopLeft,
                                    padding: new(1),
                                    style: new() {
                                        BackgroundColor = 0xFF59CADE,
                                    }
                                )
                            ]
                        )
                    ]
                )
            ]
        );

        el.OnMouseEnter += () => el.Get<BackgroundElement>().EdgeColor = 0xFF505050;
        el.OnMouseLeave += () => el.Get<BackgroundElement>().EdgeColor = 0xFF000000;
        el.OnClick      += () => _gearsetRepository.EquipGearset(gearset.Id);

        return el;
    }
}
