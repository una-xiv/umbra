namespace Umbra.Windows.Components;

public class ProgressBarNode : Node
{
    private const int PaddingSize = 2;

    public Node BarNode { get; } = new() {
        ClassList   = ["bar", "bar--current-value"],
        InheritTags = true,
    };

    public Node OverflowNode { get; } = new() {
        ClassList   = ["bar", "bar--overflow-value"],
        InheritTags = true,
    };

    /// <summary>
    /// Minimum value allowed on this bar
    /// </summary>
    public int Minimum { get; set; }

    /// <summary>
    /// Maximum value allowed on this bar
    /// </summary>
    public int Maximum { get; set; } = 100;

    /// <summary>
    /// Current value the bar should display
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Define if the bar should support overflow. Overflow will handle a value range progress from 100% to 200%.
    /// </summary>
    public bool UseOverflow { get; set; }

    /// <summary>
    /// Define if the bar should be displayed with borders.
    /// </summary>
    public bool UseBorder {
        get => TagsList.Contains("bordered");
        set
        {
            if (value) {
                TagsList.Add("bordered");
            } else {
                TagsList.Remove("bordered");
            }
        }
    }

    /// <summary>
    /// Direction in which the bars will fill.
    /// </summary>
    public FillDirection Direction { get; set; } = FillDirection.LeftToRight;

    public ProgressBarNode(string id)
    {
        Id         = id;
        ClassList  = ["progressbar"];
        ChildNodes = [BarNode, OverflowNode];
        Stylesheet = BarStylesheet;

        BeforeDraw += UpdateBar;
    }

    public ProgressBarNode()
    {
        ClassList  = ["progressbar"];
        ChildNodes = [BarNode, OverflowNode];
        Stylesheet = BarStylesheet;

        BeforeDraw += UpdateBar;
    }

    private void UpdateBar(Node _)
    {
        if (!IsVisible) return;

        if (Minimum >= Maximum) {
            // Warn that values used are wrong
            Logger.Warning($"Bar {Id} with incorrect value: Minimum {Minimum} >= Maximum {Maximum}.");
            Style.IsVisible = false;
            return;
        }

        float progress = (Value - Minimum) / (float)(Maximum - Minimum);
        float overflow = 0;

        if (UseOverflow && progress > 1) {
            overflow = progress - 1;
            progress = 1;
        }

        float scaledWidth = InnerWidth / ScaleFactor;

        progress = Math.Clamp(progress, 0, 1);
        overflow = Math.Clamp(overflow, 0, 1);

        var progressWidth = progress * scaledWidth;
        var overflowWidth = overflow * scaledWidth;

        BarNode.Style.Size      = new(progressWidth, 0);
        OverflowNode.Style.Size = new(overflowWidth, 0);

        BarNode.Style.IsVisible      = progress > 0f;
        OverflowNode.Style.IsVisible = overflow > 0f;

        BarNode.ToggleTag("full", Value >= Maximum);

        if (Direction == FillDirection.RightToLeft) {
            BarNode.Style.Anchor      = Anchor.TopRight;
            OverflowNode.Style.Anchor = Anchor.BottomRight;
        } else {
            BarNode.Style.Anchor      = Anchor.TopLeft;
            OverflowNode.Style.Anchor = Anchor.BottomLeft;
        }
    }

    /// <summary>
    /// Default style for progress bars.
    /// </summary>
    private static readonly Stylesheet BarStylesheet = new([
        new(".progressbar", new() {
            Anchor       = Anchor.None,
            AutoSize     = (AutoSize.Grow, AutoSize.Grow),
            BorderRadius = 3,
            Margin       = new(3, 0),
        }),
        new(
            ".progressbar:bordered",
            new() {
                Padding         = new(PaddingSize),
                BackgroundColor = new("Widget.Background"),
                StrokeColor     = new("Widget.Border"),
                StrokeWidth     = 1,
                StrokeInset     = 1,
                StrokeRadius    = 3,
            }
        ),
        new(
            ".bar",
            new() {
                AutoSize      = (AutoSize.Fit, AutoSize.Grow),
                Size          = new(0),
                BorderRadius  = 2,
                IsAntialiased = true,
            }
        ),
        new(
            ".bar--current-value",
            new() {
                BackgroundColor = new("Widget.ProgressBar"),
            }
        ),
        new(
            ".bar--overflow-value",
            new() {
                BackgroundColor = new("Widget.ProgressBarOverflow"),
            }
        ),
    ]);

    public enum FillDirection
    {
        LeftToRight,
        RightToLeft,
    }
}
