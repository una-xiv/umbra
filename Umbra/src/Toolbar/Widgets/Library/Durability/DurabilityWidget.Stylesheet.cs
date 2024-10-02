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
                Padding      = new EdgeSize(2, 0),
            }
        ),
    ]);
}
