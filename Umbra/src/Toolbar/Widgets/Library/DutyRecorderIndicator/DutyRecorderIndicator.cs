using Dalamud.Game.Text.SeStringHandling;
using System.Collections.Generic;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Widgets.Library.DutyRecorderIndicator;

// FIXME: This widget causes users to crash if they don't have
//        "A Realm Recorded" installed.
[InteropToolbarWidget(
    "DutyRecorderIndicator",
    "Widget.DutyRecorderIndicator.Name",
    "Widget.DutyRecorderIndicator.Description",
    "ARealmRecorded"
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
        SetupHook();

        IconNode.NodeValue = new SeStringBuilder().AddIcon(BitmapFontIcon.Recording).Build();
        Node.Tooltip       = I18N.Translate("Widget.DutyRecorderIndicator.Tooltip");
    }

    protected override void OnDisposed()
    {
        DisposeHook();
        base.OnDisposed();
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
}
