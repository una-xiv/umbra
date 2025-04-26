using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets;

[ToolbarWidget(
    "CollectionItemButton",
    "Widget.CollectionItemButton.Name",
    "Widget.CollectionItemButton.Description",
    ["button", "item", "collectible", "collection"]
)]
internal sealed partial class CollectionItemButtonWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.Icon | StandardWidgetFeatures.Text;

    protected override void OnLoad()
    {
        LoadCollectionItems();
        SetText("Collection");

        Node.OnMouseUp += Invoke;
    }

    protected override void OnDraw()
    {
        if (!Items.TryGetValue(GetConfigValue<string>("Item"), out var item)) return;

        SetGameIconId(item.Icon);
        SetText(item.Name);
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        LoadCollectionItems();

        Dictionary<string, string> items = [];

        foreach (var item in Items) {
            items.Add(item.Key, item.Value.Name);
        }

        return [
            ..base.GetConfigVariables(),

            new SelectWidgetConfigVariable(
                "Item",
                I18N.Translate("Widget.CollectionItemButton.Config.Item.Name"),
                I18N.Translate("Widget.CollectionItemButton.Config.Item.Description"),
                items.Keys.First(),
                items
            ),
        ];
    }
}
