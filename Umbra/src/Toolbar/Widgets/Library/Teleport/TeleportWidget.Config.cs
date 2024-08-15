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

internal sealed partial class TeleportWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
            new BooleanWidgetConfigVariable(
                "ShowNotification",
                I18N.Translate("Widget.Teleport.Config.ShowNotification.Name"),
                I18N.Translate("Widget.Teleport.Config.ShowNotification.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowMapNames",
                I18N.Translate("Widget.Teleport.Config.ShowMapNames.Name"),
                I18N.Translate("Widget.Teleport.Config.ShowMapNames.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "OpenFavoritesByDefault",
                I18N.Translate("Widget.Teleport.Config.OpenFavoritesByDefault.Name"),
                I18N.Translate("Widget.Teleport.Config.OpenFavoritesByDefault.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "OpenCategoryOnHover",
                I18N.Translate("Widget.Teleport.Config.OpenCategoryOnHover.Name"),
                I18N.Translate("Widget.Teleport.Config.OpenCategoryOnHover.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new SelectWidgetConfigVariable(
                "ExpansionListPosition",
                I18N.Translate("Widget.Teleport.Config.ExpansionListPosition.Name"),
                I18N.Translate("Widget.Teleport.Config.ExpansionListPosition.Description"),
                "Auto",
                new() {
                    { "Auto", I18N.Translate("Widget.Teleport.Config.ExpansionListPosition.Option.Auto") },
                    { "Left", I18N.Translate("Widget.Teleport.Config.ExpansionListPosition.Option.Left") },
                    { "Right", I18N.Translate("Widget.Teleport.Config.ExpansionListPosition.Option.Right") }
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "ColumnWidth",
                I18N.Translate("Widget.Teleport.Config.ColumnWidth.Name"),
                I18N.Translate("Widget.Teleport.Config.ColumnWidth.Description"),
                300,
                100,
                500
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "MinimumColumns",
                I18N.Translate("Widget.Teleport.Config.MinimumColumns.Name"),
                I18N.Translate("Widget.Teleport.Config.MinimumColumns.Description"),
                1,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") }
        ];
    }
}
