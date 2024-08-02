using System.Numerics;
using Una.Drawing;

namespace Umbra.Widgets.Library.EmoteList.Window;

internal sealed partial class EmotePickerWindow
{
    private static Stylesheet Stylesheet { get; } = new(
        [
            new("#EmotePickerWindow", new() { Flow = Flow.Vertical }),
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
            new(
                "#EmoteList",
                new() {
                    Flow = Flow.Vertical,
                }
            ),
            new(
                "#EmoteListWrapper",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 10,
                    Padding = new(10),
                }
            ),
            new(
                ".emote",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 42),
                    Padding         = new(5),
                    Gap             = 10,
                    IsAntialiased   = false,
                    BackgroundColor = new("Input.Background"),
                }
            ),
            new(
                ".emote:hover",
                new() {
                    BackgroundColor = new("Input.BackgroundHover"),
                    StrokeColor     = new("Input.Border"),
                    StrokeWidth     = 1,
                }
            ),
            new(
                ".emote-icon",
                new() {
                    Size = new(32, 32),
                }
            ),
            new(
                ".emote-body",
                new() {
                    Flow = Flow.Vertical,
                    Gap  = 2,
                }
            ),
            new(
                ".emote-name",
                new() {
                    FontSize     = 13,
                    Color        = new("Input.Text"),
                    OutlineColor = new("Input.TextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".emote-name:hover",
                new() {
                    Color        = new("Input.TextHover"),
                    OutlineColor = new("Input.TextOutlineHover"),
                }
            ),
            new(
                ".emote-command",
                new() {
                    Font         = 1,
                    FontSize     = 12,
                    Color        = new("Window.TextMuted"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            )
        ]
    );
}
