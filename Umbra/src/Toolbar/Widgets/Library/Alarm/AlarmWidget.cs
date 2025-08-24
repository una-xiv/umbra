using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace Umbra.Widgets.Library.Alarm;

[ToolbarWidget(
    "Alarm",
    "Widget.Alarm.Name",
    "Widget.Alarm.Description",
    ["alarm", "reminder", "timer", "clock"]
)]
public sealed class AlarmWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    protected override string          DefaultIconType        => IconTypeBitmapIcon;
    protected override BitmapFontIcon  DefaultBitmapIcon      => BitmapFontIcon.Alarm;
    protected override FontAwesomeIcon DefaultFontAwesomeIcon => FontAwesomeIcon.Bell;
    protected override SeIconChar      DefaultGlyphIcon       => SeIconChar.Clock;
    protected override uint            DefaultGameIconId      => 66317u;

    protected override void OnLoad()
    {
        Node.OnClick += _ => Framework.Service<IChatSender>().Send("/alarm");
    }
}
