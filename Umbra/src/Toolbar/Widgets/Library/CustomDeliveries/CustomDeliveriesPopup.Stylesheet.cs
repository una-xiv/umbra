using Una.Drawing;

namespace Umbra.Widgets.Library.CustomDeliveries;

internal sealed partial class CustomDeliveriesPopup
{
    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                "#Popup",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new(4),
                }
            ),
            new(
                "#AllowanceStatus",
                new() {
                    Size          = new(200, 0),
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
                ".npc",
                new() {
                    Flow          = Flow.Horizontal,
                    Gap           = 8,
                    Padding       = new(4),
                    BorderRadius  = 6,
                    IsAntialiased = false,
                }
            ),
            new(
                ".npc-icon",
                new() {
                    Size            = new(40, 48),
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.Border"),
                    StrokeWidth     = 1,
                    BorderRadius    = 6,
                    Padding         = new(2),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".npc-body",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 2,
                    Padding = new(4, 0),
                }
            ),
            new(
                ".npc-body--name",
                new() {
                    Font         = 0,
                    FontSize     = 13,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    Size         = new(120, 0),
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".npc-body--count",
                new() {
                    Anchor       = Anchor.TopRight,
                    Font         = 0,
                    FontSize     = 11,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineDisabled"),
                    TextAlign    = Anchor.TopRight,
                    Padding      = new(0, 4),
                }
            ),
            new(
                ".npc-body--hearts",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 6,
                }
            ),
            new(
                ".npc-body--hearts--heart",
                new() {
                    Size        = new(24, 24),
                    TextAlign   = Anchor.MiddleCenter,
                    TextOffset  = new(0, -1),
                    Font        = (uint)FontId.FontAwesome,
                    FontSize    = 16,
                    OutlineSize = 1,
                }
            ),
            new(
                ".npc-body--hearts--heart:empty",
                new() {
                    Color        = new("Widget.PopupMenuTextDisabled"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineDisabled"),
                    Opacity      = 0.65f,
                }
            ),
            new(
                ".npc-body--hearts--heart:filled",
                new() {
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    Opacity      = 1,
                }
            ),
            new(
                ".npc:hover",
                new() {
                    BackgroundColor = new("Widget.PopupMenuBackgroundHover"),
                }
            ),
            new(
                ".npc-body--name:hover, .npc-body--name--count:hover, .npc-body--hearts:hover",
                new() {
                    Color        = new("Widget.PopupMenuTextHover"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineHover"),
                }
            )
        ]
    );
}
