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
        StandardWidgetFeatures.SubText |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    protected override string DefaultIconType   => IconTypeGameIcon;
    protected override uint   DefaultGameIconId => 60453;
    protected override bool   DefaultShowSubText => false;

    private IPlayer Player { get; } = Framework.Service<IPlayer>();

    private string TeleportName { get; set; } = null!;
    private string AetheryteTicketName { get; set; } = null!;
    private static uint AetheryteTicketItemId => 7569;

    /// <inheritdoc/>
    protected override void OnLoad()
    {
        var dataManager = Framework.Service<IDataManager>();
        TeleportName = dataManager.GetExcelSheet<GeneralAction>().GetRow(7).Name.ToString();
        AetheryteTicketName = dataManager.GetExcelSheet<Item>().GetRow(AetheryteTicketItemId).Name.ToString();

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
        var text = $"{AetheryteTicketName}: x{Player.GetItemCount(AetheryteTicketItemId)}";
        SetSubText(text);
        SetTooltip(text);
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
