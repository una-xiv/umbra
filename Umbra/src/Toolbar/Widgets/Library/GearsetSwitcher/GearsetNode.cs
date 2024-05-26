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
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

public partial class GearsetNode : Node
{
    public const int NodeWidth  = 200;
    public const int NodeHeight = 40;

    private readonly IGearsetRepository _repository;
    private readonly Gearset            _gearset;

    public GearsetNode(IGearsetRepository repository, Gearset gearset)
    {
        _repository = repository;
        _gearset    = gearset;

        Stylesheet = GearsetSwitcherItemStylesheet;
        ClassList  = ["gearset"];

        OnMouseUp += _ => _repository.EquipGearset(gearset.Id);

        ChildNodes = [
            new() {
                Id        = "Icon",
                ClassList = ["gearset--icon"],
                Style     = new() { IconId = gearset.JobId + 62000u }
            },
            new() {
                ClassList = ["gearset--body"],
                ChildNodes = [
                    new() {
                        Id        = "Name",
                        ClassList = ["gearset--body--name"],
                        NodeValue = gearset.Name
                    },
                    new() {
                        Id        = "Info",
                        ClassList = ["gearset--body--info"],
                        NodeValue = $"Level {gearset.JobLevel} {gearset.JobName}"
                    }
                ]
            },
            new() {
                Id        = "ItemLevel",
                ClassList = ["gearset--ilvl"],
                NodeValue = gearset.Name
            },
        ];
    }

    public bool UseAlternateButtonIcon { get; set; } = false;
    public int  ButtonIconYOffset      { get; set; } = 0;

    public void Update()
    {
        IconNode.Style.IconId      = _gearset.JobId + 62000u + (UseAlternateButtonIcon ? 100u : 0u);
        IconNode.Style.ImageOffset = new(0, ButtonIconYOffset);

        NameNode.NodeValue = _gearset.Name;
        InfoNode.NodeValue = GetCurrentGearsetStatusText();
        IlvlNode.NodeValue = _gearset.ItemLevel.ToString();

        if (_gearset == _repository.CurrentGearset && !TagsList.Contains("current")) {
            TagsList.Add("current");
        } else if (_gearset != _repository.CurrentGearset && TagsList.Contains("current")) {
            TagsList.Remove("current");
        }
    }

    private Node IconNode => QuerySelector("Icon")!;
    private Node NameNode => QuerySelector("Name")!;
    private Node InfoNode => QuerySelector("Info")!;
    private Node IlvlNode => QuerySelector("ItemLevel")!;

    private string GetCurrentGearsetStatusText()
    {
        return $"{I18N.Translate("Widget.GearsetSwitcher.JobLevel", _gearset.JobLevel)} {_gearset.JobName}";
    }
}
