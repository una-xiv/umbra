using System.Collections.Generic;
using Una.Drawing;

namespace Umbra.Widgets.Library.Compass;

[ToolbarWidget("Compass", "Widget.Compass.Name", "Widget.Compass.Description")]
internal sealed partial class CompassWidget(
    WidgetInfo info,
    string? guid = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    protected override void Initialize()
    {
        Node.AppendChild(CompassNode);
        Node.BeforeDraw += node => {
            node.Style.Size = new(GetConfigValue<int>("Width"), node.Style.Size?.Height ?? SafeHeight);
            CompassNode.Style.Size = new(GetConfigValue<int>("Width") - 16, 24);
        };
    }

    protected override void OnUpdate()
    {
        CompassNode.UseCameraAngle = GetConfigValue<bool>("UseCameraAngle");
        CompassNode.FishEyePower = GetConfigValue<float>("FishEyePower");
        CompassNode.AngleRangeDegrees = GetConfigValue<int>("AngleRangeDegrees");
        CompassNode.CenterLineColor = GetConfigValue<uint>("CenterLineColor");
        CompassNode.LineColor = GetConfigValue<uint>("LineColor");
        CompassNode.TextColor = GetConfigValue<uint>("TextColor");

        SetGhost(!GetConfigValue<bool>("Decorate"));
    }
}
