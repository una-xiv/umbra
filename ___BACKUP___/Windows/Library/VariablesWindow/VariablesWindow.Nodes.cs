﻿using Dalamud.Interface;
using Lumina.Misc;
using System;
using System.Linq;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.VariablesWindow;

public partial class VariablesWindow
{
    protected override Node Node { get; } = new() {
        Stylesheet = WindowStyles.WidgetConfigWindowStylesheet,
        ClassList  = ["widget-config-window"],
        ChildNodes = [
            new() {
                Id = "SearchPanel",
                ChildNodes = [
                    new() {
                        Id        = "SearchIcon",
                        NodeValue = FontAwesomeIcon.Search.ToIconString(),
                    },
                    new() {
                        Id         = "SearchInputWrapper",
                        ChildNodes = [new StringInputNode("Search", "", 128, null, null, 0, true)]
                    }
                ]
            },
            new() {
                ClassList = ["widget-config-list--wrapper"],
                Overflow  = false,
                ChildNodes = [
                    new() {
                        ClassList = ["widget-config-list"],
                    }
                ]
            },
            new() {
                ClassList = ["widget-config-footer"],
                ChildNodes = [
                    new() {
                        ClassList = ["widget-config-footer--buttons", "left-side"],
                        ChildNodes = [..extraButtons ?? []]
                    },
                    new() {
                        ClassList = ["widget-config-footer--buttons"],
                        ChildNodes = [
                            new ButtonNode("CloseButton", I18N.Translate("Close")),
                        ]
                    },
                ]
            }
        ]
    };

    private Node            CloseButton      => Node.QuerySelector("#CloseButton")!;
    private Node            ControlsListNode => Node.QuerySelector(".widget-config-list")!;
    private StringInputNode SearchInputNode  => Node.QuerySelector<StringInputNode>("#Search")!;

    private Node CreateCategoryNode(string name)
    {
        Node contentNode = new() {
            ClassList = ["widget-config-category--content"],
        };

        Node headerNode = new() {
            ClassList = ["widget-config-category-header"],
            ChildNodes = [
                new() {
                    ClassList   = ["widget-config-category--chevron"],
                    NodeValue   = FontAwesomeIcon.ChevronCircleUp.ToIconString(),
                    InheritTags = true,
                },
                new() {
                    ClassList   = ["widget-config-category--label"],
                    NodeValue   = name,
                    InheritTags = true,
                }
            ]
        };

        Node node = new() {
            ClassList = ["widget-config-category"],
            ChildNodes = {
                headerNode,
                contentNode,
            },
        };

        ControlsListNode.AppendChild(node);

        contentNode.Style.IsVisible = true;

        headerNode.OnClick += _ => {
            contentNode.Style.IsVisible = !contentNode.Style.IsVisible;

            headerNode.QuerySelector(".widget-config-category--chevron")!.NodeValue = contentNode.Style.IsVisible.Value
                ? FontAwesomeIcon.ChevronCircleUp.ToIconString()
                : FontAwesomeIcon.ChevronCircleDown.ToIconString();
        };

        return contentNode;
    }

    private Node? CreateControlNode(Variable variable)
    {
        return variable switch {
            StringSelectVariable strSelectVar => CreateStringSelectControl(strSelectVar),
            StringVariable strVar             => CreateStringControl(strVar),
            BooleanVariable boolVar           => CreateBooleanControl(boolVar),
            ColorVariable colorVar            => CreateColorControl(colorVar),
            IntegerVariable intVar            => CreateIntegerControl(intVar),
            FloatVariable floatVar            => CreateFloatControl(floatVar),
            IconIdVariable iconVar            => CreateIconIdControl(iconVar),
            FaIconVariable faIconVar          => CreateFaIconControl(faIconVar),
            _                                 => null
        };
    }

