using FFXIVClientStructs.FFXIV.Client.System.Framework;
using System.Collections.Generic;

namespace Umbra.Widgets.Library.FPS;

[ToolbarWidget("FpsCounter", "Widget.Fps.Name", "Widget.Fps.Description")]
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

        SetLabel($"{fps} FPS");
        base.OnUpdate();

        Node.Tooltip = $"{fps} FPS";
    }
}
