using Umbra.Common;
using Una.Drawing;

namespace Umbra.Style;

internal class WidgetStyles
{
    [WhenFrameworkCompiling]
    public static void RegisterStyles()
    {
        RegisterDefaultWidgetStyles();
    }

    private static void RegisterDefaultWidgetStyles()
    {
        Stylesheet.SetClassRule(
            "toolbar-widget-default",
            new() {
                Flow            = Flow.Horizontal,
                Size            = new(0, 28),
                Anchor          = Anchor.MiddleLeft,
                Padding         = new(0, 6),
                BackgroundColor = new("Widget.Background"),
                StrokeColor     = new("Widget.Border"),
                StrokeWidth     = 1,
                StrokeInset     = 2,
                BorderRadius    = 5,
                StrokeRadius    = 4,
                Gap             = 3,
            }
        );

        Stylesheet.SetClassRule(
            "toolbar-widget-default--ghost",
            new() {
                BackgroundColor = new(0x00000000),
                BorderColor     = new(new(0x00000000)),
                BorderWidth     = new(0),
                BorderInset     = 0,
                StrokeWidth     = 0,
            }
        );
    }
}
