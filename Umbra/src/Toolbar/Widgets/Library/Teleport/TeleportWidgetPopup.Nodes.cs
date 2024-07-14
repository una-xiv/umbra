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

using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup : WidgetPopup
{
    private const int ColumnWidth = 300;

    protected override Node Node { get; } = new() {
        Id         = "Popup",
        Stylesheet = Stylesheet,
        ChildNodes = [new() { Id = "ExpansionList", }]
    };

    private void BuildNodes()
    {
        Node.ChildNodes = [new() { Id = "ExpansionList", }];

        foreach ((string xpKey, TeleportExpansion exp) in _expansions) {
            Node expNode = BuildExpansionNode(exp);
            Node regList = expNode.QuerySelector(".expansion-regions")!;

            foreach ((string regKey, TeleportRegion region) in exp.Regions) {
                Node regNode = BuildRegionNode(regList, regKey, region);
                Node dstList = regNode.QuerySelector(".region-destinations")!;

                foreach ((string dstKey, TeleportDestination destination) in region.Destinations) {
                    BuildDestinationNode(dstList, dstKey, destination);
                }
            }
        }
    }

    private Node BuildExpansionNode(TeleportExpansion expansion)
    {
        var expansionNode = new Node {
            ClassList = ["expansion"],
            SortIndex = expansion.SortIndex,
            ChildNodes = [
                new() {
                    ClassList = ["expansion-header"],
                    NodeValue = expansion.Name,
                },
                new() {
                    ClassList = ["expansion-regions"],
                }
            ]
        };

        Node.FindById("ExpansionList")!.AppendChild(expansionNode);

        return expansionNode;
    }

    private Node BuildRegionNode(Node target, string id, TeleportRegion region)
    {
        var regionNode = new Node {
            Id        = id,
            ClassList = ["region"],
            ChildNodes = [
                new() {
                    ClassList = ["region-header"],
                    NodeValue = $"{region.Name}",
                },
                new() {
                    ClassList = ["region-destinations"],
                    Style     = new() { IsVisible = true },
                }
            ]
        };

        target.AppendChild(regionNode);

        regionNode.QuerySelector(".region-header")!.OnMouseUp += _ => {
            var listNode = regionNode.QuerySelector(".region-destinations")!;
            listNode.Style.IsVisible = !listNode.Style.IsVisible;
        };

        return regionNode;
    }

    private Node BuildDestinationNode(Node target, string id, TeleportDestination destination)
    {
        var destinationNode = new Node {
            Id        = id,
            ClassList = ["destination"],
            ChildNodes = [
                new() {
                    ClassList   = ["destination-name"],
                    NodeValue   = destination.Name,
                    InheritTags = true,
                },
                new() {
                    ClassList   = ["destination-cost"],
                    NodeValue   = $"({destination.GilCost} gil)",
                    InheritTags = true,
                }
            ]
        };

        target.AppendChild(destinationNode);

        destinationNode.OnMouseUp += _ => {
            if (Framework.Service<IPlayer>().CanUseTeleportAction) {
                unsafe {
                    Telepo.Instance()->Teleport(destination.AetheryteId, destination.SubIndex);
                }

                Close();
            }
        };

        return destinationNode;
    }
}
