using Dalamud.Interface;
using Lumina.Excel.Sheets;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets.Library.EmoteList.Window;

internal sealed partial class EmotePickerWindow
{
    protected override Node Node { get; } = new() {
        Id         = "EmotePickerWindow",
        Stylesheet = Stylesheet,
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
                Id       = "EmoteList",
                Overflow = false,
                ChildNodes = [
                    new() { Id = "EmoteListWrapper" }
                ]
            }
        ]
    };

    private void AddEmoteNode(Emote emote)
    {
        Node node = new() {
            ClassList = ["emote"],
            ChildNodes = [
                new() {
                    ClassList = ["emote-icon"],
                    Style     = new() { IconId = emote.Icon },
                },
                new() {
                    ClassList = ["emote-body"],
                    ChildNodes = [
                        new() {
                            ClassList = ["emote-name"],
                            NodeValue = emote.Name.ExtractText(),
                        },
                        new() {
                            ClassList = ["emote-command"],
                            NodeValue = $"{emote.TextCommand.Value.Command.ExtractText()}",
                        }
                    ]
                }
            ]
        };

        if (UsedEmoteIds.Contains(emote.RowId)) {
            node.AppendChild(
                new() {
                    ClassList = ["emote-added-indicator"],
                    NodeValue = FontAwesomeIcon.Check.ToIconString(),
                }
            );
        }

        Node.QuerySelector("#EmoteListWrapper")!.AppendChild(node);

        Emote e = emote;
        node.Tooltip = emote.Name.ExtractText();

        node.OnMouseUp += _ => {
            SelectedEmote = e;
            Close();
        };
    }
}
