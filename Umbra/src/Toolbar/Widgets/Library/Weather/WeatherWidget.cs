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
using Umbra.Common;
using Umbra.Game;
using Umbra.Widgets.System;

namespace Umbra.Widgets;

public partial class WeatherWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    private IZoneManager? _zoneManager;

    protected override void Initialize()
    {
        _zoneManager = Framework.Service<IZoneManager>();

        SetGhost(true);
        SetIconSize(30);
        SetLeftIcon(60277u);
        SetTwoLabels("Weather name here", "1 hour and 43 minutes");
        SetTextAlignLeft();
    }

    protected override void OnUpdate()
    {
        if (!_zoneManager!.HasCurrentZone) return;
        var zone = _zoneManager.CurrentZone;

        Popup.MaxEntries = (uint)GetConfigValue<int>("MaxForecastEntries");

        var currentWeather = zone.CurrentWeather;
        if (null == currentWeather) return;

        var showIcon     = GetConfigValue<bool>("ShowIcon");
        var iconLocation = GetConfigValue<string>("IconLocation");

        switch (iconLocation) {
            case "Left":
                SetLeftIcon(showIcon ? currentWeather.IconId : null);
                SetRightIcon(null);
                break;
            case "Right":
                SetLeftIcon(null);
                SetRightIcon(showIcon ? currentWeather.IconId : null);
                break;
        }

        var textAlign = GetConfigValue<string>("TextAlign");
        var spacing   = GetConfigValue<int>("Spacing");

        switch (textAlign) {
            case "Left":
                SetTextAlignLeft();
                break;
            case "Center":
                SetTextAlignCenter();
                break;
            case "Right":
                SetTextAlignRight();
                break;
        }

        SetTwoLabels(currentWeather.Name, currentWeather.TimeString);

        Node.QuerySelector("Label")!.Style.Margin = new() {
            Left  = iconLocation == "Left" ? spacing : 0,
            Right = iconLocation == "Right" ? spacing : 0
        };
    }
}
