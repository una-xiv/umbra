using System.Collections.Generic;

namespace Umbra.Widgets.Library.RetainerList;

[ToolbarWidget("RetainerList", "Widget.RetainerList.Name", "Widget.RetainerList.Description")]
internal class RetainerListWidget(WidgetInfo info, string? guid = null, Dictionary<string, object>? configValues = null)
    : DefaultToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup { get; } = null;

    protected override void Initialize()
    {
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [];
    }
}
