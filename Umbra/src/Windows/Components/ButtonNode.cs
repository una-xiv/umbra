using Dalamud.Interface;
using Una.Drawing;

namespace Umbra.Windows.Components;

public class ButtonNode : Node
{
    public string? Label {
        set => QuerySelector("Label")!.NodeValue = value;
    }

    public FontAwesomeIcon? Icon {
        set => QuerySelector("Icon")!.NodeValue = value?.ToIconString();
    }

    public ButtonNode(string id, string? label, FontAwesomeIcon? icon = null)
    {
        Id         = id;
        ClassList  = ["button"];
        Stylesheet = ButtonStylesheet;

        ChildNodes = [
            new() { Id = "Icon", ClassList  = ["button--icon"], InheritTags  = true },
            new() { Id = "Label", ClassList = ["button--label"], InheritTags = true },
        ];

        Label = label;
        Icon  = icon;

        BeforeDraw += _ => {
            QuerySelector("Icon")!.Style.IsVisible  = QuerySelector("Icon")!.NodeValue is not null;
            QuerySelector("Label")!.Style.IsVisible = QuerySelector("Label")!.NodeValue is not null;

            if (IsDisabled) {
                QuerySelector("Label")!.TagsList.Add("disabled");
                QuerySelector("Icon")!.TagsList.Add("disabled");
            } else {
                QuerySelector("Label")!.TagsList.Remove("disabled");
                QuerySelector("Icon")!.TagsList.Remove("disabled");
            }
        };
    }

    private static Stylesheet ButtonStylesheet { get; } = new(
        new() {
            {
                ".button", new() {
                    Padding         = new(0, 8),
                    BorderRadius    = 5,
                    StrokeInset     = 1,
                    StrokeWidth     = 1,
                    Gap             = 6,
                    BackgroundColor = new("Button.Background"),
                    StrokeColor     = new("Button.Border"),
                }
            }, {
                ".button:hover", new() {
                    BackgroundColor = new("Button.BackgroundHover"),
                    StrokeColor     = new("Button.BorderHover"),
                }
            }, {
                ".button:disabled", new() {
                    Color           = new("Button.TextDisabled"),
                    OutlineColor    = new("Button.TextOutlineDisabled"),
                    BackgroundColor = new("Button.BackgroundDisabled"),
                    StrokeColor     = new("Button.BorderDisabled"),
                }
            }, {
                ".button--icon", new() {
                    Anchor    = Anchor.MiddleLeft,
                    TextAlign = Anchor.MiddleLeft,
                    FontSize  = 13,
                    Font      = 2,
                    Size      = new(0, 24),
                }
            }, {
                ".button--label", new() {
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleCenter,
                    Size         = new(0, 24),
                    FontSize     = 13,
                    OutlineSize  = 1,
                    Color        = new("Button.Text"),
                    OutlineColor = new("Button.TextOutline"),
                }
            }, {
                ".button--icon:hover", new() {
                    Color        = new("Button.TextHover"),
                    OutlineColor = new("Button.TextOutlineHover"),
                }
            }, {
                ".button--label:hover", new() {
                    Color        = new("Button.TextHover"),
                    OutlineColor = new("Button.TextOutlineHover"),
                }
            }, {
                ".button--icon:disabled", new() {
                    Color        = new("Button.TextDisabled"),
                    OutlineColor = new("Button.TextOutlineDisabled"),
                }
            }, {
                ".button--label:disabled", new() {
                    Color        = new("Button.TextDisabled"),
                    OutlineColor = new("Button.TextOutlineDisabled"),
                }
            }
        }
    );
}
