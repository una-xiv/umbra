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
            items.Add(item.Key, item.Value.Name);
        }

        return [
            new SelectWidgetConfigVariable(
                "Item",
                I18N.Translate("Widget.CollectionItemButton.Config.Item.Name"),
                I18N.Translate("Widget.CollectionItemButton.Config.Item.Description"),
                items.Keys.First(),
                items
            ),
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
        ];
    }
}
