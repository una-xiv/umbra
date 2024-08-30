using Una.Drawing;

namespace Umbra.Widgets;

internal partial class VolumeWidgetPopup
{
    private static Stylesheet VolumeWidgetPopupStylesheet { get; } = new(
        [
            new(
                ".popup",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new(8),
                    Gap     = 8,
                }
            ),
            new(
                ".channel-list",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                ".separator",
                new() {
                    Size            = new(0, 1),
                    BackgroundColor = new(0x60000000),
                    Stretch         = true,
                }
            ),
            new(
                ".channel",
                new() {
                    Flow            = Flow.Vertical,
                    Gap             = 8,
                    Size            = new(48, 0),
                    Padding         = new(4),
                    BorderRadius    = 7,
                    BackgroundColor = new(0x60000000),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".channel--name",
                new() {
                    Anchor       = Anchor.TopCenter,
                    Size         = new(40, 0),
                    Font         = 0,
                    FontSize     = 12,
                    TextAlign    = Anchor.TopCenter,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".channel--value",
                new() {
                    Anchor       = Anchor.TopCenter,
                    Size         = new(40, 0),
                    FontSize     = 12,
                    TextAlign    = Anchor.TopCenter,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineDisabled"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".channel--slider",
                new() {
                    Anchor = Anchor.TopCenter,
                    Size   = new(40, 150)
                }
            ),
            new(
                ".channel--buttons",
                new() {
                    Anchor = Anchor.TopCenter,
                    Flow   = Flow.Vertical,
                    Size   = new(40, 0),
                    Gap    = 4,
                }
            ),
            new(
                ".channel--ctrl-button",
                new() {
                    Anchor       = Anchor.TopCenter,
                    Size         = new(24, 24),
                    Font         = 2,
                    FontSize     = 12,
                    TextAlign    = Anchor.MiddleCenter,
                    StrokeWidth  = 1,
                    Color        = new("Widget.PopupMenuTextMuted"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineDisabled"),
                    OutlineSize  = 1,
                }
            ),
            new(
                ".channel--ctrl-button:hover",
                new() {
                    BackgroundColor = new("Widget.Background"),
                    StrokeColor     = new("Widget.Border"),
                }
            ),
            new(
                ".options-list",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 8,
                    Size = new(350, 0)
                }
            )
        ]
    );
}
