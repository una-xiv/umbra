using System;
using Umbra.Common;
using Umbra.Game.CustomDeliveries;
using Una.Drawing;

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

        foreach (var npc in Repository.Npcs.Values) {
            CreateOrUpdateNpcNode(npc);
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
