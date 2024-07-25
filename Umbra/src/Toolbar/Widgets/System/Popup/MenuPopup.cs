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
using Dalamud.Game.Text;
using Dalamud.Interface;
using Umbra.Common;
using Umbra.Style;
using Una.Drawing;

namespace Umbra.Widgets;

public class MenuPopup : WidgetPopup
{
    public bool IsDisabled { get; set; }

    protected sealed override Node Node { get; } = new() {
        Stylesheet = PopupStyles.MenuPopupStylesheet,
        Style = new() {
            Flow    = Flow.Vertical,
            Gap     = 6,
            Padding = new(4)
        },
    };

    public MenuPopup()
    {
        Node.BeforeDraw = node => {
            foreach (var n in node.QuerySelectorAll(".button"))
                n.QuerySelector("Icon")!.Style.ImageGrayscale = UseGrayscaleIcons;
        };

        Node.BeforeReflow = node => {
            foreach (var n in node.QuerySelectorAll(".button--label")) FillSpace(".button--label",     n);
            foreach (var n in node.QuerySelectorAll(".button--altText")) FillSpace(".button--altText", n);

            var width = 0;

            foreach (var n in node.QuerySelectorAll(".button")) {
                n.QuerySelector("Icon")!.Style.ImageGrayscale = UseGrayscaleIcons;
                n.RecomputeSize();
                width = Math.Max(width, n.InnerWidth);
            }

            foreach (var n in node.QuerySelectorAll(".button-group, .button-group--header, .button-group--items")) {
                n.Bounds.ContentSize.Width = width;
                n.Bounds.PaddingSize.Width = n.Bounds.ContentSize.Width + n.ComputedStyle.Padding.HorizontalSize;
                n.Bounds.MarginSize.Width  = n.Bounds.PaddingSize.Width + n.ComputedStyle.Margin.HorizontalSize;

                if (n.ClassList.Contains("button-group--header")) {
                    if (n.QuerySelector(".button-group--label")!.NodeValue is string str && string.IsNullOrEmpty(str)) {
                        n.QuerySelector(".button-group--label")!.Style.IsVisible = false;
                    } else {
                        n.QuerySelector(".button-group--label")!.Style.IsVisible = true;
                    }

                    int labelWidth = n.QuerySelector(".button-group--label")!.InnerWidth;
                    int lineWidth  = labelWidth > 10 ? (width - labelWidth) / 2 - 4 : (width / 2);

                    foreach (var line in n.QuerySelectorAll(".button-group--line")) {
                        line.Bounds.ContentSize.Width = lineWidth;

                        line.Bounds.PaddingSize.Width =
                            line.Bounds.ContentSize.Width + line.ComputedStyle.Padding.HorizontalSize;

                        line.Bounds.MarginSize.Width =
                            line.Bounds.PaddingSize.Width + line.ComputedStyle.Margin.HorizontalSize;
                    }
                }
            }

            Node.RecomputeSize();

            return true;
        };
    }

    /// <inheritdoc/>
    protected override bool CanOpen()
    {
        return !IsDisabled && base.CanOpen();
    }

    /// <summary>
    /// Whether to use grayscale icons for buttons that have icons associated
    /// with them.
    /// </summary>
    public bool UseGrayscaleIcons { get; set; } = true;

    /// <summary>
    /// Whether to close the popup when a button is clicked.
    /// </summary>
    public bool CloseOnItemClick { get; set; } = true;

    /// <summary>
    /// Clears all items from this popup.
    /// </summary>
    public void Clear()
    {
        Node.ChildNodes = [];
    }

