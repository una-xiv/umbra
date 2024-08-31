using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.DynamicMenu;

internal sealed partial class DynamicMenuWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
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
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new StringWidgetConfigVariable(
                "ButtonTooltip",
                I18N.Translate("Widget.DynamicMenu.Config.ButtonTooltip.Name"),
                I18N.Translate("Widget.DynamicMenu.Config.ButtonTooltip.Description"),
                ""
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "ButtonIcon",
                I18N.Translate("Widget.DynamicMenu.Config.ButtonIcon.Name"),
                I18N.Translate("Widget.DynamicMenu.Config.ButtonIcon.Description"),
                14,
                0
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
            new StringWidgetConfigVariable("Entries", "", null, "", short.MaxValue) { IsHidden = true },
        ];
    }
}
