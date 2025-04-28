﻿using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.RetainerList;

internal partial class RetainerListWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),

            new BooleanWidgetConfigVariable(
                "ShowGil",
                I18N.Translate("Widget.RetainerList.Config.ShowGil.Name"),
                I18N.Translate("Widget.RetainerList.Config.ShowGil.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowInventory",
                I18N.Translate("Widget.RetainerList.Config.ShowInventory.Name"),
                I18N.Translate("Widget.RetainerList.Config.ShowInventory.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowItemsOnSale",
                I18N.Translate("Widget.RetainerList.Config.ShowItemsOnSale.Name"),
                I18N.Translate("Widget.RetainerList.Config.ShowItemsOnSale.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowVenture",
                I18N.Translate("Widget.RetainerList.Config.ShowVenture.Name"),
                I18N.Translate("Widget.RetainerList.Config.ShowVenture.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new SelectWidgetConfigVariable(
                "RetainerIconType",
                I18N.Translate("Widget.RetainerList.Config.IconType.Name"),
                I18N.Translate("Widget.RetainerList.Config.IconType.Description"),
                "Default",
                new() {
                    { "Default", I18N.Translate("Widget.RetainerList.Config.IconType.Option.Default") },
                    { "Framed", I18N.Translate("Widget.RetainerList.Config.IconType.Option.Framed") },
                    { "Gearset", I18N.Translate("Widget.RetainerList.Config.IconType.Option.Gearset") },
                    { "Glowing", I18N.Translate("Widget.RetainerList.Config.IconType.Option.Glowing") },
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        ];
    }
}
