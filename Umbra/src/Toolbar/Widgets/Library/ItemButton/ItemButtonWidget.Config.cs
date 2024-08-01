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

internal partial class ItemButtonWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new IntegerWidgetConfigVariable(
                "ItemId",
                I18N.Translate("Widget.ItemButton.Config.ItemId.Name"),
                I18N.Translate("Widget.ItemButton.Config.ItemId.Description"),
                0,
                0
            ),
            new SelectWidgetConfigVariable(
                "ItemUsage",
                I18N.Translate("Widget.ItemButton.Config.ItemUsage.Name"),
                I18N.Translate("Widget.ItemButton.Config.ItemUsage.Description"),
                "HqBeforeNq",
                new() {
                    { "HqBeforeNq", I18N.Translate("Widget.ItemButton.Config.ItemUsage.Option.HqBeforeNq") },
                    { "NqBeforeHq", I18N.Translate("Widget.ItemButton.Config.ItemUsage.Option.NqBeforeHq") },
                    { "HqOnly", I18N.Translate("Widget.ItemButton.Config.ItemUsage.Option.HqOnly") },
                    { "NqOnly", I18N.Translate("Widget.ItemButton.Config.ItemUsage.Option.NqOnly") }
                }
            ),
            new BooleanWidgetConfigVariable(
                "HideIfNotOwned",
                I18N.Translate("Widget.ItemButton.Config.HideIfNotOwned.Name"),
                I18N.Translate("Widget.ItemButton.Config.HideIfNotOwned.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowLabel",
                I18N.Translate("Widget.ItemButton.Config.ShowLabel.Name"),
                I18N.Translate("Widget.ItemButton.Config.ShowLabel.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowCount",
                I18N.Translate("Widget.ItemButton.Config.ShowCount.Name"),
                I18N.Translate("Widget.ItemButton.Config.ShowCount.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
        ];
    }
}
