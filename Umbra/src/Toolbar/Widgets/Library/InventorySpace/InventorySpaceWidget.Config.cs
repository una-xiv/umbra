﻿/* Umbra | (c) 2024 by Una              ____ ___        ___.
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
            ..base.GetConfigVariables(),
            
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
                "ShowTotal",
                I18N.Translate("Widget.InventorySpace.Config.ShowTotal.Name"),
                I18N.Translate("Widget.InventorySpace.Config.ShowTotal.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowRemaining",
                I18N.Translate("Widget.InventorySpace.Config.ShowRemaining.Name"),
                I18N.Translate("Widget.InventorySpace.Config.ShowRemaining.Description"),
                false
            ),
            new IconIdWidgetConfigVariable(
                "InventoryIcon",
                I18N.Translate("Widget.InventorySpace.Config.InventoryIcon.Name"),
                I18N.Translate("Widget.InventorySpace.Config.InventoryIcon.Description"),
                2
            ) { Category = I18N.Translate("Widgets.Standard.Config.Category.Icon") },
            new IconIdWidgetConfigVariable(
                "SaddlebagIcon",
                I18N.Translate("Widget.InventorySpace.Config.SaddlebagIcon.Name"),
                I18N.Translate("Widget.InventorySpace.Config.SaddlebagIcon.Description"),
                74
            ) { Category = I18N.Translate("Widgets.Standard.Config.Category.Icon") }
        ];
    }
}
