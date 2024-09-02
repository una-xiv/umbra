using Dalamud.Interface;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.IconPicker;

public sealed partial class FaIconPickerWindow
{
    protected override Node Node { get; } = new() {
        Id         = "IconPickerWindow",
        Stylesheet = IconPickerWindow.Stylesheet,
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
                Id = "Body",
                ChildNodes = [
                    new FaIconGridNode(icon) {
                        Id = "IconGrid",
                    }
                ]
            },
            new() {
                Id = "Footer",
                ChildNodes = [
                    new() {
                        Id = "FooterButtons",
                        ChildNodes = [
                            new ButtonNode("Undo",        I18N.Translate("Undo")),
                            new ButtonNode("CloseButton", I18N.Translate("Close")),
                        ]
                    },
                ]
            }
        ]
    };

    private Node            BodyNode          => Node.QuerySelector("#Body")!;
    private Node            FooterNode        => Node.QuerySelector("#Footer")!;
    private Node            SearchPanelNode   => Node.QuerySelector("#SearchPanel")!;
    private Node            SearchWrapperNode => SearchPanelNode.QuerySelector("#SearchInputWrapper")!;
    private StringInputNode SearchInputNode   => SearchPanelNode.QuerySelector<StringInputNode>("#Search")!;
    private ButtonNode      CloseButtonNode   => FooterNode.QuerySelector<ButtonNode>("#CloseButton")!;
    private ButtonNode      UndoButtonNode    => FooterNode.QuerySelector<ButtonNode>("#Undo")!;

    private void UpdateNodeSizes()
    {
        Node.Style.Size              = ContentSize;
        FooterNode.Style.Size        = new(ContentSize.Width, 50);
        BodyNode.Style.Size          = new(ContentSize.Width, ContentSize.Height - 95);
        SearchPanelNode.Style.Size   = new(ContentSize.Width, 0);
        SearchWrapperNode.Style.Size = new(ContentSize.Width - 55, 0);
        UndoButtonNode.IsDisabled    = Icon == LastIcon;

        FaIconGridNode? gridNode = BodyNode.QuerySelector<FaIconGridNode>("IconGrid");

        if (gridNode is not null && gridNode.Selected != Icon) {
            Icon = gridNode.Selected;
        }
    }
}
