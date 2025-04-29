using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.DutyRecorderIndicator;

[ToolbarWidget(
    "DutyRecorderIndicator",
    "Widget.DutyRecorderIndicator.Name",
    "Widget.DutyRecorderIndicator.Description",
    ["duty", "recorder", "indicator"]
)]
internal sealed class DutyRecorderIndicator(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    protected override string          DefaultIconType        => IconTypeFontAwesome;
    protected override FontAwesomeIcon DefaultFontAwesomeIcon => FontAwesomeIcon.Video;

    protected override void OnLoad()
    {
        Node.Tooltip = I18N.Translate("Widget.DutyRecorderIndicator.Tooltip");
    }

    protected override unsafe void OnDraw()
    {
        IsVisible = AgentHUD.Instance()->IsMainCommandEnabled(79);
    }
}
