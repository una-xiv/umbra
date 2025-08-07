namespace Umbra.Widgets.Library.RetainerList;

[ToolbarWidget("RetainerList", "Widget.RetainerList.Name", "Widget.RetainerList.Description", ["retainer", "sales", "marketboard", "ventures"])]
internal sealed partial class RetainerListWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    private IPlayer Player { get; } = Framework.Service<IPlayer>();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    protected override string DefaultIconType   => IconTypeGameIcon;
    protected override uint   DefaultGameIconId => 60560;

    public override RetainerListPopup Popup { get; } = new();

    protected override void OnLoad()
    {
        SetText(Info.Name);
    }

    protected override void OnDraw()
    {
        // Not allowed to use the retainer widget while on a different world or in an instance.
        // The game performs the same checks when opening the timer window. (Thanks Hasel!)
        SetDisabled(Player.CurrentWorldName != Player.HomeWorldName || Player.IsBoundByInstancedDuty);

        Popup.JobIconType     = GetConfigValue<string>("RetainerIconType");
        Popup.ShowGil         = GetConfigValue<bool>("ShowGil");
        Popup.ShowInventory   = GetConfigValue<bool>("ShowInventory");
        Popup.ShowItemsOnSale = GetConfigValue<bool>("ShowItemsOnSale");
        Popup.ShowVenture     = GetConfigValue<bool>("ShowVenture");
    }
}
