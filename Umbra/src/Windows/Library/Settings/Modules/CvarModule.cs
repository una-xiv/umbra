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
using Dalamud.Interface;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class CvarModule : SettingsModule
{
    public sealed override string Id   { get; }
    public sealed override string Name { get; }

    public sealed override Node Node { get; } = new() {
        Stylesheet = CvarModuleStylesheet,
        Style = new() {
            Flow    = Flow.Vertical,
            Padding = new(15),
            Gap     = 15,
        }
    };

    public CvarModule(string category)
    {
        Id   = category;
        Name = I18N.Translate($"CVAR.Group.{category}");

        Node.AppendChild(
            new() {
                ClassList = ["cvar-header"],
                NodeValue = Name,
            }
        );

        List<Cvar>                     rootCategory  = [];
        Dictionary<string, List<Cvar>> subCategories = [];

        foreach (var cvar in ConfigManager.GetVariablesFromCategory(category)) {
            if (!I18N.Has($"CVAR.{cvar.Id}.Name")) continue;

            if (cvar.SubCategory is null) {
                rootCategory.Add(cvar);
                continue;
            }

            if (!subCategories.ContainsKey(cvar.SubCategory)) {
                subCategories[cvar.SubCategory] = [];
            }

            subCategories[cvar.SubCategory].Add(cvar);
        }

        if (rootCategory.Count > 0) {
            Node.AppendChild(
                new() {
                    ClassList  = ["cvar-list"],
                    ChildNodes = [..GetCvarNodeList(rootCategory)]
                }
            );
        }

        foreach (var (id, cvars) in subCategories) {
            RenderSubcategory(id, cvars);
        }

        // If the root category is empty, expand the first subcategory.
        if (rootCategory.Count == 0 && subCategories.Count > 0) {
            ToggleSubcategory(Node.QuerySelector(".cvar-subcategory")!);
        }
    }

    public override void OnOpen() { }

    public override void OnUpdate()
    {
        if (null == Node.ParentNode) return;

        var size = Node.ParentNode!.Bounds.ContentSize / Node.ScaleFactor;

        Node.QuerySelector(".cvar-header")!.Style.Size = new(size.Width - 30, 0);

        foreach (var cvarNode in Node.QuerySelectorAll(" .cvar-subcategory")) {
            cvarNode.Style.Size = new(size.Width - 30, 0);
        }

        foreach (var cvarNode in Node.QuerySelectorAll(".cvar-control-node")) {
            cvarNode.Style.Size = new(size.Width - 45, 0);
        }

        foreach (var cvarNode in Node.QuerySelectorAll(".cvar")) {
            cvarNode.Style.Size = new(size.Width - 80, 0);
        }
    }

    private List<Node> GetCvarNodeList(List<Cvar> cvars)
    {
        List<Node> nodes = [];

        foreach (var cvar in cvars) {
            var node = RenderCvar(cvar);

            if (node is not null) {
                nodes.Add(node);
            }
        }

        return nodes;
    }

    private void RenderSubcategory(string id, List<Cvar> cvars)
    {
        Node subcategoryNode = new() {
            ClassList = ["cvar-subcategory"],
            ChildNodes = [
                new() {
                    ClassList = ["cvar-subcategory-header"],
                    ChildNodes = [
                        new() {
                            ClassList   = ["cvar-subcategory--chevron"],
                            NodeValue   = FontAwesomeIcon.ChevronCircleRight.ToIconString(),
                            InheritTags = true,
                        },
                        new() {
                            ClassList   = ["cvar-subcategory--label"],
                            NodeValue   = I18N.Translate($"CVAR.SubGroup.{id}"),
                            InheritTags = true,
                        }
                    ]
                },
                new() {
                    ClassList  = ["cvar-list", "in-subcategory"],
                    ChildNodes = [..GetCvarNodeList(cvars)],
                    Style      = new() { IsVisible = false }
                }
            ]
        };

        Node header = subcategoryNode.QuerySelector(".cvar-subcategory-header")!;
        header.OnClick += _ => ToggleSubcategory(subcategoryNode);

        Node.AppendChild(subcategoryNode);
    }

    private void ToggleSubcategory(Node subcategoryNode)
    {
        Node list = subcategoryNode.QuerySelector(".cvar-list")!;
        list.Style.IsVisible = !list.Style.IsVisible;

        subcategoryNode.QuerySelector(".cvar-subcategory--chevron")!.NodeValue = list.Style.IsVisible ?? false
            ? FontAwesomeIcon.ChevronCircleDown.ToIconString()
            : FontAwesomeIcon.ChevronCircleRight.ToIconString();
    }
}
