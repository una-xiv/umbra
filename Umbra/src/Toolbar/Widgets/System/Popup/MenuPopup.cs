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
using Umbra.Common;
using Umbra.Style;
using Una.Drawing;

namespace Umbra.Widgets;

public class MenuPopup : WidgetPopup
{
    protected sealed override Node Node { get; } = new() {
        Stylesheet = PopupStyles.MenuPopupStylesheet,
        Style = new() {
            Flow    = Flow.Vertical,
            Gap     = 6,
            Padding = new(4),
        },
    };

    public MenuPopup()
    {
        Node.BeforeReflow = node => {
            foreach (var n in node.QuerySelectorAll(".button--label")) FillSpace(".button--label", n);
            foreach (var n in node.QuerySelectorAll(".button--altText")) FillSpace(".button--altText", n);

            var width = 0;
            foreach (var n in node.QuerySelectorAll(".button")) {
                n.RecomputeSize();
                width = Math.Max(width, n.InnerWidth);
            }

            foreach (var n in node.QuerySelectorAll(".button-group, .button-group--header, .button-group--items")) {
                n.Bounds.ContentSize.Width = width;
                n.Bounds.PaddingSize.Width = n.Bounds.ContentSize.Width + n.ComputedStyle.Padding.HorizontalSize;
                n.Bounds.MarginSize.Width  = n.Bounds.PaddingSize.Width + n.ComputedStyle.Margin.HorizontalSize;

                if (n.ClassList.Contains("button-group--header")) {
                    foreach (var line in n.QuerySelectorAll(".button-group--line")) {
                        line.Bounds.ContentSize.Width = (width - n.QuerySelector(".button-group--label")!.InnerWidth) / 2 - 4;
                        line.Bounds.PaddingSize.Width = line.Bounds.ContentSize.Width + line.ComputedStyle.Padding.HorizontalSize;
                        line.Bounds.MarginSize.Width  = line.Bounds.PaddingSize.Width + line.ComputedStyle.Margin.HorizontalSize;
                    }
                }
            }

            Node.RecomputeSize();

            return true;
        };
    }

    /// <summary>
    /// Adds a button to the menu.
    /// </summary>
    /// <param name="id">
    /// The ID of this button. Use this to reference it later.
    /// </param>
    /// <param name="label">
    /// The text that appears on the button.
    /// </param>
    /// <param name="sortIndex">
    /// The sort index that determines where this button appears in the menu,
    /// relative to other buttons. Lower values appear first.
    /// </param>
    /// <param name="iconId">
    /// An ID that references an optional icon. If provided, the icon will
    /// appear to the left of the button label.
    /// </param>
    /// <param name="altText">
    /// Alternative text that appears on the right side of the button. This is
    /// typically used to display a keyboard shortcut.
    /// </param>
    /// <param name="onClick">
    /// The callback that is invoked when the button is clicked.
    /// </param>
    /// <param name="groupId">
    /// An optional ID that references a button group. If provided, the button
    /// is added to the specified group.
    /// </param>
    public void AddButton(
        string  id,
        string  label,
        int?    sortIndex = null,
        uint?   iconId    = null,
        string? altText   = null,
        Action? onClick   = null,
        string? groupId   = null
    )
    {
        Node button = CreateButtonNode(id, label, sortIndex, iconId, altText, onClick);

        if (groupId is not null) {
            Node.QuerySelector($"Group_{groupId} > Items")?.AppendChild(button);
        } else {
            Node.AppendChild(button);
        }

        Node.Reflow();
    }

    /// <summary>
    /// Sets the label of a button with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the button.</param>
    /// <param name="label">The label to show on the button.</param>
    public void SetButtonLabel(string id, string label)
    {
        Node? node = Node.QuerySelector($"{id}.button > .button--label");

        if (node is null) {
            Logger.Warning($"Attempted to set label for non-existent button: {id}");
            return;
        }

        node.NodeValue = label;
    }

    /// <summary>
    /// Sets the alternative text of a button with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the button.</param>
    /// <param name="label">The alternative text to show on the button.</param>
    public void SetButtonAltLabel(string id, string label)
    {
        Node? node = Node.QuerySelector($"{id}.button > .button--altText");

        if (node is null) {
            Logger.Warning($"Attempted to set alt-label for non-existent button: {id}");
            return;
        }

        node.NodeValue = label;
    }

    /// <summary>
    /// Sets the icon of a button with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the button.</param>
    /// <param name="iconId">The ID of the icon or NULL to disable</param>
    public void SetButtonIcon(string id, uint? iconId)
    {
        Node? node = Node.QuerySelector($"{id}.button > .button--icon");

        if (node is null) {
            Logger.Warning($"Attempted to set icon for non-existent button: {id}");
            return;
        }

        node.Style.IconId = iconId;
    }

    public void SetButtonSortIndex(string id, int sortIndex)
    {
        Node? node = Node.QuerySelector($"{id}.button");

        if (node is null) {
            Logger.Warning($"Attempted to set sort-index for non-existent button: {id}");
            return;
        }

        node.SortIndex = sortIndex;
    }

    /// <summary>
    /// Disables or enables a button with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the button.</param>
    /// <param name="disabled">Whether the button should be disabled.</param>
    public void SetButtonDisabled(string id, bool disabled)
    {
        Node? node = Node.QuerySelector(id);

        if (node is null) {
            Logger.Warning($"Attempted to set disabled state for non-existent button: {id}");
            return;
        }

        node.IsDisabled = disabled;
    }

