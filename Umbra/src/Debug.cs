using Dalamud.Plugin.Services;
using ImGuiNET;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra;

[Service]
public sealed class UmbraDebug
{
    private static readonly Style TmpStyle = new() {
        BorderColor     = 0x80FFAA77,
        BorderRadius    = 4,
        BorderWidth     = 2,
        Gradient        = new(0),
        Font            = Font.MiedingerLarge,
        ForegroundColor = 0xFF00AA00,
        TextAlign       = Anchor.MiddleCenter,
        OutlineWidth    = 1,
        OutlineColor    = 0xFF000000
    };

    private readonly Element _el = new(
        id: "RootTest",
        flow: Flow.Horizontal,
        anchor: Anchor.BottomRight,
        size: new(800, 1000),
        padding: new(0),
        text: "Hello World!",
        style: new() {
            ForegroundColor = 0xFF00FFFF,
            BackgroundColor = 0xFF212021,
            Gradient        = new(0x50AACCFF, 0x30FFCCAA),
            BorderRadius    = 8,
            BorderColor     = 0xFF0000FF,
            BorderWidth     = 1,
            RoundedCorners  = RoundedCorners.All,
            TextAlign       = Anchor.BottomCenter,
            Font            = Font.AxisLarge,
            OutlineColor    = 0xFF000000,
            OutlineWidth    = 2,
        },
        children: [
            new(
                "TL",
                anchor: Anchor.TopLeft,
                flow: Flow.Horizontal,
                children: [
                    new(
                        "TL1",
                        anchor: Anchor.TopLeft,
                        size: new(100, 100),
                        style: TmpStyle,
                        margin: new(0),
                        padding: new(4)
                    ),
                    new(
                        "TL2",
                        anchor: Anchor.TopLeft,
                        size: new(100, 100),
                        style: TmpStyle,
                        margin: new(0),
                        padding: new(4)
                    ),
                    new(
                        "TL3",
                        anchor: Anchor.TopLeft,
                        size: new(100, 100),
                        style: new() { Image = new(14), Shadow = new(size: 64, inset: new(6)) },
                        margin: new(0),
                        padding: new(4),
                        text: "I has icon"
                    ),
                    new(
                        "TL4",
                        anchor: Anchor.TopLeft,
                        size: new(100, 100),
                        style: new() { Image = new("images\\icon.png"), Shadow = new(size: 64, inset: new(6)) },
                        margin: new(0),
                        padding: new(4)
                    ),
                ]
            ),
            new("TR1", anchor: Anchor.TopRight, size: new(100, 100), style: TmpStyle, margin: new(0), padding: new(4)),
            new(
                "ML1",
                anchor: Anchor.MiddleLeft,
                size: new(100, 100),
                style: TmpStyle,
                margin: new(0),
                padding: new(4)
            ),
            new(
                "ML2",
                anchor: Anchor.MiddleLeft,
                size: new(100, 100),
                style: TmpStyle,
                margin: new(0),
                padding: new(4)
            ),
            new(
                "MC1",
                anchor: Anchor.MiddleCenter,
                size: new(100, 100),
                style: TmpStyle,
                margin: new(0),
                padding: new(4)
            ),
            new(
                "MC2",
                anchor: Anchor.MiddleCenter,
                size: new(100, 100),
                style: TmpStyle,
                margin: new(0),
                padding: new(4)
            ),
            new(
                "MR1",
                anchor: Anchor.MiddleRight,
                size: new(100, 100),
                style: TmpStyle,
                margin: new(0),
                padding: new(4)
            ),
            new(
                "MR2",
                anchor: Anchor.MiddleRight,
                size: new(100, 100),
                style: TmpStyle,
                margin: new(0),
                padding: new(4)
            ),

            new(
                "BC",
                anchor: Anchor.BottomCenter,
                flow: Flow.Horizontal,
                size: new(800, 200),
                children: [
                    new(
                        "BR1",
                        anchor: Anchor.BottomCenter,
                        size: new(100, 200),
                        style: TmpStyle,
                        margin: new(0),
                        padding: new(4),
                        text: "BR1"
                    ),
                    new(
                        "BR2",
                        anchor: Anchor.BottomCenter,
                        size: new(100, 200),
                        style: TmpStyle,
                        margin: new(0),
                        padding: new(4),
                        text: "BR2"
                    ),
                    new(
                        "BR3",
                        anchor: Anchor.BottomCenter,
                        size: new(100, 0),
                        fit: true,
                        style: TmpStyle,
                        margin: new(0),
                        padding: new(4),
                        text: "BR3"
                    ),
                    new(
                        "BR4",
                        anchor: Anchor.BottomCenter,
                        size: new(0, 200),
                        stretch: true,
                        style: TmpStyle,
                        margin: new(0),
                        padding: new(4),
                        children: [
                            new(
                                "BTL1",
                                anchor: Anchor.MiddleLeft,
                                size: new(100, 200),
                                style: TmpStyle,
                                margin: new(0),
                                padding: new(4),
                                text: "BTL1"
                            ),
                            new(
                                "BTL2",
                                anchor: Anchor.MiddleCenter,
                                size: new(300, 150),
                                style: TmpStyle,
                                margin: new(0),
                                padding: new(4),
                                text: "BTL2"
                            ),
                            new(
                                "BTL3",
                                anchor: Anchor.MiddleRight,
                                size: new(100, 100),
                                style: TmpStyle,
                                margin: new(0),
                                padding: new(4),
                                text: "BTL3"
                            ),
                        ]
                    ),
                ]
            ),
        ]
    );

    public UmbraDebug(IDataManager dm)
    {
        // Element.EnableDebug();
    }

    [OnDraw]
    public void OnDraw()
    {
        var s = ImGui.GetIO().DisplaySize;
        var x = s.X / 2;
        var y = s.Y / 2;

        // _el.Render(ImGui.GetBackgroundDrawList(), new(s.X - 64, s.Y - 64));
    }
}
