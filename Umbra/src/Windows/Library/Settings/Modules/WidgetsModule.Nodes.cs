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
using Dalamud.Interface;
using ImGuiNET;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
using Umbra.Windows.Components;
using Umbra.Windows.Library.AddWidget;
using Umbra.Windows.Library.WidgetConfig;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

public partial class WidgetsModule
{
    public sealed override Node Node { get; } = new() {
        Stylesheet = WidgetsModuleStylesheet,
        ClassList  = ["widgets-module"],
        ChildNodes = [
            new() {
                ClassList = ["module-header"],
                NodeValue = I18N.Translate("Settings.WidgetsModule.Name")
            },
            new() {
                ClassList = ["widgets-column-wrapper"],
                ChildNodes = [
                    CreateColumn("Left",   "Left Side"),
                    CreateColumn("Center", "Center"),
                    CreateColumn("Right",  "Right Side")
                ]
            },
            new() {
                ClassList = ["widgets-footer-text"],
                NodeValue = I18N.Translate("Settings.WidgetsModule.DangerHint")
            }
        ]
    };

    /// <summary>
    /// Updates the sizes of the nodes based on the window dimensions.
    /// </summary>
    private void UpdateNodeSizes()
    {
        var size    = (Node.ParentNode!.Bounds.ContentSize / Node.ScaleFactor);
        int colSize = (size.Width / 3) - 20;

        Node.QuerySelector(".module-header")!.Style.Size = new(size.Width - 30, 0);

        foreach (var column in Node.QuerySelectorAll(".widgets-column")) {
            column.Style.Size                                                 = new(colSize, size.Height - 108);
            column.QuerySelector(".widgets-column--header")!.Style.Size       = new(colSize - 30, 0);
            column.QuerySelector(".widgets-column--list-wrapper")!.Style.Size = new(colSize - 20, size.Height - 208);
        }

        foreach (var item in Node.QuerySelectorAll(".widgets-column-stretched-item")) {
            item.Style.Size = new(colSize - 30, 0);
        }

        foreach (var item in Node.QuerySelectorAll(".widget-instance--name")) {
            item.Style.Size = new(colSize - 60, 0);
        }
    }

    /// <summary>
    /// Creates a column node that represents a panel that contains widgets.
    /// </summary>
    private static Node CreateColumn(string id, string label)
    {
        return new() {
            Id        = id,
            ClassList = ["widgets-column"],
            ChildNodes = [
                new() {
                    ClassList = ["widgets-column--header"],
                    NodeValue = label
                },
                new() {
                    ClassList = ["widgets-column--list-wrapper"],
                    Overflow  = false,
                    ChildNodes = [
                        new() {
                            ClassList = ["widgets-column--list"],
                        },
                    ]
                }
            ]
        };
    }

    private Node LeftColumn   => Node.QuerySelector("#Left")!.QuerySelector(".widgets-column--list")!;
    private Node CenterColumn => Node.QuerySelector("#Center")!.QuerySelector(".widgets-column--list")!;
    private Node RightColumn  => Node.QuerySelector("#Right")!.QuerySelector(".widgets-column--list")!;

    private Node GetColumn(string location) =>
        location switch {
            "Left"   => LeftColumn,
            "Center" => CenterColumn,
            "Right"  => RightColumn,
            _        => throw new ArgumentOutOfRangeException(nameof(location))
        };

    private Node CreateAddNewButton(string id)
    {
        Node node = new() {
            Id        = id,
            ClassList = ["widgets-column-stretched-item", "widgets-column--add-new"],
            SortIndex = int.MaxValue,
            ChildNodes = [
                new() {
                    ClassList   = ["widgets-column--add-new--label"],
                    NodeValue   = I18N.Translate("Settings.WidgetsModule.AddWidget"),
                    InheritTags = true,
                }
            ]
        };

        node.OnMouseUp += _ => {
            Framework
                .Service<WindowManager>()
                .Present(
                    "AddWidget",
                    new AddWidgetWindow(),
                    window => {
                        if (window.SelectedWidgetId == null) return;

                        Framework.Service<WidgetManager>().CreateWidget(window.SelectedWidgetId, id);
                    }
                );
        };

        return node;
    }

