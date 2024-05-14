using Umbra.Common;
using Una.Drawing;

namespace Umbra.Style;

internal class ToolbarStyles
{
    [WhenFrameworkCompiling]
    public static void RegisterStyles()
    {
        Stylesheet.SetClassRule(
            "toolbar",
            new() {
                Flow        = Flow.Horizontal,
                Size        = new(0, 32),
                Padding     = new(0, 6),
                BorderColor = new(new("Toolbar.Border")),
            }
        );

        Stylesheet.SetClassRule(
            "toolbar-stretched-top",
            new() {
                Anchor             = Anchor.TopCenter,
                BackgroundGradient = GradientColor.Vertical(new("Toolbar.Background2"), new("Toolbar.Background1")),
                BorderWidth        = new() { Bottom = 1 },
            }
        );

        Stylesheet.SetClassRule(
            "toolbar-floating-top",
            new() {
                Anchor             = Anchor.TopCenter,
                BackgroundGradient = GradientColor.Vertical(new("Toolbar.Background2"), new("Toolbar.Background1")),
                BorderWidth        = new() { Bottom = 1, Left = 1, Right = 1 },
                BorderRadius       = 5,
                RoundedCorners     = RoundedCorners.BottomLeft | RoundedCorners.BottomRight,
            }
        );

        Stylesheet.SetClassRule(
            "toolbar-shadow-top",
            new() {
                Anchor = Anchor.TopCenter,
                BackgroundGradient = GradientColor.Vertical(
                    new("Toolbar.ShadowOpaque"),
                    new("Toolbar.ShadowTransparent")
                ),
            }
        );

        Stylesheet.SetClassRule(
            "toolbar-stretched-bottom",
            new() {
                Anchor             = Anchor.BottomCenter,
                BackgroundGradient = GradientColor.Vertical(new("Toolbar.Background1"), new("Toolbar.Background2")),
                BorderWidth        = new() { Top = 1 },
            }
        );

        Stylesheet.SetClassRule(
            "toolbar-floating-bottom",
            new() {
                Anchor             = Anchor.BottomCenter,
                BackgroundGradient = GradientColor.Vertical(new("Toolbar.Background1"), new("Toolbar.Background2")),
                BorderWidth        = new() { Top = 1, Left = 1, Right = 1 },
                BorderRadius       = 5,
                RoundedCorners     = RoundedCorners.TopLeft | RoundedCorners.TopRight,
            }
        );

        Stylesheet.SetClassRule(
            "toolbar-shadow-bottom",
            new() {
                Anchor = Anchor.BottomCenter,
                BackgroundGradient = GradientColor.Vertical(
                    new("Toolbar.ShadowTransparent"),
                    new("Toolbar.ShadowOpaque")
                ),
            }
        );

        Stylesheet.SetClassRule(
            "toolbar-panel",
            new() {
                Flow = Flow.Horizontal,
                Gap  = 6,
            }
        );
    }
}
