using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace Umbra.Widgets;

[ToolbarWidget(
    "FpsCounter",
    "Widget.Fps.Name",
    "Widget.Fps.Description",
    ["fps", "counter", "performance"]
)]
internal sealed class FpsCounterWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.Text;

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),

            new IntegerWidgetConfigVariable(
                "HideThreshold",
                I18N.Translate("Widget.Fps.Config.HideThreshold.Name"),
                I18N.Translate("Widget.Fps.Config.HideThreshold.Description"),
                1000,
                0,
                1000
            ),
            new StringWidgetConfigVariable(
                "Label",
                I18N.Translate("Widget.Fps.Config.Label.Name"),
                I18N.Translate("Widget.Fps.Config.Label.Description"),
                "FPS",
                32
            ),
        ];
    }

    protected override unsafe void OnDraw()
    {
        int    fps   = (int)Framework.Instance()->FrameRate;
        string label = $"{fps} {GetConfigValue<string>("Label")}";

        IsVisible    = fps < GetConfigValue<int>("HideThreshold");
        Node.Tooltip = label;

        SetText(label);
    }
}
