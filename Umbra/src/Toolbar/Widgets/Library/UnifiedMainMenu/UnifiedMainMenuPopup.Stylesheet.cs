using Una.Drawing;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

internal sealed partial class UnifiedMainMenuPopup
{
    private const int CategoriesWidth = 200;
    private const int EntriesWidth    = 350;

    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                "#Popup",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new(0, 1),
                }
            ),
            new(
                "#Header",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new((CategoriesWidth + EntriesWidth) - 4, 64),
                    IsAntialiased   = false,
                    BackgroundColor = new("Window.Background"),
                    BorderRadius    = 6,
                    Margin          = new(2),
                    Padding         = new(8),
                    Gap             = 8,
                }
            ),
            new(
                "#Header:bottom",
                new() {
                    RoundedCorners     = RoundedCorners.TopLeft | RoundedCorners.TopRight,
                    BackgroundGradient = GradientColor.Vertical(new("Window.AccentColor"), new(0)),
                }
            ),
            new(
                "#Header:top",
                new() {
                    RoundedCorners     = RoundedCorners.BottomLeft | RoundedCorners.BottomRight,
                    BackgroundGradient = GradientColor.Vertical(new(0), new("Window.AccentColor")),
                }
            ),
            new(
                "#HeaderIcon",
                new() {
                    Anchor          = Anchor.MiddleLeft,
                    Size            = new(48, 48),
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.BorderHover"),
                    StrokeWidth     = 1,
                    BorderRadius    = 4,
                }
            ),
            new(
                "#HeaderLabel",
                new() {
                    Flow   = Flow.Vertical,
                    Anchor = Anchor.MiddleLeft,
                    Size   = new(0, 38),
                }
            ),
            new(
                "#HeaderLabelName",
                new() {
                    FontSize     = 20,
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                "#HeaderLabelInfo",
                new() {
                    FontSize     = 12,
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                "#Body",
                new() {
                    Flow          = Flow.Horizontal,
                    BorderColor   = new() { Top = new("Widget.PopupBorder") },
                    BorderWidth   = new() { Top = 1 },
                    IsAntialiased = false,
                    Padding       = new() { Top = 1 },
                }
            ),
            new(
                "#Categories",
                new() {
                    Flow                      = Flow.Vertical,
                    Size                      = new(CategoriesWidth, 475),
                    BackgroundColor           = new("Window.Background"),
                    BorderColor               = new() { Right = new("Window.Border") },
                    BorderWidth               = new() { Right = 1 },
                    Padding                   = new(4, 2),
                    IsAntialiased             = false,
                    ScrollbarTrackColor       = new("Window.ScrollbarTrack"),
                    ScrollbarThumbColor       = new("Window.ScrollbarThumb"),
                    ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
                    ScrollbarThumbHoverColor  = new("Window.ScrollbarThumbHover"),
                }
            ),
            new("#CategoriesWrapper", new() { Flow = Flow.Vertical }),
            new(
                ".category",
                new() {
                    Size          = new(CategoriesWidth - 4, 32),
                    TextAlign     = Anchor.MiddleLeft,
                    FontSize      = 13,
                    Color         = new("Window.Text"),
                    OutlineColor  = new("Window.TextOutline"),
                    OutlineSize   = 1,
                    Padding       = new(4, 8),
                    Gap           = 4,
                    IsAntialiased = false,
                }
            ),
            new(
                ".categories-list",
                new() {
                    Flow = Flow.Vertical,
                }
            ),
            new(
                ".category:selected",
                new() {
                    BackgroundColor = new("Input.Background"),
                    Size            = new(CategoriesWidth, 32),
                    BorderColor     = new() { Top = new("Window.Border"), Bottom = new("Window.Border") },
                    BorderWidth     = new() { Top = 1, Bottom                    = 1 }
                }
            ),
            new(
                ".category--icon",
                new() {
                    Size = new(24, 24),
                }
            ),
            new(
                ".category--label",
                new() {
                    Size         = new(CategoriesWidth - 52, 24),
                    TextAlign    = Anchor.MiddleLeft,
                    FontSize     = 13,
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".category--label:selected",
                new() {
                    Color        = new("Input.Text"),
                    OutlineColor = new("Input.TextOutline"),
                }
            ),
            new(
                ".separator",
                new() {
                    Size = new(CategoriesWidth, 24),
                }
            ),
            new(
                ".separator--line",
                new() {
                    Anchor          = Anchor.MiddleCenter,
                    Size            = new(CategoriesWidth - 32, 1),
                    BackgroundColor = new("Window.AccentColor"),
                    IsAntialiased   = false,
                }
            ),
            new(
                "#PinnedList",
                new() {
                    Flow    = Flow.Vertical,
                    Size    = new(CategoriesWidth, 0),
                    Gap     = 8,
                    Padding = new(8),
                }
            ),
            new(
                "#Entries",
                new() {
                    Flow                      = Flow.Vertical,
                    Size                      = new(EntriesWidth, 475),
                    BackgroundColor           = new("Input.Background"),
                    ScrollbarTrackColor       = new("Window.ScrollbarTrack"),
                    ScrollbarThumbColor       = new("Window.ScrollbarThumb"),
                    ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
                    ScrollbarThumbHoverColor  = new("Window.ScrollbarThumbHover"),
                }
            ),
            new(
                ".entries",
                new() {
                    Flow      = Flow.Vertical,
                    Size      = new(EntriesWidth, 475),
                    Gap       = 8,
                    IsVisible = false,
                    Padding   = new(8),
                }
            ),
            new(
                ".entry",
                new() {
                    Flow    = Flow.Horizontal,
                    Size    = new(EntriesWidth - 16, 24),
                    Padding = new(2, 0),
                    Gap     = 4,
                }
            ),
            new(
                ".entry--icon",
                new() {
                    Anchor    = Anchor.MiddleLeft,
                    Size      = new(24, 24),
                    TextAlign = Anchor.MiddleCenter,
                    FontSize  = 16,
                }
            ),
            new(
                ".entry--name",
                new() {
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleLeft,
                    Size         = new(EntriesWidth - 136, 28),
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".entry--info",
                new() {
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleRight,
                    Size         = new(EntriesWidth - 20 - 250, 28),
                    FontSize     = 12,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineDisabled"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".entry-separator",
                new() {
                    Size = new(EntriesWidth - 8, 4),
                }
            ),
            new(
                ".entry-separator--line",
                new() {
                    Anchor          = Anchor.MiddleCenter,
                    Size            = new(EntriesWidth - 8, 1),
                    BackgroundColor = new("Widget.PopupBorder"),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".category:hover, .entry:hover, .pinned-entry:hover",
                new() {
                    BackgroundColor = new("Widget.PopupMenuBackgroundHover"),
                }
            ),
            new(
                ".category--label:hover, .entry--name:hover, .entry--info:hover, .pinned-entry--name:hover",
                new() {
                    Color        = new("Widget.PopupMenuTextHover"),
                    OutlineColor = new("Widget.PopupMenuTextHoverOutline"),
                }
            ),
            new(
                ".pinned-entry",
                new() {
                    Flow    = Flow.Horizontal,
                    Size    = new(CategoriesWidth - 16, 24),
                    Padding = new(2, 0),
                    Gap     = 4,
                }
            ),
            new(
                ".pinned-entry--icon",
                new() {
                    Anchor    = Anchor.MiddleLeft,
                    Size      = new(24, 24),
                    TextAlign = Anchor.MiddleCenter,
                    FontSize  = 16,
                }
            ),
            new(
                ".pinned-entry--name",
                new() {
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleLeft,
                    Size         = new(CategoriesWidth - 42, 28),
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".entry:disabled, .pinned-entry:disabled",
                new() {
                    Opacity         = 0.5f,
                    BackgroundColor = new(0),
                    Color           = new("Widget.PopupMenuTextDisabled"),
                    OutlineColor    = new("Widget.PopupMenuTextOutlineDisabled"),
                }
            ),
        ]
    );
}
