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

namespace Umbra.Widgets;

public partial class WeatherWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "ShowIcon",
                I18N.Translate("Widget.Weather.Config.ShowIcon.Name"),
                I18N.Translate("Widget.Weather.Config.ShowIcon.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowTime",
                I18N.Translate("Widget.Weather.Config.ShowTime.Name"),
                I18N.Translate("Widget.Weather.Config.ShowTime.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.Weather.Config.Decorate.Name"),
                I18N.Translate("Widget.Weather.Config.Decorate.Description"),
                false
            ),
            new SelectWidgetConfigVariable(
                "IconLocation",
                I18N.Translate("Widget.Weather.Config.IconLocation.Name"),
                I18N.Translate("Widget.Weather.Config.IconLocation.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.Weather.Config.IconLocation.Option.Left") },
                    { "Right", I18N.Translate("Widget.Weather.Config.IconLocation.Option.Right") },
                }
            ),
            new SelectWidgetConfigVariable(
                "TextAlign",
                I18N.Translate("Widget.Weather.Config.TextAlign.Name"),
                I18N.Translate("Widget.Weather.Config.TextAlign.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.Weather.Config.TextAlign.Option.Left") },
                    { "Center", I18N.Translate("Widget.Weather.Config.TextAlign.Option.Center") },
                    { "Right", I18N.Translate("Widget.Weather.Config.TextAlign.Option.Right") },
                }
            ),
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.Weather.Config.TextYOffset.Name"),
                I18N.Translate("Widget.Weather.Config.TextYOffset.Description"),
                -1,
                -5,
                5
            ),
            new IntegerWidgetConfigVariable(
                "Spacing",
                I18N.Translate("Widget.Weather.Config.Spacing.Name"),
                I18N.Translate("Widget.Weather.Config.Spacing.Description"),
                0,
                0,
                64
            ),
            new IntegerWidgetConfigVariable(
                "MaxForecastEntries",
                I18N.Translate("Widget.Weather.Config.MaxForecastEntries.Name"),
                I18N.Translate("Widget.Weather.Config.MaxForecastEntries.Description"),
                4,
                0,
                8
            )
        ];
    }
}
