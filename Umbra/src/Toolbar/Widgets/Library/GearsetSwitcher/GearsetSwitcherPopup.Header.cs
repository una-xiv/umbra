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

using ImGuiNET;
using Umbra.Common;
using Umbra.Game;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets;

public sealed partial class GearsetSwitcherPopup
{
    private void CreateHeaderNode()
    {
        ButtonNode updateButton    = new("Update", "Update", isSmall: true);
        ButtonNode duplicateButton = new("Duplicate", "Duplicate", isSmall: true);
        ButtonNode moveUpButton    = new("MoveUp", "Move Up", isSmall: true);
        ButtonNode moveDownButton  = new("MoveDown", "Move Down", isSmall: true);

        ButtonNode deleteButton = new("Delete", "Delete", isSmall: true) {
            IsGhost = true,
            Tooltip = I18N.Translate("Widget.GearsetSwitcher.DeleteButtonTooltip")
        };

        Node node = new() {
            Stylesheet = GearsetSwitcherHeaderStylesheet,
            Id         = "Header",
            ChildNodes = [
                new() {
                    Id = "HeaderIcon"
                },
                new() {
                    Id = "HeaderBody",
                    ChildNodes = [
                        new() {
                            Id        = "HeaderGearsetName",
                            NodeValue = "Gearset Name Here"
                        },
                        new() {
                            Id        = "HeaderGearsetInfo1",
                            NodeValue = "Level 100 Warrior • Item Level 660"
                        },
                        new() {
                            Id = "HeaderControls",
                            ChildNodes = [
                                updateButton,
                                duplicateButton,
                                moveUpButton,
                                moveDownButton,
                                deleteButton
                            ]
                        }
                    ]
                },
                new() {
                    Id        = "HeaderItemLevel",
                    NodeValue = "660"
                }
            ]
        };

        updateButton.OnMouseUp    += _ => _gearsetRepository.UpdateEquippedGearset();
        duplicateButton.OnMouseUp += _ => _gearsetRepository.DuplicateEquippedGearset();
        moveUpButton.OnMouseUp    += _ => _gearsetRepository.MoveEquippedGearsetUp();
        moveDownButton.OnMouseUp  += _ => _gearsetRepository.MoveEquippedGearsetDown();

        deleteButton.OnMouseUp += _ => {
            if (ImGui.GetIO().KeyShift) _gearsetRepository.DeleteEquippedGearset();
        };

        node.BeforeDraw += _ => {
            Gearset? gearset = _gearsetRepository.CurrentGearset;
            if (null == gearset) return;

            moveUpButton.IsDisabled   = _gearsetRepository.FindPrevIdInCategory(gearset) == null;
            moveDownButton.IsDisabled = _gearsetRepository.FindNextIdInCategory(gearset) == null;

            node.QuerySelector("#HeaderItemLevel")!.NodeValue    = gearset.ItemLevel.ToString();
            node.QuerySelector("#HeaderGearsetName")!.NodeValue  = gearset.Name;
            node.QuerySelector("#HeaderGearsetInfo1")!.NodeValue = GetCurrentGearsetStatusText();
        };

        node.BeforeReflow += _ => {
            int width = node.ParentNode!.InnerWidth - node.ComputedStyle.Padding.HorizontalSize;

            node.Bounds.ContentSize = new(width, 90);
            node.Bounds.PaddingSize = node.Bounds.ContentSize + new Size(node.ComputedStyle.Padding.HorizontalSize, 0);
            node.Bounds.MarginSize  = node.Bounds.PaddingSize + node.ComputedStyle.Margin.Size;

            var  size = Node.Bounds.ContentSize - new Size(0, 90);
            Node bg   = Node.QuerySelector("#Background")!;

            bg.Bounds.ContentSize = size.Copy();
            bg.Bounds.PaddingSize = size.Copy();
            bg.Bounds.MarginSize  = size.Copy();

            return true;
        };

        Node.AppendChild(node);
    }

    private string GetCurrentGearsetStatusText()
    {
        if (null == _currentGearset) return "";

        short jobLevel  = _currentGearset.JobLevel;
        short jobXp     = _currentGearset.JobXp;
        short itemLevel = _currentGearset.ItemLevel;

        return _currentGearset.IsMaxLevel
            ? I18N.Translate("Widget.GearsetSwitcher.ItemLevel", itemLevel)
            : $"{I18N.Translate("Widget.GearsetSwitcher.JobLevel", jobLevel)} - {I18N.Translate("Widget.GearsetSwitcher.JobXp", jobXp)}";
    }
}
