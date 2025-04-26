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
using Umbra.Plugins;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class PluginsModule
{
    public override Node Node { get; } = new() {
        Stylesheet = PluginsModuleStylesheet,
        ClassList  = ["plugins"],
        ChildNodes = [
            new() {
                ClassList = ["plugins-header"],
                NodeValue = I18N.Translate("Settings.PluginsModule.Title"),
            },
            new() {
                ClassList = ["plugins-description"],
                NodeValue = I18N.Translate("Settings.PluginsModule.Description")
            },
            new() {
                ClassList = ["plugins-list"]
            },
            new() {
                ClassList = ["plugins-footer"],
                ChildNodes = [
                    new ButtonNode(
                        "AddPlugin",
                        I18N.Translate("Settings.PluginsModule.AddPlugin"),
                        FontAwesomeIcon.FolderOpen
                    ),
                ]
            },
            new() {
                ClassList = ["plugins-changed"],
                ChildNodes = [
                    new() {
                        ClassList = ["plugins-changed-text"],
                        NodeValue = I18N.Translate("Settings.PluginsModule.PluginsChanged"),
                    },
                    new ButtonNode(
                        "RestartButton",
                        I18N.Translate("Settings.Window.RestartUmbra"),
                        FontAwesomeIcon.Sync
                    ),
                ]
            }
        ]
    };

    private void AddPluginNode(Plugins.Plugin plugin)
    {
        Node node = new() {
            ClassList = ["plugin"],
            ChildNodes = [
                new() {
                    ClassList = ["plugin-name"],
                    NodeValue = plugin.File.Name,
                },
                new() {
                    ClassList = ["plugin-load-error"],
                    ChildNodes = [
                        new() {
                            ClassList = ["plugin-load-error-icon"],
                            NodeValue = FontAwesomeIcon.ExclamationTriangle.ToIconString(),
                        },
                        new() {
                            ClassList = ["plugin-load-error-message"],
                            NodeValue = plugin.LoadError
                        }
                    ]
                },
                new() {
                    ClassList = ["plugin-load-info"],
                    NodeValue = ""
                },
                new() {
                    ClassList = ["plugin-buttons"],
                    ChildNodes = [
                        new ButtonNode(
                            "RemoveButton",
                            I18N.Translate("Settings.PluginsModule.RemovePluginButton"),
                            FontAwesomeIcon.Trash
                        ) {
                            IsDisabled = string.IsNullOrEmpty(plugin.LoadError) && plugin.IsDisposed
                        },
                    ]
                },
            ]
        };

        if (string.IsNullOrEmpty(plugin.LoadError)) {
            node.QuerySelector(".plugin-load-error")!.Style.IsVisible = false;
            node.QuerySelector(".plugin-load-info")!.Style.IsVisible  = true;

            if (plugin.Assembly is not null) {
                var n = plugin.Assembly.GetName();

                node.QuerySelector(".plugin-load-info")!.NodeValue = I18N.Translate(
                    "Settings.PluginsModule.Loaded",
                    n.Name,
                    n.Version!.ToString()
                );
            } else if (plugin.IsDisposed) {
                node.QuerySelector(".plugin-load-info")!.NodeValue =
                    I18N.Translate("Settings.PluginsModule.IsDisposed");
            } else {
                node.QuerySelector(".plugin-load-info")!.NodeValue =
                    I18N.Translate("Settings.PluginsModule.NotLoaded");
            }
        } else {
            node.QuerySelector(".plugin-load-info")!.Style.IsVisible  = false;
            node.QuerySelector(".plugin-load-error")!.Style.IsVisible = true;
        }

        node.QuerySelector<ButtonNode>("RemoveButton")!.OnMouseUp += _ => {
            // If the plugin was never really loaded, we can just remove it.
            if (plugin.Assembly is null) {
                PluginManager.RemovePlugin(plugin);
                Framework.DalamudFramework.RunOnTick(() => node.Remove());
                return;
            }

            PluginManager.RemovePlugin(plugin);
            node.QuerySelector<ButtonNode>("RemoveButton")!.IsDisabled = true;

            node.QuerySelector(".plugin-load-info")!.NodeValue =
                I18N.Translate("Settings.PluginsModule.IsDisposed");
        };

        Node.QuerySelector(".plugins-list")!.AppendChild(node);
    }

    private void UpdateNodeSizes()
    {
        var size = (Node.ParentNode!.Bounds.ContentSize / Node.ScaleFactor);
        Node.QuerySelector(".plugins-header")!.Style.Size       = new(size.Width - 30, 0);
        Node.QuerySelector(".plugins-description")!.Style.Size  = new(size.Width - 30, 0);
        Node.QuerySelector(".plugins-list")!.Style.Size         = new(size.Width - 30, 0);
        Node.QuerySelector(".plugins-footer")!.Style.Size       = new(size.Width - 30, 0);
        Node.QuerySelector(".plugins-changed")!.Style.IsVisible = PluginManager.IsRestartRequired();


        foreach (Node node in Node.QuerySelectorAll(".plugin")) {
            node.Style.Size = new(size.Width - 30, 0);
        }

        foreach (Node node in Node.QuerySelectorAll(".plugin-load-error-message")) {
            node.Style.Size = new(size.Width - 250, 0);
        }
    }
}
