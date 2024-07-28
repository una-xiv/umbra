using Una.Drawing;

namespace Umbra.Widgets.Library.RetainerList;

internal partial class RetainerListPopup
{
    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                "#Popup",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new(8),
                    Gap     = 8,
                }
            ),
            new(
                ".row",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                ".row.header",
                new() {
                    Padding     = new() { Bottom = 5 },
                    BorderColor = new() { Bottom = new("Widget.Border") },
                    BorderWidth = new() { Bottom = 1 },
                }
            ),
            new(
                ".cell",
                new() {
                    Size         = new(250, 0),
                    TextOverflow = false,
                    WordWrap     = false,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    FontSize     = 13,
                    TextAlign    = Anchor.MiddleLeft,
                    Anchor       = Anchor.MiddleLeft,
                }
            ),
            new(".cell.name", new() { Size         = new(150, 0) }),
            new(".cell.job-icon", new() { Size     = new(18, 18) }),
            new(".cell.job-level", new() { Size    = new(32, 0) }),
            new(".cell.gil", new() { Size          = new(100, 0) }),
            new(".cell.items.inv", new() { Size    = new(100, 0) }),
            new(".cell.items.market", new() { Size = new(100, 0) }),
            new(".cell.venture-time", new() { Size = new(100, 0) }),
        ]
    );
}
