using Una.Drawing;

namespace Umbra.Widgets.Library.DynamicMenu;

internal sealed partial class DynamicMenuPopup
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
                "#ItemList",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 4,
                }
            ),
            new(
                "#EmptyButtonPlaceholder",
                new() {
                    TextAlign     = Anchor.MiddleCenter,
                    Padding       = new(8),
                    Font          = (int)FontId.Default,
                    FontSize      = 11,
                    Color         = new("Widget.PopupMenuTextMuted"),
                    OutlineColor  = new("Widget.PopupMenuTextOutline"),
                    OutlineSize   = 1,
                    BorderWidth   = new() { Top = 1 },
                    BorderColor   = new() { Top = new("Widget.PopupBorder") },
                    IsAntialiased = false,
                }
            ),
            new(
                "#EmptyButtonPlaceholder:hover",
                new() {
                    Color = new("Widget.PopupMenuText"),
                }
            ),
            new(
                ".item",
                new() {
                    Gap  = 8,
                    Size = new(0, 36),
                }
            ),
            new(
                ".item:hover",
                new() {
                    BackgroundColor = new("Widget.PopupMenuBackgroundHover"),
                }
            ),
            new(
                ".item.separator",
                new() {
                    BorderWidth   = new() { Top = 1 },
                    BorderColor   = new() { Top = new("Widget.PopupBorder") },
                    BorderInset   = new() { Top = 3 },
                    Size          = new(0, 6),
                    IsAntialiased = false,
                }
            ),
            new(
                ".item.separator.has-text",
                new() {
                    BorderColor = new(),
                    BorderWidth = new(),
                    BorderInset = new(),
                    Size        = new(0, 24),
                }
            ),
            new(
                ".item.separator:hover",
                new() {
                    BackgroundColor = new(0),
                }
            ),
            new(
                ".separator--line",
                new() {
                    BorderWidth   = new() { Top = 1 },
                    BorderColor   = new() { Top = new("Widget.PopupBorder") },
                    BorderInset   = new() { Top = 3 },
                    Size          = new(0, 6),
                    IsAntialiased = false,
                }
            ),
            new(".separator--line.left", new() { Anchor  = Anchor.MiddleLeft }),
            new(".separator--line.right", new() { Anchor = Anchor.MiddleRight }),
            new(
                ".separator--text",
                new() {
                    Anchor       = Anchor.MiddleCenter,
                    Font         = (int)FontId.Default,
                    FontSize     = 11,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".item--icon-wrapper",
                new() {
                    Anchor  = Anchor.MiddleLeft,
                    Size    = new(36, 36),
                    Padding = new(2),
                }
            ),
            new(
                ".item--icon-main",
                new() {
                    Anchor = Anchor.MiddleCenter,
                    Size   = new(31, 31),
                }
            ),
            new(
                ".item--icon-main.disabled",
                new() {
                    Anchor         = Anchor.MiddleCenter,
                    Size           = new(31, 31),
                    ImageGrayscale = true,
                }
            ),
            new(
                ".item--icon-sub",
                new() {
                    Anchor = Anchor.TopLeft,
                    Size   = new(16, 16),
                }
            ),
            new(
                ".item--count",
                new() {
                    Anchor       = Anchor.BottomRight,
                    Font         = (int)FontId.Default,
                    FontSize     = 11,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".item--text",
                new() {
                    Anchor       = Anchor.MiddleLeft,
                    Font         = (int)FontId.Default,
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    Padding      = new() { Right = 8 },
                }
            ),
            new(
                ".item--text:hover",
                new() {
                    Color        = new("Widget.PopupMenuTextHover"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineHover"),
                }
            ),
            new(
                ".item--text:disabled, .item--text.disabled",
                new() {
                    Color       = new("Widget.PopupMenuTextDisabled"),
                    OutlineSize = 0,
                }
            ),
            new(
                ".item--text-alt",
                new() {
                    Anchor       = Anchor.MiddleRight,
                    TextAlign    = Anchor.MiddleRight,
                    Font         = (int)FontId.Default,
                    FontSize     = 11,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    Padding      = new() { Right = 8 },
                }
            ),
        ]
    );
}
