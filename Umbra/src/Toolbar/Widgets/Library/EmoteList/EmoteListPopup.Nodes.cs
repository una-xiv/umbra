using Umbra.Common;
using Umbra.Style;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets.Library.EmoteList;

internal sealed partial class EmoteListPopup
{
    private Node GetCategoryButton(byte id)              => Node.QuerySelector($"CategoryButton_{id}")!;
    private Node GetEmoteContainer(byte id)              => Node.QuerySelector($"EmoteContainer_{id}")!;
    private Node GetEmoteButton(byte    listId, byte id) => Node.QuerySelector($"EmoteButton_{listId}_{id}")!;

    private Node         CategoryBarNode => Node.QuerySelector("CategoryBar")!;
    private CheckboxNode WriteToChatNode => Node.QuerySelector<CheckboxNode>("WriteToChat")!;
    private CheckboxNode KeepOpenNode    => Node.QuerySelector<CheckboxNode>("KeepOpen")!;

    protected override Node Node { get; } = new() {
        Id         = "Popup",
        Stylesheet = PopupStyles.ButtonGridStylesheet,
        ChildNodes = [
            new() {
                Id = "CategoryBar",
                ChildNodes = [
                    new() {
                        Id       = "CategoryButton_0", ClassList = ["category-button"], NodeValue = "Category 1",
                        TagsList = ["active"]
                    },
                    new() {
                        Id        = "CategoryButton_1", ClassList = ["category-button"],
                        NodeValue = "Category 2 with a very long name"
                    },
                    new() { Id = "CategoryButton_2", ClassList = ["category-button"], NodeValue = "Category 3" },
                    new() { Id = "CategoryButton_3", ClassList = ["category-button"], NodeValue = "Category 4" },
                ]
            },
            CreateEmoteContainer(0, true),
            CreateEmoteContainer(1),
            CreateEmoteContainer(2),
            CreateEmoteContainer(3),
            new() {
                Id = "Footer",
                ChildNodes = [
                    new CheckboxNode("WriteToChat", false, I18N.Translate("Widget.EmoteList.Option.WriteToChat")),
                    new CheckboxNode("KeepOpen",    false, I18N.Translate("Widget.EmoteList.Option.KeepPopupOpen")),
                ]
            }
        ]
    };

    private static Node CreateEmoteContainer(byte listId, bool isVisible = false)
    {
        return new() {
            Id        = $"EmoteContainer_{listId}",
            ClassList = ["slot-container"],
            ChildNodes = [
                CreateEmoteRow(listId, 0),
                CreateEmoteRow(listId, 1),
                CreateEmoteRow(listId, 2),
                CreateEmoteRow(listId, 3),
            ],
            Style = new() { IsVisible = isVisible },
        };
    }

    private static Node CreateEmoteRow(byte listId, byte rowId)
    {
        return new() {
            Id        = $"WidgetRow_{listId}_{rowId}",
            ClassList = ["slot-row"],
            ChildNodes = [
                CreateEmoteButton(listId, rowId, 0),
                CreateEmoteButton(listId, rowId, 1),
                CreateEmoteButton(listId, rowId, 2),
                CreateEmoteButton(listId, rowId, 3),
                CreateEmoteButton(listId, rowId, 4),
                CreateEmoteButton(listId, rowId, 5),
                CreateEmoteButton(listId, rowId, 6),
                CreateEmoteButton(listId, rowId, 7),
            ]
        };
    }

    private static Node CreateEmoteButton(byte listId, byte rowId, byte index)
    {
        var id = (rowId * 8) + index;

        return new() {
            Id        = $"EmoteButton_{listId}_{id}",
            ClassList = ["slot-button"],
            ChildNodes = [
                new() {
                    ClassList = ["slot-button--icon", "empty"],
                }
            ]
        };
    }
}
