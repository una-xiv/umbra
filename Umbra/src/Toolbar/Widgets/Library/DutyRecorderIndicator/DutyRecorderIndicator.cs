using Dalamud.Game.Text.SeStringHandling;
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
        IconNode.NodeValue = new SeStringBuilder().AddIcon(BitmapFontIcon.Recording).Build();
        Node.Tooltip       = I18N.Translate("Widget.DutyRecorderIndicator.Tooltip");
    }

    protected override void OnUpdate()
    {
        Node.Style.IsVisible = IsRecordingDuty;

        if (IsRecordingDuty) {
            SetGhost(!GetConfigValue<bool>("Decorate"));
            IconNode.Style.TextAlign = Anchor.TopLeft;
            IconNode.Style.Padding   = new() { Top = GetConfigValue<int>("IconYOffset") + 5, Left = -5 };
        }
    }

    private static unsafe bool IsRecordingDuty => AgentHUD.Instance()->IsMainCommandEnabled(79);
}