    /// <summary>
    /// Sets the visibility of a button with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the button.</param>
    /// <param name="visible">Whether the button should be visible.</param>
    public void SetButtonVisibility(string id, bool visible)
    {
        Node? node = Node.QuerySelector(id);

        if (node is null) {
            Logger.Warning($"Attempted to set visibility state for non-existent button: {id}");
            return;
        }

        node.Style.IsVisible = visible;
    }

    /// <summary>
    /// Removes a button with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the button.</param>
    public void RemoveButton(string id)
    {
        Node? node = Node.QuerySelector(id);

        if (node is null) {
            Logger.Warning($"Attempted to remove non-existent button: {id}");
            return;
        }

        node.Remove();
    }

    /// <summary>
    /// Adds a button group to the menu.
    /// </summary>
    /// <param name="id">
    /// The ID of the group. Use this to reference it in <see cref="AddButton"/>
    /// groupId parameter or when removing the group or updating its label.
    /// </param>
    /// <param name="label">
    /// The label of the group. This is displayed at the top of the group.
    /// </param>
    /// <param name="sortIndex">
    /// The sort index of this group. This determines where the group appears
    /// relative to other groups and buttons. Lower values appear first.
    /// </param>
    public void AddGroup(string id, string label, int sortIndex = 0)
    {
        Node group = CreateGroupNode(id, label, sortIndex);
        Node.AppendChild(group);
    }

    /// <summary>
    /// Sets the label of a group with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the group.</param>
    /// <param name="label">The label to show on the group.</param>
    public void SetGroupLabel(string id, string label)
    {
        Node? node = Node.QuerySelector($"Group_{id} > Header > Label");

        if (node is null) {
            Logger.Warning($"Attempted to set label for non-existent group: {id}");
            return;
        }

        node.NodeValue = label;
    }

    /// <summary>
    /// Sets the sort index of a group with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the group.</param>
    /// <param name="sortIndex">The sort index to set.</param>
    public void SetGroupSortIndex(string id, int sortIndex)
    {
        Node? node = Node.QuerySelector($"Group_{id}");

        if (node is null) {
            Logger.Warning($"Attempted to set sort-index for non-existent group: {id}");
            return;
        }

        node.SortIndex = sortIndex;
    }

    /// <summary>
    /// Sets the visibility of a group with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the group.</param>
    /// <param name="visible">Whether the group should be visible.</param>
    public void SetGroupVisibility(string id, bool visible)
    {
        Node? node = Node.QuerySelector($"Group_{id}");

        if (node is null) {
            Logger.Warning($"Attempted to set visibility state for non-existent group: {id}");
            return;
        }

        node.Style.IsVisible = visible;
    }

    /// <summary>
    /// Removes a group with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the group.</param>
    public void RemoveGroup(string id)
    {
        Node? node = Node.QuerySelector($"Group_{id}");

        if (node is null) {
            Logger.Warning($"Attempted to remove non-existent group: {id}");
            return;
        }

        node.Remove();
    }

    #region Factories

    private Node CreateGroupNode(string id, string label, int sortIndex)
    {
        return new() {
            Id        = $"Group_{id}",
            SortIndex = sortIndex,
            ClassList = ["button-group"],
            ChildNodes = [
                new() {
                    Id        = "Header",
                    ClassList = ["button-group--header"],
                    ChildNodes = [
                        new() { ClassList = ["button-group--line"] },
                        new() {
                            Id        = "Label",
                            NodeValue = label,
                            ClassList = ["button-group--label"],
                        },
                        new() { ClassList = ["button-group--line"] },
                    ]
                },
                new() {
                    Id        = "Items",
                    ClassList = ["button-group--items"],
                }
            ],
            BeforeReflow = node => {
                node.Style.IsVisible = node.QuerySelector("Items")!.ChildNodes.Count > 0;
                return true;
            }
        };
    }

    /// <summary>
    /// Creates and returns a button node.
    /// </summary>
    private Node CreateButtonNode(
        string  id,
        string  label,
        int?    sortIndex,
        uint?   iconId,
        string? altText = null,
        Action? onClick = null
    )
    {
        Node button = new() {
            Id        = id,
            SortIndex = sortIndex ?? 0,
            ClassList = ["button"],
            ChildNodes = [
                new() {
                    Id        = "Icon",
                    ClassList = ["button--icon"],
                    Style     = new() { IconId = iconId }
                },
                new() {
                    Id        = "Label",
                    NodeValue = label,
                    ClassList = ["button--label"],
                },
                new() {
                    Id        = "AltText",
                    NodeValue = altText ?? " ",
                    ClassList = ["button--altText"],
                }
            ],
            BeforeDraw = node => {
                Node labelNode = node.QuerySelector("Label")!;

                switch (node.IsMouseOver) {
                    case true when !labelNode.TagsList.Contains("hover"):
                        labelNode.TagsList.Add("hover");
                        break;
                    case false when labelNode.TagsList.Contains("hover"):
                        labelNode.TagsList.Remove("hover");
                        break;
                }
            },
        };

        if (onClick is not null) {
            button.OnClick += _ => {
                Close();
                onClick();
            };
        }

        return button;
    }

    private void FillSpace(string selector, Node node)
    {
        IEnumerable<Node> nodes = Node.QuerySelectorAll(selector);

        int maxWidth = nodes
            .Where(n => n.IsVisible)
            .Select(labelNode => labelNode.InnerWidth)
            .Prepend(node.InnerWidth)
            .Max();

        node.Bounds.ContentSize.Width = Math.Max(node.InnerWidth, maxWidth);
        node.Bounds.PaddingSize.Width = node.Bounds.ContentSize.Width + node.ComputedStyle.Padding.HorizontalSize;
        node.Bounds.MarginSize.Width  = node.Bounds.PaddingSize.Width + node.ComputedStyle.Margin.HorizontalSize;
    }

    #endregion
}
