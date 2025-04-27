using Dalamud.Game.Text;
using System.Collections.Immutable;
using Umbra.Common;
using Umbra.Game;
using Umbra.Widgets.Popup;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    private Node ExtendedInterfaceNode  => Node.QuerySelector("#extended-ui")!;

    private Node FavoritesNode { get; set; } = null!;

    private void BuildExtendedInterface()
    {
        foreach (var expansion in _expansions.Values) {
            var expansionNode = Document.CreateNodeFromTemplate("extended-expansion", new() { { "label", expansion.Name } });

            expansionNode.SortIndex = expansion.SortIndex;

            ExtendedInterfaceNode.AppendChild(expansionNode);

            foreach (var region in expansion.Regions.Values) {
                var regionNode = Document.CreateNodeFromTemplate("extended-region", new() { { "label", region.Name } });

                expansionNode.QuerySelector(".list")!.AppendChild(regionNode);

                foreach (var destinations in region.Maps.Values) {
                    foreach (var destination in destinations.Destinations.Values) {
                        regionNode.QuerySelector(".list")!.AppendChild(CreateTeleportButton(destination, false));
                    }
                }
            }
        }

        // Build "other" section.
        ExtendedInterfaceNode.AppendChild(new() { ClassList = ["separator"], SortIndex = int.MaxValue - 1 });
        var otherNode = Document.CreateNodeFromTemplate("extended-expansion", new() {
            { "label", I18N.Translate("Widget.Teleport.Other") }
        });

        otherNode.Id        = "other";
        otherNode.SortIndex = int.MaxValue;
        otherNode.ToggleClass("other", true);
        ExtendedInterfaceNode.AppendChild(otherNode);

        var miscNode = Document.CreateNodeFromTemplate("extended-region", new() {
            { "label", I18N.Translate("Widget.Teleport.Misc") }
        });

        FavoritesNode = Document.CreateNodeFromTemplate("extended-region", new() {
            { "label", I18N.Translate("Widget.Teleport.Favorites") }
        });

        otherNode.QuerySelector(".list")!.AppendChild(miscNode);
        otherNode.QuerySelector(".list")!.AppendChild(FavoritesNode);

        foreach (MainMenuItem item in Framework.Service<IMainMenuRepository>().GetCategory(MenuCategory.Travel).Items) {
            switch (item.Type) {
                case MainMenuItemType.MainCommand when item.CommandId != 36:
                case MainMenuItemType.Separator:
                    continue;
                case MainMenuItemType.ChatCommand:
                case MainMenuItemType.Callback:
                default: {
                    var menuItemNode = Document.CreateNodeFromTemplate("extended-teleport");

                    menuItemNode.Id                                = item.Id;
                    menuItemNode.SortIndex                         = item.SortIndex;
                    menuItemNode.QuerySelector(".text")!.NodeValue = item.Name;
                    menuItemNode.QuerySelector(".cost")!.NodeValue = item.ShortKey;

                    switch (item.Icon) {
                        case uint iconId:
                            menuItemNode.QuerySelector(".icon")!.Style.IconId = iconId;
                            menuItemNode.QuerySelector(".icon")!.NodeValue    = null;
                            break;
                        case SeIconChar seIconChar:
                            menuItemNode.QuerySelector(".icon")!.Style.IconId = null;
                            menuItemNode.QuerySelector(".icon")!.Style.Color  = item.IconColor != null ? new(item.IconColor.Value) : null;
                            menuItemNode.QuerySelector(".icon")!.NodeValue    = seIconChar.ToIconString();
                            break;
                    }

                    menuItemNode.SortIndex =  item.SortIndex;
                    menuItemNode.OnClick   += _ => item.Invoke();

                    miscNode.QuerySelector(".list")!.AppendChild(menuItemNode);
                    break;
                }
            }
        }

        if (Favorites.Count == 0) return;
        BuildExtendedFavorites();
    }

    private Node CreateTeleportButton(TeleportDestination destination, bool isSortable)
    {
        Node node = new() {
            Id        = isSortable ? $"Sortable_{destination.NodeId}" : destination.NodeId,
            ClassList = ["teleport", "uld"],
            SortIndex = destination.SortIndex,
            ChildNodes = [
                new() { ClassList = ["icon"] },
                new() { ClassList = ["text"] },
                new() { ClassList = ["cost"] },
            ]
        };

        node.SortIndex = Favorites.IndexOf(destination.NodeId);

        node.ToggleClass("uld", true);
        node.QuerySelector(".icon")!.Style.UldPartId = destination.UldPartId;
        node.QuerySelector(".text")!.NodeValue       = destination.Name;
        node.QuerySelector(".cost")!.NodeValue       = $"{SeIconChar.Gil.ToIconChar()} {I18N.FormatNumber(destination.GilCost)}";

        node.OnClick      += _ => Teleport(destination);
        node.OnRightClick += _ => OpenContextMenu(destination, isSortable);

        return node;
    }

    private void BuildExtendedFavorites()
    {
        if (Favorites.Count == 0) {
            FavoritesNode.Style.IsVisible = false;
            return;
        }

        FavoritesNode.Style.IsVisible = true;

        foreach (var favoriteId in Favorites) {
            if (_destinations.TryGetValue(favoriteId, out var destination)) {
                ExtendedBuildFavoritesButton(destination);
            }
        }
    }

    private void ExtendedBuildFavoritesButton(TeleportDestination destination)
    {
        FavoritesNode.Style.IsVisible = true;

        var target = FavoritesNode.QuerySelector(".list");
        if (null == target) {
            Logger.Warning("Target node is unavailable.");
            return;
        }

        var button = CreateTeleportButton(destination, true);

        if (FavoritesNode.QuerySelector($"#{button.Id}") != null) {
            Logger.Warning($"This button is already added! (ID: {button}.Id}})");
            return;
        }

        target.AppendChild(button);
    }

    private void ExtendedRemoveFavoritesButton(TeleportDestination destination)
    {
        FavoritesNode.QuerySelector($"#Sortable_{destination.NodeId}")?.Dispose();

        if (Favorites.Count == 0) {
            FavoritesNode.Style.IsVisible = false;
        }
    }

    private void ExtendedSortFavorites()
    {
        Node list = FavoritesNode.QuerySelector(".list")!;

        foreach (var favoriteId in Favorites) {
            if (_destinations.TryGetValue(favoriteId, out var destination)) {
                var node = list.QuerySelector($"#Sortable_{destination.NodeId}");
                if (node != null) {
                    node.SortIndex = Favorites.IndexOf(favoriteId);
                }
            }
        }
    }
}
