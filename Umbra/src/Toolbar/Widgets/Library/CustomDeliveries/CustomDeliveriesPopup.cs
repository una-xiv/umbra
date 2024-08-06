using System;
using Umbra.Common;
using Umbra.Game.CustomDeliveries;
using Una.Drawing;

namespace Umbra.Widgets.Library.CustomDeliveries;

internal sealed partial class CustomDeliveriesPopup : WidgetPopup
{
    public Action<int>? OnNpcSelected;

    private ICustomDeliveriesRepository Repository { get; } = Framework.Service<ICustomDeliveriesRepository>();

    private int _selectedNpcId;

    protected override void OnOpen()
    {
        foreach (var npc in Repository.Npcs.Values) {
            CreateOrUpdateNpcNode(npc);
        }

        ContextMenu = new(
            [
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
