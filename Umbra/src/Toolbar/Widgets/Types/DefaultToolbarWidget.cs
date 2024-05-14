using Dalamud.Game.Text.SeStringHandling;
using Una.Drawing;

namespace Umbra.Widgets;

public abstract class DefaultToolbarWidget : ToolbarWidget
{
    public sealed override Node Node { get; } = new() {
        ClassList = ["toolbar-widget-default"],
        ChildNodes = [
            new() {
                Id = "LeftIcon",
                Style = new() {
                    Size               = new(20, 20),
                    Anchor             = Anchor.MiddleLeft,
                    IconId             = 62101,
                    IconRounding       = 3,
                    IconRoundedCorners = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
                    Margin             = new() { Left = -2 },
                    IsVisible          = false,
                }
            },
            new() {
                Id        = "Label",
                NodeValue = "",
                Style = new() {
                    Flow         = Flow.Vertical,
                    Size         = new(0, 28),
                    Padding      = new(0, 2),
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleCenter,
                    Font         = 0,
                    FontSize     = 13,
                    Color        = new("Widget.Text"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    TextOffset   = new(0, 1),
                },
                ChildNodes = [
                    new() {
                        Id = "Top",
                        Style = new() {
                            Size      = new(0, 12),
                            Margin    = new() { Top = 1 },
                            Anchor    = Anchor.MiddleCenter,
                            TextAlign = Anchor.MiddleRight,
                            Color     = new("Widget.Text"),
                            IsVisible = false,
                            FontSize  = 11,
                            Stretch   = true,
                        }
                    },
                    new() {
                        Id = "Bottom",
                        Style = new() {
                            Size       = new(0, 12),
                            Margin     = new() { Top = -3 },
                            Anchor     = Anchor.MiddleCenter,
                            TextAlign  = Anchor.MiddleRight,
                            TextOffset = new(0, -1),
                            Color      = new("Widget.TextMuted"),
                            IsVisible  = false,
                            FontSize   = 10,
                            Stretch    = true,
                        }
                    }
                ]
            },
            new() {
                Id = "RightIcon",
                Style = new() {
                    Size               = new(20, 20),
                    Anchor             = Anchor.MiddleLeft,
                    IconId             = 62101,
                    IconRounding       = 3,
                    IconRoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
                    Margin             = new() { Right = -2 },
                    IsVisible          = false,
                }
            },
        ]
    };

    protected void SetGhost(bool isGhost)
    {
        switch (isGhost) {
            case true when !Node.ClassList.Contains("toolbar-widget-default--ghost"):
                Node.ClassList.Add("toolbar-widget-default--ghost");
                Node.QuerySelector("Label")!.Style.TextOffset = new(0, 1);
                break;
            case false when Node.ClassList.Contains("toolbar-widget-default--ghost"):
                Node.ClassList.Remove("toolbar-widget-default--ghost");
                Node.QuerySelector("Label")!.Style.TextOffset = new(0, 0);
                break;
        }
    }

    protected void SetLeftIcon(uint? iconId)
    {
        Node.QuerySelector("LeftIcon")!.Style.IconId    = iconId;
        Node.QuerySelector("LeftIcon")!.Style.IsVisible = iconId.HasValue;

        int lp = iconId.HasValue ? 0 : 6;
        int rp = LabelNode.Style.Padding!.Value.Right;
        LabelNode.Style.Padding = new(0, rp, 0, lp);
    }

    protected void SetRightIcon(uint? iconId)
    {
        Node.QuerySelector("RightIcon")!.Style.IconId    = iconId;
        Node.QuerySelector("RightIcon")!.Style.IsVisible = iconId.HasValue;

        int lp = LabelNode.Style.Padding!.Value.Left;
        int rp = iconId.HasValue ? 0 : 6;
        LabelNode.Style.Padding = new(0, rp, 0, lp);
    }

    protected void SetLabel(string? label)
    {
        Node.QuerySelector("Label")!.NodeValue = label;

        TopLabelNode.NodeValue          = null;
        BottomLabelNode.NodeValue       = null;
        TopLabelNode.Style.IsVisible    = false;
        BottomLabelNode.Style.IsVisible = false;
    }

    protected void SetLabel(SeString? label)
    {
        LabelNode.NodeValue = label;

        TopLabelNode.NodeValue          = null;
        BottomLabelNode.NodeValue       = null;
        TopLabelNode.Style.IsVisible    = false;
        BottomLabelNode.Style.IsVisible = false;
    }

    protected void SetTwoLabels(string? topLabel, string? bottomLabel)
    {
        LabelNode.NodeValue = null;

        TopLabelNode.NodeValue    = topLabel;
        BottomLabelNode.NodeValue = bottomLabel;

        TopLabelNode.Style.IsVisible    = !string.IsNullOrEmpty(topLabel);
        BottomLabelNode.Style.IsVisible = !string.IsNullOrEmpty(bottomLabel);
    }

    protected void SetLabelMaxWidth(int maxWidth)
    {
        Node.QuerySelector("Label")!.Style.Size!.Width = maxWidth;
    }

    protected void SetDisabled(bool isDisabled)
    {
        Node.IsDisabled    = isDisabled;
        Node.Style.Opacity = isDisabled ? .66f : 1f;

        Node.QuerySelector("LeftIcon")!.Style.IconGrayscale  = isDisabled;
        Node.QuerySelector("RightIcon")!.Style.IconGrayscale = isDisabled;
    }

    protected void SetTextAlignLeft()
    {
        LabelNode.Style.TextAlign       = Anchor.MiddleLeft;
        TopLabelNode.Style.TextAlign    = Anchor.MiddleLeft;
        BottomLabelNode.Style.TextAlign = Anchor.MiddleLeft;
    }

    protected void SetTextAlignRight()
    {
        LabelNode.Style.TextAlign       = Anchor.MiddleRight;
        TopLabelNode.Style.TextAlign    = Anchor.MiddleRight;
        BottomLabelNode.Style.TextAlign = Anchor.MiddleRight;
    }

    protected void SetTextAlignCenter()
    {
        LabelNode.Style.TextAlign       = Anchor.MiddleCenter;
        TopLabelNode.Style.TextAlign    = Anchor.MiddleCenter;
        BottomLabelNode.Style.TextAlign = Anchor.MiddleCenter;
    }

    private Node LabelNode       => Node.QuerySelector("Label")!;
    private Node TopLabelNode    => Node.QuerySelector("Label > Top")!;
    private Node BottomLabelNode => Node.QuerySelector("Label > Bottom")!;
}
