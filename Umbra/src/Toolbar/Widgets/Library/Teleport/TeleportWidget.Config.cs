using System.Collections.Generic;
using Umbra.Common;
using Umbra.Widgets.Popup;

namespace Umbra.Widgets;

internal sealed partial class TeleportWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),

            new EnumWidgetConfigVariable<PopupDisplayMode>(
                "PopupDisplayMode",
                I18N.Translate("Widget.Teleport.Config.PopupDisplayMode.Name"),
                I18N.Translate("Widget.Teleport.Config.PopupDisplayMode.Description"),
                PopupDisplayMode.Condensed
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            
            new SelectWidgetConfigVariable(
                "DefaultOpenedGroupName",
                I18N.Translate("Widget.Teleport.Config.DefaultOpenedGroupName.Name"),
                I18N.Translate("Widget.Teleport.Config.DefaultOpenedGroupName.Description"),
                "Auto",
                new() {
                    { "Auto", I18N.Translate("Widget.Teleport.Config.DefaultOpenedGroupName.Option.Auto") },
                    { "Favorites", I18N.Translate("Widget.Teleport.Config.DefaultOpenedGroupName.Option.Favorites") },
                    { "Other", I18N.Translate("Widget.Teleport.Config.DefaultOpenedGroupName.Option.Other") }
                }
            ) {
                Category  = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                DisplayIf = () => GetConfigValue<PopupDisplayMode>("PopupDisplayMode") == PopupDisplayMode.Condensed,
            },
            new BooleanWidgetConfigVariable(
                "OpenCategoryOnHover",
                I18N.Translate("Widget.Teleport.Config.OpenCategoryOnHover.Name"),
                I18N.Translate("Widget.Teleport.Config.OpenCategoryOnHover.Description"),
                false
            ) {
                Category  = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                DisplayIf = () => GetConfigValue<PopupDisplayMode>("PopupDisplayMode") == PopupDisplayMode.Condensed,
            },
            new IntegerWidgetConfigVariable(
                "PopupHeight",
                I18N.Translate("Widget.Teleport.Config.PopupHeight.Name"),
                I18N.Translate("Widget.Teleport.Config.PopupHeight.Description"),
                400,
                0,
                1200
            ) {
                Category  = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                DisplayIf = () => GetConfigValue<PopupDisplayMode>("PopupDisplayMode") == PopupDisplayMode.Condensed,
            },
            
            new BooleanWidgetConfigVariable(
                "ShowNotification",
                I18N.Translate("Widget.Teleport.Config.ShowNotification.Name"),
                I18N.Translate("Widget.Teleport.Config.ShowNotification.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            
            new IntegerWidgetConfigVariable(
                "PopupFontSize",
                I18N.Translate("Widgets.MenuPopup.Config.ItemFontSize.Name"),
                I18N.Translate("Widgets.MenuPopup.Config.ItemFontSize.Description"),
                11,
                8,
                24
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        ];
    }
}
