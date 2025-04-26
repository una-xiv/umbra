using System.Linq;
using System.Numerics;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.IconPicker;

public sealed partial class IconPickerWindow
{
    protected override Node Node { get; } = new() {
        Id         = "IconPickerWindow",
        Stylesheet = Stylesheet,
        ChildNodes = [
            new() {
                Id = "Header",
                ChildNodes = [
                    new() {
                        Id = "IconPreview",
                    },
                    new() {
                        Id = "HeaderContent",
                        ChildNodes = [
                            new() {
                                Id = "CategoryWrapper",
                                ChildNodes = [
                                    new SelectNode(
                                        "CategoryId",
                                        "General",
                                        CategoryLabels.Keys.ToList(),
                                        I18N.Translate("Window.IconPicker.Category")
                                    )
                                ]
                            },
                            new() {
                                Id = "IconIdWrapper",
                                ChildNodes = [
                                    new IntegerInputNode(
                                        "IconId",
                                        0,
                                        0,
                                        int.MaxValue,
                                        I18N.Translate("Window.IconPicker.IconId")
                                    )
                                ]
                            },
                        ]
                    }
                ]
            },
            new() {
                Id = "Body",
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

    private Node             IconPreviewNode    => Node.QuerySelector("#IconPreview")!;
    private Node             HeaderNode         => Node.QuerySelector("#Header")!;
    private Node             HeaderContentNode  => Node.QuerySelector("#HeaderContent")!;
    private Node             CategoryWrapper    => Node.QuerySelector("#CategoryWrapper")!;
    private Node             IconIdWrapper      => Node.QuerySelector("#IconIdWrapper")!;
    private SelectNode       CategorySelectNode => CategoryWrapper.QuerySelector<SelectNode>("#CategoryId")!;
    private IntegerInputNode IconIdInputNode    => Node.QuerySelector<IntegerInputNode>("#IconId")!;
    private Node             BodyNode           => Node.QuerySelector("#Body")!;
    private Node             FooterNode         => Node.QuerySelector("#Footer")!;
    private ButtonNode       CloseButtonNode    => FooterNode.QuerySelector<ButtonNode>("#CloseButton")!;
    private ButtonNode       UndoButtonNode     => FooterNode.QuerySelector<ButtonNode>("#Undo")!;

    private void UpdateNodeSizes()
    {
        float headerWidth = ContentSize.Width - 48;

        Node.Style.Size                  = ContentSize;
        IconPreviewNode.Style.IconId     = IconId;
        HeaderNode.Style.Size            = new(ContentSize.Width, 64);
        FooterNode.Style.Size            = new(ContentSize.Width, 50);
        HeaderContentNode.Style.Size     = new(headerWidth, 48);
        IconIdWrapper.Style.Size         = new((headerWidth / 2) - 4, 48);
        CategoryWrapper.Style.Size       = new((headerWidth / 2) - 4, 48);
        BodyNode.Style.Size              = new(ContentSize.Width, ContentSize.Height - 114);
        IconIdInputNode.Style.Padding    = new(0);
        IconIdInputNode.Style.Gap        = 0;
        CategorySelectNode.Style.Padding = new(0);
        CategorySelectNode.Style.Gap     = 0;
        UndoButtonNode.IsDisabled        = IconId == LastIconId;

        IconGridNode? gridNode = BodyNode.QuerySelector<IconGridNode>("IconGrid");

        if (gridNode is not null && gridNode.SelectedId != IconId) {
            IconId                = gridNode.SelectedId;
            IconIdInputNode.Value = (int)IconId;
        }
    }
}
