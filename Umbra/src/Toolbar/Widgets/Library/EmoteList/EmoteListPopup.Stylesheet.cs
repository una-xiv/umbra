using Una.Drawing;

namespace Umbra.Widgets.Library.EmoteList;

internal sealed partial class EmoteListPopup
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
                "#CategoryBar",
                new() {
                    Flow          = Flow.Horizontal,
                    Size          = new(424, 0),
                    BorderColor   = new() { Bottom = new("Widget.PopupBorder") },
                    BorderWidth   = new() { Bottom = 1 },
                    IsAntialiased = false,
                }
            ),
            new(
                ".category-button",
                new() {
                    Size         = new(102, 24),
                    Padding      = new(0, 4),
                    TextAlign    = Anchor.MiddleLeft,
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineDisabled"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".category-button:hover",
                new() {
                    Color           = new("Widget.PopupMenuText"),
                    OutlineColor    = new("Widget.PopupMenuTextOutline"),
                    BackgroundColor = new(0x40FFFFFF),
                }
            ),
            new(
                ".category-button:selected",
                new() {
                    Color         = new("Widget.PopupMenuText"),
                    OutlineColor  = new("Widget.PopupMenuTextOutline"),
                    BorderColor   = new() { Bottom = new("Window.AccentColor") },
                    BorderWidth   = new() { Bottom = 3 },
                    IsAntialiased = false,
                }
            ),
            new(
                "#Footer",
                new() {
                    Flow          = Flow.Vertical,
                    Size          = new(424, 0),
                    Padding       = new(8, 0),
                    BorderColor   = new() { Top = new("Widget.PopupBorder") },
                    BorderWidth   = new() { Top = 1 },
                    IsAntialiased = false,
                    Gap           = 4,
                }
            ),
            new(
                ".emote-container",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 8,
                }
            ),
            new(
                ".emote-row",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                ".emote-button",
                new() {
                    Size            = new(46, 46),
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.Border"),
                    StrokeWidth     = 1,
                    BorderRadius    = 6,
                    Padding         = new(2),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".emote-button:hover",
                new() {
                    BackgroundColor = new("Input.BackgroundHover"),
                    StrokeColor     = new("Input.BorderHover"),
                }
            ),
            new(
                ".emote-button:empty",
                new() {
                    Opacity         = 0.15f,
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.Border"),
                }
            ),
            new(
                ".emote-button:empty:hover",
                new() {
                    Opacity = 0.35f,
                }
            ),
            new(
                ".emote-button--icon",
                new() {
                    Size          = new(42, 42),
                    BorderRadius  = 5,
                    IsAntialiased = false,
                    FontSize      = 10,
                }
            ),
        ]
    );
}
