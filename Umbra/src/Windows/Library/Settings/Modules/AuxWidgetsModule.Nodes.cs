﻿using Dalamud.Game.Text;
using Dalamud.Interface;
using ImGuiNET;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
using Umbra.Windows.Components;
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
                        Overflow   = false,
                        ChildNodes = [
                            new() {
                                Id = "AuxSettingsList",
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
                                    ),
                                    new() { ClassList = ["separator"] },
                                    new CheckboxNode(
                                        "AuxHideInCutscenes",
                                        Toolbar.AuxBarHideInCutscenes,
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxHideInCutscenes.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxHideInCutscenes.Description")
                                    ),
                                    new CheckboxNode(
                                        "AuxHideInPvP",
                                        Toolbar.AuxBarHideInPvP,
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxHideInPvP.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxHideInPvP.Description")
                                    ),
                                    new CheckboxNode(
                                        "AuxHideInDuty",
                                        Toolbar.AuxBarHideInDuty,
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxHideInDuty.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxHideInDuty.Description")
                                    ),
                                    new CheckboxNode(
                                        "AuxHideInCombat",
                                        Toolbar.AuxBarHideInCombat,
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxHideInCombat.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxHideInCombat.Description")
                                    ),
                                    new CheckboxNode(
                                        "AuxHideIfUnsheathed",
                                        Toolbar.AuxBarHideInCombat,
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxHideIfUnsheathed.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxHideIfUnsheathed.Description")
                                    ),
                                    new() { ClassList = ["separator"] },
                                    new CheckboxNode(
                                        "AuxConditionalVisibility",
                                        Toolbar.AuxBarIsConditionallyVisible,
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.ConditionalVisibility.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.ConditionalVisibility.Description")
                                    ),
                                    new SelectNode(
                                        "AuxHoldKey",
                                        Toolbar.AuxBarHoldKey,
                                        ["None", "Shift", "Ctrl", "Alt"],
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.HoldKey.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.HoldKey.Description")
                                    ),
                                    new CheckboxNode(
                                        "AuxShowInCutscene",
                                        Toolbar.AuxBarShowInCutscene,
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.ShowInCutscene.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.ShowInCutscene.Description")
                                    ),
                                    new CheckboxNode(
                                        "AuxShowInGPose",
                                        Toolbar.AuxBarShowInGPose,
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.ShowInGPose.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.ShowInGPose.Description")
                                    ),
                                    new CheckboxNode(
                                        "AuxShowInInstance",
                                        Toolbar.AuxBarShowInInstance,
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.ShowInInstance.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.ShowInInstance.Description")
                                    ),
                                    new CheckboxNode(
                                        "AuxShowInCombat",
                                        Toolbar.AuxBarShowInCombat,
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.ShowInCombat.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.ShowInCombat.Description")
                                    ),
                                    new CheckboxNode(
                                        "AuxShowUnsheathed",
                                        Toolbar.AuxBarShowUnsheathed,
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxShowUnsheathed.Name"),
                                        I18N.Translate("Settings.AuxWidgetsModule.Config.AuxShowUnsheathed.Description")
                                    ),
                                ]
                            },
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
                            new () {
                                ClassList = ["widgets-column-stretched-item"],
                                SortIndex = int.MaxValue,
                                ChildNodes = [
                                    new () {
                                        Id        = "AuxWidgetAdd",
                                        ClassList = ["widgets-column-button","widgets-column--add-new"],
                                        SortIndex = int.MaxValue,
                                        ChildNodes = [
                                            new() {
                                                ClassList = ["widgets-column--label", "widgets-column--add-new--label"],
                                                NodeValue =
                                                    $"{SeIconChar.BoxedPlus.ToIconString()} {I18N.Translate("Settings.WidgetsModule.AddWidget")}",
                                                InheritTags = true,
                                            }
                                        ]
                                    },
                                    new () {
                                        Id = "AuxWidgetClear",
                                        ClassList = ["widgets-column-button","widgets-column--clear-all"],
                                        SortIndex = int.MaxValue,
                                        ChildNodes = [
                                            new() {
                                                ClassList = ["widgets-column--label", "widgets-column--clear-all--label"],
                                                NodeValue = SeIconChar.Prohibited.ToIconString(),
                                                InheritTags = true
                                            }
                                        ],
                                        Tooltip = I18N.Translate("Settings.WidgetsModule.ClearColumn")}
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
        Node.QuerySelector("#AuxSettingsList")!.Style.Size  = new((colSize * 2) - 8, 0);

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

        settingsButton.OnMouseUp += _ => Framework.Service<WidgetInstanceEditor>().OpenEditor(widget);

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

    private Node AuxWidgetAddNode => Node.QuerySelector("#AuxWidgetAdd")!;
    private Node AuxWidgetClearNode => Node.QuerySelector("#AuxWidgetClear")!;
    private CheckboxNode AuxEnabledNode => Node.QuerySelector<CheckboxNode>("AuxEnabled")!;
    private CheckboxNode AuxDecorateNode => Node.QuerySelector<CheckboxNode>("AuxDecorate")!;
    private IntegerInputNode AuxXPositionNode => Node.QuerySelector<IntegerInputNode>("AuxXPosition")!;
    private IntegerInputNode AuxYPositionNode => Node.QuerySelector<IntegerInputNode>("AuxYPosition")!;
    private SelectNode AuxXAlignNode => Node.QuerySelector<SelectNode>("AuxXAlign")!;
    private CheckboxNode AuxShadowNode => Node.QuerySelector<CheckboxNode>("AuxShadow")!;
    private CheckboxNode AuxHideInCutscenesNode => Node.QuerySelector<CheckboxNode>("AuxHideInCutscenes")!;
    private CheckboxNode AuxHideInPvPNode => Node.QuerySelector<CheckboxNode>("AuxHideInPvP")!;
    private CheckboxNode AuxHideInDutyNode => Node.QuerySelector<CheckboxNode>("AuxHideInDuty")!;
    private CheckboxNode AuxHideInCombatNode => Node.QuerySelector<CheckboxNode>("AuxHideInCombat")!;
    private CheckboxNode AuxHideIfUnsheathedNode => Node.QuerySelector<CheckboxNode>("AuxHideIfUnsheathed")!;
    private CheckboxNode AuxConditionalVisibilityNode => Node.QuerySelector<CheckboxNode>("AuxConditionalVisibility")!;
    private SelectNode AuxHoldKeyNode => Node.QuerySelector<SelectNode>("AuxHoldKey")!;
    private CheckboxNode AuxShowInCutsceneNode => Node.QuerySelector<CheckboxNode>("AuxShowInCutscene")!;
    private CheckboxNode AuxShowInGPoseNode => Node.QuerySelector<CheckboxNode>("AuxShowInGPose")!;
    private CheckboxNode AuxShowInInstanceNode => Node.QuerySelector<CheckboxNode>("AuxShowInInstance")!;
    private CheckboxNode AuxShowInCombatNode => Node.QuerySelector<CheckboxNode>("AuxShowInCombat")!;
    private CheckboxNode AuxShowUnsheathedNode => Node.QuerySelector<CheckboxNode>("AuxShowUnsheathed")!;
    private Node AuxWidgetsListNode => Node.QuerySelector(".widgets-column--list")!;
}
