using System;
using Umbra.Common;
using Una.Drawing;

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
    public int Minimum { get; set; } = 0;

    /// <summary>
    /// Maximum value allowed on this bar
    /// </summary>
    public int Maximum { get; set; } = 100;

    /// <summary>
    /// Current value the bar should display
    /// </summary>
    public int Value { get; set; } = 0;

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

        progress = Math.Clamp(progress, 0, 1);
        overflow = Math.Clamp(overflow, 0, 1);

        int padding     = UseBorder ? PaddingSize * 2 : 0;
        int innerHeight = Height - padding;

        var progressWidth = (int)(progress * (Width - padding));
        var overflowWidth = (int)(overflow * (Width - padding));

        BarNode.Style.Size      = new(progressWidth, innerHeight);
        OverflowNode.Style.Size = new(overflowWidth, innerHeight);

        if (Value >= Maximum) {
            BarNode.TagsList.Add("full");
        } else {
            BarNode.TagsList.Remove("full");
        }

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
            BorderRadius  = 5,
            IsAntialiased = true,
        }),
        new(
            ".progressbar:bordered",
            new() {
                Padding         = new(PaddingSize),
                BackgroundColor = new("Widget.Background"),
                StrokeColor     = new("Widget.Border"),
                StrokeWidth     = 1,
                StrokeInset     = 1,
                StrokeRadius    = 4,
            }
        ),
        new(
            ".bar",
            new() {
                Size          = new(0),
                BorderRadius  = 3,
                IsAntialiased = true,
            }
        ),
        new(
            ".bar--current-value",
            new() {
                BackgroundColor = new("Window.TextMuted"),
            }
        ),
        new(
            ".bar--overflow-value",
            new() {
                BackgroundColor = new("Window.Text"),
            }
        ),
    ]);

    public enum FillDirection
    {
        LeftToRight,
        RightToLeft,
    }
}
