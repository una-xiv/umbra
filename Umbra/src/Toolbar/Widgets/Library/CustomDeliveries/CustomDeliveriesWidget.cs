
using Umbra.Game.CustomDeliveries;

namespace Umbra.Widgets.Library.CustomDeliveries;

[ToolbarWidget(
    "CustomDeliveries",
    "Widget.CustomDeliveries.Name",
    "Widget.CustomDeliveries.Description",
    ["delivery", "deliveries", "npc"]
)]
internal sealed class CustomDeliveriesWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override CustomDeliveriesPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    protected override string DefaultIconType   => IconTypeGameIcon;
    protected override uint   DefaultGameIconId => 60927;

    private ICustomDeliveriesRepository Repository { get; } = Framework.Service<ICustomDeliveriesRepository>();

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            
            new BooleanWidgetConfigVariable(
                "ReverseOrder",
                I18N.Translate("Widget.CustomDeliveries.Config.ReverseOrder.Name"),
                I18N.Translate("Widget.CustomDeliveries.Config.ReverseOrder.Description"),
                false
            ),
            
            new StringWidgetConfigVariable(
                "ButtonLabel",
                I18N.Translate("Widget.CustomDeliveries.Config.ButtonLabel.Name"),
                I18N.Translate("Widget.CustomDeliveries.Config.ButtonLabel.Description"),
                Info.Name
            ),
            new SelectWidgetConfigVariable(
                "PrimaryAction",
                I18N.Translate("Widget.CustomDeliveries.Config.PrimaryAction.Name"),
                I18N.Translate("Widget.CustomDeliveries.Config.PrimaryAction.Description"),
                "Teleport",
                new() {
                    { "Track", I18N.Translate("Widget.CustomDeliveries.Config.PrimaryAction.Option.Track") },
                    { "Teleport", I18N.Translate("Widget.CustomDeliveries.Config.PrimaryAction.Option.Teleport") },
                    { "OpenWindow", I18N.Translate("Widget.CustomDeliveries.Config.PrimaryAction.Option.OpenWindow") },
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable("TrackedNpcId", "", null, 0) { IsHidden = true }
        ];
    }

    protected override void OnLoad()
    {
        Popup.OnNpcSelected += OnNpcSelected;

        Node.OnRightClick += _ => {
            bool isShiftOrCtrl = ImGui.GetIO().KeyShift || ImGui.GetIO().KeyCtrl;
            int  trackedNpcId  = GetConfigValue<int>("TrackedNpcId");

            if (!isShiftOrCtrl && trackedNpcId != 0) {
                Repository.TeleportToNearbyAetheryte(trackedNpcId);
                return;
            }

            Repository.OpenCustomDeliveriesWindow(null);
        };
    }

    protected override void OnUnload()
    {
        Popup.OnNpcSelected -= OnNpcSelected;
    }

    protected override void OnDraw()
    {
        int                npcId = GetConfigValue<int>("TrackedNpcId");
        CustomDeliveryNpc? npc   = Repository.Npcs.GetValueOrDefault(npcId);

        Popup.TrackedNpcId  = npcId;
        Popup.PrimaryAction = GetConfigValue<string>("PrimaryAction");

        if (npcId == 0 || npc == null) {
            SetText(GetConfigValue<string>("ButtonLabel"));
            SetSubText(null);
        } else {
            SetText(npc.Name);
            SetSubText($"{npc.DeliveriesThisWeek} / {npc.MaxDeliveriesPerWeek}");
        }

        if (GetConfigValue<string>("DisplayMode") == "IconOnly") {
            Node.Tooltip = npc?.Name;
        }
    }

    private void OnNpcSelected(int npcId)
    {
        SetConfigValue("TrackedNpcId", npcId == GetConfigValue<int>("TrackedNpcId") ? 0 : npcId);
    }
}
