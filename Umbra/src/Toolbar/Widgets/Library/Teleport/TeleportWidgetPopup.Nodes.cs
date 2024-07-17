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
using Una.Drawing;

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
            Anchor = Toolbar.IsTopAligned
                ? (ExpansionMenuPosition == "Left" ? Anchor.TopLeft : Anchor.TopRight)
                : (ExpansionMenuPosition == "Left" ? Anchor.BottomLeft : Anchor.BottomRight),
        };

        Node.ChildNodes = [
            new() { Id = "ExpansionList", Style   = alignmentStyle },
            new() { Id = "DestinationList", Style = alignmentStyle }
        ];

        Node expansionList   = Node.FindById("ExpansionList")!;
        Node destinationList = Node.FindById("DestinationList")!;

        foreach (TeleportExpansion expansion in _expansions.Values) {
            BuildExpansionNode(expansionList, destinationList, expansion, !Toolbar.IsTopAligned);
        }

        BuildFavoritesNodes(expansionList, destinationList);
    }

    private void BuildFavoritesNodes(Node expansionList, Node destinationList)
    {
        Node node = new() {
            Id        = "Favorites",
            NodeValue = I18N.Translate("Widget.Teleport.Favorites"),
            SortIndex = int.MinValue + 1,
            ClassList = ["expansion"],
            Style = new() {
                Anchor = !Toolbar.IsTopAligned ? Anchor.BottomLeft : Anchor.TopLeft,
                Margin = new() {
                    Top    = !Toolbar.IsTopAligned ? 16 : 0,
                    Bottom = Toolbar.IsTopAligned ? 16 : 0,
                }
            }
        };

        expansionList.AppendChild(node);
        node.OnMouseUp += n => ActivateExpansion(n.Id!);

        Node destinationListFavorites = new() {
            Id        = "Favorites",
            ClassList = ["region-container"],
            Style     = new() { IsVisible = false },
            ChildNodes = [
                new() {
                    ClassList = ["region"],
                    ChildNodes = [
                        new() {
                            ClassList = ["region-header"],
                            NodeValue = I18N.Translate("Widget.Teleport.Favorites"),
                        },
                        new() {
                            ClassList = ["favorite-destinations"],
                        }
                    ]
                }
            ]
        };

        destinationList.AppendChild(destinationListFavorites);
    }

    private void BuildExpansionNode(Node targetNode, Node destinations, TeleportExpansion expansion, bool reverse)
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

        Node destinationList = new() {
            Id        = expansion.NodeId,
            ClassList = ["region-container"],
            Style     = new() { IsVisible = false }
        };

        ExpansionLists[expansion.NodeId] = destinationList;
        destinations.AppendChild(destinationList);

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

    private void BuildDestinationNode(
        Node                targetNode,
        TeleportDestination destination,
        int?                sortIndex     = null,
        bool                showSortables = false
    )
    {
        Node node = new() {
            Id        = destination.NodeId,
            ClassList = ["destination"],
            SortIndex = sortIndex ?? destination.SortIndex,
            ChildNodes = [
                new() {
                    ClassList   = ["destination-icon"],
                    Style       = new() { UldPartId = destination.UldPartId },
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

        node.OnRightClick += _ => {
            _selectedDestination = destination;

            ContextMenu!.SetEntryVisible("AddFav", !IsFavorite(destination));
            ContextMenu!.SetEntryVisible("DelFav", IsFavorite(destination));
            ContextMenu!.SetEntryVisible("MoveUp", showSortables);
            ContextMenu!.SetEntryVisible("MoveDown", showSortables);

            if (showSortables && IsFavorite(destination)) {
                var indexAt = Favorites.IndexOf($"{destination.AetheryteId}:{destination.SubIndex}");
                ContextMenu!.SetEntryDisabled("MoveUp", indexAt == 0);
                ContextMenu!.SetEntryDisabled("MoveDown", indexAt == Favorites.Count - 1);
            }

            ContextMenu!.Present();
        };

        node.OnMouseUp += _ => Teleport(destination);
    }
}
