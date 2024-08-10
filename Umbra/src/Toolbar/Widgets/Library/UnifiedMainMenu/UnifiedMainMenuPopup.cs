using Dalamud.Game.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

internal sealed partial class UnifiedMainMenuPopup : WidgetPopup
{
    public event Action<List<string>>? OnPinnedItemsChanged;

    public int          MenuHeight          { get; set; }
    public uint         AvatarIconId        { get; set; }         = 76985;
    public string       BannerLocation      { get; set; }         = "Auto";
    public string       BannerNameStyle     { get; set; }         = "FirstName";
    public string       BannerColorStyle    { get; set; }         = "AccentColor";
    public bool         DesaturateIcons     { get; set; }         = false;
    public bool         OpenSubMenusOnHover { get; set; }         = false;
    public List<string> PinnedItems         { get; private set; } = [];

    private IMainMenuRepository MainMenuRepository { get; } = Framework.Service<IMainMenuRepository>();
    private IPlayer             Player             { get; } = Framework.Service<IPlayer>();

    private MainMenuItem? SelectedMenuItem { get; set; }

    public UnifiedMainMenuPopup()
    {
        foreach (var category in MainMenuRepository.GetCategories()) {
            Node node = CreateCategory($"Category_{category.Category}", category.GetIconId(), category.Name);

            node.OnMouseUp += _ => ActivateCategory(category);

            node.OnMouseEnter += _ => {
                if (OpenSubMenusOnHover) ActivateCategory(category);
            };

            CategoryListNode.AppendChild(node);
            EntriesNode.AppendChild(CreateEntriesForCategory(category));

            category.OnItemAdded   += OnCategoryItemAdded;
            category.OnItemRemoved += OnCategoryItemRemoved;
        }

        ActivateCategory(MainMenuRepository.GetCategory(MenuCategory.Character));

        ContextMenu = new(
            [
                new("MoveUp") {
                    Label   = I18N.Translate("Widget.UnifiedMainMenu.ContextMenu.MoveUp"),
                    OnClick = () => ContextMoveAction(-1),
                    IconId  = 60541u,
                },
                new("MoveDown") {
                    Label   = I18N.Translate("Widget.UnifiedMainMenu.ContextMenu.MoveDown"),
                    OnClick = () => ContextMoveAction(1),
                    IconId  = 60545u,
                },
                new("Pin") {
                    Label   = I18N.Translate("Widget.UnifiedMainMenu.ContextMenu.Pin"),
                    OnClick = ContextPinAction,
                },
                new("Unpin") {
                    Label   = I18N.Translate("Widget.UnifiedMainMenu.ContextMenu.Unpin"),
                    OnClick = ContextUnpinAction,
                },
            ]
        );
    }

    public void SetPinnedItems(List<string> pinnedItems)
    {
        PinnedItems = pinnedItems;
        UpdatePinnedItems();
    }

    protected override void OnDisposed()
    {
        foreach (var category in MainMenuRepository.GetCategories()) {
            category.OnItemAdded   -= OnCategoryItemAdded;
            category.OnItemRemoved -= OnCategoryItemRemoved;
        }

        base.OnDisposed();
    }

    protected override void OnOpen()
    {
        JobInfo job = Player.GetJobInfo(Player.JobId);

        bool isTop = BannerLocation == "Top" || (BannerLocation == "Auto" && !Toolbar.IsTopAligned);

        HeaderNode.SortIndex          = !isTop ? 1000 : -1000;
        HeaderLabelNameNode.NodeValue = GetPlayerName();
        HeaderLabelInfoNode.NodeValue = $"{I18N.Translate("Widget.GearsetSwitcher.JobLevel", job.Level)} {job.Name}";

        Node.TagsList.Remove(!isTop ? "bottom" : "top");
        Node.TagsList.Add(!isTop ? "top" : "bottom");

        UpdateBannerColor();

        // Find the max height of the popup.
        if (MenuHeight == 0) {
            int maxHeight = (CategoryListNode.ChildNodes.Count * 38) + (CategoryListNode.ChildNodes.Count * 28) + 32;

            maxHeight = EntriesNode
                .ChildNodes.Select(
                    entriesList => entriesList.ChildNodes
                            .Sum(n => (n.Style.Size?.Height ?? 28) + 4)
                        + 8
                )
                .Prepend(maxHeight)
                .Max();

            CategoriesNode.Style.Size = new(CategoriesWidth, maxHeight);
            EntriesNode.Style.Size    = new(EntriesWidth, maxHeight);
        } else {
            CategoriesNode.Style.Size = new(CategoriesWidth, MenuHeight);
            EntriesNode.Style.Size    = new(EntriesWidth, MenuHeight);
        }

        HeaderIconNode.Style.IconId = AvatarIconId;
    }

