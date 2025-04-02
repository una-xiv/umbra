using Una.Drawing;

namespace Umbra.Widgets.Library.Compass;

internal sealed partial class CompassWidget
{
    private static Stylesheet CompassWidgetStylesheet { get; } = new([
        new(
            ".CompassWidget",
            new() {
                Anchor = Anchor.MiddleLeft,
                Padding = new(0),
                Margin = new(0),
                Size = new(250, 24),
            }
        ),
        new(
            ".CompassWidgetPoint",
            new() {
                Anchor = Anchor.None,
                Size = new(1, 24),
                BackgroundColor = new Color(255, 255, 255, 255),
            }
        )
    ]);
}
