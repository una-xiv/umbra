using System.Runtime.InteropServices;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using ImGuiNET;
using SixLabors.ImageSharp.Processing;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra;

[Service]
public sealed class UmbraDebug
{
    // private static readonly Style TmpStyle = new() {
    //     BackgroundBorderColor = 0x80FFAA77,
    //     BackgroundRounding    = 4,
    //     BackgroundBorderWidth = 2,
    //     Gradient              = new(0),
    //     Font                  = Font.MiedingerLarge,
    //     TextColor             = 0xFF00AA00,
    //     TextAlign             = Anchor.MiddleCenter,
    //     OutlineWidth          = 1,
    //     OutlineColor          = 0xFF000000
    // };
    //
    // private readonly Element _el = new(
    //     id: "RootTest",
    //     flow: Flow.Horizontal,
    //     anchor: Anchor.BottomRight,
    //     size: new(800, 1000),
    //     padding: new(0),
    //     text: "Hello World!",
    //     style: new() {
    //         TextColor             = 0xFF00FFFF,
    //         BackgroundColor       = 0xFF212021,
    //         Gradient              = new(0x50AACCFF, 0x30FFCCAA),
    //         BackgroundRounding    = 8,
    //         BackgroundBorderColor = 0xFF0000FF,
    //         BackgroundBorderWidth = 1,
    //         RoundedCorners        = RoundedCorners.All,
    //         TextAlign             = Anchor.BottomCenter,
    //         Font                  = Font.AxisLarge,
    //         OutlineColor          = 0xFF000000,
    //         OutlineWidth          = 2,
    //     },
    //     children: [
    //         new("BG1", anchor: Anchor.None, padding: new(16), style: new() { BackgroundColor = 0x200000FF }),
    //         new(
    //             "TL",
    //             anchor: Anchor.TopLeft,
    //             flow: Flow.Horizontal,
    //             children: [
    //                 new(
    //                     "TL1",
    //                     anchor: Anchor.TopLeft,
    //                     size: new(0, 100),
    //                     style: TmpStyle,
    //                     margin: new(0),
    //                     padding: new(8, 4),
    //                     gap: 4,
    //                     children: [
    //                         new(
    //                             "TL1-1",
    //                             padding: new(16),
    //                             gap: 6,
    //                             children: [
    //                                 new ButtonElement("MyButton1", "Click me quickly!", FontAwesomeIcon.Home),
    //                                 new ButtonElement("MyButton2", "Awesome button",    14u),
    //                                 new ButtonElement("MyButton3", "Awesome button",    SeIconChar.Clock),
    //                                 new ButtonElement("MyButton4", "Text Only!"),
    //                                 new ButtonElement("MyButton5", null, 66001u),
    //                             ]
    //                         ),
    //                     ]
    //                 ),
    //                 new(
    //                     "TL2",
    //                     anchor: Anchor.TopLeft,
    //                     size: new(100, 100),
    //                     style: TmpStyle,
    //                     margin: new(0),
    //                     padding: new(4),
    //                     isVisible: false
    //                 ),
    //                 new(
    //                     "TL3",
    //                     anchor: Anchor.TopLeft,
    //                     size: new(100, 100),
    //                     style: new() {
    //                         Opacity       = 0.25f, Image          = 14u, ImageRounding = 16, ImageBlackAndWhite = true,
    //                         ImageContrast = 0.5f, ImageBrightness = 1.5f
    //                     },
    //                     margin: new(0),
    //                     padding: new(4),
    //                     text: "I has icon"
    //                 ),
    //                 new(
    //                     "TL4",
    //                     anchor: Anchor.TopLeft,
    //                     size: new(100, 100),
    //                     style: new() {
    //                         Image = "images\\icon.png", Shadow = new(size: 64, inset: new(6)), ImageRounding = 64,
    //                         ImageBlackAndWhite = true, ImageContrast = 0.5f, ImageBrightness = 1.5f
    //                     },
    //                     margin: new(0),
    //                     padding: new(4)
    //                 ),
    //             ]
    //         ),
    //         new("TR1", anchor: Anchor.TopRight, size: new(100, 100), style: TmpStyle, margin: new(0), padding: new(4)),
    //         new(
    //             "ML1",
    //             anchor: Anchor.MiddleLeft,
    //             size: new(100, 100),
    //             style: TmpStyle,
    //             margin: new(0),
    //             padding: new(4)
    //         ),
    //         new(
    //             "ML2",
    //             anchor: Anchor.MiddleLeft,
    //             size: new(100, 100),
    //             style: TmpStyle,
    //             margin: new(0),
    //             padding: new(4)
    //         ),
    //         new(
    //             "MC1",
    //             anchor: Anchor.MiddleCenter,
    //             size: new(100, 100),
    //             style: TmpStyle,
    //             margin: new(0),
    //             padding: new(4)
    //         ),
    //         new(
    //             "MC2",
    //             anchor: Anchor.MiddleCenter,
    //             size: new(100, 100),
    //             style: TmpStyle,
    //             margin: new(0),
    //             padding: new(4),
    //             children: [
    //                 new("MC2-1", anchor: Anchor.MiddleLeft, size: new(32, 92), style: TmpStyle),
    //                 new("MC2-2", anchor: Anchor.MiddleLeft, size: new(32, 32), style: TmpStyle),
    //             ]
    //         ),
    //         new(
    //             "MR1",
    //             anchor: Anchor.MiddleRight,
    //             size: new(100, 100),
    //             style: TmpStyle,
    //             margin: new(0),
    //             padding: new(4)
    //         ),
    //         new(
    //             "MR2",
    //             anchor: Anchor.MiddleRight,
    //             size: new(100, 100),
    //             style: new() {
    //                 BorderWidth = new(1, 0, 4, 0),
    //                 BorderColor = new(0xFF00FF00, 0, 0xFFFF0000, 0)
    //             },
    //             margin: new(0),
    //             padding: new(4)
    //         ),
    //
    //         new(
    //             "BC",
    //             anchor: Anchor.BottomCenter,
    //             flow: Flow.Horizontal,
    //             size: new(800, 200),
    //             children: [
    //                 new(
    //                     "BR1",
    //                     anchor: Anchor.BottomCenter,
    //                     size: new(100, 200),
    //                     style: TmpStyle,
    //                     margin: new(0),
    //                     padding: new(4),
    //                     text: "BR1"
    //                 ),
    //                 new(
    //                     "BR2",
    //                     anchor: Anchor.BottomCenter,
    //                     size: new(100, 200),
    //                     style: TmpStyle,
    //                     margin: new(0),
    //                     padding: new(4),
    //                     text: "BR2"
    //                 ),
    //                 new(
    //                     "BR3",
    //                     anchor: Anchor.BottomCenter,
    //                     size: new(100),
    //                     fit: true,
    //                     style: TmpStyle,
    //                     margin: new(0),
    //                     padding: new(4),
    //                     text: "BR3"
    //                 ),
    //                 new(
    //                     "BR4",
    //                     anchor: Anchor.BottomCenter,
    //                     size: new(0, 200),
    //                     stretch: true,
    //                     style: TmpStyle,
    //                     margin: new(0),
    //                     padding: new(4),
    //                     children: [
    //                         new(
    //                             "BTL1",
    //                             anchor: Anchor.MiddleLeft,
    //                             size: new(150, 200),
    //                             style: TmpStyle,
    //                             margin: new(0),
    //                             padding: new(4),
    //                             text: "BTL1",
    //                             isVisible: false
    //                         ),
    //                         new(
    //                             "BTL2",
    //                             anchor: Anchor.MiddleLeft,
    //                             size: new(150, 150),
    //                             style: TmpStyle,
    //                             margin: new(0),
    //                             padding: new(4),
    //                             text: "BTL2",
    //                             isVisible: true
    //                         ),
    //                         new(
    //                             "BTL3",
    //                             anchor: Anchor.MiddleLeft,
    //                             size: new(150, 100),
    //                             style: TmpStyle,
    //                             margin: new(0),
    //                             padding: new(4),
    //                             text: "BTL3"
    //                         ),
    //                     ]
    //                 ),
    //             ]
    //         ),
    //     ]
    // );
    //
    // private DropdownElement _el2 = new(
    //     id: "RootTest2",
    //     anchor: Anchor.BottomLeft,
    //     flow: Flow.Vertical,
    //     children: [
    //         new(
    //             "Items",
    //             Flow.Vertical,
    //             padding: new(8, 4),
    //             gap: 6,
    //             children: [
    //                 new DropdownButtonElement("Test1", "A dropdown button",       14u,                     "CTRL+S"),
    //                 new DropdownButtonElement("Test2", "Another dropdown button", SeIconChar.BoxedLetterD, null, 0xFF4499FF),
    //                 new DropdownSeparatorElement(),
    //                 new DropdownButtonElement("Test3", "A third dropdown button", FontAwesomeIcon.Home, "CTRL+H"),
    //                 new DropdownButtonElement("Test4", "Another test"),
    //                 new DropdownButtonElement("Test5", "Another test 2", null, "CTRL-SHIFT-T"),
    //             ]
    //         ),
    //     ]
    // );
    //
    // public unsafe UmbraDebug()
    // {
    //     // Element.EnableDebug();
    //     uint id = 14;
    //
    //     _el.Get("TL.TL3").OnClick += () => {
    //         id++;
    //         _el.Get("TL.TL3").Style.Image = id;
    //     };
    //
    //     var btn  = _el.Get<ButtonElement>("TL.TL1.TL1-1.MyButton1");
    //     var btn2 = _el.Get<ButtonElement>("TL.TL1.TL1-1.MyButton2");
    //
    //     btn.OnClick += () => {
    //         Logger.Info("Clicked!");
    //         btn2.IsDisabled = !btn2.IsDisabled;
    //         var id = (uint?)btn2.Icon;
    //
    //         if (id.HasValue) {
    //             btn2.Icon = id + 1u;
    //         }
    //     };
    //
    //     var ipl        = (InfoProxyLetterButCooler*)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.Letter);
    //     var numLetters = ipl->NumLetters;
    //
    //     Logger.Info($"You have {numLetters} letters!");
    // }
    //
    // [OnDraw]
    // public void OnDraw()
    // {
    //     var s = ImGui.GetIO().DisplaySize;
    //
    //     _el.Render(ImGui.GetBackgroundDrawList(), new(s.X      - 64, s.Y - 64));
    //     _el2.Render(ImGui.GetForegroundDrawList(), new(64, s.Y - 32));
    // }
    //
    // [StructLayout(LayoutKind.Explicit)]
    // public struct InfoProxyLetterButCooler
    // {
    //     [FieldOffset(0x26)] public byte NumLetters;
    // }
}
