using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.EmoteList;

internal sealed partial class EmoteListWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new StringWidgetConfigVariable(
                "Label",
                I18N.Translate("Widget.EmoteList.Config.Label.Name"),
                I18N.Translate("Widget.EmoteList.Config.Label.Description"),
                I18N.Translate("Widget.EmoteList.Config.Label.Default")
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "IconId",
                I18N.Translate("Widget.EmoteList.Config.IconId.Name"),
                I18N.Translate("Widget.EmoteList.Config.IconId.Description"),
                14,
                0
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
            ..GetEmoteListConfigVariables(0),
            ..GetEmoteListConfigVariables(1),
            ..GetEmoteListConfigVariables(2),
            ..GetEmoteListConfigVariables(3),
            new BooleanWidgetConfigVariable(
                "ShowEmptySlots",
                I18N.Translate("Widget.ShortcutPanel.Config.ShowEmptySlots.Name"),
                I18N.Translate("Widget.ShortcutPanel.Config.ShowEmptySlots.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable("KeepOpenAfterUse", "", "", false) { IsHidden        = true },
            new IntegerWidgetConfigVariable("LastSelectedCategory", "", "", 0) { IsHidden        = true },
            new StringWidgetConfigVariable("EmoteList", "", null, "", short.MaxValue) { IsHidden = true },
        ];
    }

    private static List<IWidgetConfigVariable> GetEmoteListConfigVariables(byte listId)
    {
        List<IWidgetConfigVariable> variables = [
            new StringWidgetConfigVariable(
                $"Category_{listId}_Name",
                I18N.Translate("Widget.EmoteList.Config.CategoryName.Name", listId + 1),
                I18N.Translate("Widget.EmoteList.Config.CategoryName.Description"),
                listId == 0 ? I18N.Translate("Widget.EmoteList.Name") : ""
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        ];

        return variables;
    }
}
