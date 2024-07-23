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

namespace Umbra.Widgets.Library.InventorySpace;

internal partial class InventorySpaceWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new SelectWidgetConfigVariable(
                "Source",
                I18N.Translate("Widget.InventorySpace.Config.Source.Name"),
                I18N.Translate("Widget.InventorySpace.Config.Source.Description"),
                "Inventory",
                new() {
                    { "Inventory", I18N.Translate("Widget.InventorySpace.Config.Source.Option.Inventory") },
                    { "SaddleBag", I18N.Translate("Widget.InventorySpace.Config.Source.Option.SaddleBag") },
                    { "SaddleBagPremium", I18N.Translate("Widget.InventorySpace.Config.Source.Option.SaddleBagPremium") }
                }
            ),
            new IntegerWidgetConfigVariable(
                "WarningThreshold",
                I18N.Translate("Widget.InventorySpace.Config.WarningThreshold.Name"),
                I18N.Translate("Widget.InventorySpace.Config.WarningThreshold.Description"),
                6,
                1,
                100
            ),
            new IntegerWidgetConfigVariable(
                "CriticalThreshold",
                I18N.Translate("Widget.InventorySpace.Config.CriticalThreshold.Name"),
                I18N.Translate("Widget.InventorySpace.Config.CriticalThreshold.Description"),
                1,
                0,
                100
            ),
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.InventorySpace.Config.Decorate.Name"),
                I18N.Translate("Widget.InventorySpace.Config.Decorate.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "DesaturateIcon",
                I18N.Translate("Widget.InventorySpace.Config.DesaturateIcon.Name"),
                I18N.Translate("Widget.InventorySpace.Config.DesaturateIcon.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowTotal",
                I18N.Translate("Widget.InventorySpace.Config.ShowTotal.Name"),
                I18N.Translate("Widget.InventorySpace.Config.ShowTotal.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowRemaining",
                I18N.Translate("Widget.InventorySpace.Config.ShowRemaining.Name"),
                I18N.Translate("Widget.InventorySpace.Config.ShowRemaining.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "IconLocation",
                I18N.Translate("Widget.InventorySpace.Config.IconLocation.Name"),
                I18N.Translate("Widget.InventorySpace.Config.IconLocation.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.InventorySpace.Config.IconLocation.Option.Left") },
                    { "Right", I18N.Translate("Widget.InventorySpace.Config.IconLocation.Option.Right") }
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.InventorySpace.Config.TextYOffset.Name"),
                I18N.Translate("Widget.InventorySpace.Config.TextYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        ];
    }
}
