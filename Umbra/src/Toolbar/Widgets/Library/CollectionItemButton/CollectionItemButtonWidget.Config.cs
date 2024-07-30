using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets.Library.CollectionItemButton;

internal partial class CollectionItemButtonWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        LoadCollectionItems();

        Dictionary<string, string> items = [];
        foreach (var item in Items) {
            items.Add(item.Key.ToString(), item.Value.Name);
        }

        return [
            new SelectWidgetConfigVariable(
                "Item",
                I18N.Translate("Widget.CollectionItemButton.Config.Item.Name"),
                I18N.Translate("Widget.CollectionItemButton.Config.Item.Description"),
                items.Keys.First(),
                items
            ),
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.CollectionItemButton.Config.Decorate.Name"),
                I18N.Translate("Widget.CollectionItemButton.Config.Decorate.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "DesaturateIcon",
                I18N.Translate("Widget.CollectionItemButton.Config.DesaturateIcon.Name"),
                I18N.Translate("Widget.CollectionItemButton.Config.DesaturateIcon.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "DisplayMode",
                I18N.Translate("Widget.CollectionItemButton.Config.DisplayMode.Name"),
                I18N.Translate("Widget.CollectionItemButton.Config.DisplayMode.Description"),
                "TextAndIcon",
                new() {
                    { "TextAndIcon", I18N.Translate("Widget.CollectionItemButton.Config.DisplayMode.Option.TextAndIcon") },
                    { "TextOnly", I18N.Translate("Widget.CollectionItemButton.Config.DisplayMode.Option.TextOnly") },
                    { "IconOnly", I18N.Translate("Widget.CollectionItemButton.Config.DisplayMode.Option.IconOnly") }
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "IconLocation",
                I18N.Translate("Widget.CollectionItemButton.Config.IconLocation.Name"),
                I18N.Translate("Widget.CollectionItemButton.Config.IconLocation.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.CollectionItemButton.Config.IconLocation.Option.Left") },
                    { "Right", I18N.Translate("Widget.CollectionItemButton.Config.IconLocation.Option.Right") }
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "IconYOffset",
                I18N.Translate("Widget.CollectionItemButton.Config.IconYOffset.Name"),
                I18N.Translate("Widget.CollectionItemButton.Config.IconYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.CollectionItemButton.Config.TextYOffset.Name"),
                I18N.Translate("Widget.CollectionItemButton.Config.TextYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            ..DefaultToolbarWidgetConfigVariables,
        ];
    }
}
