namespace Umbra.Widgets;

internal partial class DurabilityWidget
{
    private static Stylesheet Stylesheet { get; } = new([
        new(
            "#BarWrapper",
            new() {
                Anchor   = Anchor.MiddleCenter,
                AutoSize = (AutoSize.Grow, AutoSize.Grow),
                Flow     = Flow.Vertical,
                Gap      = 2,
                Padding  = new EdgeSize(4, 2),
            }
        ),
    ]);
}
