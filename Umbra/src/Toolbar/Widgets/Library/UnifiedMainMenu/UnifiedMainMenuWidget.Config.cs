using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

internal sealed partial class UnifiedMainMenuWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            
            new StringWidgetConfigVariable(
                "Label",
                I18N.Translate("Widget.UnifiedMainMenu.Config.Label.Name"),
                I18N.Translate("Widget.UnifiedMainMenu.Config.Label.Description"),
                "Start"
            ),
            new BooleanWidgetConfigVariable(
                "OpenSubMenusOnHover",
                I18N.Translate("Widget.UnifiedMainMenu.Config.OpenSubMenusOnHover.Name"),
                I18N.Translate("Widget.UnifiedMainMenu.Config.OpenSubMenusOnHover.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "DesaturateIcons",
                I18N.Translate("Widgets.MenuPopup.Config.DesaturateMenuIcons.Name"),
                I18N.Translate("Widgets.MenuPopup.Config.DesaturateMenuIcons.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IconIdWidgetConfigVariable(
                "AvatarIconId",
                I18N.Translate("Widget.UnifiedMainMenu.Config.AvatarIconId.Name"),
                I18N.Translate("Widget.UnifiedMainMenu.Config.AvatarIconId.Description"),
                114003
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new SelectWidgetConfigVariable(
                "BannerLocation",
                I18N.Translate("Widget.UnifiedMainMenu.Config.BannerLocation.Name"),
                I18N.Translate("Widget.UnifiedMainMenu.Config.BannerLocation.Description"),
                "Auto",
                new() {
                    { "Auto", I18N.Translate("Widget.UnifiedMainMenu.Config.BannerLocation.Option.Auto") },
                    { "Top", I18N.Translate("Widget.UnifiedMainMenu.Config.BannerLocation.Option.Top") },
                    { "Bottom", I18N.Translate("Widget.UnifiedMainMenu.Config.BannerLocation.Option.Bottom") },
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new SelectWidgetConfigVariable(
                "BannerNameStyle",
                I18N.Translate("Widget.UnifiedMainMenu.Config.BannerNameStyle.Name"),
                I18N.Translate("Widget.UnifiedMainMenu.Config.BannerNameStyle.Description"),
                "FirstName",
                new() {
                    { "FirstName", I18N.Translate("Widget.UnifiedMainMenu.Config.BannerNameStyle.Option.FirstName") },
                    { "LastName", I18N.Translate("Widget.UnifiedMainMenu.Config.BannerNameStyle.Option.LastName") },
                    { "FullName", I18N.Translate("Widget.UnifiedMainMenu.Config.BannerNameStyle.Option.FullName") },
                    { "Initials", I18N.Translate("Widget.UnifiedMainMenu.Config.BannerNameStyle.Option.Initials") },
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new SelectWidgetConfigVariable(
                "BannerColorStyle",
                I18N.Translate("Widget.UnifiedMainMenu.Config.BannerColorStyle.Name"),
                I18N.Translate("Widget.UnifiedMainMenu.Config.BannerColorStyle.Description"),
                "AccentColor",
                new() {
                    { "AccentColor", I18N.Translate("Widget.UnifiedMainMenu.Config.BannerColorStyle.Option.AccentColor") },
                    { "RoleColor", I18N.Translate("Widget.UnifiedMainMenu.Config.BannerColorStyle.Option.RoleColor") },
                    { "None", I18N.Translate("Widget.UnifiedMainMenu.Config.BannerColorStyle.Option.None") },
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "VerticalItemSpacing",
                I18N.Translate("Widgets.MenuPopup.Config.VerticalItemSpacing.Name"),
                I18N.Translate("Widgets.MenuPopup.Config.VerticalItemSpacing.Description"),
                2,
                0,
                100
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "MenuHeight",
                I18N.Translate("Widgets.MenuPopup.Config.MenuHeight.Name"),
                I18N.Translate("Widgets.MenuPopup.Config.MenuHeight.Description"),
                400,
                300,
                1000
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ReverseCategories",
                I18N.Translate("Widgets.MenuPopup.Config.ReverseCategories.Name"),
                I18N.Translate("Widgets.MenuPopup.Config.ReverseCategories.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "PopupFontSize",
                I18N.Translate("Widgets.MenuPopup.Config.ItemFontSize.Name"),
                I18N.Translate("Widgets.MenuPopup.Config.ItemFontSize.Description"),
                12,
                8,
                24
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new StringWidgetConfigVariable(
                "PinnedItems",
                "",
                null,
                "[]",
                short.MaxValue
            ) { IsHidden = true }
        ];
    }
}
