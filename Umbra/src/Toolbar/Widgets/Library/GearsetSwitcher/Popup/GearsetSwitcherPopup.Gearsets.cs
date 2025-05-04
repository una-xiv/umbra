using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets.Popup;

internal sealed partial class GearsetSwitcherPopup
{
    private readonly Dictionary<Node, Gearset> _nodeToGearset = [];

    private void UpdateGearsetButtons()
    {
        foreach (var gsNode in Node.QuerySelectorAll(".gearset")) {
            if (!(_nodeToGearset.TryGetValue(gsNode, out var gearset)) || !gearset.IsValid) continue;
            var jobInfo = Player.GetJobInfo(gearset.JobId);
            var barNode = gsNode.QuerySelector(".progress")!;

            gsNode.ToggleClass("capped", jobInfo.IsMaxLevel);
            gsNode.ToggleClass("active", CurrentGearset?.Id == gearset.Id);
            gsNode.ToggleClass("tank", gearset.Category == GearsetCategory.Tank);
            gsNode.ToggleClass("healer", gearset.Category == GearsetCategory.Healer);
            gsNode.ToggleClass("melee", gearset.Category == GearsetCategory.Melee);
            gsNode.ToggleClass("ranged", gearset.Category == GearsetCategory.Ranged);
            gsNode.ToggleClass("caster", gearset.Category == GearsetCategory.Caster);
            gsNode.ToggleClass("crafter", gearset.Category == GearsetCategory.Crafter);
            gsNode.ToggleClass("gatherer", gearset.Category == GearsetCategory.Gatherer);

            if (gearset.IsMaxLevel) {
                barNode.Style.IsVisible = false;
                continue;
            }

            float barWidth = barNode.Bounds.ContentSize.Width;
            float xpPct    = jobInfo.XpPercent;
            float barValue = (xpPct / 100f) * barWidth;

            barNode.Style.IsVisible                   = true;
            barNode.QuerySelector(".bar")!.Style.Size = new(barValue, 0);
        }
    }

    private void OnGearsetCreatedOrUpdated(Gearset gearset)
    {
        string  nodeId = $"Gearset_{gearset.Id}";
        Node?   node   = Node.QuerySelector($"#{nodeId}");
        Node    group  = GearsetGroupNodes[gearset.Category];
        JobInfo job    = Player.GetJobInfo(gearset.JobId);

        if (_hidePrefix.Length > 0 && gearset.Name.StartsWith(_hidePrefix)) {
            node?.Dispose();
            return;
        }
        
        if (null == node) {
            node = Document.CreateNodeFromTemplate("gearset", []);
            node.ToggleClass("with-gradient", _showGradientButtons);
            node.ToggleClass("gradient-tb", _gradientButtonType == "TB");
            node.ToggleClass("gradient-bt", _gradientButtonType == "BT");
            node.ToggleClass("gradient-lr", _gradientButtonType == "LR");
            node.ToggleClass("gradient-rl", _gradientButtonType == "RL");

            _nodeToGearset.Add(node, gearset);

            node.OnMouseUp += _ => {
                GearsetRepository.EquipGearset(gearset.Id);
                Close();
            };

            node.OnRightClick += _ => {
                UpdateContextMenuFor(gearset);
                ContextMenu!.Present();
            };
        }

        node.Id        = nodeId;
        node.SortIndex = gearset.Id;

        node.Style.Size                         = new(_buttonWidth, _buttonHeight);
        node.QuerySelector(".icon")!.Style.Size = new(_buttonHeight - 4, _buttonHeight - 4);

        node.QuerySelector(".progress-bar-wrapper")!.Style.Margin = new(_buttonHeight - 8, 8, 4, _buttonHeight - 3);
        
        node.QuerySelector(".icon")!.Style.IconId    = job.Icons[_buttonIconType];
        node.QuerySelector(".name")!.NodeValue       = gearset.Name;
        node.QuerySelector(".level")!.NodeValue      = GearsetSwitcherInfoDisplayProvider.GetInfoText(GearsetSwitcherInfoDisplayType.JobLevel, gearset, false);
        node.QuerySelector(".item-level")!.NodeValue = $"{gearset.ItemLevel}";

        group.QuerySelector(".body")!.AppendChild(node);
    }

    private void OnGearsetRemoved(Gearset gearset)
    {
        Node? node = Node.QuerySelector($"#Gearset_{gearset.Id}");
        if (node == null) return;

        node.Dispose();
        _nodeToGearset.Remove(node);
    }

    private void OnGearsetEquipped(Gearset gearset)
    {
    }
}
