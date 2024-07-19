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

internal partial class CurrenciesWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        Precache();
        Dictionary<string, string> trackedSelectOptions = new() { { "", "None" } };

        foreach (Currency currency in Currencies.Values)
            trackedSelectOptions.Add(currency.Type.ToString(), currency.Name);

        return [
            new SelectWidgetConfigVariable(
                "TrackedCurrency",
                I18N.Translate("Widget.Currencies.Config.TrackedCurrency.Name"),
                I18N.Translate("Widget.Currencies.Config.TrackedCurrency.Description"),
                "",
                trackedSelectOptions,
                true
            ),
            new StringWidgetConfigVariable(
                "CurrencySeparator",
                I18N.Translate("Widget.Currencies.Config.CurrencySeparator.Name"),
                I18N.Translate("Widget.Currencies.Config.CurrencySeparator.Description"),
                ".",
                1
            ),
            new StringWidgetConfigVariable(
                "CustomCurrencyIds",
                I18N.Translate("Widget.Currencies.Config.CustomCurrencyIds.Name"),
                I18N.Translate("Widget.Currencies.Config.CustomCurrencyIds.Description"),
                ""
            ),
            new BooleanWidgetConfigVariable(
                "EnableMouseInteraction",
                I18N.Translate("Widget.Currencies.Config.EnableMouseInteraction.Name"),
                I18N.Translate("Widget.Currencies.Config.EnableMouseInteraction.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.Currencies.Config.Decorate.Name"),
                I18N.Translate("Widget.Currencies.Config.Decorate.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new StringWidgetConfigVariable(
                "CustomLabel",
                I18N.Translate("Widget.Currencies.Config.CustomWidgetLabel.Name"),
                I18N.Translate("Widget.Currencies.Config.CustomWidgetLabel.Description"),
                "",
                32
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowIcon",
                I18N.Translate("Widget.Currencies.Config.ShowIcon.Name"),
                I18N.Translate("Widget.Currencies.Config.ShowIcon.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowName",
                I18N.Translate("Widget.Currencies.Config.ShowName.Name"),
                I18N.Translate("Widget.Currencies.Config.ShowName.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowCap",
                I18N.Translate("Widget.Currencies.Config.ShowCap.Name"),
                I18N.Translate("Widget.Currencies.Config.ShowCap.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "DesaturateIcon",
                I18N.Translate("Widget.Currencies.Config.DesaturateIcon.Name"),
                I18N.Translate("Widget.Currencies.Config.DesaturateIcon.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "IconLocation",
                I18N.Translate("Widget.Currencies.Config.IconLocation.Name"),
                I18N.Translate("Widget.Currencies.Config.IconLocation.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.Currencies.Config.IconLocation.Option.Left") },
                    { "Right", I18N.Translate("Widget.Currencies.Config.IconLocation.Option.Right") }
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.Currencies.Config.TextYOffset.Name"),
                I18N.Translate("Widget.Currencies.Config.TextYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "DesaturateIcons",
                I18N.Translate("Widget.Currencies.Config.DesaturateIcons.Name"),
                I18N.Translate("Widget.Currencies.Config.DesaturateIcons.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        ];
    }
}
