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

using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

internal sealed partial class GearsetSwitcherPopup
{
    private void CreateHeaderNode()
    {
        const string uld = "ui/uld/Character";

        Node bestInSlotButton = CreateUldHeaderButtonNode(
            "BestInSlot",
            uld,
            13,
            2,
            I18N.Translate("Widget.GearsetSwitcher.EquipRecommendedGear")
        );

        Node openGlamButton = CreateUldHeaderButtonNode(
            "OpenGlam",
            uld,
            13,
            3,
            I18N.Translate("Widget.GearsetSwitcher.ContextMenu.LinkGlamourPlate")
        );

        Node updateButton = CreateUldHeaderButtonNode(
            "Update",
            uld,
            15,
            4,
            I18N.Translate("Widget.GearsetSwitcher.UpdateGearset")
        );

        Node duplicateButton = CreateUldHeaderButtonNode(
            "Duplicate",
            uld,
            15,
            8,
            I18N.Translate("Widget.GearsetSwitcher.CreateGearset")
        );

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
                            Id        = "HeaderGearsetInfo",
                            NodeValue = "Level 100 Warrior • Item Level 660"
                        },
                        new() {
                            Id = "HeaderControls",
                            ChildNodes = [
                                bestInSlotButton,
                                openGlamButton,
                                updateButton,
                                duplicateButton,
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

        openGlamButton.OnMouseUp += _ => {
            _player.UseGeneralAction(25); // Glamour Plate.
            Close();
        };

        bestInSlotButton.OnMouseUp += _ => {
            unsafe {
                AgentModule.Instance()->GetAgentByInternalId(AgentId.RecommendEquip)->Show();
            }

            Close();
        };

        node.BeforeDraw += _ => {
            Gearset? gearset = _gearsetRepository.CurrentGearset;
            if (null == gearset) return;

            node.QuerySelector("#HeaderItemLevel")!.NodeValue   = gearset.ItemLevel.ToString();
            node.QuerySelector("#HeaderGearsetName")!.NodeValue = gearset.Name;
            node.QuerySelector("#HeaderGearsetInfo")!.NodeValue = GetCurrentGearsetStatusText();
        };

        node.BeforeReflow += _ => {
            int width = node.ParentNode!.InnerWidth - node.ComputedStyle.Padding.HorizontalSize;

            node.Bounds.ContentSize = new(width, (int)(90 * Node.ScaleFactor));
            node.Bounds.PaddingSize = node.Bounds.ContentSize + new Size(node.ComputedStyle.Padding.HorizontalSize, 0);
            node.Bounds.MarginSize  = node.Bounds.PaddingSize + node.ComputedStyle.Margin.Size;

            var  size = Node.Bounds.ContentSize - new Size(0, (int)(90 * Node.ScaleFactor));
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

        short  jobLevel  = _currentGearset.JobLevel;
        short  jobXp     = _currentGearset.JobXp;
        short  itemLevel = _currentGearset.ItemLevel;
        string jobName   = _currentGearset.JobName != _currentGearset.Name ? $" {_currentGearset.JobName}" : "";

        return _currentGearset.IsMaxLevel
            ? $"{I18N.Translate("Widget.GearsetSwitcher.JobLevel", jobLevel)}{jobName} - {I18N.Translate("Widget.GearsetSwitcher.ItemLevel", itemLevel)}"
            : $"{I18N.Translate("Widget.GearsetSwitcher.JobLevel", jobLevel)}{jobName} - {I18N.Translate("Widget.GearsetSwitcher.JobXp", jobXp)}";
    }

    private Node CreateUldHeaderButtonNode(
        string id, string uld, int partsId, int partId, string tooltip, short rotation = 0
    )
    {
        Node node = new() {
            Id        = id,
            ClassList = ["header-button"],
            Style = new() {
                UldResource   = uld,
                UldPartsId    = partsId,
                UldPartId     = partId,
                UldStyle      = UldStyle.Light,
                ImageRotation = rotation,
            }
        };

        node.Tooltip = tooltip;

        return node;
    }
}
