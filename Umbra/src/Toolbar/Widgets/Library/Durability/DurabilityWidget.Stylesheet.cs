using Una.Drawing;

namespace Umbra.Widgets;

internal partial class DurabilityWidget
{

    private static Stylesheet Stylesheet { get; } = new([
        new (
            "#BarWrapper",
            new () {
                Flow         = Flow.Vertical,
                Gap          = 1,
                Padding      = new EdgeSize(2, 0)
            }
        ),
        new (
            ".bar-container",
            new () {
                BorderRadius = 5
            }
        ),
        new (
            ".bar-container:bordered",
            new () {
                BackgroundColor = new("Widget.Background"),
                StrokeColor     = new("Widget.Border"),
                StrokeWidth     = 1,
                StrokeInset     = 1,
                BorderRadius    = 5,
                StrokeRadius    = 4,
                Padding         = new EdgeSize(2)
            }
        ),
        new (
            ".bar",
            new () {
                Size = new(0, 0),
                BorderRadius = 3,
                IsAntialiased = true
            }
        ),
        new(
            "#DurabilityBar",
            new() {
                BackgroundColor = new("Misc.DurabilityBar"),
                Anchor = Anchor.BottomLeft
            }
        ),
        new(
            "#SpiritbondBar",
            new() {
                BackgroundColor = new("Misc.SpiritbondBar"),
            }
        )
    ]);
}
