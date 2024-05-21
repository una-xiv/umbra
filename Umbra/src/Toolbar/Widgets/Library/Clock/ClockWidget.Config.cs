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

public partial class ClockWidget
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
                    ["LT"] = I18N.Translate("Widget.Clock.Config.TimeSource.LocalTime"),
                    ["ST"] = I18N.Translate("Widget.Clock.Config.TimeSource.ServerTime"),
                    ["ET"] = I18N.Translate("Widget.Clock.Config.TimeSource.EorzeaTime"),
                }
            ),
            new BooleanWidgetConfigVariable(
                "UseCustomPrefix",
                I18N.Translate("Widget.Clock.Config.UseCustomPrefix.Name"),
                I18N.Translate("Widget.Clock.Config.UseCustomPrefix.Description"),
                false
            ),
            new StringWidgetConfigVariable(
                "PrefixText",
                I18N.Translate("Widget.Clock.Config.PrefixText.Name"),
                I18N.Translate("Widget.Clock.Config.PrefixText.Description"),
                "LT",
                16
            ),
            new BooleanWidgetConfigVariable(
                "ShowSeconds",
                I18N.Translate("Widget.Clock.Config.ShowSeconds.Name"),
                I18N.Translate("Widget.Clock.Config.ShowSeconds.Description"),
                false
            ),
            new BooleanWidgetConfigVariable(
                "Use24HourFormat",
                I18N.Translate("Widget.Clock.Config.Use24HourFormat.Name"),
                I18N.Translate("Widget.Clock.Config.Use24HourFormat.Description"),
                true
            ),
            new StringWidgetConfigVariable(
                "AmLabel",
                I18N.Translate("Widget.Clock.Config.AmLabel.Name"),
                I18N.Translate("Widget.Clock.Config.AmLabel.Description"),
                "am",
                16
            ),
            new StringWidgetConfigVariable(
                "PmLabel",
                I18N.Translate("Widget.Clock.Config.PmLabel.Name"),
                I18N.Translate("Widget.Clock.Config.PmLabel.Description"),
                "pm",
                16
            ),
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.Clock.Config.TextYOffset.Name"),
                I18N.Translate("Widget.Clock.Config.TextYOffset.Description"),
                0,
                -5,
                5
            ),
            new IntegerWidgetConfigVariable(
                "CustomWidth",
                I18N.Translate("Widget.Clock.Config.CustomWidth.Name"),
                I18N.Translate("Widget.Clock.Config.CustomWidth.Description"),
                0,
                0,
                256
            )
        ];
    }
}
