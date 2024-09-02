using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.RetainerList;

[ToolbarWidget("RetainerList", "Widget.RetainerList.Name", "Widget.RetainerList.Description")]
[ToolbarWidgetTags(["retainer", "sales", "marketboard", "ventures"])]
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
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        SetIcon(GetConfigValue<uint>("IconId"));

        // Not allowed to use the retainer widget while on a different world or in an instance.
        // The game performs the same checks when opening the timer window. (Thanks Hasel!)
        SetDisabled(Player.CurrentWorldName != Player.HomeWorldName || Player.IsBoundByInstancedDuty);

        Popup.JobIconType       = GetConfigValue<string>("IconType");
        Popup.ShowGil           = GetConfigValue<bool>("ShowGil");
        Popup.ShowInventory     = GetConfigValue<bool>("ShowInventory");
        Popup.ShowItemsOnSale   = GetConfigValue<bool>("ShowItemsOnSale");
        Popup.ShowVenture       = GetConfigValue<bool>("ShowVenture");

        base.OnUpdate();
    }
}
