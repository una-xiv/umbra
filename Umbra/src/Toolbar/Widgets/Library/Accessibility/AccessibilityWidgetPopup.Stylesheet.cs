using Una.Drawing;

namespace Umbra.Widgets;

internal partial class AccessibilityWidgetPopup : WidgetPopup
{
    private static Stylesheet AccessibilityStylesheet { get; } = new(
        [
            new(
                ".popup",
                new() {
                    Flow    = Flow.Horizontal,
                    Padding = new(15),
                    Gap     = 15,
                }
            ),
            new(
                ".left-side",
                new() {
                    Flow = Flow.Horizontal,
                    Size = new(68 + 38, 100),
                    Gap  = 8,
                }
            ),
            new(
                ".right-side",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 8,
                    Size = new(200, 0),
                }
            ),
            new(
                ".slider",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 8,
                    Size = new(30, 100)
                }
            ),
            new(
                ".slider-label",
                new() {
                    Size      = new(30, 0),
                    TextAlign = Anchor.TopCenter,
                }
            )
        ]
    );
}
