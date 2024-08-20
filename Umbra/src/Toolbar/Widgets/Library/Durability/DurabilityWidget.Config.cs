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

internal partial class DurabilityWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "HideWhenOkay",
                I18N.Translate("Widget.Durability.Config.HideWhenOkay.Name"),
                I18N.Translate("Widget.Durability.Config.HideWhenOkay.Description"),
                false
            ),
            new IntegerWidgetConfigVariable(
                "WarningThreshold",
                I18N.Translate("Widget.Durability.Config.WarningThreshold.Name"),
                I18N.Translate("Widget.Durability.Config.WarningThreshold.Description"),
                50,
                1,
                100
            ),
            new IntegerWidgetConfigVariable(
                "CriticalThreshold",
                I18N.Translate("Widget.Durability.Config.CriticalThreshold.Name"),
                I18N.Translate("Widget.Durability.Config.CriticalThreshold.Description"),
                25,
                0,
                100
            ),
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.Durability.Config.Decorate.Name"),
                I18N.Translate("Widget.Durability.Config.Decorate.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "DesaturateIcon",
                I18N.Translate("Widget.Durability.Config.DesaturateIcon.Name"),
                I18N.Translate("Widget.Durability.Config.DesaturateIcon.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "DisplayMode",
                I18N.Translate("Widget.Durability.Config.DisplayMode.Name"),
                I18N.Translate("Widget.Durability.Config.DisplayMode.Description"),
                "Full",
                new() {
                    { "Full", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.Full") },
                    { "Short", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.Short") },
                    { "ShortStacked", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.ShortStacked") },
                    { "DurabilityOnly", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.DurabilityOnly") },
                    { "SpiritbondOnly", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.SpiritbondOnly") },
                    { "IconOnly", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.IconOnly") }
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "TextAlign",
                I18N.Translate("Widget.Durability.Config.TextAlign.Name"),
                I18N.Translate("Widget.Durability.Config.TextAlign.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.Durability.Config.TextAlign.Option.Left") },
                    { "Center", I18N.Translate("Widget.Durability.Config.TextAlign.Option.Center") },
                    { "Right", I18N.Translate("Widget.Durability.Config.TextAlign.Option.Right") }
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "IconLocation",
                I18N.Translate("Widget.Durability.Config.IconLocation.Name"),
                I18N.Translate("Widget.Durability.Config.IconLocation.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.Durability.Config.IconLocation.Option.Left") },
                    { "Right", I18N.Translate("Widget.Durability.Config.IconLocation.Option.Right") },
                    { "Hidden", I18N.Translate("Widget.Durability.Config.IconLocation.Option.Hidden") },
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextSize",
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextSize.Name"),
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextSize.Description"),
                13,
                8,
                24
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextSizeTop",
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextSizeTop.Name"),
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextSizeTop.Description"),
                12,
                8,
                24
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextSizeBottom",
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextSizeBottom.Name"),
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextSizeBottom.Description"),
                9,
                8,
                24
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.Durability.Config.TextYOffset.Name"),
                I18N.Translate("Widget.Durability.Config.TextYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffsetTop",
                I18N.Translate("Widget.Durability.Config.TextYOffsetTop.Name"),
                I18N.Translate("Widget.Durability.Config.TextYOffsetTop.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffsetBottom",
                I18N.Translate("Widget.Durability.Config.TextYOffsetBottom.Name"),
                I18N.Translate("Widget.Durability.Config.TextYOffsetBottom.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        ];
    }
}
