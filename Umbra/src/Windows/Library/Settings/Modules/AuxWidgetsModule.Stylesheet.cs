using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class AuxWidgetsModule
{
    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                "#AuxSettingsPanel",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new(8),
                    Gap     = 16,
                }
            ),
            new(
                ".separator",
                new() {
                    Size            = new(200, 1),
                    BackgroundColor = new("Window.Border"),
                }
            )
        ]
    );
}
