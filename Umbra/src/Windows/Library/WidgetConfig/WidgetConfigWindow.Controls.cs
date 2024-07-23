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
using System.Linq;
using Dalamud.Interface;
using Lumina.Misc;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.WidgetConfig;

internal partial class WidgetConfigWindow
{
    private void RenderControl(IWidgetConfigVariable cvar, Node targetNode)
    {
        Node? node = null;

        switch (cvar) {
            case SelectWidgetConfigVariable s:
                node = RenderSelectControl(s);
                break;
            case BooleanWidgetConfigVariable b:
                node = RenderBooleanControl(b);
                break;
            case IntegerWidgetConfigVariable i:
                node = RenderIntegerControl(i);
                break;
            case FloatWidgetConfigVariable f:
                node = RenderFloatControl(f);
                break;
            case StringWidgetConfigVariable s:
                node = RenderStringControl(s);
                break;
            default:
                Logger.Warning($"Cannot render widget control for {cvar.Id} ({cvar.GetType().Name})");
                break;
        }

        if (node is not null) {
            node.ClassList.Add("control");
            targetNode.AppendChild(node);
        }
    }

    private Node RenderCategory(string name)
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

    private SelectNode RenderSelectControl(SelectWidgetConfigVariable cvar)
    {
        if (cvar.Options.Count == 0)
            throw new InvalidOperationException("A select control must have at least one option.");

        if (!cvar.Options.TryGetValue(Instance.GetConfigValue<string>(cvar.Id), out string? selectedValue)) {
            selectedValue = cvar.Options.First().Value;
        }

        var node = new SelectNode(
            GetNextControlId(cvar),
            selectedValue,
            cvar.Options.Values.ToList(),
            cvar.Name,
            cvar.Description
        );

        node.OnValueChanged += newValue => {
            if (cvar.Options.ContainsValue(newValue)) {
                cvar.SetValue(cvar.Options.First(x => x.Value == newValue).Key);
            }
        };

        return node;
    }

    private CheckboxNode RenderBooleanControl(BooleanWidgetConfigVariable cvar)
    {
        CheckboxNode node = new(
            GetNextControlId(cvar),
            Instance.GetConfigValue<bool>(cvar.Id),
            cvar.Name,
            cvar.Description
        );

        node.OnValueChanged += newValue => cvar.SetValue(newValue);

        return node;
    }

    private IntegerInputNode RenderIntegerControl(IntegerWidgetConfigVariable cvar)
    {
        var node = new IntegerInputNode(
            GetNextControlId(cvar),
            Instance.GetConfigValue<int>(cvar.Id),
            cvar.MinValue,
            cvar.MaxValue,
            cvar.Name,
            cvar.Description
        );

        node.OnValueChanged += newValue => cvar.SetValue(newValue);

        return node;
    }

    private FloatInputNode RenderFloatControl(FloatWidgetConfigVariable cvar)
    {
        var node = new FloatInputNode(
            GetNextControlId(cvar),
            Instance.GetConfigValue<float>(cvar.Id),
            cvar.MinValue,
            cvar.MaxValue,
            cvar.Name,
            cvar.Description
        );

        node.OnValueChanged += newValue => cvar.SetValue(newValue);

        return node;
    }

    private StringInputNode RenderStringControl(StringWidgetConfigVariable cvar)
    {
        var node = new StringInputNode(
            GetNextControlId(cvar),
            Instance.GetConfigValue<string>(cvar.Id),
            (uint)cvar.MaxLength,
            cvar.Name,
            cvar.Description
        );

        node.OnValueChanged += cvar.SetValue;

        return node;
    }

    /// <summary>
    /// Returns a new ID for the control node.
    /// </summary>
    private string GetNextControlId(IWidgetConfigVariable cvar)
    {
        return $"control-{Crc32.Get(cvar.Id)}";
    }
}
