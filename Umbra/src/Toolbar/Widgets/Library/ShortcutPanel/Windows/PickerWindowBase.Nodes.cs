using Dalamud.Interface;
using System;
using Umbra.Windows;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal abstract partial class PickerWindowBase
{
    protected override Node Node { get; } = new() {
        Id         = "ItemPickerWindow",
        Stylesheet = WindowStyles.ItemPickerStylesheet,
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
                Id = "ItemList",
                Overflow = false,
                ChildNodes = [
                    new() { Id = "ItemListWrapper" }
                ]
            }
        ]
    };

    protected void AddItem(string label, string subLabel, uint iconId, Action onClick)
    {
        Node node = new() {
            ClassList = ["item"],
            ChildNodes = [
                new() {
                    ClassList = ["item-icon"],
                    Style     = new() { IconId = iconId },
                },
                new() {
                    ClassList = ["item-body"],
                    ChildNodes = [
                        new() {
                            ClassList = ["item-name"],
                            NodeValue = label,
                        },
                        new() {
                            ClassList = ["item-command"],
                            NodeValue = subLabel,
                        }
                    ]
                },
            ]
        };

        ItemListNode.AppendChild(node);

        node.Tooltip   =  label;
        node.OnMouseUp += _ => onClick();
    }

    protected Node ItemListNode => Node.QuerySelector("#ItemListWrapper")!;
}
