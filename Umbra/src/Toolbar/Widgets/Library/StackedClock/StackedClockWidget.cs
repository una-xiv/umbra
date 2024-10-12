using Dalamud.Game.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using Umbra.Common;

namespace Umbra.Widgets.Library.StackedClock;

[ToolbarWidget("StackedClock", "Widget.StackedClock.Name", "Widget.StackedClock.Description")]
[ToolbarWidgetTags(["clock", "time"])]
internal sealed partial class StackedClockWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override MenuPopup Popup { get; } = new();

    protected override void Initialize()
    {
        LabelNode.Style.Font       = 1;
        TopLabelNode.Style.Font    = 1;
        BottomLabelNode.Style.Font = 1;

        SetTwoLabels("00:00", "00:00");
        TopLabelNode.NodeValue    = $"{SeIconChar.ServerTimeEn.ToIconChar()} 00:00";
        BottomLabelNode.NodeValue = $"{SeIconChar.EorzeaTimeEn.ToIconChar()} 00:00";

        Popup.AddButton("LT", "Local Time",  0, null, "00:00");
        Popup.AddButton("ST", "Server Time", 0, null, "00:00");
        Popup.AddButton("ET", "Eorzea Time", 0, null, "00:00");

        BottomLabelNode.Style.Color = new("Widget.Text");
    }

    protected override void OnUpdate()
    {
        Popup.IsDisabled = !GetConfigValue<bool>("EnablePopup");

        var src1 = GetConfigValue<string>("TimeSourceTop");
        var src2 = GetConfigValue<string>("TimeSourceBottom");
        var fmt1 = GetConfigValue<string>("TimeFormatTop");
        var fmt2 = GetConfigValue<string>("TimeFormatBottom");
        var fmt3 = GetConfigValue<string>("TimeFormatPopup");

        if (src2 == "None") {
            SetLabel(FormatTime(src1, fmt1, true));
        } else {
            SetTwoLabels(
                FormatTime(src1, fmt1, true),
                FormatTime(src2, fmt2, true)
            );
        }

        Popup.SetButtonAltLabel("LT", FormatTime("LT", fmt3, false));
        Popup.SetButtonAltLabel("ST", FormatTime("ST", fmt3, false));
        Popup.SetButtonAltLabel("ET", FormatTime("ET", fmt3, false));

        base.OnUpdate();
    }

    private string FormatTime(string timeSource, string format, bool showPrefix)
    {
        string prefixPos = GetConfigValue<string>("PrefixPosition");

        string prefixStr = GetConfigValue<string>("DisplayMode") != "TextOnly" && showPrefix
            ? $"{GetPrefixIcon(timeSource).ToIconChar()} "
            : string.Empty;

        string l = prefixPos == "Left" ? prefixStr : string.Empty;
        string r = prefixPos == "Right" ? prefixStr : string.Empty;

        try {
            return $"{l}{GetTime(timeSource).ToString(format, CultureInfo.InvariantCulture)} {r}".TrimEnd();
        } catch (FormatException) {
            return $"Invalid format: {format}";
        }
    }

    private SeIconChar GetPrefixIcon(string timeSource)
    {
        return I18N.GetCurrentLanguage() switch {
            "de" => timeSource switch {
                "ET" => SeIconChar.EorzeaTimeDe,
                "LT" => SeIconChar.LocalTimeDe,
                "ST" => SeIconChar.ServerTimeDe,
                _    => throw new ArgumentOutOfRangeException()
            },
            "fr" => timeSource switch {
                "ET" => SeIconChar.EorzeaTimeFr,
                "LT" => SeIconChar.LocalTimeFr,
                "ST" => SeIconChar.ServerTimeFr,
                _    => throw new ArgumentOutOfRangeException()
            },
            "jp" => timeSource switch {
                "ET" => SeIconChar.EorzeaTimeJa,
                "LT" => SeIconChar.LocalTimeJa,
                "ST" => SeIconChar.ServerTimeJa,
                _    => throw new ArgumentOutOfRangeException()
            },
            _ => timeSource switch {
                "ET" => SeIconChar.EorzeaTimeEn,
                "LT" => SeIconChar.LocalTimeEn,
                "ST" => SeIconChar.ServerTimeEn,
                _    => throw new ArgumentOutOfRangeException()
            }
        };
    }
}
