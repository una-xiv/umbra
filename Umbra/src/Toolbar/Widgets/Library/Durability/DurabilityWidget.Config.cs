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
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            new SelectWidgetConfigVariable(
                "WidgetDisplayMode",
                I18N.Translate("Widget.Durability.Config.DisplayMode.Name"),
                I18N.Translate("Widget.Durability.Config.DisplayMode.Description"),
                "Full",
                new() {
                    { "Full", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.Full") },
                    { "Short", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.Short") },
                    { "ShortStacked", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.ShortStacked") },
                    { "DurabilityOnly", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.DurabilityOnly") },
                    { "SpiritbondOnly", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.SpiritbondOnly") },
                    { "IconOnly", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.IconOnly") },
                    { "StackedBars", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.StackedBars") },
                }
            ),
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
                "UseBarBorder",
                I18N.Translate("Widget.Durability.Config.UseBarBorder.Name"),
                I18N.Translate("Widget.Durability.Config.UseBarBorder.Description"),
                false
            ),
            new SelectWidgetConfigVariable(
                "BarDirection",
                I18N.Translate("Widget.Durability.Config.BarDirection.Name"),
                I18N.Translate("Widget.Durability.Config.BarDirection.Description"),
                "L2R",
                new() {
                    { "L2R", I18N.Translate("Widget.Durability.Config.BarDirection.Option.L2R") },
                    { "R2L", I18N.Translate("Widget.Durability.Config.BarDirection.Option.R2L") },
                }
            ),
            new IntegerWidgetConfigVariable(
                "BarWidth",
                I18N.Translate("Widget.Durability.Config.BarWidth.Name"),
                I18N.Translate("Widget.Durability.Config.BarWidth.Description"),
                150,
                50,
                150
            ),
            new SelectWidgetConfigVariable(
                "DurabilityCalculation",
                I18N.Translate("Widget.Durability.Config.Calculation.Durability.Name"),
                I18N.Translate("Widget.Durability.Config.Calculation.Durability.Description"),
                "Min",
                new() {
                    { "Min", I18N.Translate("Widget.Durability.Config.Calculation.Option.Min") },
                    { "Avg", I18N.Translate("Widget.Durability.Config.Calculation.Option.Avg") },
                    { "Max", I18N.Translate("Widget.Durability.Config.Calculation.Option.Max") },
                }
            ),
            new SelectWidgetConfigVariable(
                "SpiritbondCalculation",
                I18N.Translate("Widget.Durability.Config.Calculation.Spiritbond.Name"),
                I18N.Translate("Widget.Durability.Config.Calculation.Spiritbond.Description"),
                "Max",
                new() {
                    { "Min", I18N.Translate("Widget.Durability.Config.Calculation.Option.Min") },
                    { "Avg", I18N.Translate("Widget.Durability.Config.Calculation.Option.Avg") },
                    { "Max", I18N.Translate("Widget.Durability.Config.Calculation.Option.Max") },
                }
            ),
        ];
    }
}