    private Node CreateWidgetInstanceNode(ToolbarWidget widget)
    {
        Node node = new() {
            Id        = $"widget-{widget.Id}",
            ClassList = ["widgets-column-stretched-item", "widget-instance"],
            ChildNodes = [
                new() {
                    ClassList = ["widget-instance--name"],
                    NodeValue = widget.Info.Name
                },
                new() {
                    ClassList = ["widget-instance--controls"],
                    ChildNodes = [
                        new() {
                            ClassList = ["widget-instance--controls--buttons"],
                            ChildNodes = [
                                new ButtonNode("MoveUp", null, FontAwesomeIcon.ArrowUp)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.MoveUp") },
                                new ButtonNode("MoveDown", null, FontAwesomeIcon.ArrowDown)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.MoveDown") },
                                new ButtonNode("MoveToLeftPanel", null, FontAwesomeIcon.ArrowLeft)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.MoveToLeft") },
                                new ButtonNode("MoveToCenterPanel", null, FontAwesomeIcon.ArrowLeft)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.MoveToCenter") },
                                new ButtonNode("MoveToRightPanel", null, FontAwesomeIcon.ArrowRight)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.MoveToRight") },
                                new ButtonNode("SettingsButton", null, FontAwesomeIcon.Cog)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.EditWidget") },
                                new ButtonNode("DeleteButton", null, FontAwesomeIcon.TrashAlt)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.DeleteWidget") },
                            ]
                        }
                    ],
                }
            ]
        };

        var moveUp             = node.QuerySelector<ButtonNode>("#MoveUp")!;
        var moveDown           = node.QuerySelector<ButtonNode>("#MoveDown")!;
        var moveToLeftPanel    = node.QuerySelector<ButtonNode>("#MoveToLeftPanel")!;
        var moveToCenterPanel  = node.QuerySelector<ButtonNode>("#MoveToCenterPanel")!;
        var moveToRightPanel   = node.QuerySelector<ButtonNode>("#MoveToRightPanel")!;
        var settingsButton     = node.QuerySelector<ButtonNode>("#SettingsButton")!;
        var deleteButton       = node.QuerySelector<ButtonNode>("#DeleteButton")!;

        settingsButton.IsDisabled = widget.GetConfigVariableList().Count == 0;

        moveUp.OnMouseUp            += _ => Framework.Service<WidgetManager>().UpdateWidgetSortIndex(widget.Id, -1);
        moveDown.OnMouseUp          += _ => Framework.Service<WidgetManager>().UpdateWidgetSortIndex(widget.Id, 1);
        moveToLeftPanel.OnMouseUp   += _ => widget.Location    = "Left";
        moveToCenterPanel.OnMouseUp += _ => widget.Location    = "Center";
        moveToRightPanel.OnMouseUp  += _ => widget.Location    = "Right";
        deleteButton.OnMouseUp      += _ => Framework.Service<WidgetManager>().RemoveWidget(widget.Id);
        settingsButton.OnMouseUp    += _ => Framework.Service<WindowManager>().Present(
            "WidgetInstanceConfig",
            new WidgetConfigWindow(widget.Id),
            _ => {
                Framework.Service<WidgetManager>().SaveWidgetState(widget.Id);
                Framework.Service<WidgetManager>().SaveState();
            }
        );

        node.BeforeDraw += _ => {
            deleteButton.IsDisabled           = !ImGui.GetIO().KeyShift;

            node.SortIndex                    = widget.SortIndex;
            moveToLeftPanel.Style.IsVisible   = widget.Location != "Left";
            moveToCenterPanel.Style.IsVisible = widget.Location != "Center";
            moveToRightPanel.Style.IsVisible  = widget.Location != "Right";

            moveUp.IsDisabled                 = widget.SortIndex == 0;
            moveDown.IsDisabled               = widget.SortIndex == GetColumn(widget.Location).ChildNodes.Count - 1;

            node.QuerySelector(".widget-instance--name")!.NodeValue = widget.Info.Name;

            moveToCenterPanel.Icon = widget.Location == "Left"
                ? FontAwesomeIcon.ArrowRight
                : FontAwesomeIcon.ArrowLeft;

            moveToRightPanel.Icon = widget.Location == "Left"
                ? FontAwesomeIcon.AngleDoubleRight
                : FontAwesomeIcon.ArrowRight;

            moveToLeftPanel.Icon = widget.Location == "Right"
                ? FontAwesomeIcon.AngleDoubleLeft
                : FontAwesomeIcon.ArrowLeft;
        };

        return node;
    }
}
