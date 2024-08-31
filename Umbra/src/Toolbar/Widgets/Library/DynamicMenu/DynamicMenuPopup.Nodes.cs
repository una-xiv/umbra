using System;
using Umbra.Common;
using Umbra.Widgets.Library.ShortcutPanel.Providers;
using Una.Drawing;

namespace Umbra.Widgets.Library.DynamicMenu;

internal sealed partial class DynamicMenuPopup
{
    protected override Node Node { get; } = new() {
        Id         = "Popup",
        Stylesheet = Stylesheet,
        ChildNodes = [
            new() {
                Id = "ItemList",
            },
            new() {
                Id        = "EmptyButtonPlaceholder",
                NodeValue = I18N.Translate("Widget.DynamicMenu.EmptyButtonPlaceholder"),
            }
        ],
        BeforeReflow = node => {
            int width = node.Bounds.ContentSize.Width;

            foreach (var entry in node.QuerySelectorAll(".item, #EmptyButtonPlaceholder")) {
                entry.Bounds.MarginSize  = new(width, entry.Bounds.MarginSize.Height);
                entry.Bounds.PaddingSize = new(width, entry.Bounds.PaddingSize.Height);
                entry.Bounds.ContentSize = new(width, entry.Bounds.ContentSize.Height);
            }

            return true;
        },
    };

    private Node? CreateEntryNode(DynamicMenuEntry entry)
    {
        int itemIndex = Entries.IndexOf(entry);
        if (itemIndex == -1) return null;

        Node textNode  = new() { ClassList = ["item--text"], InheritTags      = true };
        Node iconNode  = new() { ClassList = ["item--icon-main"], InheritTags = true };
        Node icon2Node = new() { ClassList = ["item--icon-sub"], InheritTags  = true };
        Node countNode = new() { ClassList = ["item--count"], InheritTags     = true };

        Node node = new() {
            ClassList = ["item"],
            SortIndex = Entries.IndexOf(entry),
            ChildNodes = [
                new() {
                    ClassList  = ["item--icon-wrapper"],
                    ChildNodes = [iconNode, icon2Node, countNode],
                },
                textNode,
            ]
        };

        if (entry is { Pt: not null, Pi: not null }) {
            AbstractShortcutProvider? provider = Providers.GetProvider(entry.Pt);

            var shortcut = provider?.GetShortcut(entry.Pi.Value, WidgetInstanceId);
            if (null == shortcut) return null;

            node.ToggleClass("disabled", shortcut.Value.IsDisabled);
            textNode.ToggleClass("disabled", shortcut.Value.IsDisabled);
            iconNode.ToggleClass("disabled", shortcut.Value.IsDisabled);

            iconNode.Style.IconId  = shortcut.Value.IconId;
            icon2Node.Style.IconId = shortcut.Value.SubIconId;
            countNode.NodeValue    = shortcut.Value.Count is > 1 ? I18N.FormatNumber(shortcut.Value.Count.Value) : null;
            textNode.NodeValue     = shortcut.Value.Name;

            node.OnMouseUp += n => {
                // We shouldn't fully disable the item, so the context menu can still be opened.
                if (n.ClassList.Contains("disabled")) return;
                provider?.OnInvokeShortcut(0, itemIndex, entry.Pi.Value, WidgetInstanceId);
                Close();
            };
        } else {
            iconNode.Style.IconId = entry.Ci;
            textNode.NodeValue    = entry.Cl;

            node.OnMouseUp += _ => {
                InvokeCustomEntry(entry);
                Close();
            };
        }

        node.OnRightClick += _ => OpenContextMenu(itemIndex);

        return node;
    }

    private Node ItemList               => Node.FindById("ItemList")!;
    private Node EmptyButtonPlaceholder => Node.FindById("EmptyButtonPlaceholder")!;
}
