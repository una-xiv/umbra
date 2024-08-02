using Dalamud.Interface;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets.Library.EmoteList.Window;

internal sealed partial class EmotePickerWindow : Windows.Window
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
                Id = "EmoteList",
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
                            NodeValue = emote.Name.ToDalamudString().TextValue,
                        },
                        new() {
                            ClassList = ["emote-command"],
                            NodeValue = $"{emote.TextCommand.Value!.Command.ToDalamudString().TextValue}",
                        }
                    ]
                },
            ]
        };

        Node.QuerySelector("#EmoteListWrapper")!.AppendChild(node);

        Emote e = emote;
        node.Tooltip = emote.Name.ToDalamudString().TextValue;
        node.OnMouseUp += _ => {
            SelectedEmote = e;
            Close();
        };
    }
}
