using Una.Drawing;

namespace Umbra.Widgets.Library.Societies;

internal sealed partial class SocietiesWidgetPopup
{
    private const int ItemWidth = 250;

    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                "#Popup",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new(8),
                    Gap     = 4,
                }
            ),
            new(
                "#AllowanceStatus",
                new() {
                    Size          = new(ItemWidth, 0),
                    Padding       = new(6, 4) { Top = 0 },
                    Font          = 0,
                    FontSize      = 11,
                    Color         = new("Widget.PopupMenuTextMuted"),
                    OutlineColor  = new("Widget.PopupMenuTextOutlineDisabled"),
                    OutlineSize   = 1,
                    TextAlign     = Anchor.MiddleCenter,
                    BorderColor   = new() { Bottom = new("Widget.Border") },
                    BorderWidth   = new() { Bottom = 1 },
                    IsAntialiased = false,
                }
            ),
            new(
                "#List",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                "#List:vertical",
                new() {
                    Flow = Flow.Vertical,
                }
            ),
            new(
                ".expansion",
                new() {
                    Flow = Flow.Vertical,
                    Size = new(ItemWidth, 0),
                    Gap  = 8,
                }
            ),
            new(
                ".expansion--label",
                new() {
                    Size          = new(ItemWidth, 0),
                    FontSize      = 11,
                    Color         = new("Widget.PopupMenuTextMuted"),
                    OutlineColor  = new("Widget.PopupMenuTextOutlineDisabled"),
                    OutlineSize   = 1,
                    Padding       = new(4, 0),
                    BorderColor   = new() { Bottom = new("Widget.PopupBorder") },
                    BorderWidth   = new() { Bottom = 1 },
                    IsAntialiased = false,
                }
            ),
            new(
                ".expansion--items",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 8,
                }
            ),
            new(
                ".society",
                new() {
                    Flow          = Flow.Horizontal,
                    Size          = new(ItemWidth, 0),
                    Gap           = 4,
                    Padding       = new(6),
                    StrokeColor   = new("Widget.Border"),
                    StrokeWidth   = 1,
                    BorderRadius  = 6,
                    IsAntialiased = false,
                }
            ),
            new(
                ".society:hover",
                new() {
                    BackgroundColor = new("Widget.PopupMenuBackgroundHover"),
                }
            ),
            new(
                ".society--icon",
                new() {
                    Size = new(38, 38),
                }
            ),
            new(
                ".society--body",
                new() {
                    Flow = Flow.Vertical,
                    Size = new(ItemWidth - (38 + 38 + 12 + 8), 0)
                }
            ),
            new(
                ".society--name",
                new() {
                    Size         = new(ItemWidth - (38 + 38 + 12 + 8), 0),
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".society--rank",
                new() {
                    Size         = new(ItemWidth - (38 + 38 + 12 + 8), 0),
                    FontSize     = 11,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".society--rank--value",
                new() {
                    Size         = new(32, 0),
                    Anchor       = Anchor.BottomRight,
                    TextAlign    = Anchor.TopRight,
                    FontSize     = 11,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".society--name:hover, .society--rank:hover, .society--rank--value:hover",
                new() {
                    Color        = new("Widget.PopupMenuTextHover"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineHover")
                }
            ),
            new(
                ".society--exp-bar",
                new() {
                    Size            = new(ItemWidth - (38 + 38 + 12 + 8), 4),
                    BackgroundColor = new("Widget.Background"),
                    StrokeColor     = new("Widget.Border"),
                    StrokeWidth     = 1,
                    IsAntialiased   = false,
                    Padding         = new(1),
                }
            ),
            new(
                ".society--exp-bar--bar",
                new() {
                    IsAntialiased   = false,
                    Size            = new(100, 2),
                    BackgroundColor = new("Window.AccentColor"),
                }
            ),
            new(
                ".society--currency",
                new() {
                    Size            = new(38, 38),
                    Padding         = new(2),
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.Border"),
                    StrokeWidth     = 1,
                    BorderRadius    = 6,
                    IsAntialiased   = false,
                }
            ),
            new(
                ".society--currency--icon",
                new() {
                    Size = new(34, 34),
                }
            ),
            new(
                ".society--currency--value",
                new() {
                    Anchor       = Anchor.BottomRight,
                    TextAlign    = Anchor.TopLeft,
                    FontSize     = 14,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 2,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
        ]
    );
}