    protected override void OnUpdate()
    {
        foreach (var category in MainMenuRepository.GetCategories()) {
            foreach (var entry in category.Items) {
                if (entry.Type == MainMenuItemType.Separator) continue;
                Node? node1 = EntriesNode.QuerySelector($"#{entry.Id}");
                Node? node2 = PinnedListNode.QuerySelector($"#Pin_{entry.Id}");

                if (node1 == null) {
                    if (PinnedItems.Contains(entry.Id)) {
                        PinnedItems.Remove(entry.Id);
                        UpdatePinnedItems();
                    }

                    continue;
                }

                if (node2 != null) {
                    node2.IsDisabled                                                 = entry.IsDisabled;
                    node2.QuerySelector(".pinned-entry--icon")!.Style.ImageGrayscale = DesaturateIcons;
                }

                node1.IsDisabled                                          = entry.IsDisabled;
                node1.QuerySelector(".entry--info")!.NodeValue            = entry.ShortKey;
                node1.QuerySelector(".entry--icon")!.Style.ImageGrayscale = DesaturateIcons;
            }
        }

        foreach (Node node in Node.QuerySelectorAll(".category--icon")) {
            node.Style.ImageGrayscale = DesaturateIcons;
        }
    }

    private void ActivateCategory(MainMenuCategory category)
    {
        foreach (Node node in CategoryListNode.ChildNodes) {
            node.TagsList.Remove("selected");
        }

        foreach (Node node in EntriesNode.ChildNodes) {
            node.Style.IsVisible = false;
        }

        CategoryListNode.ChildNodes.First(n => n.Id == $"Category_{category.Category}").TagsList.Add("selected");
        EntriesNode.QuerySelector($"Category_{category.Category}")!.Style.IsVisible = true;
    }

    private void OnCategoryItemAdded(MainMenuItem entry)
    {
        Node target = EntriesNode.QuerySelector($"Category_{entry.Category!.Category}")!;
        Node node   = CreateMainMenuEntry(entry);

        if (entry.Type == MainMenuItemType.Separator) {
            target.AppendChild(node);
            return;
        }

        entry.OnUpdate += () => {
            uint?   iconId  = entry.Icon is uint icon ? icon : null;
            string? iconStr = entry.Icon is SeIconChar iconChar ? iconChar.ToIconString() : null;

            Node iconNode = node.QuerySelector(".entry--icon")!;
            Node nameNode = node.QuerySelector(".entry--name")!;
            Node infoNode = node.QuerySelector(".entry--info")!;

            node.IsDisabled       = entry.IsDisabled;
            iconNode.Style.IconId = iconId;
            iconNode.Style.Color  = entry.IconColor != null ? new(entry.IconColor.Value) : null;
            iconNode.NodeValue    = iconStr;
            nameNode.NodeValue    = entry.Name;
            infoNode.NodeValue    = entry.ShortKey;
        };

        target.AppendChild(node);
    }

    private void OnCategoryItemRemoved(MainMenuItem item)
    {
        Node target = EntriesNode.QuerySelector($"Category_{item.Category!.Category}")!;
        Node node   = target.ChildNodes.First(n => n.Id == item.Id);

        target.RemoveChild(node);
    }

    private void ContextPinAction()
    {
        if (null == SelectedMenuItem) return;

        PinnedItems.Add(SelectedMenuItem.Id);
        UpdatePinnedItems();

        OnPinnedItemsChanged?.Invoke(PinnedItems);
    }

    private void ContextUnpinAction()
    {
        if (null == SelectedMenuItem) return;

        PinnedItems.Remove(SelectedMenuItem.Id);

        Node? node = PinnedListNode.QuerySelector($"#Pin_{SelectedMenuItem.Id}");
        if (null != node) PinnedListNode.RemoveChild(node);

        UpdatePinnedItems();

        OnPinnedItemsChanged?.Invoke(PinnedItems);
    }

    private void ContextMoveAction(int direction)
    {
        if (null == SelectedMenuItem) return;

        int index = PinnedItems.IndexOf(SelectedMenuItem.Id);

        if (index == -1) return;

        PinnedItems.RemoveAt(index);
        PinnedItems.Insert(index + direction, SelectedMenuItem.Id);

        UpdatePinnedItems();

        OnPinnedItemsChanged?.Invoke(PinnedItems);
    }

    private void UpdatePinnedItems()
    {
        var sortIndex = 0;

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

                PinnedListNode.AppendChild(node);
                continue;
            }

            node.SortIndex = sortIndex;
        }
    }

    private string GetPlayerName()
    {
        string[] name = Player.Name.Split(' ');

        return BannerNameStyle switch {
            "FirstName" => $"{name[0]}",
            "LastName"  => $"{name[1]}",
            "FullName"  => $"{name[0]} {name[1]}",
            "Initials"  => $"{name[0][0]}. {name[1][0]}.",
            _           => Player.Name
        };
    }

    private void UpdateBannerColor()
    {
        Color? color = new(0);

        switch (BannerColorStyle) {
            case "AccentColor":
                color = new("Window.AccentColor");
                break;
            case "RoleColor":
                color = new(Player.GetJobInfo(Player.JobId).ColorName);
                break;
            case "None":
                color = new(0);
                break;
        }

        bool isTop = HeaderNode.TagsList.Contains("top");

        HeaderNode.Style.BackgroundGradient = GradientColor.Vertical(
            isTop ? null : color,
            isTop ? color : null
        );
    }
}
