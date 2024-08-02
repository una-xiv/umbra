using System.Collections.Generic;
using NotImplementedException = System.NotImplementedException;

namespace Umbra.Widgets.Library.EmoteList;

internal sealed partial class EmoteListWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup { get; }

    protected override void Initialize()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        throw new NotImplementedException();
    }
}
