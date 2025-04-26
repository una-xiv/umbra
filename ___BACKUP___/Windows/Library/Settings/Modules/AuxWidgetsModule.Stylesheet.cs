using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class AuxWidgetsModule
{
    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                "#AuxSettingsPanel",
                new() {
                    Padding = new(8),
                }
            ),
            new(
                "#AuxSettingsList",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 16,
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
