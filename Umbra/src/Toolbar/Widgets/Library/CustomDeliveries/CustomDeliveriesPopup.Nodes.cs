using Dalamud.Interface;
using System.Collections.Generic;
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
                SortIndex = -1,
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

            return;
        }

        node = new() {
            Id        = $"Npc_{npc.Id}",
            ClassList = ["npc"],
            SortIndex = npc.Id,
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
                            ChildNodes = [
                                ..CreateHearts(npc)
                            ]
                        },
                    ]
                },
            ]
        };

        node.OnMouseUp += _ => OnNpcSelected?.Invoke(npc.Id);

        node.OnRightClick += _ => {
            _selectedNpcId = npc.Id;
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
}
