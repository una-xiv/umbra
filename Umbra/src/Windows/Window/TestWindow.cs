using System.Numerics;
using Umbra.Windows.Clipping;
using Una.Drawing;

namespace Umbra.Windows;

public class TestWindow: Window
{
    protected override Vector2 MinSize     { get; } = new(200, 300);
    protected override Vector2 MaxSize     { get; } = new(800, 600);
    protected override Vector2 DefaultSize { get; } = new(400, 300);

    protected override Node Node { get; } = new() {
        NodeValue = "Test Window!",
        Style = new() {
            Padding  = new(20),
            Color    = new(0xFFFFFFFF),
            FontSize = 18,
        }
    };

    protected override void OnOpen()
    {
    }

    protected override void OnUpdate(int instanceId)
    {
    }

    protected override void OnClose()
    {
    }
}
