using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

internal sealed partial class UnifiedMainMenuPopup
{
    private Node SidePanelNode  => Node.QuerySelector("#side-panel")!;
    private Node ContentsNode   => Node.QuerySelector("#contents")!;
    private Node PinnedListNode => Node.QuerySelector("#pinned")!;

    private MainMenuItem? SelectedMenuItem { get; set; }

    private void CreateSidePanelNodes()
    {
        SidePanelNode.Clear();
        SidePanelNode.Style.FlowOrder = IsTopAligned ? FlowOrder.Normal : FlowOrder.Reverse;

        foreach (var category in MainMenuRepository.GetCategories()) {
            SidePanelNode.AppendChild(CreateCategoryButton($"Category_{category.Category.ToString()}", category.Name, category.GetIconId()));
        }

        SidePanelNode.AppendChild(new() {
            Id        = "pinned",
            ClassList = ["pinned-list"]
        });
    }

    private void CreateContentNodes()
    {
        ContentsNode.Clear();
        
        foreach (var category in MainMenuRepository.GetCategories()) {
            Node categoryNode = new() {
                Id        = $"Category_{category.Category.ToString()}",
                ClassList = ["category"]
            };

            categoryNode.Style.IsVisible = false;

            ContentsNode.AppendChild(categoryNode);

            foreach (var button in category.Items) {
                categoryNode.AppendChild(CreateContentButton(button));
            }
        }
    }

    private Node CreateCategoryButton(string id, string label, object? icon)
    {
        Node iconNode = new() { ClassList = ["icon"] };
        Node nameNode = new() { ClassList = ["name"], NodeValue = label };
        Node node = new() {
            Id         = $"{id}_Button",
            ClassList  = ["category-button"],
            ChildNodes = [iconNode, nameNode]
        };

        SetNodeIcon(iconNode, icon, null);

        node.OnClick += _ => ActivateCategory(id);
        node.OnMouseEnter += _ => {
            if (OpenSubMenusOnHover) ActivateCategory(id);
        };

        return node;
    }

    private Node CreateContentButton(MainMenuItem item, bool isSortable = false)
    {
        Node node = new() { ClassList = ["item"] };

        switch (item.Type) {
            case MainMenuItemType.Separator:
                node.ToggleClass("separator", true);
                break;
            default:
                node.AppendChild(new() { ClassList = ["icon"] });
                node.AppendChild(new() { ClassList = ["text"], NodeValue = item.Name });
                node.AppendChild(new() { ClassList = ["alt"], NodeValue  = item.ShortKey });

                SetNodeIcon(node.QuerySelector(".icon")!, item.Icon, item.IconColor);
                break;
        }

        node.OnMouseUp += _ => {
            item.Invoke();
            Close();
        };

        node.OnRightClick += _ => {
            SelectedMenuItem = item;
            ContextMenu?.SetEntryVisible("MoveUp", isSortable);
            ContextMenu?.SetEntryVisible("MoveDown", isSortable);
            ContextMenu?.SetEntryDisabled("MoveUp", isSortable && PinnedItems.IndexOf(item.Id) == 0);
            ContextMenu?.SetEntryDisabled("MoveDown", isSortable && PinnedItems.IndexOf(item.Id) == PinnedItems.Count - 1);
            ContextMenu?.SetEntryVisible("Pin", !PinnedItems.Contains(item.Id));
            ContextMenu?.SetEntryVisible("Unpin", PinnedItems.Contains(item.Id));
            ContextMenu?.Present();
        };

        return node;
    }

    private void SetNodeIcon(Node node, object? icon, uint? iconColor)
    {
        switch (icon) {
            case uint iconId:
                node.Style.IconId         = iconId;
                node.Style.BitmapFontIcon = null;
                node.Style.Font           = 0;
                node.NodeValue            = null;
                break;
            case SeIconChar iconChar:
                node.Style.IconId         = null;
                node.Style.BitmapFontIcon = null;
                node.Style.Font           = 0;
                node.NodeValue            = iconChar.ToIconString();
                break;
            case FontAwesomeIcon faIcon:
                node.Style.IconId         = null;
                node.Style.BitmapFontIcon = null;
                node.Style.Font           = 0;
                node.NodeValue            = faIcon.ToIconString();
                break;
            case BitmapFontIcon bitmapFontIcon:
                node.Style.IconId         = null;
                node.Style.BitmapFontIcon = bitmapFontIcon;
                node.Style.Font           = 0;
                node.NodeValue            = null;
                break;
            default:
                node.Style.IconId         = 0;
                node.Style.BitmapFontIcon = null;
                node.Style.Font           = 0;
                node.NodeValue            = null;
                break;
        }

        Color? color = iconColor.HasValue ? new(iconColor.Value) : null;

        node.Style.Color      = color;
        node.Style.ImageColor = color;
    }

    private void UpdatePinnedItems()
    {
        PinnedListNode.ToggleClass("top", IsTopAligned);
        
        if (PinnedItems.Count == 0) {
            Logger.Info("No pinned items found.");
            PinnedListNode.Style.IsVisible = false;
            return;
        }

        PinnedListNode.Style.IsVisible = true;
        Logger.Info($"Pinned items found : {PinnedItems.Count}");

        int sortIndex = 0;

        foreach (var id in PinnedItems.ToArray()) {
            sortIndex++;

            Node? node = PinnedListNode.QuerySelector($"#Pin_{id}");

            if (node == null) {
                node = CreatePinnedItem(id, sortIndex);

                if (null == node) {
                    Logger.Warning($"Detected an invalid menu item id in pinned items: {id}");
                    PinnedItems.Remove(id);
                    continue;
                }

                node.ToggleClass("pinned", true);
                
                PinnedListNode.AppendChild(node);
                continue;
            }

            node.SortIndex = sortIndex;
        }
    }

    private Node? CreatePinnedItem(string id, int sortIndex)
    {
        MainMenuItem? entry = MainMenuRepository.FindById(id);
        if (entry == null) return null;

        Node node = CreateContentButton(entry, true);
        node.Id        = $"Pin_{id}";
        node.SortIndex = sortIndex;

        return node;
    }
}
