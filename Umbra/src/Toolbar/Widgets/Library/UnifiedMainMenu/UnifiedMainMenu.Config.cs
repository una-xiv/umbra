using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

internal sealed partial class UnifiedMainMenu
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new StringWidgetConfigVariable(
                "Label",
                I18N.Translate("Widget.UnifiedMainMenu.Config.Label.Name"),
                I18N.Translate("Widget.UnifiedMainMenu.Config.Label.Description"),
                "Start"
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "IconId",
                I18N.Translate("Widget.UnifiedMainMenu.Config.IconId.Name"),
                I18N.Translate("Widget.UnifiedMainMenu.Config.IconId.Description"),
                14,
                0
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
            new BooleanWidgetConfigVariable(
                "DesaturateIcons",
                I18N.Translate("Widget.UnifiedMainMenu.Config.DesaturateIcons.Name"),
                I18N.Translate("Widget.UnifiedMainMenu.Config.DesaturateIcons.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "AvatarIconId",
                I18N.Translate("Widget.UnifiedMainMenu.Config.AvatarIconId.Name"),
                I18N.Translate("Widget.UnifiedMainMenu.Config.AvatarIconId.Description"),
                76985,
                0
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "FixedMenuHeight",
                I18N.Translate("Widget.UnifiedMainMenu.Config.FixedMenuHeight.Name"),
                I18N.Translate("Widget.UnifiedMainMenu.Config.FixedMenuHeight.Description"),
                0,
                0,
                1000
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
