using Dalamud.Interface;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game.CustomDeliveries;
using Una.Drawing;

namespace Umbra.Widgets.Library.CustomDeliveries;

internal sealed partial class CustomDeliveriesPopup
{
    protected override Node Node { get; } = new() {
        Id         = "Popup",
        Stylesheet = Stylesheet,
        ChildNodes = [
            new() {
                Id        = "AllowanceStatus",
                NodeValue = I18N.Translate("Widget.CustomDeliveries.AllowanceStatus", "12"),
                SortIndex = int.MinValue,
            }
        ],
    };

    private void CreateOrUpdateNpcNode(CustomDeliveryNpc npc)
    {
        Node? node = Node.QuerySelector($"#Npc_{npc.Id}");

        if (node != null) {
            node.QuerySelector(".npc-body--hearts")!.ChildNodes = new(CreateHearts(npc));

            node.QuerySelector(".npc-body--count")!.NodeValue =
                $"{npc.DeliveriesThisWeek} / {npc.MaxDeliveriesPerWeek}";

            node.QuerySelector(".npc-bonus")!.ChildNodes = new(CreateBonus(npc));
            
            return;
        }

        node = new() {
            Id        = $"Npc_{npc.Id}",
            ClassList = ["npc"],
            SortIndex = -npc.Id,
            ChildNodes = [
                new() {
                    ClassList   = ["npc-icon"],
                    Style       = new() { IconId = npc.IconId },
                    InheritTags = true,
                },
                new() {
                    ClassList   = ["npc-body"],
                    InheritTags = true,
                    ChildNodes = [
                        new() {
                            ClassList = ["npc-body--count"],
                            NodeValue = $"{npc.DeliveriesThisWeek} / {npc.MaxDeliveriesPerWeek}"
                        },
                        new() {
                            ClassList   = ["npc-body--name"],
                            InheritTags = true,
                            NodeValue   = $"{npc.Name}",
                        },
                        new() {
                            ClassList   = ["npc-body--hearts"],
                            InheritTags = true,
                            ChildNodes  = [
                                ..CreateHearts(npc)
                            ]
                        }
                    ]
                },
                new () {
                    ClassList   = ["npc-bonus"],
                    InheritTags = true,
                    ChildNodes = [
                        ..CreateBonus(npc)
                    ]
                }
            ]
        };

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

        Node.AppendChild(node);
    }

    private static List<Node> CreateHearts(CustomDeliveryNpc npc)
    {
        var hearts = new List<Node>();

        for (var i = 0; i < 5; i++) {
            hearts.Add(
                new() {
                    ClassList = ["npc-body--hearts--heart"],
                    TagsList  = [npc.HeartCount > i ? "filled" : "empty"],
                    NodeValue = FontAwesomeIcon.Heart.ToIconString()
                }
            );
        }

        return hearts;
    }

    private static List<Node> CreateBonus(CustomDeliveryNpc npc)
    {
        return npc.BonusType.Select(t => new Node {
            ClassList   = ["npc-bonus--bonus"], 
            Style       = new() { UldPartId = t }, 
            InheritTags = true,
        }).ToList();
    }
}
