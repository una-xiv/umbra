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
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;

namespace Umbra.Game;

[Service]
public class WeatherForecastProvider(IDataManager dataManager)
{
    private const double Seconds       = 1;
    private const double Minutes       = 60 * Seconds;
    private const double WeatherPeriod = 23 * Minutes + 20 * Seconds;

    public List<WeatherForecast> GetForecast(
        WeatherRate weatherRate, uint count = 1, double secondIncrement = WeatherPeriod,
        double      initialOffset = 0 * Minutes
    )
    {
        if (count == 0) return [];

        var current = GetCurrentWeather(weatherRate, initialOffset);
        if (current == null) return [];

        try {
            List<WeatherForecast> forecast = [
                new WeatherForecast(
                    current.Value.Item2,
                    FormatForecastTime(current.Value.Item2),
                    current.Value.Item1.Name,
                    (uint)current.Value.Item1.Icon
                )
            ];

            var lastForecast = forecast[0];

            for (var i = 1; i < count; i++) {
                var time          = forecast[0].Time.AddSeconds(i * secondIncrement);
                var weatherTarget = CalculateTarget(time);
                var weather       = GetWeatherFromRate(weatherRate, weatherTarget);

                if (weather == null) continue;

                if (lastForecast.Name == weather.Name) {
                    lastForecast.Time       = time;
                    lastForecast.TimeString = FormatForecastTime(time);
                    continue;
                }

                lastForecast = new WeatherForecast(time, FormatForecastTime(time), weather.Name, (uint)weather.Icon);
                forecast.Add(lastForecast);
            }

            if (forecast.Count > 1) {
                var first   = forecast.First();
                var timeStr = forecast[1].TimeString;
                timeStr          = timeStr.Replace("In ", "")[..1].ToUpper() + timeStr[1..];
                first.TimeString = timeStr;
            }

            return forecast;
        } catch (Exception) {
            // ignored
        }

        return [];
    }

    private (Weather, DateTime)? GetCurrentWeather(WeatherRate weatherRate, double initialOffset = 0 * Minutes)
    {
        var rootTime = GetRootTime(initialOffset);
        var target   = CalculateTarget(rootTime);
        var weather  = GetWeatherFromRate(weatherRate, target);

        if (weather == null) return null;

        return (weather, rootTime);
    }

    private Weather? GetWeatherFromRate(WeatherRate weatherRateIndex, int target)
    {
        int rateAccumulator = 0;
        int weatherId       = -1;

        if (weatherRateIndex.UnkData0.Length == 0) return null;

        for (var i = 0; i < weatherRateIndex.UnkData0.Length; i++) {
            var w = weatherRateIndex.UnkData0[i];

            rateAccumulator += w.Rate;

            if (target < rateAccumulator) {
                weatherId = w.Weather;
                break;
            }
        }

        if (weatherId == -1) {
            return null;
        }

        return dataManager.GetExcelSheet<Weather>()!.GetRow((uint)weatherId);
    }

    private static DateTime GetRootTime(double initialOffset)
    {
        var now      = DateTime.UtcNow;
        var rootTime = now.AddMilliseconds(-now.Millisecond).AddSeconds(initialOffset);
        var seconds  = (long)(rootTime - DateTime.UnixEpoch).TotalSeconds % WeatherPeriod;

        rootTime = rootTime.AddSeconds(-seconds);

        return rootTime;
    }

    // https://github.com/xivapi/ffxiv-datamining/blob/master/docs/Weather.md
    private static int CalculateTarget(DateTime time)
    {
        var unix      = (int)(time - DateTime.UnixEpoch).TotalSeconds;
        var bell      = unix                            / 175;
        var increment = ((uint)(bell + 8 - (bell % 8))) % 24;

        var totalDays = (uint)(unix / 4200);
        var calcBase  = (totalDays  * 0x64) + increment;

        var step1 = (calcBase << 0xB) ^ calcBase;
        var step2 = (step1    >> 8)   ^ step1;

        return (int)(step2 % 0x64);
    }

    private static string FormatForecastTime(DateTime forecastTime)
    {
        TimeSpan timeDifference = forecastTime - DateTime.UtcNow;
        double   totalMinutes   = timeDifference.TotalMinutes;

        if (totalMinutes <= 0.01) return "Now";
        if (totalMinutes < 1) return "In a minute";
        if (totalMinutes < 60) return $"{Math.Round(totalMinutes)} minute{(totalMinutes != 1 ? "s" : "")}";

        int hours            = (int)(totalMinutes / 60);
        int remainingMinutes = (int)(totalMinutes % 60);
        if (remainingMinutes == 0) return $"{hours} hour{(hours > 1 ? "s" : "")}";

        return $"{hours} hour{(hours > 1 ? "s" : "")} and {remainingMinutes} minute{(remainingMinutes > 1 ? "s" : "")}";
    }
}
