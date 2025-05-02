using System;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets.Popup;

internal sealed partial class GearsetSwitcherPopup
{
    private Node LeftColumnNode   => Node.QuerySelector("#LeftColumn")!;
    private Node MiddleColumnNode => Node.QuerySelector("#MiddleColumn")!;
    private Node RightColumnNode  => Node.QuerySelector("#RightColumn")!;

    private Dictionary<string, Node>          ColumnNodesByName { get; } = [];
    private Dictionary<GearsetCategory, Node> GearsetGroupNodes { get; } = [];

    private void CreateGroupNodes()
    {
        ColumnNodesByName["LeftColumn"]   = LeftColumnNode;
        ColumnNodesByName["MiddleColumn"] = MiddleColumnNode;
        ColumnNodesByName["RightColumn"]  = RightColumnNode;

        CreateGroupNode(GearsetCategory.Tank, I18N.Translate("Widget.GearsetSwitcher.Role.Tank"));
        CreateGroupNode(GearsetCategory.Healer, I18N.Translate("Widget.GearsetSwitcher.Role.Healer"));
        CreateGroupNode(GearsetCategory.Melee, I18N.Translate("Widget.GearsetSwitcher.Role.Melee"));
        CreateGroupNode(GearsetCategory.Ranged, I18N.Translate("Widget.GearsetSwitcher.Role.PhysicalRanged"));
        CreateGroupNode(GearsetCategory.Caster, I18N.Translate("Widget.GearsetSwitcher.Role.MagicalRanged"));
        CreateGroupNode(GearsetCategory.Crafter, I18N.Translate("Widget.GearsetSwitcher.Role.Crafter"));
        CreateGroupNode(GearsetCategory.Gatherer, I18N.Translate("Widget.GearsetSwitcher.Role.Gatherer"));
    }

    private void UpdateGroupPositions()
    {
        UpdateGroupPosition(GearsetGroupNodes[GearsetCategory.Tank], _showTankGroup, _showTankGroupTitle, _tankGroupLocation, _tankGroupSortIndex, _tankGroupMaxChildren);
        UpdateGroupPosition(GearsetGroupNodes[GearsetCategory.Healer], _showHealerGroup, _showHealerGroupTitle, _healerGroupLocation, _healerGroupSortIndex, _healerGroupMaxChildren);
        UpdateGroupPosition(GearsetGroupNodes[GearsetCategory.Melee], _showMeleeGroup, _showMeleeGroupTitle, _meleeGroupLocation, _meleeGroupSortIndex, _meleeGroupMaxChildren);
        UpdateGroupPosition(GearsetGroupNodes[GearsetCategory.Ranged], _showRangedGroup, _showRangedGroupTitle, _rangedGroupLocation, _rangedGroupSortIndex, _rangedGroupMaxChildren);
        UpdateGroupPosition(GearsetGroupNodes[GearsetCategory.Caster], _showCasterGroup, _showCasterGroupTitle, _casterGroupLocation, _casterGroupSortIndex, _casterGroupMaxChildren);
        UpdateGroupPosition(GearsetGroupNodes[GearsetCategory.Crafter], _showCrafterGroup, _showCrafterGroupTitle, _crafterGroupLocation, _crafterGroupSortIndex, _crafterGroupMaxChildren);
        UpdateGroupPosition(GearsetGroupNodes[GearsetCategory.Gatherer], _showGathererGroup, _showGathererGroupTitle, _gathererGroupLocation, _gathererGroupSortIndex, _gathererGroupMaxChildren);
    }

    private void UpdateGroupPosition(Node groupNode, bool enabled, bool showTitle, string columnId, int sortIndex, int maxChildren)
    {
        groupNode.SortIndex       = sortIndex;
        groupNode.Style.IsVisible = enabled;

        Node bodyNode = groupNode.QuerySelector(".body")!;
        
        groupNode.QuerySelector(".title")!.Style.IsVisible = showTitle;

        if (_enableRoleScrolling) {
            int   count = Math.Min(maxChildren, bodyNode.ChildNodes.Count);
            float gap   = bodyNode.ComputedStyle.Gap;

            bodyNode.ToggleClass("scrolling", count < bodyNode.ChildNodes.Count);
            bodyNode.Overflow   = false;
            bodyNode.Style.Size = new Size(0, (40 * count) + (gap * (count - 1)));
        } else {
            bodyNode.ToggleClass("scrolling", false);
            bodyNode.Overflow   = true;
            bodyNode.Style.Size = null;
        }

        if (groupNode.ParentNode?.Id != columnId) {
            Node columnNode = ColumnNodesByName[columnId];
            columnNode.AppendChild(groupNode);
        }
    }

    private void CreateGroupNode(GearsetCategory category, string label)
    {
        Node groupNode = Document.CreateNodeFromTemplate("gearset-group", []);

        groupNode.Id                                 = $"GearsetGroup_{category}";
        groupNode.QuerySelector(".title")!.NodeValue = label;
        groupNode.QuerySelector(".body")!.Id         = $"GearsetList_{category}";

        LeftColumnNode.AppendChild(groupNode);

        GearsetGroupNodes.Add(category, groupNode);
    }
}
