using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace Umbra.Interface;

public class TestWindow : Window
{
    protected override string Id => "TestWindow2";

    private readonly Element       _element;

    public TestWindow()
    {
        _element = new(
            id: "TestThing",
            flow: Flow.Vertical,
            size: new(500, 300),
            anchor: Anchor.TopLeft,
            children: [
                new OverflowContainer(
                    id: "Container",
                    anchor: Anchor.None,
                    children: [
                        new(id: "Content", flow: Flow.Vertical)
                    ]
                )
            ]
        );

        for (var i = 0; i < 50; i++) {
            _element.Get("Container.Content").AddChild(new (
                id: $"TestElement_{i}",
                anchor: Anchor.TopLeft,
                flow: Flow.Horizontal,
                size: new (200, 20),
                padding: new(4),
                text: $"Test Element {i}",
                sortIndex: i
            ));
        }
    }

    protected override void OnDraw(int instanceId)
    {
        ImGui.Text("Hello, world!");

        // ImGui.SetCursorPos(new(32, 32));
        // ImGui.BeginChildFrame((uint)instanceId + 1, new (200, 200), ImGuiWindowFlags.None);

        _element.Render(ImGui.GetWindowDrawList(), ImGui.GetCursorScreenPos());
        // ImGui.SetCursorPos(new(0, _element.BoundingBox.Height));

        // ImGui.EndChildFrame();
    }
}
