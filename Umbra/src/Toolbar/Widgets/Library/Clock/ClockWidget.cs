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
using System.Collections.Generic;
using Dalamud.Game.Text;
using Dalamud.Utility;
using Umbra.Common;
using Umbra.Widgets.System;
using Una.Drawing;

namespace Umbra.Widgets;

public partial class ClockWidget(WidgetInfo info, string? guid = null, Dictionary<string, object>? configValues = null)
    : ToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup { get; } = null;

    private bool? _isInteractive;

    public override string GetInstanceName()
    {
        return $"{Info.Name} - {I18N.Translate($"Widget.Clock.Config.TimeSource.{GetConfigValue<string>("TimeSource")}")}";
    }

    protected override void Initialize() { }

    protected override void OnUpdate()
    {
        var useCustomPrefix = GetConfigValue<bool>("UseCustomPrefix");
        var isInteractive   = GetConfigValue<bool>("ClickToSwitch");

        bool isPrefixVisible = !useCustomPrefix
            || (useCustomPrefix && !GetConfigValue<string>("PrefixText").Trim().IsNullOrEmpty());

        switch (useCustomPrefix) {
            case true when PrefixNode.TagsList.Contains("native"):
                PrefixNode.TagsList.Remove("native");
                break;
            case false when !PrefixNode.TagsList.Contains("native"):
                PrefixNode.TagsList.Add("native");
                break;
        }

        switch (GetConfigValue<bool>("Decorate")) {
            case true when Node.TagsList.Contains("ghost"):
                Node.TagsList.Remove("ghost");
                break;
            case false when !Node.TagsList.Contains("ghost"):
                Node.TagsList.Add("ghost");
                break;
        }

        if (_isInteractive != isInteractive) {
            _isInteractive = isInteractive;

            if (isInteractive) {
                Node.OnClick += OnClick;
            } else {
                Node.OnClick -= OnClick;
            }
        }

        Node.Tooltip = isInteractive ? I18N.Translate($"Widget.Clock.Config.TimeSource.{GetConfigValue<string>("TimeSource")}") : null;

        PrefixNode.NodeValue       = useCustomPrefix ? GetConfigValue<string>("PrefixText") : null;
        PrefixNode.Style.IsVisible = isPrefixVisible;
        PrefixNode.Style.Glyph     = !useCustomPrefix ? GetPrefixIcon() : null;
        TimeNode.Style.TextOffset  = new(0, GetConfigValue<int>("TextYOffset"));
        TimeNode.Style.Size        = new(GetConfigValue<int>("CustomWidth"), 28);
        TimeNode.Style.Padding     = new() { Left = isPrefixVisible ? 0 : 6, Right = 6 };

        DateTime time        = GetTime();
        var      use24H      = GetConfigValue<bool>("Use24HourFormat");
        var      showSeconds = GetConfigValue<bool>("ShowSeconds");
        var      am          = GetConfigValue<string>("AmLabel");
        var      pm          = GetConfigValue<string>("PmLabel");
        string   timeFormat  = (use24H ? "HH:mm" : "hh:mm") + (showSeconds ? ":ss" : string.Empty);
        string   suffix      = use24H ? string.Empty : (time.Hour >= 12 ? $" {pm}" : $" {am}");

        TimeNode.NodeValue = $"{time.ToString(timeFormat)}{suffix}";
    }

    private void OnClick(Node _)
    {
        var timeSource = GetConfigValue<string>("TimeSource");

        string nextTimeSource = timeSource switch {
            "ET" => "LT",
            "LT" => "ST",
            "ST" => "ET",
            _    => throw new ArgumentOutOfRangeException()
        };

        SetConfigValue("TimeSource", nextTimeSource);
    }

    private SeIconChar GetPrefixIcon()
    {
        var timeSource = GetConfigValue<string>("TimeSource");

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
