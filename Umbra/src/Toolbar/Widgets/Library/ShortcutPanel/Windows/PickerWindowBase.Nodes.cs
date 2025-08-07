namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal abstract partial class PickerWindowBase
{
    protected void AddItem(string label, string subLabel, uint iconId, Action onClick)
    {
        Node node = new() {
            ClassList = ["item"],
            ChildNodes = [
                new() {
                    ClassList = ["icon"],
                    Style     = new() { IconId = iconId },
                },
                new() {
                    ClassList = ["body"],
                    ChildNodes = [
                        new() {
                            ClassList = ["name"],
                            NodeValue = label,
                        },
                        new() {
                            ClassList = ["command"],
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

    protected void AddTooManyResultsMessage()
    {
        ItemListNode.AppendChild(
            new() {
                Id        = "TooManyResultsMessage",
                NodeValue = I18N.Translate("Widget.ShortcutPanel.PickerWindow.TooManyResults"),
            }
        );
    }

    protected Node ItemListNode => RootNode.QuerySelector("#ItemList")!;
}
