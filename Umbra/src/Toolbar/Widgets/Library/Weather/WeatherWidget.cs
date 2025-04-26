﻿using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget(
    "Weather",
    "Widget.Weather.Name",
    "Widget.Weather.Description",
    ["weather", "forecast", "location"]
)]
internal class WeatherWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override WeatherWidgetPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText;

    private IZoneManager? _zoneManager;

    protected override void OnLoad()
    {
        _zoneManager = Framework.Service<IZoneManager>();

        SetGameIconId(60277u);
        SetText("Weather name here");
        SetSubText("1 hour and 43 minutes");
    }

    protected override void OnDraw()
    {
        if (!_zoneManager!.HasCurrentZone) return;
        var zone = _zoneManager.CurrentZone;

        var currentWeather = zone.CurrentWeather;
        if (null == currentWeather) return;

        Popup.MaxEntries = (uint)GetConfigValue<int>("MaxForecastEntries");

        BodyNode.QuerySelector(".body")!.Style.Padding = new() { Left = GetConfigValue<int>("Spacing") };

        SetText(currentWeather.Name);
        SetSubText(currentWeather.TimeString);
        SetGameIconId(currentWeather.IconId);
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            new IntegerWidgetConfigVariable(
                "Spacing",
                I18N.Translate("Widget.Weather.Config.Spacing.Name"),
                I18N.Translate("Widget.Weather.Config.Spacing.Description"),
                0,
                0,
                2048
            ),
            new IntegerWidgetConfigVariable(
                "MaxForecastEntries",
                I18N.Translate("Widget.Weather.Config.MaxForecastEntries.Name"),
                I18N.Translate("Widget.Weather.Config.MaxForecastEntries.Description"),
                4,
                0,
                8
            ),
        ];
    }
}
