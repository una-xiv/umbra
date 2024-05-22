using Una.Drawing;

namespace Umbra.Widgets;

public partial class GearsetNode
{
    private static Stylesheet GearsetSwitcherItemStylesheet { get; } = new(
        [
            new(
                ".gearset",
                new() {
                    Size            = new(NodeWidth, NodeHeight),
                    IsAntialiased   = false,
                    BackgroundColor = new("Input.Background"),
                    BorderRadius    = 7,
                    Padding         = new(5),
                    StrokeWidth     = 1,
                }
            ),
            new(
                ".gearset:current",
                new() {
                    StrokeColor = new("Input.TextMuted"),
                    StrokeWidth = 1,
                }
            ),
            new(
                ".gearset:hover",
                new() {
                    BackgroundColor = new("Input.BackgroundHover"),
                    StrokeColor     = new("Input.BorderHover")
                }
            ),
            new(
                ".gearset--icon",
                new() {
                    Size            = new(30, 30),
                    ImageInset      = new(2),
                    BackgroundColor = new("Window.Background")
                }
            ),
            new(
                ".gearset--body",
                new() {
                    Flow          = Flow.Vertical,
                    Size          = new(NodeWidth - 30 - 60),
                    Padding       = new() { Left = 6 },
                    Gap           = 4,
                }
            ),
            new(
                ".gearset--body--name",
                new() {
                    Font         = 0,
                    FontSize     = 13,
                    Color        = new("Input.Text"),
                    Size         = new(NodeWidth - 30 - 60, 0),
                    TextOverflow = false,
                    WordWrap     = false,
                    Padding      = new() { Bottom = 2 },
                }
            ),
            new(
                ".gearset--body--info",
                new() {
                    Font         = 0,
                    FontSize     = 11,
                    Color        = new("Input.Text"),
                    Size         = new(NodeWidth - 30 - 60, 0),
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".gearset--ilvl",
                new() {
                    Size      = new(50, NodeHeight - 10),
                    TextAlign = Anchor.MiddleRight,
                    Color     = new("Widget.PopupMenuTextMuted"),
                    FontSize  = 20,
                }
            )
        ]
    );
}
