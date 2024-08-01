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

internal partial class WeatherWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "ShowName",
                I18N.Translate("Widget.Weather.Config.ShowName.Name"),
                I18N.Translate("Widget.Weather.Config.ShowName.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowTime",
                I18N.Translate("Widget.Weather.Config.ShowTime.Name"),
                I18N.Translate("Widget.Weather.Config.ShowTime.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
            ..TwoLabelTextOffsetVariables,
            new IntegerWidgetConfigVariable(
                "Spacing",
                I18N.Translate("Widget.Weather.Config.Spacing.Name"),
                I18N.Translate("Widget.Weather.Config.Spacing.Description"),
                0,
                0,
                64
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "MaxForecastEntries",
                I18N.Translate("Widget.Weather.Config.MaxForecastEntries.Name"),
                I18N.Translate("Widget.Weather.Config.MaxForecastEntries.Description"),
                4,
                0,
                8
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        ];
    }
}
