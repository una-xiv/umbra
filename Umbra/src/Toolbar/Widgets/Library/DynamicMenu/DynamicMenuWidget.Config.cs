using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.DynamicMenu;

internal sealed partial class DynamicMenuWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
                
            new BooleanWidgetConfigVariable(
                "EditModeEnabled",
                I18N.Translate("Widget.DynamicMenu.Config.EditModeEnabled.Name"),
                I18N.Translate("Widget.DynamicMenu.Config.EditModeEnabled.Description"),
                true
            ),
            new StringWidgetConfigVariable(
                "ButtonLabel",
                I18N.Translate("Widget.DynamicMenu.Config.ButtonLabel.Name"),
                I18N.Translate("Widget.DynamicMenu.Config.ButtonLabel.Description"),
                "Dynamic Menu",
                64
            ),
            new StringWidgetConfigVariable(
                "ButtonTooltip",
                I18N.Translate("Widget.DynamicMenu.Config.ButtonTooltip.Name"),
                I18N.Translate("Widget.DynamicMenu.Config.ButtonTooltip.Description"),
                ""
            ),
            new IntegerWidgetConfigVariable(
                "MenuEntryHeight",
                I18N.Translate("Widget.DynamicMenu.Config.MenuEntryHeight.Name"),
                I18N.Translate("Widget.DynamicMenu.Config.MenuEntryHeight.Description"),
                36,
                20,
                64
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "MenuFontSize",
                I18N.Translate("Widget.DynamicMenu.Config.MenuFontSize.Name"),
                I18N.Translate("Widget.DynamicMenu.Config.MenuFontSize.Description"),
                13,
                10,
                20
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "MenuAltFontSize",
                I18N.Translate("Widget.DynamicMenu.Config.MenuAltFontSize.Name"),
                I18N.Translate("Widget.DynamicMenu.Config.MenuAltFontSize.Description"),
                11,
                10,
                20
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowSubIcons",
                I18N.Translate("Widget.DynamicMenu.Config.ShowSubIcons.Name"),
                I18N.Translate("Widget.DynamicMenu.Config.ShowSubIcons.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowItemCount",
                I18N.Translate("Widget.DynamicMenu.Config.ShowItemCount.Name"),
                I18N.Translate("Widget.DynamicMenu.Config.ShowItemCount.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new StringWidgetConfigVariable("Entries", "", null, "", short.MaxValue) { IsHidden = true },
        ];
    }
}
