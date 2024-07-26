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

internal partial class LocationWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "ShowDistrict",
                I18N.Translate("Widget.Location.Config.ShowDistrict.Name"),
                I18N.Translate("Widget.Location.Config.ShowDistrict.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "UseTwoLabels",
                I18N.Translate("Widget.Location.Config.UseTwoLabels.Name"),
                I18N.Translate("Widget.Location.Config.UseTwoLabels.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.Location.Config.Decorate.Name"),
                I18N.Translate("Widget.Location.Config.Decorate.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "TextAlign",
                I18N.Translate("Widget.Location.Config.TextAlign.Name"),
                I18N.Translate("Widget.Location.Config.TextAlign.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.Location.Config.TextAlign.Option.Left") },
                    { "Center", I18N.Translate("Widget.Location.Config.TextAlign.Option.Center") },
                    { "Right", I18N.Translate("Widget.Location.Config.TextAlign.Option.Right") },
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.Location.Config.TextYOffset.Name"),
                I18N.Translate("Widget.Location.Config.TextYOffset.Description"),
                -1,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffsetTop",
                I18N.Translate("Widget.Location.Config.TextYOffsetTop.Name"),
                I18N.Translate("Widget.Location.Config.TextYOffsetTop.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffsetBottom",
                I18N.Translate("Widget.Location.Config.TextYOffsetBottom.Name"),
                I18N.Translate("Widget.Location.Config.TextYOffsetBottom.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            ..DefaultToolbarWidgetConfigVariables
        ];
    }
}
