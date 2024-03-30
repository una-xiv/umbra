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

using Umbra.Common;
using Umbra.Drawing;
using Umbra.Game;

namespace Umbra;

[Service]
internal sealed partial class LocationWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Height")] private static int Height { get; set; } = 32;

    private readonly IZoneManager _manager;

    public LocationWidget(IZoneManager manager, ToolbarDropdownContext ctx)
    {
        _manager = manager;

        BuildWeatherList();
        ctx.RegisterDropdownActivator(Element, DropdownElement);
    }

    [OnDraw]
    public void OnDraw()
    {
        Zone zone = _manager.CurrentZone;

        string zoneName = zone.Name;
        string distName = zone.CurrentDistrictName;

        if (distName == "" && zoneName.Contains(" - ")) {
            string[] split = zoneName.Split(" - ");
            zoneName = split[1];
            distName = split[0];
        }

        // Fallback to region name if zone name is empty. This may occur in an Inn room.
        if (zoneName == "") zoneName = zone.RegionName;
        if (distName == "") distName = zone.RegionName;

        Element.Get("Location.Name").GetNode<TextNode>().Text     = zoneName;
        Element.Get("Location.District").GetNode<TextNode>().Text = distName;
        Element.Get("Weather.Name").GetNode<TextNode>().Text      = zone.CurrentWeather?.Name       ?? "";
        Element.Get("Weather.Time").GetNode<TextNode>().Text      = zone.CurrentWeather?.TimeString ?? "";
        Element.Get("Icon").GetNode<IconNode>().IconId            = zone.CurrentWeather?.IconId     ?? 0;

        // Update height.
        Element.Size                          = new(0, Height     - 4);
        Element.Get("Location").Size          = new(240           - (Height / 2 - 3), Height - 6);
        Element.Get("Location.Name").Size     = new(0, Height / 2 - 3);
        Element.Get("Location.District").Size = new(0, Height / 2 - 3);
        Element.Get("Weather").Size           = new(240           - (Height / 2 - 3), Height - 6);
        Element.Get("Weather.Name").Size      = new(0, Height / 2 - 3);
        Element.Get("Weather.Time").Size      = new(0, Height / 2 - 3);
        Element.Get("Icon").Size              = new(Height        - 6, Height - 6);

        if (DropdownElement.IsVisible == false) return;

        var forecast   = zone.WeatherForecast;
        var headerIcon = DropdownElement.Content.Get("Header.Icon").GetNode<IconNode>();
        var location   = DropdownElement.Content.Get("Header.Text.Location").GetNode<TextNode>();
        var district   = DropdownElement.Content.Get("Header.Text.District").GetNode<TextNode>();
        var gradTop    = DropdownElement.Content.Get("Header").GetNode<RectNode>("GradTop");

        headerIcon.IconId = forecast[0].IconId;
        location.Text     = zoneName;
        district.Text     = $"{distName}, {forecast[0].Name}";

        var color = AssetManager.GetIconFile(forecast[0].IconId).GetDominantColor();

        gradTop.Gradients = new(bottomLeft: color.ToUint(0.70f), bottomRight: color.ToUint(0.70f));

        DropdownElement.Content.GetNode<RectNode>("GradientBackground").Gradients = new(
            topLeft: color.ToUint(0.85f),
            topRight: color.ToUint(0.85f)
        );

        for (int i = 1; i < 7; i++) {
            var row  = DropdownElement.Content.Get($"WeatherList.WeatherRow_{i - 1}");
            var icon = row.Get("Icon").GetNode<IconNode>();
            var name = row.Get("Text.Name").GetNode<TextNode>();
            var time = row.Get("Text.Time").GetNode<TextNode>();

            if (i < forecast.Count) {
                icon.IconId   = forecast[i].IconId;
                name.Text     = forecast[i].Name;
                time.Text     = forecast[i].TimeString;
                row.IsVisible = true;
            } else {
                row.IsVisible = false;
            }
        }
    }
}
