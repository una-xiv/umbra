using Umbra.Widgets.Library.ShortcutPanel.Providers;

namespace Umbra.Widgets;

internal sealed partial class DynamicMenuPopup
{
    private Node? CreateEntryNode(DynamicMenuEntry entry)
    {
        int itemIndex = Entries.IndexOf(entry);
        if (itemIndex == -1) return null;

        if (entry.Ct == null && entry.Cl == "-") {
            Node separator = new() { 
                ClassList = ["separator"],
                ChildNodes = [new() { ClassList = ["line"] }]
            };
            
            Node separatorBase = new() {
                ClassList = ["item", "separator"],
                SortIndex = itemIndex,
                ChildNodes = [separator],
            };

            if (!string.IsNullOrEmpty(entry.Sl)) {
                separator.AppendChild(
                    new() {
                        ClassList = ["separator-text"],
                        NodeValue = entry.Sl,
                    }
                );

                separator.AppendChild(new() { ClassList = ["line"] });
                separator.ClassList.Add("has-text");
            }

            separatorBase.OnRightClick += _ => OpenContextMenu(itemIndex);
            return separatorBase;
        }

        Node textNode    = new() { ClassList = ["text"], InheritTags       = true };
        Node iconNode    = new() { ClassList = ["icon"], InheritTags       = true };
        Node icon2Node   = new() { ClassList = ["icon-sub"], InheritTags   = true };
        Node countNode   = new() { ClassList = ["item-count"], InheritTags = true };
        Node altTextNode = new() { ClassList = ["text-alt"], InheritTags   = true };

        iconNode.Style.Size = new(EntryHeight - 5, EntryHeight - 5);

        if (entry.Cj != 0) {
            iconNode.Style.ImageColor = new(entry.Cj);
        }

        icon2Node.Style.IsVisible  = ShowSubIcons;
        countNode.Style.IsVisible  = ShowItemCount;
        textNode.Style.FontSize    = EntryFontSize;
        altTextNode.Style.FontSize = AltEntryFontSize;
        altTextNode.NodeValue      = entry.Cm;

        Node node = new() {
            ClassList = ["item"],
            SortIndex = Entries.IndexOf(entry),
            Style     = new() { Size = new(0, EntryHeight) },
            ChildNodes = [
                new() {
                    ClassList  = ["icon-wrapper"],
                    ChildNodes = [iconNode, icon2Node, countNode],
                    Style = new() {
                        Size = new(EntryHeight, EntryHeight),
                    }
                },
                textNode,
                altTextNode,
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
