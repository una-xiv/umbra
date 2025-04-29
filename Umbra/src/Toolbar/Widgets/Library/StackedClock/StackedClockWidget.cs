using Dalamud.Game.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using Umbra.Common;

namespace Umbra.Widgets.Library.StackedClock;

[ToolbarWidget("StackedClock", "Widget.StackedClock.Name", "Widget.StackedClock.Description", ["clock", "time"])]
internal sealed partial class StackedClockWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText;

    protected override bool DefaultDecorate                    => false;
    protected override int  DefaultMultiLabelTextSizeTop       => 11;
    protected override int  DefaultMultiLabelTextSizeBottom    => 11;
    protected override int  DefaultMultiLabelTextYOffsetTop    => 3;
    protected override int  DefaultMultiLabelTextYOffsetBottom => 0;

    public override MenuPopup Popup { get; } = new();

    private readonly Dictionary<string, MenuPopup.Button> _buttons = new() {
        { "LT", new("LT") { Label = "00:00" } },
        { "ST", new("ST") { Label = "00:00" } },
        { "ET", new("ET") { Label = "00:00" } }
    };

    protected override void OnLoad()
    {
        SingleLabelTextNode.Style.Font      = 1; // Monospace.
        MultiLabelTextTopNode.Style.Font    = 1;
        MultiLabelTextBottomNode.Style.Font = 1;

        SetText($"{SeIconChar.ServerTimeEn.ToIconChar()} 00:00");
        SetSubText($"{SeIconChar.EorzeaTimeEn.ToIconChar()} 00:00");

        foreach (var btn in _buttons.Values) Popup.Add(btn);
    }

    protected override void OnDraw()
    {
        Popup.IsDisabled = !GetConfigValue<bool>("EnablePopup");

        var src1 = GetConfigValue<string>("TimeSourceTop");
        var src2 = GetConfigValue<string>("TimeSourceBottom");
        var fmt1 = GetConfigValue<string>("TimeFormatTop");
        var fmt2 = GetConfigValue<string>("TimeFormatBottom");
        var fmt3 = GetConfigValue<string>("TimeFormatPopup");

        SetText(FormatTime(src1, fmt1, true));
        SetSubText(src2 != "None" ? FormatTime(src2, fmt2, true) : null);

        _buttons["LT"].Icon  = GetPrefixIcon("LT");
        _buttons["LT"].Label = FormatTime("LT", fmt3, false);
        _buttons["ST"].Icon  = GetPrefixIcon("ST");
        _buttons["ST"].Label = FormatTime("ST", fmt3, false);
        _buttons["ET"].Icon  = GetPrefixIcon("ET");
        _buttons["ET"].Label = FormatTime("ET", fmt3, false);
    }

    private string FormatTime(string timeSource, string format, bool showPrefix)
    {
        string prefixPos = GetConfigValue<string>("PrefixPosition");
        string prefixStr = showPrefix ? $"{GetPrefixIcon(timeSource).ToIconChar()} " : string.Empty;

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
