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
            new("BG1", anchor: Anchor.None, padding: new(16), style: new() { BackgroundColor = 0x600000FF }),
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
                        padding: new(4),
                        isVisible: false
                    ),
                    new(
                        "TL3",
                        anchor: Anchor.TopLeft,
                        size: new(100, 100),
                        style: new() { Image = new(14), ImageRounding = 16, ImageBlackAndWhite = true, ImageContrast = 0.5f, ImageBrightness = 1.5f },
                        margin: new(0),
                        padding: new(4),
                        text: "I has icon"
                    ),
                    new(
                        "TL4",
                        anchor: Anchor.TopLeft,
                        size: new(100, 100),
                        style: new() { Image = new("images\\icon.png"), Shadow = new(size: 64, inset: new(6)), ImageRounding = 64, ImageBlackAndWhite = true, ImageContrast = 0.5f, ImageBrightness = 1.5f},
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
                padding: new(4),
                children: [
                    new ("MC2-1", anchor: Anchor.MiddleLeft, size: new(32, 92), style: TmpStyle),
                    new ("MC2-2", anchor: Anchor.MiddleLeft, size: new(32, 32), style: TmpStyle),
                ]
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
                        size: new(100),
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
                                size: new(150, 200),
                                style: TmpStyle,
                                margin: new(0),
                                padding: new(4),
                                text: "BTL1",
                                isVisible: false
                            ),
                            new(
                                "BTL2",
                                anchor: Anchor.MiddleLeft,
                                size: new(150, 150),
                                style: TmpStyle,
                                margin: new(0),
                                padding: new(4),
                                text: "BTL2",
                                isVisible: true
                            ),
                            new(
                                "BTL3",
                                anchor: Anchor.MiddleLeft,
                                size: new(150, 100),
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

    public UmbraDebug()
    {
        // Element.EnableDebug();
        // uint id = 14;
        // _el.Get("TL.TL3").OnClick += () => {
        //     id++;
        //     _el.Get("TL.TL3").Style.Image = new (id);
        // };
    }

    [OnDraw]
    public void OnDraw()
    {
        var s = ImGui.GetIO().DisplaySize;

        // _el.Render(ImGui.GetBackgroundDrawList(), new(s.X - 64, s.Y - 64));
    }
}
