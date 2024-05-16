using Una.Drawing;

namespace Umbra.Windows;

public static class WindowStyles
{
    public static readonly Stylesheet WindowStylesheet = new(
        new() {
            {
                ".window",
                new() {
                    Anchor          = Anchor.TopLeft,
                    Flow            = Flow.Vertical,
                    BackgroundColor = new("Window.Background"),
                    StrokeColor     = new("Window.Border"),
                    StrokeWidth     = 1,
                    BorderRadius    = 5,
                    ShadowSize      = new(64),
                    ShadowInset     = 5,
                    Padding         = new(1),
                }
            }, {
                ".window--titlebar",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 24),
                    Color           = new("Window.TitlebarText"),
                    BackgroundColor = new("Window.TitlebarBackground"),
                    BackgroundGradient = GradientColor.Vertical(
                        new("Window.TitlebarGradient1"),
                        new("Window.TitlebarGradient2")
                    ),
                    BackgroundGradientInset = new(2),
                    StrokeColor             = new("Window.BorderColor"),
                    StrokeWidth             = 1,
                    BorderRadius            = 4,
                    RoundedCorners          = RoundedCorners.TopLeft | RoundedCorners.TopRight,
                    FontSize                = 13,
                    OutlineColor            = new("Window.TitlebarTextOutline"),
                    OutlineSize             = 1,
                    Padding                 = new() { Left = 8 }
                }
            }
        }
    );
}
