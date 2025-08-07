using Lumina.Excel.Sheets;

namespace Umbra.Widgets;

[ToolbarWidget(
    "Teleport", 
    "Widget.Teleport.Name", 
    "Widget.Teleport.Description", 
    ["teleport", "travel", "favorites", "locations", "housing", "estate"]
)]
internal sealed partial class TeleportWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override TeleportWidgetPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;
    
    protected override string DefaultIconType   => IconTypeGameIcon;
    protected override uint   DefaultGameIconId => 60453;

    private IPlayer Player { get; set; } = Framework.Service<IPlayer>();

    private string TeleportName { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnLoad()
    {
        var teleportAction = Framework.Service<IDataManager>().GetExcelSheet<GeneralAction>().GetRow(7);
        TeleportName = teleportAction.Name.ToString();

        Node.OnRightClick += OpenTeleportWindow;
    }

    protected override void OnUnload()
    {
        Node.OnRightClick -= OpenTeleportWindow;
    }

    protected override void OnDraw()
    {
        SetDisabled(!Player.CanUseTeleportAction);
        SetText(TeleportName);
    }

    private string GetExpansionMenuPosition()
    {
        return GetConfigValue<string>("ExpansionListPosition") switch {
            "Auto"  => Node.ParentNode!.Id == "Right" ? "Right" : "Left",
            "Left"  => "Left",
            "Right" => "Right",
            _       => "Top"
        };
    }

    private void OpenTeleportWindow(Node _)
    {
        Player.UseGeneralAction(7);
    }
}
