/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using Dalamud.Game.Text;
using Umbra.Common;
using Umbra.Interface;
using Umbra.Toolbar.Widgets.Time;

namespace Umbra.Toolbar.Widgets.Clock2;

[Service]
internal partial class StackedClockWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.StackedClock.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.EorzeaTime.Use24HourFormat", "ToolbarSettings", "ClockSettings")]
    private static bool EtUse24HourFormatOption { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.EorzeaTime.ShowSeconds", "ToolbarSettings", "ClockSettings")]
    private static bool EtShowSecondsOption { get; set; } = false;

    [ConfigVariable("Toolbar.Widget.EorzeaTime.AmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string EtAmSuffixOption { get; set; } = "am";

    [ConfigVariable("Toolbar.Widget.EorzeaTime.PmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string EtPmSuffixOption { get; set; } = "pm";

    [ConfigVariable("Toolbar.Widget.ServerTime.Use24HourFormat", "ToolbarSettings", "ClockSettings")]
    private static bool StUse24HourFormatOption { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.ServerTime.ShowSeconds", "ToolbarSettings", "ClockSettings")]
    private static bool StShowSecondsOption { get; set; } = false;

    [ConfigVariable("Toolbar.Widget.ServerTime.AmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string StAmSuffixOption { get; set; } = "am";

    [ConfigVariable("Toolbar.Widget.ServerTime.PmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string StPmSuffixOption { get; set; } = "pm";

    [ConfigVariable("Toolbar.Widget.LocalTime.Use24HourFormat", "ToolbarSettings", "ClockSettings")]
    private static bool LtUse24HourFormatOption { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.LocalTime.ShowSeconds", "ToolbarSettings", "ClockSettings")]
    private static bool LtShowSecondsOption { get; set; } = false;

    [ConfigVariable("Toolbar.Widget.LocalTime.AmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string LtAmSuffixOption { get; set; } = "am";

    [ConfigVariable("Toolbar.Widget.LocalTime.PmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string LtPmSuffixOption { get; set; } = "pm";

    private Element EtPrefix { get; }
    private Element EtTime   { get; }
    private Element EtSuffix { get; }
    private Element LtPrefix { get; }
    private Element LtTime   { get; }
    private Element LtSuffix { get; }
    private Element StPrefix { get; }
    private Element StTime   { get; }
    private Element StSuffix { get; }

    private bool _isShowingServerTime;

    public StackedClockWidget()
    {
        EtPrefix = Element.Get("ET.Container.Prefix");
        EtTime   = Element.Get("ET.Container.Time");
        EtSuffix = Element.Get("ET.Container.Suffix");
        LtPrefix = Element.Get("LT.Container.Prefix");
        LtTime   = Element.Get("LT.Container.Time");
        LtSuffix = Element.Get("LT.Container.Suffix");
        StPrefix = Element.Get("ST.Container.Prefix");
        StTime   = Element.Get("ST.Container.Time");
        StSuffix = Element.Get("ST.Container.Suffix");

        Element.Get("LT").OnClick += () => _isShowingServerTime = true;
        Element.Get("ST").OnClick += () => _isShowingServerTime = false;
    }

    public void OnUpdate()
    {
        if (!Enabled) return;

        UpdateEorzeaTime();
        UpdateServerTime();
        UpdateLocalTime();
    }

    public void OnDraw()
    {
        Element.IsVisible = Enabled;
        if (!Enabled) return;

        Element.Get("LT").IsVisible = !_isShowingServerTime;
        Element.Get("ST").IsVisible = _isShowingServerTime;
    }

    private void UpdateEorzeaTime()
    {
        DateTime time = GetEorzeaTime();

        string timeFormat                   = EtUse24HourFormatOption ? "HH:mm" : "hh:mm";
        if (EtShowSecondsOption) timeFormat += ":ss";

        EtPrefix.Text = GetPrefixIcon(TimeType.EorzeaTime);
        EtTime.Text   = time.ToString(timeFormat);

        EtSuffix.Text = EtUse24HourFormatOption
            ? ""
            : time.Hour < 12
                ? EtAmSuffixOption
                : EtPmSuffixOption;
    }

    private void UpdateServerTime()
    {
        DateTime time = GetServerTime();

        string timeFormat                   = StUse24HourFormatOption ? "HH:mm" : "hh:mm";
        if (StShowSecondsOption) timeFormat += ":ss";

        StPrefix.Text = GetPrefixIcon(TimeType.ServerTime);
        StTime.Text   = time.ToString(timeFormat);

        StSuffix.Text = StUse24HourFormatOption
            ? ""
            : time.Hour < 12
                ? StAmSuffixOption
                : StPmSuffixOption;
    }

    private void UpdateLocalTime()
    {
        DateTime time = GetLocalTime();

        string timeFormat                   = LtUse24HourFormatOption ? "HH:mm" : "hh:mm";
        if (LtShowSecondsOption) timeFormat += ":ss";

        LtPrefix.Text = GetPrefixIcon(TimeType.LocalTime);
        LtTime.Text   = time.ToString(timeFormat);

        LtSuffix.Text = LtUse24HourFormatOption
            ? ""
            : time.Hour < 12
                ? LtAmSuffixOption
                : LtPmSuffixOption;
    }

    private DateTime GetServerTime()
    {
        long serverTime = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.GetServerTime();
        long hours      = serverTime / 3600 % 24;
        long minutes    = serverTime / 60   % 60;
        long seconds    = serverTime        % 60;

        return new(1, 1, 1, (int)hours, (int)minutes, (int)seconds);
    }

    private DateTime GetLocalTime()
    {
        return DateTime.Now;
    }

    private unsafe DateTime GetEorzeaTime()
    {
        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();

        long eorzeaTime = fw->ClientTime.EorzeaTime;
        long hours      = eorzeaTime / 3600 % 24;
        long minutes    = eorzeaTime / 60   % 60;
        long seconds    = eorzeaTime        % 60;

        return new(1, 1, 1, (int)hours, (int)minutes, (int)seconds);
    }

    private static string GetPrefixIcon(TimeType type)
    {
        return I18N.GetCurrentLanguage() switch {
            "de" => type switch {
                TimeType.EorzeaTime => SeIconChar.EorzeaTimeDe.ToIconString(),
                TimeType.LocalTime  => SeIconChar.LocalTimeDe.ToIconString(),
                TimeType.ServerTime => SeIconChar.ServerTimeDe.ToIconString(),
                _                   => throw new ArgumentOutOfRangeException()
            },
            "fr" => type switch {
                TimeType.EorzeaTime => SeIconChar.EorzeaTimeFr.ToIconString(),
                TimeType.LocalTime  => SeIconChar.LocalTimeFr.ToIconString(),
                TimeType.ServerTime => SeIconChar.ServerTimeFr.ToIconString(),
                _                   => throw new ArgumentOutOfRangeException()
            },
            "jp" => type switch {
                TimeType.EorzeaTime => SeIconChar.EorzeaTimeJa.ToIconString(),
                TimeType.LocalTime  => SeIconChar.LocalTimeJa.ToIconString(),
                TimeType.ServerTime => SeIconChar.ServerTimeJa.ToIconString(),
                _                   => throw new ArgumentOutOfRangeException()
            },
            _ => type switch {
                TimeType.EorzeaTime => SeIconChar.EorzeaTimeEn.ToIconString(),
                TimeType.LocalTime  => SeIconChar.LocalTimeEn.ToIconString(),
                TimeType.ServerTime => SeIconChar.ServerTimeEn.ToIconString(),
                _                   => throw new ArgumentOutOfRangeException()
            }
        };
    }
}
