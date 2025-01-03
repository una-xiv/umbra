using System;
using System.Linq;
using Umbra.Common;
using Umbra.Game.CustomDeliveries;

namespace Umbra.Widgets.Library.CustomDeliveries;

internal sealed partial class CustomDeliveriesPopup : WidgetPopup
{
    public Action<int>? OnNpcSelected;
    public int          TrackedNpcId  { get; set; }
    public string       PrimaryAction { get; set; } = "Teleport";

    private ICustomDeliveriesRepository Repository { get; } = Framework.Service<ICustomDeliveriesRepository>();

    private int _selectedNpcId;

    protected override void OnOpen()
    {
        Node.FindById("AllowanceStatus")!.NodeValue = I18N.Translate(
            "Widget.CustomDeliveries.AllowanceStatus",
            Repository.DeliveriesRemainingThisWeek
        );

        int maxBonus  = 0;
        int menuWidth = 200;

        foreach (var npc in Repository.Npcs.Values) {
            CreateOrUpdateNpcNode(npc);
            maxBonus = Math.Max(maxBonus, npc.BonusType.Length);
        }

        menuWidth += (maxBonus > 0 ? 8 + (maxBonus * 42) : 0);

        Node.QuerySelector("#AllowanceStatus")!.Style.Size = new(menuWidth, 0);
        foreach (var node in Node.QuerySelectorAll(".npc")) {
            node.Style.Size = new(menuWidth, 0);
        }

        ContextMenu = new(
            [
                new("Track") {
                    Label = I18N.Translate("Widget.CustomDeliveries.ContextMenu.Track"),
                    OnClick = () => {
                        if (_selectedNpcId == 0) return;
                        OnNpcSelected?.Invoke(_selectedNpcId);
                        Close();
                    }
                },
                new("Untrack") {
                    Label = I18N.Translate("Widget.CustomDeliveries.ContextMenu.Untrack"),
                    OnClick = () => {
                        if (_selectedNpcId == 0) return;
                        OnNpcSelected?.Invoke(0);
                        Close();
                    }
                },
                new("OpenWindow") {
                    Label = I18N.Translate("Widget.CustomDeliveries.ContextMenu.OpenWindow"),
                    OnClick = () => {
                        if (_selectedNpcId == 0) return;
                        Repository.OpenCustomDeliveriesWindow(_selectedNpcId);
                        Close();
                    }
                },
                new("Teleport") {
                    Label  = I18N.Translate("Widget.CustomDeliveries.ContextMenu.Teleport"),
                    IconId = 60453u,
                    OnClick = () => {
                        if (_selectedNpcId == 0) return;
                        Repository.TeleportToNearbyAetheryte(_selectedNpcId);
                        Close();
                    }
                }
            ]
        );
    }

    protected override void OnClose() { }
}
