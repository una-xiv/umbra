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
                    StrokeInset     = 1,
                    BorderRadius    = 6,
                    RoundedCorners  = RoundedCorners.All,
                    ShadowSize      = new(64),
                    ShadowInset     = 8,
                    Padding         = new(2),
                }
            }, {
                ".window--titlebar",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 32),
                    Color           = new("Window.TitlebarText"),
                    BackgroundColor = new("Window.TitlebarBackground"),
                    BackgroundGradient = GradientColor.Vertical(
                        new("Window.TitlebarGradient1"),
                        new("Window.TitlebarGradient2")
                    ),
                    BackgroundGradientInset = new(0) { Bottom = 0 },
                    BorderColor             = new(new("Window.TitlebarBorder")),
                    BorderWidth             = new() { Bottom = 1 },
                    BorderRadius            = 4,
                    RoundedCorners          = RoundedCorners.TopLeft | RoundedCorners.TopRight,
                    Margin                  = new(1) { Bottom = -1 },
                }
            }, {
                ".window--titlebar-text",
                new() {
                    FontSize     = 13,
                    OutlineColor = new("Window.TitlebarTextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.MiddleLeft,
                    TextOffset   = new(0, -1),
                    TextOverflow = false,
                    WordWrap     = false,
                    Size         = new(0, 32),
                    Padding      = new(0, 6)
                }
            }, {
                ".window--close-button",
                new() {
                    Anchor          = Anchor.TopRight,
                    Size            = new(23, 23),
                    BackgroundColor = new("Widget.TitlebarCloseButton"),
                    StrokeColor     = new("Window.TitlebarCloseButtonBorder"),
                    StrokeWidth     = 2,
                    StrokeInset     = 1,
                    BorderRadius    = 3,
                    TextAlign       = Anchor.MiddleCenter,
                    Font            = 2,
                    FontSize        = 12,
                    Color           = new("Window.TitlebarCloseButtonX"),
                    OutlineColor    = new("Window.TitlebarCloseButtonXOutline"),
                    TextOverflow    = true,
                    Margin          = new() { Top = 2, Right = 4 }
                }
            }, {
                ".window--close-button:hover",
                new() {
                    BackgroundColor = new("Window.TitlebarCloseButtonHover"),
                    Color           = new("Window.TitlebarCloseButtonXHover"),
                    StrokeInset     = 0,
                }
            }, {
                ".window--content",
                new() {
                    Anchor                    = Anchor.TopLeft,
                    Flow                      = Flow.Vertical,
                    BorderRadius              = 5,
                    RoundedCorners            = RoundedCorners.BottomLeft | RoundedCorners.BottomRight,
                    ScrollbarTrackColor       = new("Window.ScrollbarTrack"),
                    ScrollbarThumbColor       = new("Window.ScrollbarThumb"),
                    ScrollbarThumbHoverColor  = new("Window.ScrollbarThumbHover"),
                    ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
                }
            }
        }
    );
}
