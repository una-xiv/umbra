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

using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class WeatherWidgetPopup : WidgetPopup
{
    private IZoneManager? _zoneManager;

    public uint MaxEntries { get; set; } = 8;

    /// <inheritdoc/>
    protected override bool CanOpen()
    {
        if (MaxEntries == 0) return false;

        _zoneManager ??= Framework.Service<IZoneManager>();
        return _zoneManager is { HasCurrentZone: true, CurrentZone.WeatherForecast.Count: > 1 };
    }

    /// <inheritdoc/>
    protected override void OnClose() { }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        Node header = Node.QuerySelector(".header")!;
        Node body   = Node.QuerySelector(".body")!;
        Node bg     = Node.QuerySelector("#Background")!;

        Node.QuerySelector("#Line")!.Style.Size = new(1, (int)(body.Height / Node.ScaleFactor) + 16);

        List<WeatherForecast> forecast        = _zoneManager!.CurrentZone.WeatherForecast;
        WeatherForecast?      currentForecast = _zoneManager!.CurrentZone.CurrentWeather;

        header.QuerySelector(".header-icon")!.Style.IconId        = currentForecast!.IconId;
        header.QuerySelector(".header-text--title")!.NodeValue    = currentForecast.Name;
        header.QuerySelector(".header-text--subtitle")!.NodeValue = _zoneManager.CurrentZone.Name;

        uint color = GetDominantColor(currentForecast.IconId);
        header.Style.BackgroundGradient = GradientColor.Vertical(new(0), new((uint)(0x80 << 24 | color)));
        bg.Style.BackgroundGradient     = GradientColor.Vertical(new((uint)(0x80 << 24 | color)), new(0));

        for (var i = 1; i < 9; i++) {
            Node node = Node.QuerySelector($"#ForecastItem{i}")!;

            WeatherForecast? forecastItem = forecast.ElementAtOrDefault(i);

            node.Style.IsVisible = i <= MaxEntries && forecastItem is not null;
            if (forecastItem is null) continue;

            node.QuerySelector(".forecast-item--icon")!.Style.IconId    = forecastItem.IconId;
            node.QuerySelector(".forecast-item--text--name")!.NodeValue = forecastItem.Name;
            node.QuerySelector(".forecast-item--text--time")!.NodeValue = forecastItem.TimeString;
        }
    }
}
