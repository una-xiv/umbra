/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Game is free software: you can          \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Game is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal unsafe class WeatherForecastProvider
{
    private const double Seconds       = 1;
    private const double Minutes       = 60 * Seconds;
    private const double WeatherPeriod = 23 * Minutes + 20 * Seconds;

    private readonly IDataManager _dataManager;

    public WeatherForecastProvider(IDataManager dataManager, IGameInteropProvider interopProvider)
    {
        _dataManager = dataManager;
        interopProvider.InitializeFromAttributes(this);
    }

    public List<WeatherForecast> GetWeatherForecast(ushort territoryId)
    {
        WeatherManager* wm = WeatherManager.Instance();
        if (null == wm) return [];

        byte currentWeatherId = wm->GetCurrentWeather();

        Weather currentWeather = _dataManager.GetExcelSheet<Weather>().GetRow(currentWeatherId);
        Weather lastWeather    = currentWeather;

        List<WeatherForecast> result = [BuildResultObject(currentWeather, GetRootTime(0))];

        try {
            for (var i = 1; i < 24; i++) {
                byte weatherId = wm->GetWeatherForDaytime(territoryId, i);
                var  weather   = _dataManager.GetExcelSheet<Weather>().FindRow(weatherId)!;
                var  time      = GetRootTime(i * WeatherPeriod);

                if (lastWeather.RowId != weather.Value.RowId) {
                    lastWeather = weather.Value;
                    result.Add(BuildResultObject(weather.Value, time));
                }
            }
        } catch (Exception e) {
            Logger.Error(e.Message);
        }

        return result;
    }

    private static WeatherForecast BuildResultObject(Weather weather, DateTime time)
    {
        var timeString = FormatForecastTime(time);
        var name       = weather.Name.ToDalamudString().TextValue;
        var iconId     = (uint)weather.Icon;

        return new(time, timeString, name, iconId);
    }

    private static DateTime GetRootTime(double initialOffset)
    {
        var now      = DateTime.UtcNow;
        var rootTime = now.AddMilliseconds(-now.Millisecond).AddSeconds(initialOffset);
        var seconds  = (long)(rootTime - DateTime.UnixEpoch).TotalSeconds % WeatherPeriod;

        rootTime = rootTime.AddSeconds(-seconds);

        return rootTime;
    }

    private static string FormatForecastTime(DateTime forecastTime)
    {
        TimeSpan timeDifference = forecastTime - DateTime.UtcNow;
        double   totalMinutes   = timeDifference.TotalMinutes;

        switch (totalMinutes) {
            case <= 0.01:
                return I18N.Translate("WeatherForecast.Now");
            case < 1:
                return $"{I18N.Translate("WeatherForecast.LessThan")} {I18N.Translate("WeatherForecast.AMinute")}";
            case < 2:
                return $"{I18N.Translate("WeatherForecast.AMinute")}";
            case < 60:
                return $"{I18N.Translate("WeatherForecast.XMinutes", (int)totalMinutes)}";
        }

        var hours            = (int)(totalMinutes / 60);
        var remainingMinutes = (int)(totalMinutes % 60);

        if (remainingMinutes == 0)
            return hours == 1
                ? I18N.Translate("WeatherForecast.AnHour")
                : I18N.Translate("WeatherForecast.XHours", hours);

        string hoursStr = hours == 1
            ? I18N.Translate("WeatherForecast.AnHour")
            : I18N.Translate("WeatherForecast.XHours", hours);

        string minutesStr = remainingMinutes == 1
            ? I18N.Translate("WeatherForecast.AMinute")
            : I18N.Translate("WeatherForecast.XMinutes", remainingMinutes);

        return $"{hoursStr} {I18N.Translate("WeatherForecast.And")} {minutesStr}";
    }
}
