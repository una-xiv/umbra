namespace Umbra.Widgets;

[ToolbarWidget(
    "Compass",
    "Widget.Compass.Name",
    "Widget.Compass.Description",
    ["compass", "direction", "indicator"]
)]
internal sealed partial class CompassWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.None;

    private CompassNode CompassNode { get; } = new() {
        ClassList = ["CompassWidget"],
        Style = new() {
            Size   = new(240, 24),
            Anchor = Anchor.MiddleLeft,
        }
    };

    protected override void OnLoad()
    {
        BodyNode.AppendChild(CompassNode);
        BodyNode.BeforeDraw += node => {
            node.Style.Size        = new(GetConfigValue<int>("Width"), node.Style.Size?.Height ?? SafeHeight);
            CompassNode.Style.Size = new(GetConfigValue<int>("Width") - 6, 24);
        };
    }

    protected override void OnDraw()
    {
        CompassNode.UseCameraAngle    = GetConfigValue<bool>("UseCameraAngle");
        CompassNode.FishEyePower      = GetConfigValue<float>("FishEyePower");
        CompassNode.AngleRangeDegrees = GetConfigValue<int>("AngleRangeDegrees");
        CompassNode.CenterLineColor   = GetConfigValue<uint>("CenterLineColor");
        CompassNode.LineColor         = GetConfigValue<uint>("LineColor");
        CompassNode.TextColor         = GetConfigValue<uint>("TextColor");
    }
}
