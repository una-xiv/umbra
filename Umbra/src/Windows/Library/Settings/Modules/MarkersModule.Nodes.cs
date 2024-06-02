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

using Dalamud.Interface;
using Umbra.Common;
using Umbra.Markers;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal sealed partial class MarkersModule
{
    /// <inheritdoc/>
    public override Node Node { get; } = new() {
        Stylesheet = MarkersModuleStylesheet,
        ClassList  = ["markers"],
        ChildNodes = [
            new() {
                ClassList = ["markers-header"],
                NodeValue = I18N.Translate("Settings.MarkersModule.Name"),
            },
            new() {
                ClassList  = ["markers-list"],
                ChildNodes = []
            }
        ]
    };

    private void UpdateNodeSizes()
    {
        var size = (Node.ParentNode!.Bounds.ContentSize / Node.ScaleFactor);

        Node.QuerySelector(".markers-header")!.Style.Size = new(size.Width - 30, 0);

        foreach (Node node in Node.QuerySelectorAll("marker")) {
            node.Style.Size = new(size.Width - 30, 0);
        }

        foreach (Node node in Node.QuerySelectorAll(".marker-header-text-desc")) {
            node.Style.Size = new(size.Width - 80, 0);
        }

        foreach (Node node in Node.QuerySelectorAll(".marker-body")) {
            node.Style.Size = new(size.Width - 80, 0);
        }
    }

    private void RenderMarkerSection(string id)
    {
        WorldMarkerFactory factory = Registry.GetFactory(id);

        Node node = new() {
            Id        = id,
            ClassList = ["marker"],
            ChildNodes = [
                new() {
                    ClassList = ["marker-header"],
                    ChildNodes = [
                        new() {
                            ClassList   = ["marker-header-chevron"],
                            NodeValue   = FontAwesomeIcon.ChevronCircleRight.ToIconString(),
                            InheritTags = true,
                        },
                        new() {
                            ClassList   = ["marker-header-text"],
                            InheritTags = true,
                            ChildNodes = [
                                new() {
                                    ClassList   = ["marker-header-text-name"],
                                    NodeValue   = factory.Name,
                                    InheritTags = true,
                                },
                                new() {
                                    ClassList = ["marker-header-text-desc"],
                                    NodeValue = factory.Description,
                                }
                            ]
                        }
                    ],
                },
                new() {
                    ClassList = ["marker-body"],
                    Style     = new() { IsVisible = false },
                }
            ]
        };

        node.QuerySelector(".marker-header")!.OnMouseUp += _ => {
            Node markerBody = node.QuerySelector(".marker-body")!;
            markerBody.Style.IsVisible = !markerBody.Style.IsVisible;

            node.QuerySelector(".marker-header-chevron")!.NodeValue = markerBody.Style.IsVisible!.Value
                ? FontAwesomeIcon.ChevronCircleDown.ToIconString()
                : FontAwesomeIcon.ChevronCircleRight.ToIconString();
        };

        foreach (IMarkerConfigVariable cvar in factory.GetConfigVariables()) {
            Node? controlNode = RenderControlNode(factory, cvar);

            if (controlNode is not null) {
                node.QuerySelector(".marker-body")!.AppendChild(controlNode);
            }
        }

        Node.QuerySelector(".markers-list")!.AppendChild(node);
    }
}
