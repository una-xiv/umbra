using Una.Drawing;

namespace Umbra.Windows.Library.IconPicker;

public sealed partial class IconPickerWindow
{
    internal static Stylesheet Stylesheet { get; } = new(
        [
            new(
                "#IconPickerWindow",
                new() {
                    Flow = Flow.Vertical,
                }
            ),
            new(
                "#Header",
                new() {
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderColor     = new() { Bottom = new("Window.Border") },
                    BorderWidth     = new() { Bottom = 1 },
                    IsAntialiased   = false,
                    Gap             = 8,
                    Size            = new(64, 0),
                    Padding         = new(6),
                }
            ),
            new(
                "#IconPreview",
                new() {
                    Size            = new(52, 52),
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.Border"),
                    StrokeWidth     = 1,
                    BorderRadius    = 6,
                    IsAntialiased   = false,
                }
            ),
            new(
                "#HeaderContent",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                "#Body",
                new() {
                    Flow = Flow.Vertical,
                }
            ),
            new(
                "#Content",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new(8),
                }
            ),
            new(
                "#Footer",
                new() {
                    Flow            = Flow.Horizontal,
                    Gap             = 15,
                    Padding         = new(0, 15),
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderWidth     = new() { Top = 1 },
                    BorderColor     = new() { Top = new("Window.Border") },
                    IsAntialiased   = false,
                }
            ),
            new(
                "#FooterButtons",
                new() {
                    Anchor = Anchor.MiddleRight,
                    Gap    = 15,
                }
            ),
            new(
                "#SearchPanel",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 45),
                    FontSize        = 16,
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderColor     = new() { Bottom = new("Window.Border") },
                    BorderWidth     = new() { Bottom = 1 },
                    IsAntialiased   = false,
                    Padding         = new(10, 15),
                    Gap             = 5,
                }
            ),
            new(
                "#SearchInputWrapper",
                new() {
                    Flow = Flow.Horizontal,
                    Size = new(0, 30),
                }
            ),
            new(
                "#SearchIcon",
                new() {
                    Size         = new(26, 26),
                    Font         = 2,
                    FontSize     = 18,
                    Color        = new("Window.TextMuted"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.MiddleLeft,
                    TextOffset   = new(0, -1),
                }
            ),
        ]
    );
}
