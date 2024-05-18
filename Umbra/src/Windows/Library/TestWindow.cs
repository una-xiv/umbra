using System.Numerics;
using Dalamud.Interface;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows;

public class TestWindow : Window
{
    public int Result { get; private set; } = 0;

    protected override string Title => _windowTitle;

    protected override Vector2 MinSize     { get; } = new(200, 300);
    protected override Vector2 MaxSize     { get; } = new(800, 600);
    protected override Vector2 DefaultSize { get; } = new(400, 300);

    private string _windowTitle = "Yes!";

    protected override Node Node { get; } = new() {
        Style = new() {
            Anchor             = Anchor.TopLeft,
            Flow               = Flow.Vertical,
            Padding            = new(20),
            Color              = new(0xFFFFFFFF),
            FontSize           = 18,
            BackgroundColor    = new(0xFF406090),
            BackgroundGradient = GradientColor.Vertical(new(0xFF42302A), new(0xFF72402A)),
            Gap                = 16,
        },
        ChildNodes = [
        ]
    };

    protected override void OnOpen()
    {
        Node.AppendChild(new ButtonNode("Button1", "Just text button"));
        Node.AppendChild(new ButtonNode("Button2", "Text with Icon", FontAwesomeIcon.Ad));
        Node.AppendChild(new ButtonNode("Button3", "Just Disabled Text") { IsDisabled = true });

        Node.AppendChild(
            new ButtonNode("Button4", "Disabled Text with Icon", FontAwesomeIcon.Ad) { IsDisabled = true }
        );

        Node.AppendChild(new ButtonNode("Button5", null, FontAwesomeIcon.Ambulance));

        Node.AppendChild(new CheckboxNode("Checkbox0", false, "This is a checkbox without a description"));
        Node.AppendChild(new CheckboxNode("Checkbox1", true, "This is a checkbox with a very long name", "This is the description of the checkbox. This is the description of the checkbox. This is the description of the checkbox. This is the description of the checkbox."));

        Node.QuerySelector("Button1")!.OnClick += _ => Result = 1;
        Node.QuerySelector("Button2")!.OnClick += _ => Result = 2;
        Node.QuerySelector("Button3")!.OnClick += _ => Result = 3;
        Node.QuerySelector("Button4")!.OnClick += _ => Result = 4;
        Node.QuerySelector("Button5")!.OnClick += _ => Result = 5;

        Node.QuerySelector<CheckboxNode>("Checkbox0")!.OnValueChanged += value => {
            _windowTitle = value ? "The checkbox is checked! This is now a very long title." : "Unchecked window";
        };
    }

    protected override void OnUpdate(int instanceId)
    {
        Node.Style.Size = ContentSize;
    }

    protected override void OnClose() { }
}
