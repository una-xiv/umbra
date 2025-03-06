using Dalamud.Game.Text;
using System;
using System.Xml.Serialization;
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
                NodeValue = $"{SeIconChar.MouseRightClick.ToIconString()} {I18N.Translate("Widget.DynamicMenu.EmptyButtonPlaceholder")}",
            }
        ],
        BeforeReflow = node => {
            int maxLabelWidth    = 0;
            int maxAltLabelWidth = 0;

            foreach (var childNode in node.QuerySelectorAll(".item")) {
                if (childNode.ClassList.Contains("separator")) continue;

                Node? textNode    = childNode.QuerySelector(".item--text");
                Node? altTextNode = childNode.QuerySelector(".item--text-alt");

                if (textNode != null) {
                    maxLabelWidth = Math.Max(maxLabelWidth, textNode.Bounds.ContentSize.Width);
                }

                if (altTextNode != null) {
                    maxAltLabelWidth = Math.Max(maxAltLabelWidth, altTextNode.Bounds.ContentSize.Width);
                }
            }

            foreach (var childNode in node.QuerySelectorAll(".item")) {
                if (childNode.ClassList.Contains("separator")) continue;

                Node? textNode    = childNode.QuerySelector(".item--text");
                Node? altTextNode = childNode.QuerySelector(".item--text-alt");

                if (textNode != null) {
                    Size padding = textNode.Bounds.PaddingSize - textNode.Bounds.ContentSize;
                    Size margin  = textNode.Bounds.MarginSize - textNode.Bounds.ContentSize;
                    Size size    = new(maxLabelWidth, textNode.Bounds.ContentSize.Height);
                    textNode.Bounds.ContentSize = size;
                    textNode.Bounds.PaddingSize = size + padding;
                    textNode.Bounds.MarginSize  = size + margin;
                }

                if (altTextNode != null) {
                    Size padding = altTextNode.Bounds.PaddingSize - altTextNode.Bounds.ContentSize;
                    Size margin  = altTextNode.Bounds.MarginSize - altTextNode.Bounds.ContentSize;
                    Size size    = new(maxAltLabelWidth, altTextNode.Bounds.ContentSize.Height);

                    altTextNode.Bounds.ContentSize = size;
                    altTextNode.Bounds.PaddingSize = size + padding;
                    altTextNode.Bounds.MarginSize  = size + margin;
                }
            }

            Size paddingSize = node.Bounds.PaddingSize - node.Bounds.ContentSize;
            Size marginSize  = node.Bounds.MarginSize - node.Bounds.ContentSize;
            Size newNodeSize = new(maxLabelWidth + maxAltLabelWidth + (int)((36 + 48) * Node.ScaleFactor), node.Bounds.ContentSize.Height);

            node.Bounds.ContentSize = newNodeSize;
            node.Bounds.PaddingSize = (newNodeSize + paddingSize);
            node.Bounds.MarginSize  = (newNodeSize + marginSize);

            int width = node.Bounds.ContentSize.Width;

            foreach (var entry in node.QuerySelectorAll(".item, #EmptyButtonPlaceholder")) {
                entry.Bounds.MarginSize  = new(width, entry.Bounds.MarginSize.Height);
                entry.Bounds.PaddingSize = new(width, entry.Bounds.PaddingSize.Height);
                entry.Bounds.ContentSize = new(width, entry.Bounds.ContentSize.Height);

                if (entry.ClassList.Contains("separator")) {
                    Node? lineLeft  = entry.QuerySelector(".separator--line.left");
                    Node? lineRight = entry.QuerySelector(".separator--line.right");
                    Node? lineText  = entry.QuerySelector(".separator--text");

                    if (lineLeft != null && lineRight != null && lineText != null) {
                        var textWidth = lineText.Bounds.MarginSize.Width;
                        var lineWidth = (int)Math.Max(0, ((width - textWidth) / 2 - (4 * Node.ScaleFactor)));

                        lineLeft.Bounds.MarginSize   = new(lineWidth, lineLeft.Bounds.MarginSize.Height);
                        lineLeft.Bounds.PaddingSize  = new(lineWidth, lineLeft.Bounds.PaddingSize.Height);
                        lineLeft.Bounds.ContentSize  = new(lineWidth, lineLeft.Bounds.ContentSize.Height);
                        lineRight.Bounds.MarginSize  = new(lineWidth, lineRight.Bounds.MarginSize.Height);
                        lineRight.Bounds.PaddingSize = new(lineWidth, lineRight.Bounds.PaddingSize.Height);
                        lineRight.Bounds.ContentSize = new(lineWidth, lineRight.Bounds.ContentSize.Height);
                    }
                }
            }

            return true;
        },
        BeforeDraw = node => {

        }
    };

    private Node? CreateEntryNode(DynamicMenuEntry entry)
    {
        int itemIndex = Entries.IndexOf(entry);
        if (itemIndex == -1) return null;

        if (entry.Ct == null && entry.Cl == "-") {
            Node separator = new() {
                ClassList = ["item", "separator"],
                SortIndex = itemIndex,
            };

            if (!string.IsNullOrEmpty(entry.Sl)) {
                separator.AppendChild(new() { ClassList = ["separator--line", "left"] });

                separator.AppendChild(
                    new() {
                        ClassList = ["separator--text"],
                        NodeValue = entry.Sl,
                    }
                );

                separator.AppendChild(new() { ClassList = ["separator--line", "right"] });
                separator.ClassList.Add("has-text");
            }

            separator.OnRightClick += _ => OpenContextMenu(itemIndex);
            return separator;
        }

        Node textNode    = new() { ClassList = ["item--text"], InheritTags      = true };
        Node iconNode    = new() { ClassList = ["item--icon-main"], InheritTags = true };
        Node icon2Node   = new() { ClassList = ["item--icon-sub"], InheritTags  = true };
        Node countNode   = new() { ClassList = ["item--count"], InheritTags     = true };
        Node altTextNode = new() { ClassList = ["item--text-alt"], InheritTags  = true };

        iconNode.Style.Size        = new(EntryHeight - 5, EntryHeight - 5);

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
                    ClassList  = ["item--icon-wrapper"],
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
