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

internal partial class ClockWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new SelectWidgetConfigVariable(
                "TimeSource",
                I18N.Translate("Widget.Clock.Config.TimeSource.Name"),
                I18N.Translate("Widget.Clock.Config.TimeSource.Description"),
                "LT",
                new() {
                    ["LT"] = I18N.Translate("Widget.Clock.Config.TimeSource.LT"),
                    ["ST"] = I18N.Translate("Widget.Clock.Config.TimeSource.ST"),
                    ["ET"] = I18N.Translate("Widget.Clock.Config.TimeSource.ET"),
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.TimeSource") },
            new BooleanWidgetConfigVariable(
                "ClickToSwitch",
                I18N.Translate("Widget.Clock.Config.ClickToSwitch.Name"),
                I18N.Translate("Widget.Clock.Config.ClickToSwitch.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.TimeSource") },
            new BooleanWidgetConfigVariable(
                "ShowSeconds",
                I18N.Translate("Widget.Clock.Config.ShowSeconds.Name"),
                I18N.Translate("Widget.Clock.Config.ShowSeconds.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.FormatOptions") },
            new BooleanWidgetConfigVariable(
                "Use24HourFormat",
                I18N.Translate("Widget.Clock.Config.Use24HourFormat.Name"),
                I18N.Translate("Widget.Clock.Config.Use24HourFormat.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.FormatOptions") },
            new StringWidgetConfigVariable(
                "AmLabel",
                I18N.Translate("Widget.Clock.Config.AmLabel.Name"),
                I18N.Translate("Widget.Clock.Config.AmLabel.Description"),
                "am",
                16
            ) { Category = I18N.Translate("Widget.ConfigCategory.FormatOptions") },
            new StringWidgetConfigVariable(
                "PmLabel",
                I18N.Translate("Widget.Clock.Config.PmLabel.Name"),
                I18N.Translate("Widget.Clock.Config.PmLabel.Description"),
                "pm",
                16
            ) { Category = I18N.Translate("Widget.ConfigCategory.FormatOptions") },
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.Clock.Config.Decorate.Name"),
                I18N.Translate("Widget.Clock.Config.Decorate.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "UseCustomPrefix",
                I18N.Translate("Widget.Clock.Config.UseCustomPrefix.Name"),
                I18N.Translate("Widget.Clock.Config.UseCustomPrefix.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new StringWidgetConfigVariable(
                "PrefixText",
                I18N.Translate("Widget.Clock.Config.PrefixText.Name"),
                I18N.Translate("Widget.Clock.Config.PrefixText.Description"),
                "LT",
                16
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.Clock.Config.TextYOffset.Name"),
                I18N.Translate("Widget.Clock.Config.TextYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "PrefixYOffset",
                I18N.Translate("Widget.Clock.Config.PrefixYOffset.Name"),
                I18N.Translate("Widget.Clock.Config.PrefixYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "CustomWidth",
                I18N.Translate("Widget.Clock.Config.CustomWidth.Name"),
                I18N.Translate("Widget.Clock.Config.CustomWidth.Description"),
                0,
                0,
                256
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        ];
    }
}
