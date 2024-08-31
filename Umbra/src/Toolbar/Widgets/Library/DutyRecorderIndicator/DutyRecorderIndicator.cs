using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Collections.Generic;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Widgets.Library.DutyRecorderIndicator;

[ToolbarWidget(
    "DutyRecorderIndicator",
    "Widget.DutyRecorderIndicator.Name",
    "Widget.DutyRecorderIndicator.Description"
)]
internal sealed partial class DutyRecorderIndicator(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : IconToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup => null;

    protected override void Initialize()
    {
        Node.Tooltip = I18N.Translate("Widget.DutyRecorderIndicator.Tooltip");
        SetIcon(FontAwesomeIcon.Video);
    }

    protected override void OnUpdate()
    {
        Node.Style.IsVisible = IsRecordingDuty;
        base.OnUpdate();
    }

    private static unsafe bool IsRecordingDuty => AgentHUD.Instance()->IsMainCommandEnabled(79);
}
