using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.Societies;

internal sealed partial class SocietiesWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new StringWidgetConfigVariable(
                "ButtonLabel",
                I18N.Translate("Widget.Societies.Config.ButtonLabel.Name"),
                I18N.Translate("Widget.Societies.Config.ButtonLabel.Description"),
                Info.Name
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "ButtonIconId",
                I18N.Translate("Widget.Societies.Config.ButtonIconId.Name"),
                I18N.Translate("Widget.Societies.Config.ButtonIconId.Description"),
                65042,
                0
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
            ..TwoLabelTextOffsetVariables,
            new SelectWidgetConfigVariable(
                "PrimaryAction",
                I18N.Translate("Widget.Societies.Config.PrimaryAction.Name"),
                I18N.Translate("Widget.Societies.Config.PrimaryAction.Description"),
                "Teleport",
                new() {
                    { "Track", I18N.Translate("Widget.Societies.Config.PrimaryAction.Option.Track") },
                    { "Teleport", I18N.Translate("Widget.Societies.Config.PrimaryAction.Option.Teleport") },
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "MinItemsBeforeHorizontalView",
                I18N.Translate("Widget.Societies.Config.MinItemsBeforeHorizontalView.Name"),
                I18N.Translate("Widget.Societies.Config.MinItemsBeforeHorizontalView.Description"),
                10,
                0,
                100
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable("TrackedTribeId", "", null, 0, 0) { IsHidden = true }
        ];
    }
}
