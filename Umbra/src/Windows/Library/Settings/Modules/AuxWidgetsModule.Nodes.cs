using Dalamud.Game.Text;
using Dalamud.Interface;
using ImGuiNET;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
using Umbra.Windows.Components;
using Umbra.Windows.Library.WidgetConfig;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class AuxWidgetsModule
{
    private string? _selectedInstanceId;

    public override Node Node { get; } = new() {
        Id         = "AuxWidgetsModule",
        Stylesheet = WidgetsModule.WidgetsModuleStylesheet,
        ClassList  = ["widgets-module"],
        ChildNodes = [
            new() {
                ClassList = ["module-header"],
                NodeValue = I18N.Translate("Settings.AuxWidgetsModule.Name"),
            },
            new() {
                ClassList = ["widgets-column-wrapper"],
                ChildNodes = [
                    new() {
                        Id         = "AuxSettingsPanel",
                        Stylesheet = Stylesheet,
                        ChildNodes = [
                            new CheckboxNode(
                                "AuxEnabled",
                                Toolbar.AuxBarEnabled,
                                I18N.Translate("Settings.AuxWidgetsModule.Config.Enabled.Name"),
                                I18N.Translate("Settings.AuxWidgetsModule.Config.Enabled.Description")
                            ),
                            new() { ClassList = ["separator"] },
                            new CheckboxNode(
                                "AuxDecorate",
                                Toolbar.AuxBarDecorate,
                                I18N.Translate("Settings.AuxWidgetsModule.Config.Decorate.Name"),
                                I18N.Translate("Settings.AuxWidgetsModule.Config.Decorate.Description")
                            ),
                            new CheckboxNode(
                                "AuxShadow",
                                Toolbar.AuxEnableShadow,
                                I18N.Translate("Settings.AuxWidgetsModule.Config.EnableShadow.Name"),
                                I18N.Translate("Settings.AuxWidgetsModule.Config.EnableShadow.Description")
                            ),
                            new SelectNode(
                                "AuxXAlign",
                                Toolbar.AuxBarXAlign,
                                ["Left", "Center", "Right"],
                                I18N.Translate("Settings.AuxWidgetsModule.Config.XAlign.Name"),
                                I18N.Translate("Settings.AuxWidgetsModule.Config.XAlign.Description")
                            ),
                            new IntegerInputNode(
                                "AuxXPosition",
                                0,
                                -10000,
                                10000,
                                I18N.Translate("Settings.AuxWidgetsModule.Config.XPosition.Name"),
                                I18N.Translate("Settings.AuxWidgetsModule.Config.XPosition.Description")
                            ),
                            new IntegerInputNode(
                                "AuxYPosition",
                                0,
                                -10000,
                                10000,
                                I18N.Translate("Settings.AuxWidgetsModule.Config.YPosition.Name"),
                                I18N.Translate("Settings.AuxWidgetsModule.Config.YPosition.Description")
                            )
                        ]
                    },
                    new() {
                        Id        = "AuxWidgetsList",
                        ClassList = ["widgets-column"],
                        ChildNodes = [
                            new() {
                                ClassList = ["widgets-column--header"],
                                NodeValue = I18N.Translate("Settings.AuxWidgetsModule.ListHeader"),
                            },
                            new() {
                                ClassList = ["widgets-column--list-wrapper"],
                                Overflow  = false,
                                ChildNodes = [
                                    new() {
                                        ClassList = ["widgets-column--list"],
                                    },
                                ]
                            },
                            new() {
                                Id        = "AuxWidgetAdd",
                                ClassList = ["widgets-column-stretched-item", "widgets-column--add-new"],
                                SortIndex = int.MaxValue,
                                ChildNodes = [
                                    new() {
                                        ClassList = ["widgets-column--add-new--label"],
                                        NodeValue =
                                            $"{SeIconChar.BoxedPlus.ToIconString()} {I18N.Translate("Settings.WidgetsModule.AddWidget")}",
                                        InheritTags = true,
                                    }
                                ]
                            }
                        ]
                    },
                ]
            }
        ]
    };

    private void UpdateNodeSizes()
    {
        var size    = (Node.ParentNode!.Bounds.ContentSize / Node.ScaleFactor);
        int colSize = (size.Width / 3) - 20;

        Node.QuerySelector(".module-header")!.Style.Size    = new(size.Width - 30, 0);
        Node.QuerySelector("#AuxSettingsPanel")!.Style.Size = new(colSize * 2, size.Height - 88);

        foreach (var separator in Node.QuerySelectorAll(".separator")) {
            separator.Style.Size = new(colSize * 2, 1);
        }

        foreach (var column in Node.QuerySelectorAll(".widgets-column")) {
            column.Style.Size                                                 = new(colSize, size.Height - 88);
            column.QuerySelector(".widgets-column--header")!.Style.Size       = new(colSize - 30, 0);
            column.QuerySelector(".widgets-column--list-wrapper")!.Style.Size = new(colSize - 20, size.Height - 182);
        }

        foreach (var item in Node.QuerySelectorAll(".widgets-column-stretched-item")) {
            item.Style.Size = new(colSize - 30, 0);
        }

        foreach (var item in Node.QuerySelectorAll(".widget-instance--name")) {
            item.Style.Size = new(colSize - 60, 0);
        }

        foreach (var item in Node.QuerySelectorAll(".widget-instance--controls")) {
            item.Style.Size = new(colSize - 60, 0);
        }
    }

    private Node CreateWidgetInstanceNode(ToolbarWidget widget)
    {
        Node node = new() {
            Stylesheet = WidgetsModule.WidgetsModuleStylesheet,
            Id         = $"widget-{widget.Id}",
            ClassList  = ["widgets-column-stretched-item", "widget-instance"],
            ChildNodes = [
                new() {
                    ClassList = ["widget-instance--name"],
                    NodeValue = widget.GetInstanceName()
                },
                new() {
                    ClassList = ["widget-instance--controls"],
                    Style     = new() { IsVisible = false },
                    ChildNodes = [
                        new() {
                            ClassList = ["widget-instance--controls--buttons"],
                            ChildNodes = [
                                new ButtonNode("MoveUp", null, FontAwesomeIcon.ArrowUp)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.MoveUp") },
                                new ButtonNode("MoveDown", null, FontAwesomeIcon.ArrowDown)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.MoveDown") },
                                new ButtonNode("SettingsButton", null, FontAwesomeIcon.Cog)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.EditWidget") },
                                new ButtonNode("CopyButton", null, FontAwesomeIcon.Copy)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.CopyWidget") },
                                new ButtonNode("DeleteButton", null, FontAwesomeIcon.TrashAlt)
                                    { Tooltip = I18N.Translate("Settings.WidgetsModule.DeleteWidget") },
                            ]
                        }
                    ],
                }
            ]
        };

        var nameNode       = node.QuerySelector(".widget-instance--name")!;
        var controlsNode   = node.QuerySelector(".widget-instance--controls")!;
        var moveUp         = node.QuerySelector<ButtonNode>("#MoveUp")!;
        var moveDown       = node.QuerySelector<ButtonNode>("#MoveDown")!;
        var settingsButton = node.QuerySelector<ButtonNode>("#SettingsButton")!;
        var copyButton     = node.QuerySelector<ButtonNode>("#CopyButton")!;
        var deleteButton   = node.QuerySelector<ButtonNode>("#DeleteButton")!;

        settingsButton.IsDisabled = widget.GetConfigVariableList().Count == 0;

        WidgetManager wm = Framework.Service<WidgetManager>();

        moveUp.OnMouseUp     += _ => wm.UpdateWidgetSortIndex(widget.Id, -1, ImGui.GetIO().KeyCtrl);
        moveDown.OnMouseUp   += _ => wm.UpdateWidgetSortIndex(widget.Id, 1,  ImGui.GetIO().KeyCtrl);
        copyButton.OnMouseUp += _ => wm.CreateCopyOfWidget(widget.Id);

        deleteButton.OnMouseUp += _ => {
            if (ImGui.GetIO().KeyShift) {
                Framework.DalamudFramework.Run(() => wm.RemoveWidget(widget.Id));
            }
        };

        settingsButton.OnMouseUp += _ => Framework
            .Service<WindowManager>()
            .Present(
                "WidgetInstanceConfig",
                new WidgetConfigWindow(widget.Id),
                _ => {
                    wm.SaveWidgetState(widget.Id);
                    wm.SaveState();
                }
            );

        nameNode.OnMouseUp += _ => {
            _selectedInstanceId = _selectedInstanceId == widget.Id ? "" : widget.Id;
        };

        node.BeforeDraw += _ => {
            controlsNode.Style.IsVisible = _selectedInstanceId == widget.Id;

            node.SortIndex      = widget.SortIndex;
            node.Style.Opacity  = widget.IsEnabled ? 1 : 0.5f;
            moveUp.IsDisabled   = widget.SortIndex == 0;
            moveDown.IsDisabled = widget.SortIndex == AuxWidgetsListNode.ChildNodes.Count - 1;
            nameNode.NodeValue  = widget.GetInstanceName();
        };

        return node;
    }

    private Node             AuxWidgetAddNode   => Node.QuerySelector("#AuxWidgetAdd")!;
    private CheckboxNode     AuxEnabledNode     => (CheckboxNode)Node.QuerySelector("AuxEnabled")!;
    private CheckboxNode     AuxDecorateNode    => (CheckboxNode)Node.QuerySelector("AuxDecorate")!;
    private IntegerInputNode AuxXPositionNode   => (IntegerInputNode)Node.QuerySelector("AuxXPosition")!;
    private IntegerInputNode AuxYPositionNode   => (IntegerInputNode)Node.QuerySelector("AuxYPosition")!;
    private SelectNode       AuxXAlignNode      => (SelectNode)Node.QuerySelector("AuxXAlign")!;
    private CheckboxNode     AuxShadowNode      => (CheckboxNode)Node.QuerySelector("AuxShadow")!;
    private Node             AuxWidgetsListNode => Node.QuerySelector(".widgets-column--list")!;
}
