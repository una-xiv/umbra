using System.Collections.Generic;
using Umbra.Common;
using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace Umbra.Widgets.Library.FPS;

[ToolbarWidget("FpsCounter", "Widget.Fps.Name", "Widget.Fps.Description")]
[ToolbarWidgetTags(["fps", "counter", "performance"])]
internal sealed class FpsCounterWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override WidgetPopup? Popup => null;


    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
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
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
        ];
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
    }

    protected override unsafe void OnUpdate()
    {
        int fps = (int)Framework.Instance()->FrameRate;

        Node.Style.IsVisible = fps < GetConfigValue<int>("HideThreshold");

        string label = $"{fps} {GetConfigValue<string>("Label")}";

        SetLabel(label);
        base.OnUpdate();

        Node.Tooltip = label;
    }
}
