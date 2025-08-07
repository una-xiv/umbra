namespace Umbra.Widgets;

internal partial class ExperienceBarWidget : ToolbarWidget
{
    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                ".experience-bar",
                new() {
                    Flow            = Flow.Horizontal,
                    Anchor          = Anchor.MiddleLeft,
                    Size            = new(150, SafeHeight),
                    BackgroundColor = new("Widget.Background"),
                    StrokeColor     = new("Widget.Border"),
                    StrokeWidth     = 1,
                    StrokeInset     = 1,
                    BorderRadius    = 5,
                    Padding         = new(0, 4),
                }
            ),
            new(
                ".experience-bar:ghost",
                new() {
                    BackgroundColor = new(0),
                    StrokeColor     = new(0),
                }
            ),
            new(
                ".sanctuary-icon",
                new() {
                    Font         = 2,
                    FontSize     = 14,
                    Color        = new("Widget.Text"),
                    Padding      = new(0, 4),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    Size         = new(0, SafeHeight),
                    Anchor       = Anchor.TopLeft,
                    TextAlign    = Anchor.MiddleLeft,
                }
            ),
            new(
                ".sync-icon",
                new() {
                    Font         = 1,
                    FontSize     = 14,
                    Color        = new("Widget.Text"),
                    Padding      = new(0, 0, 0, 3),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    Size         = new(0, SafeHeight),
                    Anchor       = Anchor.TopLeft,
                    TextAlign    = Anchor.MiddleLeft,
                    TextOffset   = new(0, 3),
                }
            ),
            new(
                ".label",
                new() {
                    FontSize     = 13,
                    Color        = new("Widget.Text"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    Size         = new(0, SafeHeight),
                    TextOverflow = false,
                    WordWrap     = false,
                    Padding      = new(0, 4),
                }
            ),
            new(
                ".label.left",
                new() {
                    Anchor    = Anchor.TopLeft,
                    TextAlign = Anchor.MiddleLeft,
                }
            ),
            new(
                ".label.right",
                new() {
                    Anchor    = Anchor.TopRight,
                    TextAlign = Anchor.MiddleRight,
                }
            ),
            new(
                ".bar",
                new() {
                    Size          = new(50, SafeHeight - 8),
                    BorderRadius  = 3,
                }
            ),
            new(
                ".bar.normal",
                new() {
                    Anchor          = Anchor.MiddleLeft,
                    BackgroundColor = new("Misc.ExperienceBar"),
                }
            ),
            new(
                ".bar.rested",
                new() {
                    Anchor          = Anchor.MiddleLeft,
                    BackgroundColor = new("Misc.ExperienceBarRested"),
                }
            )
        ]
    );
}