    /// <summary>
    /// Adds a button to the menu.
    /// </summary>
    /// <param name="id">
    ///     The ID of this button. Use this to reference it later.
    /// </param>
    /// <param name="label">
    ///     The text that appears on the button.
    /// </param>
    /// <param name="sortIndex">
    ///     The sort index that determines where this button appears in the menu,
    ///     relative to other buttons. Lower values appear first.
    /// </param>
    /// <param name="iconId">
    ///     An ID that references an optional icon. If provided, the icon will
    ///     appear to the left of the button label.
    /// </param>
    /// <param name="altText">
    ///     Alternative text that appears on the right side of the button. This is
    ///     typically used to display a keyboard shortcut.
    /// </param>
    /// <param name="onClick">
    ///     The callback that is invoked when the button is clicked.
    /// </param>
    /// <param name="groupId">
    ///     An optional ID that references a button group. If provided, the button
    ///     is added to the specified group.
    /// </param>
    /// <param name="glyphColor">
    ///     The color of the icon glyph. This is only effective when
    ///     <paramref name="iconId"/> is a <see cref="SeIconChar"/>.
    /// </param>
    public void AddButton(
        string  id,
        string  label,
        int?    sortIndex  = null,
        object? iconId     = null,
        string? altText    = null,
        Action? onClick    = null,
        string? groupId    = null,
        Color?  glyphColor = null
    )
    {
        Node button = CreateButtonNode(id, label, sortIndex, iconId, altText, onClick, glyphColor);

        if (groupId is not null) {
            Node.QuerySelector($"Group_{groupId} > Items")?.AppendChild(button);
        } else {
            Node.AppendChild(button);
        }

        Node.Reflow();
    }

    public bool HasButton(string id)
    {
        return Node.QuerySelector(id) is not null;
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
    public void SetButtonAltLabel(string id, string? label)
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
    public void SetButtonIcon(string id, object? iconId)
    {
        Node? node = Node.QuerySelector($"{id}.button > .button--icon");

        if (node is null) {
            Logger.Warning($"Attempted to set icon for non-existent button: {id}");
            return;
        }

        switch (iconId) {
            case uint u:
                node.Style.IconId = u;
                node.NodeValue    = null;
                node.ClassList.Remove("button--icon--glyph");
                if (!node.ClassList.Contains("button--icon--image")) node.ClassList.Add("button--icon--image");
                break;
            case SeIconChar s:
                node.Style.IconId = null;
                node.Style.Font   = 4;
                node.NodeValue    = s.ToIconString();
                node.ClassList.Remove("button--icon--image");
                if (!node.ClassList.Contains("button--icon--glyph")) node.ClassList.Add("button--icon--glyph");
                break;
            case FontAwesomeIcon f:
                node.Style.IconId = null;
                node.Style.Font   = 2;
                node.NodeValue    = f.ToIconString();
                break;
            default:
                node.Style.IconId = null;
                node.NodeValue    = null;
                node.ClassList.Remove("button--icon--image");
                node.ClassList.Remove("button--icon--glyph");
                break;
        }
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
    /// Returns true if a group with the given ID exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool HasGroup(string id)
    {
        return Node.QuerySelector($"Group_{id}") is not null;
    }

    public uint GetGroupItemCount(string id)
    {
        return (uint)(Node.QuerySelector($"Group_{id} > Items")?.ChildNodes.Count ?? 0);
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
        object? iconId,
        string? altText    = null,
        Action? onClick    = null,
        Color?  glyphColor = null
    )
    {
        Node button = new() {
            Id        = id,
            SortIndex = sortIndex ?? 0,
            ClassList = ["button"],
            ChildNodes = [
                new() {
                    Id = "Icon",
                    ClassList = ["button--icon", iconId is SeIconChar ? "button--icon--glyph" : "button--icon--image"],
                    InheritTags = true,
                    Style = new() {
                        IconId = iconId is uint u ? u : null,
                        Font   = iconId is SeIconChar ? 4u : 0u,
                        Color  = iconId is SeIconChar ? glyphColor : new("Widget.PopupMenuText"),
                    }
                },
                new() {
                    Id          = "Label",
                    NodeValue   = label,
                    ClassList   = ["button--label"],
                    InheritTags = true,
                },
                new() {
                    Id          = "AltText",
                    NodeValue   = altText ?? " ",
                    ClassList   = ["button--altText"],
                    InheritTags = true,
                }
            ],
        };

        if (onClick is not null) {
            button.OnMouseUp += _ => {
                if (CloseOnItemClick) Close();
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
