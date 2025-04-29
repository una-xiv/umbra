using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game.CustomDeliveries;
using Una.Drawing;

namespace Umbra.Widgets.Library.CustomDeliveries;

internal sealed partial class CustomDeliveriesPopup : WidgetPopup
{
    public Action<int>? OnNpcSelected;
    public int          TrackedNpcId  { get; set; }
    public string       PrimaryAction { get; set; } = "Teleport";

    protected override Node Node { get; }

    private ICustomDeliveriesRepository Repository { get; } = Framework.Service<ICustomDeliveriesRepository>();

    private UdtDocument Document { get; } = UmbraDrawing.DocumentFrom("umbra.widgets.popup_custom_deliveries.xml");

    private int _selectedNpcId;

    public CustomDeliveriesPopup()
    {
        Node = Document.RootNode!;
        CreateContextMenu();
    }

    protected override void OnOpen()
    {
        Node.QuerySelector(".header > .text")!.NodeValue = I18N.Translate(
            "Widget.CustomDeliveries.AllowanceStatus",
            Repository.DeliveriesRemainingThisWeek
        );

        Node.QuerySelector(".list")!.Clear();
        
        foreach (var npc in Repository.Npcs.Values) {
            CreateOrUpdateNpcNode(npc);
        }
    }

    protected override void OnClose() { }

    private void CreateOrUpdateNpcNode(CustomDeliveryNpc npc)
    {
        Node node = Document.CreateNodeFromTemplate("npc", new() {
            { "name", npc.Name },
            { "icon", npc.IconId.ToString() }
        });

        for (int i = 0; i < 5; i++) {
            Node heart = node.QuerySelector($".heart{i}")!;
            heart.ToggleClass("empty", i > npc.HeartCount - 1);
            heart.ToggleClass("filled", i <= npc.HeartCount - 1);
        }

        foreach (var bonusNode in CreateBonus(npc)) {
            node.QuerySelector(".bonus-icons")!.AppendChild(bonusNode);
        }
        
        node.OnMouseUp += _ => {
            switch (PrimaryAction) {
                case "Track":
                    OnNpcSelected?.Invoke(npc.Id);
                    break;
                case "Teleport":
                    Repository.TeleportToNearbyAetheryte(npc.Id);
                    break;
                case "OpenWindow":
                    Repository.OpenCustomDeliveriesWindow(npc.Id);
                    break;
            }

            Close();
        };

        node.OnRightClick += _ => {
            _selectedNpcId = npc.Id;
            ContextMenu?.SetEntryDisabled("Track", TrackedNpcId == npc.Id);
            ContextMenu?.SetEntryDisabled("Untrack", TrackedNpcId != npc.Id);
            ContextMenu?.Present();
        };
        
        Node.QuerySelector(".list")!.AppendChild(node);
    }
    
    private static List<Node> CreateBonus(CustomDeliveryNpc npc)
    {
        return npc.BonusType.Select(t => new Node {
            ClassList   = ["bonus-icon"], 
            Style       = new() { UldPartId = t }, 
            InheritTags = true,
        }).ToList();
    }
}