    private SelectNode CreateStringSelectControl(StringSelectVariable variable)
    {
        if (variable.Choices.Count == 0)
            throw new InvalidOperationException("A select control must have at least one option.");

        if (!variable.Choices.TryGetValue(variable.Value, out string? selectedValue)) {
            selectedValue = variable.Choices.First().Value;
        }

        var node = new SelectNode(
            GetNextControlId(variable),
            selectedValue,
            variable.Choices.Values.ToList(),
            variable.Name,
            variable.Description
        );

        node.ClassList.Add("widget-config-control");

        node.OnValueChanged += v => {
            if (variable.Choices.ContainsValue(v)) {
                variable.Value = variable.Choices.First(x => x.Value == v).Key;
            }
        };

        variable.ValueChanged += v => {
            if (variable.Choices.TryGetValue(v, out string? value)) {
                node.Value = value;
            }
        };

        return node;
    }

    private StringInputNode CreateStringControl(StringVariable variable)
    {
        var node = new StringInputNode(
            GetNextControlId(variable),
            variable.Value,
            variable.MaxLength,
            variable.Name,
            variable.Description
        );

        node.ClassList.Add("widget-config-control");
        node.OnValueChanged += v => variable.Value = v;
        variable.ValueChanged += v => node.Value = v;

        return node;
    }

    private CheckboxNode CreateBooleanControl(BooleanVariable variable)
    {
        CheckboxNode node = new(
            GetNextControlId(variable),
            variable.Value,
            variable.Name,
            variable.Description
        );

        node.ClassList.Add("widget-config-control");
        node.OnValueChanged += v => variable.Value = v;
        variable.ValueChanged += v => node.Value = v;

        return node;
    }

    private ColorInputNode CreateColorControl(ColorVariable variable)
    {
        var node = new ColorInputNode(
            GetNextControlId(variable),
            variable.Value,
            variable.Name,
            variable.Description
        );

        node.ClassList.Add("widget-config-control");
        node.OnValueChanged += v => variable.Value = v;
        variable.ValueChanged += v => node.Value = v;

        return node;
    }

    private IntegerInputNode CreateIntegerControl(IntegerVariable variable)
    {
        var node = new IntegerInputNode(
            GetNextControlId(variable),
            variable.Value,
            variable.Min,
            variable.Max,
            variable.Name,
            variable.Description
        );

        node.ClassList.Add("widget-config-control");
        node.OnValueChanged += v => variable.Value = v;
        variable.ValueChanged += v => node.Value = v;

        return node;
    }

    private FloatInputNode CreateFloatControl(FloatVariable variable)
    {
        var node = new FloatInputNode(
            GetNextControlId(variable),
            variable.Value,
            variable.Min,
            variable.Max,
            variable.Name,
            variable.Description
        );

        node.ClassList.Add("widget-config-control");
        node.OnValueChanged += v => variable.Value = v;
        variable.ValueChanged += v => node.Value = v;

        return node;
    }

    private IconIdInputNode CreateIconIdControl(IconIdVariable variable)
    {
        var node = new IconIdInputNode(
            GetNextControlId(variable),
            variable.Value,
            variable.Name,
            variable.Description
        );

        node.ClassList.Add("widget-config-control");
        node.OnValueChanged += v => variable.Value = v;
        variable.ValueChanged += v => node.Value = v;

        return node;
    }

    private FaIconInputNode CreateFaIconControl(FaIconVariable variable)
    {
        var node = new FaIconInputNode(
            GetNextControlId(variable),
            variable.Value,
            variable.Name,
            variable.Description
        );

        node.ClassList.Add("widget-config-control");
        node.OnValueChanged += v => variable.Value = v;
        variable.ValueChanged += v => {
            node.Value = v;
        };

        return node;
    }

    /// <summary>
    /// Returns a new ID for the control node.
    /// </summary>
    private static string GetNextControlId(Variable variable)
    {
        return $"control-{Crc32.Get(variable.Id)}";
    }
}
