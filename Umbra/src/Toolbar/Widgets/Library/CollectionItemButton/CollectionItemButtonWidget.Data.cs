using System.Collections.Generic;
using NotImplementedException = System.NotImplementedException;

namespace Umbra.Widgets.Library.CollectionItemButton;

internal sealed partial class CollectionItemButtonWidget(WidgetInfo info, string? guid = null, Dictionary<string, object>? configValues = null) : DefaultToolbarWidget(info, guid, configValues) {
    public override    WidgetPopup? Popup        { get; }
    protected override void         Initialize()
    {
        throw new NotImplementedException();
    }


}
