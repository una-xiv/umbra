using Lumina.Data.Parsing;
using System.Collections.Generic;
using System.Numerics;

namespace Umbra.Widgets.Library.CollectionItemButton;

[ToolbarWidget("CollectionItemButton", "Widget.CollectionItemButton.Name", "Widget.CollectionItemButton.Description")]
internal sealed partial class CollectionItemButtonWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup { get; } = null;

    protected override void Initialize()
    {
        LoadCollectionItems();
        SetLabel("Collection");

        Node.OnMouseUp += Invoke;
    }

    protected override void OnDisposed()
    {
        Node.OnMouseUp -= Invoke;
    }

    protected override void OnUpdate()
    {
        SetGhost(!GetConfigValue<bool>("Decorate"));

        if (!Items.TryGetValue(GetConfigValue<string>("Item"), out var item)) return;

        SetIcon(item.Icon);
        SetLabel(GetConfigValue<string>("DisplayMode") is "TextOnly" or "TextAndIcon" ? item.Name : null);

        base.OnUpdate();
    }
}
