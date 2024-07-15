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

using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using SkiaSharp;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonRelicNoteBook;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    protected override Node Node { get; } = new() {
        Id         = "Popup",
        Stylesheet = Stylesheet,
        ChildNodes = [],
    };

    private static Dictionary<string, Node> ExpansionLists { get; set; } = [];

    private void BuildNodes()
    {
        Una.Drawing.Style alignmentStyle = new() {
            Anchor = Toolbar.IsTopAligned ? Anchor.TopLeft : Anchor.BottomLeft,
        };

        Node.ChildNodes = [
            new() { Id = "ExpansionList", Style   = alignmentStyle },
            new() { Id = "DestinationList", Style = alignmentStyle }
        ];

        Node expansionList = Node.FindById("ExpansionList")!;
        expansionList.AppendChild(BuildTitleNode(!Toolbar.IsTopAligned));

        foreach (TeleportExpansion expansion in _expansions.Values) {
            BuildExpansionNode(expansionList, expansion, !Toolbar.IsTopAligned);
        }
    }

    private static Node BuildTitleNode(bool reverse)
    {
        return new() {
            Id        = "Title",
            Style     = new() { Anchor = reverse ? Anchor.BottomLeft : Anchor.TopLeft },
            SortIndex = reverse ? int.MinValue : int.MaxValue,
            ChildNodes = [
                new() { Id = "TitleIcon" },
                new() { Id = "TitleText", NodeValue = I18N.Translate("Widget.Teleport.Name"), },
            ]
        };
    }

    private void BuildExpansionNode(Node targetNode, TeleportExpansion expansion, bool reverse)
    {
        Node node = new() {
            Id        = expansion.NodeId,
            NodeValue = expansion.Name,
            SortIndex = reverse ? 1 - expansion.SortIndex : expansion.SortIndex,
            ClassList = ["expansion"],
            Style     = new() { Anchor = reverse ? Anchor.BottomLeft : Anchor.TopLeft }
        };

        node.OnMouseUp += n => ActivateExpansion(n.Id!);
        targetNode.AppendChild(node);

        Node destinationList = new() { Id = "RegionContainer" };
        ExpansionLists[expansion.NodeId] = destinationList;

        foreach (var region in expansion.Regions.Values) {
            BuildRegionNode(destinationList, region);
        }

        while (ExpansionLists[expansion.NodeId].ChildNodes.Count < MinimumColumns) {
            ExpansionLists[expansion.NodeId]
                .AppendChild(
                    new() {
                        ClassList = ["region"],
                        ChildNodes = [
                            new() {
                                ClassList = ["region-spacer"]
                            }
                        ]
                    }
                );
        }
    }

    private void BuildRegionNode(Node targetNode, TeleportRegion region)
    {
        Node regionNode = new() {
            ClassList = ["region"],
            ChildNodes = [
                new() {
                    ClassList = ["region-header"],
                    NodeValue = region.Name,
                },
                new() {
                    ClassList = ["region-destinations"],
                }
            ]
        };

        targetNode.AppendChild(regionNode);
        Node destinationList = regionNode.QuerySelector(".region-destinations")!;

        foreach (var map in region.Maps.Values) {
            BuildMapNode(destinationList, map);
        }
    }

    private void BuildMapNode(Node targetNode, TeleportMap map)
    {
        Node mapNode = new() {
            ClassList = ["map"],
            ChildNodes = [
                new() {
                    ClassList = ["map-header"],
                    NodeValue = map.Name,
                },
                new() {
                    ClassList = ["map-destinations"],
                }
            ]
        };

        targetNode.AppendChild(mapNode);
        Node destinationList = mapNode.QuerySelector(".map-destinations")!;

        foreach (var destination in map.Destinations.Values) {
            BuildDestinationNode(destinationList, destination);
        }
    }

    private void BuildDestinationNode(Node targetNode, TeleportDestination destination)
    {
        Node node = new() {
            ClassList = ["destination"],
            SortIndex = destination.SortIndex,
            ChildNodes = [
                new() {
                    ClassList   = ["destination-icon"],
                    Style       = new()
                    {
                        UldPartId = destination.UldPartId, 
                        UldPartsId = 15, 
                        UldResource = "ui/uld/Teleport"
                    },
                    InheritTags = true,
                },
                new() {
                    ClassList   = ["destination-name"],
                    NodeValue   = destination.Name,
                    InheritTags = true,
                },
                new() {
                    ClassList   = ["destination-cost"],
                    NodeValue   = $"{destination.GilCost} gil",
                    InheritTags = true,
                }
            ]
        };

        targetNode.AppendChild(node);

        node.OnMouseUp += _ => {
            if (!Framework.Service<IPlayer>().CanUseTeleportAction) return;

            Framework
                .Service<IToastGui>()
                .ShowQuest(
                    $"{I18N.Translate("Widget.Teleport.Name")}: {destination.Name}",
                    new() { IconId = 111, PlaySound = true, DisplayCheckmark = false }
                );

            unsafe {
                Telepo.Instance()->Teleport(destination.AetheryteId, destination.SubIndex);
            }

            Close();
        };
    }
}
