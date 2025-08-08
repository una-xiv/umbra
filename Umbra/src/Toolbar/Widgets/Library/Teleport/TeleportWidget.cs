using Lumina.Excel.Sheets;
using Umbra.Widgets.Popup;

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

    private IPlayer Player { get; } = Framework.Service<IPlayer>();

    private string TeleportName { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnLoad()
    {
        var teleportAction = Framework.Service<IDataManager>().GetExcelSheet<GeneralAction>().GetRow(7);
        TeleportName = teleportAction.Name.ToString();

        Node.OnClick += OpenPopupMenu;
        Node.OnRightClick += OpenTeleportWindow;
    }

    protected override void OnUnload()
    {
        Node.OnClick -= OpenPopupMenu;
        Node.OnRightClick -= OpenTeleportWindow;
    }

    protected override void OnDraw()
    {
        SetDisabled(!Player.CanUseTeleportAction);
        SetText(TeleportName);
    }

    private ExpansionListPosition GetExpansionMenuPosition()
    {
        return GetConfigValue<ExpansionListPosition>("ExpansionListPosition") switch {
            ExpansionListPosition.Auto  => Node.ParentNode!.Id == "Right" ? ExpansionListPosition.Right : ExpansionListPosition.Left,
            ExpansionListPosition.Left  => ExpansionListPosition.Left,
            ExpansionListPosition.Right => ExpansionListPosition.Right,
            _                           => ExpansionListPosition.Left
        };
    }

    private void OpenPopupMenu(Node _)
    {
        Popup.ReverseCondensedElements = GetExpansionMenuPosition() == ExpansionListPosition.Right;
    }
    
    private void OpenTeleportWindow(Node _)
    {
        Player.UseGeneralAction(7);
    }
}
