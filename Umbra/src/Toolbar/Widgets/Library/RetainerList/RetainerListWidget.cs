using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.RetainerList;

[ToolbarWidget("RetainerList", "Widget.RetainerList.Name", "Widget.RetainerList.Description")]
internal partial class RetainerListWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    private IPlayer Player { get; } = Framework.Service<IPlayer>();

    /// <inheritdoc/>
    public override RetainerListPopup Popup { get; } = new();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        SetLabel(Info.Name);
        SetIcon(60560u);
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        // Not allowed to use the retainer widget while on a different world or in an instance.
        // The game performs the same checks when opening the timer window. (Thanks Hasel!)
        SetDisabled(Player.CurrentWorldName != Player.HomeWorldName || Player.IsBoundByInstancedDuty);

        Popup.JobIconType = GetConfigValue<string>("IconType");

        base.OnUpdate();
    }
}
