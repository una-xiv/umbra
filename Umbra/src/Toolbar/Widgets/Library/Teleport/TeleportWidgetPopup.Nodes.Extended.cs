using Dalamud.Game.Text;
using System.Collections.Immutable;
using Umbra.Widgets.Popup;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    private Node ExtendedInterfaceNode => Node.QuerySelector("#extended-ui")!;

    private Node FavoritesNode { get; set; } = null!;

    private void BuildExtendedInterface()
    {
        // Changing an attached node invalidates the popup layout.
        foreach (var expansion in _expansions.Values) {
            var expansionNode = Document.CreateNodeFromTemplate("extended-expansion", new() { { "label", expansion.Name } });
            var expansionList = expansionNode.QuerySelector(".list")!;

            expansionNode.SortIndex = expansion.SortIndex;

            foreach (var region in expansion.Regions.Values) {
                var regionNode = Document.CreateNodeFromTemplate("extended-region", new() { { "label", region.Name } });
                var regionList = regionNode.QuerySelector(".list")!;

                foreach (var destinations in region.Maps.Values) {
                    foreach (var destination in destinations.Destinations.Values) {
                        regionList.AppendChild(CreateTeleportButton(destination, false));
                    }
                }

                expansionList.AppendChild(regionNode);
            }

            ExtendedInterfaceNode.AppendChild(expansionNode);
        }

        // Build "other" section.
        ExtendedInterfaceNode.AppendChild(new() { ClassList = ["separator"], SortIndex = int.MaxValue - 1 });
        var otherNode = Document.CreateNodeFromTemplate("extended-expansion", new() {
            { "label", I18N.Translate("Widget.Teleport.Other") }
        });

        otherNode.Id        = "other";
        otherNode.SortIndex = int.MaxValue;
        otherNode.ToggleClass("other", true);

        var miscNode = Document.CreateNodeFromTemplate("extended-region", new() {
            { "label", I18N.Translate("Widget.Teleport.Misc") }
        });

        FavoritesNode = Document.CreateNodeFromTemplate("extended-region", new() {
            { "label", I18N.Translate("Widget.Teleport.Favorites") }
        });

        var otherList = otherNode.QuerySelector(".list")!;
        otherList.AppendChild(miscNode);
        otherList.AppendChild(FavoritesNode);

        var miscList = miscNode.QuerySelector(".list")!;

        foreach (MainMenuItem item in GetTravelMenuItems()) {
            var menuItemNode = Document.CreateNodeFromTemplate("extended-teleport");

            menuItemNode.Id        =  item.Id;
            menuItemNode.SortIndex =  item.SortIndex;
            menuItemNode.OnClick   += _ => { item.Invoke(); Close(); };

            ApplyMenuItem(menuItemNode, item);
            _menuItemNodes.Add((menuItemNode, item));

            miscList.AppendChild(menuItemNode);
        }

        BuildExtendedFavorites();

        ExtendedInterfaceNode.AppendChild(otherNode);
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
        node.QuerySelector(".cost")!.NodeValue       = FormatGilCost(destination);

        RegisterCostNode(node, destination);

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
