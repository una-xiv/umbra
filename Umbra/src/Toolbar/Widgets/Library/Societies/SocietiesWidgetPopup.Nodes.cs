using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using System.Linq;
using Umbra.Common;
using Umbra.Game.Societies;
using Una.Drawing;

namespace Umbra.Widgets.Library.Societies;

internal sealed partial class SocietiesWidgetPopup
{
    protected override Node Node { get; } = new() {
        Id         = "Popup",
        Stylesheet = Stylesheet,
        ChildNodes = [
            new() {
                Id        = "AllowanceStatus",
                NodeValue = I18N.Translate("Widget.CustomDeliveries.AllowanceStatus", "12"),
                SortIndex = -1,
                BeforeDraw = node => {
                    var height = node.Height;
                    var width  = node.ParentNode!.Width - 16;
                    node.Bounds.ContentSize = new(width, height);
                    node.Bounds.PaddingSize = new(width, height);
                    node.Bounds.MarginSize  = new(width, height);
                }
            },
            new() {
                Id = "List"
            }
        ]
    };

    private Node GetOrCreateExpansionNodeFor(uint id, string name)
    {
        Node? expansionNode = Node.QuerySelector($"#Expansion_{id} > .expansion--items");

        if (null != expansionNode) {
            return expansionNode;
        }

        expansionNode = new() {
            Id        = $"Expansion_{id}",
            ClassList = ["expansion"],
            ChildNodes = [
                new() {
                    ClassList = ["expansion--label"],
                    NodeValue = name,
                },
                new() {
                    ClassList  = ["expansion--items"],
                    ChildNodes = []
                }
            ]
        };

        Node.FindById("List")!.AppendChild(expansionNode);

        return expansionNode.QuerySelector(".expansion--items")!;
    }

    private Node GetOrCreateSocietyNode(Society society, Node expansionNode)
    {
        Node? node = expansionNode.QuerySelector($"#Society_{society.Id}");

        if (null != node) {
            return node;
        }

        node = new() {
            Id        = $"Society_{society.Id}",
            ClassList = ["society"],
            ChildNodes = [
                new() {
                    ClassList = ["society--icon"],
                    Style     = { IconId = society.IconId },
                },
                new() {
                    ClassList = ["society--body"],
                    ChildNodes = [
                        new() {
                            ClassList = ["society--name"],
                            NodeValue = society.Name,
                        },
                        new() {
                            ClassList = ["society--rank"],
                            NodeValue = $"{society.Rank}",
                            ChildNodes = [
                                new() {
                                    ClassList = ["society--rank--value"],
                                    NodeValue = "0%",
                                },
                            ]
                        },
                        new() {
                            ClassList = ["society--exp-bar"],
                            ChildNodes = [
                                new() { ClassList = ["society--exp-bar--bar"] }
                            ]
                        }
                    ]
                },
                new() {
                    ClassList = ["society--currency"],
                    ChildNodes = [
                        new() {
                            ClassList = ["society--currency--icon"],
                            Style = new()
                                { IconId = DataManager.GetExcelSheet<Item>()!.GetRow(society.CurrencyItemId)!.Icon },
                        },
                        new() {
                            ClassList = ["society--currency--value"],
                            NodeValue = $"{Player.GetItemCount(society.CurrencyItemId)}"
                        }
                    ]
                }
            ]
        };

        expansionNode.AppendChild(node);

        node.OnMouseUp += _ => {
            OnSocietySelected?.Invoke(society);
            Close();
        };

        node.OnRightClick += _ => {
            _selectedSocietyId = society.Id;
            ContextMenu?.Present();
        };

        return node;
    }
}
