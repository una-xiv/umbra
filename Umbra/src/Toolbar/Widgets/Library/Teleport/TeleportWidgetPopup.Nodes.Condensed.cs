using Dalamud.Game.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Widgets.Popup;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    private Node CondensedInterfaceNode => Node.QuerySelector("#condensed-ui")!;
    private Node CondensedSidePanelNode => CondensedInterfaceNode.QuerySelector(".side-panel")!;
    private Node CondensedContentsNode  => CondensedInterfaceNode.QuerySelector(".contents > .list")!;
    private Node FavoritesButton        => CondensedSidePanelNode.QuerySelector("#Favorites_Button")!;
    private Node FavoritesList          => CondensedContentsNode.QuerySelector("#Favorites_Content > .condensed-region > .list")!;

    private Dictionary<string, Node> ExpansionNodes { get; } = [];

    private void BuildCondensedInterface()
    {
        CondensedInterfaceNode.Clear();
        CondensedInterfaceNode.AppendChild(new() { ClassList = ["side-panel", Toolbar.IsTopAligned ? "top" : "bottom"] });
        CondensedSidePanelNode.AppendChild(new() { ClassList = ["side-panel-spacer"], SortIndex = int.MinValue });

        Node contentsWrapper = new() {
            ClassList  = ["contents"],
            ChildNodes = [new() { ClassList = ["list"] }],
        };

        contentsWrapper.Overflow   = false;
        contentsWrapper.Style.Size = new(300, 400);

        CondensedInterfaceNode.AppendChild(contentsWrapper);
        
        foreach (var expansion in _expansions.Values) {
            BuildCondensedSidePanelExpansionButton(expansion);
            BuildCondensedExpansionContent(expansion);
        }

        CondensedSidePanelNode.AppendChild(new() { ClassList = ["side-panel-separator"], SortIndex = int.MaxValue - 10 });
        BuildMiscellaneousEntries();
        BuildCondensedFavoriteEntries();

        switch (DefaultOpenedGroupName) {
            case "Favorites" when Favorites.Count > 0:
                ActivateExpansion("Favorites");
                break;
            case "Other":
                ActivateExpansion("Other");
                break;
            default:
                ActivateExpansion(_selectedExpansion ?? _expansions.Keys.First());
                break;
        }
    }

    private void BuildCondensedSidePanelExpansionButton(TeleportExpansion expansion)
    {
        Node node = Document.CreateNodeFromTemplate("condensed-side-panel-button", new() {
            { "label", expansion.Name }
        });

        node.Id        = $"{expansion.NodeId}_Button";
        node.SortIndex = expansion.SortIndex;
        node.ToggleClass("selected", _selectedExpansion == expansion.NodeId);

        node.OnClick += _ => ActivateExpansion(expansion.NodeId);
        node.OnMouseEnter += _ => {
            if (OpenCategoryOnHover) {
                ActivateExpansion(expansion.NodeId);
            }
        };

        CondensedSidePanelNode.AppendChild(node);
    }

    private void BuildCondensedExpansionContent(TeleportExpansion expansion)
    {
        Node node = new() { ClassList = ["condensed-expansion"] };

        node.Style.IsVisible = false;
        node.Id              = $"{expansion.NodeId}_Content";
        node.SortIndex       = expansion.SortIndex;

        ExpansionNodes[expansion.NodeId] = node;

        foreach (var region in expansion.Regions.Values) {
            Node regionNode = Document.CreateNodeFromTemplate("condensed-region", new() {
                { "label", region.Name }
            });
            node.AppendChild(regionNode);

            foreach (var map in region.Maps.Values) {
                foreach (var destination in map.Destinations.Values) {
                    Node destinationNode = Document.CreateNodeFromTemplate("condensed-teleport", new() {
                        { "label", destination.Name },
                        { "cost", $"{SeIconChar.Gil.ToIconChar()} {I18N.FormatNumber(destination.GilCost)}" }
                    });

                    regionNode.QuerySelector(".list")!.AppendChild(destinationNode);
                    destinationNode.QuerySelector(".icon")!.Style.UldPartId = destination.UldPartId;

                    destinationNode.OnClick      += _ => Teleport(destination);
                    destinationNode.OnRightClick += _ => OpenContextMenu(destination);
                }
            }
        }

        CondensedContentsNode.AppendChild(node);
    }

    private void BuildMiscellaneousEntries()
    {
        Node button = Document.CreateNodeFromTemplate("condensed-side-panel-button", new() { { "label", I18N.Translate("Widget.Teleport.Misc") } });

        button.Id        = "Other_Button";
        button.SortIndex = int.MaxValue - 2;
        button.ToggleClass("selected", _selectedExpansion == "Other");
        button.OnClick += _ => ActivateExpansion("Other");
        button.OnMouseEnter += _ => {
            if (OpenCategoryOnHover) {
                ActivateExpansion("Other");
            }
        };
        CondensedSidePanelNode.AppendChild(button);

        Node expansionNode = new() { ClassList = ["condensed-expansion"] };

        expansionNode.Style.IsVisible = false;
        expansionNode.Id              = "Other_Content";
        expansionNode.SortIndex       = int.MaxValue - 1;

        CondensedContentsNode.AppendChild(expansionNode);
        ExpansionNodes["Other"] = expansionNode;

        Node regionNode = Document.CreateNodeFromTemplate("condensed-region", new() { { "label", I18N.Translate("Widget.Teleport.Misc") } });
        expansionNode.AppendChild(regionNode);

        foreach (MainMenuItem item in Framework.Service<IMainMenuRepository>().GetCategory(MenuCategory.Travel).Items) {
            switch (item.Type) {
                case MainMenuItemType.MainCommand when item.CommandId != 36:
                case MainMenuItemType.Separator:
                    continue;
                case MainMenuItemType.ChatCommand:
                case MainMenuItemType.Callback:
                default: {
                    var menuItemNode = Document.CreateNodeFromTemplate("condensed-teleport", new() {
                        { "label", item.Name },
                        { "cost", item.ShortKey }
                    });

                    menuItemNode.Id        = item.Id;
                    menuItemNode.SortIndex = item.SortIndex;

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

                    regionNode.QuerySelector(".list")!.AppendChild(menuItemNode);
                    break;
                }
            }
        }
    }

    private void BuildCondensedFavoriteEntries()
    {
        Node button = Document.CreateNodeFromTemplate("condensed-side-panel-button", new() { { "label", I18N.Translate("Widget.Teleport.Favorites") } });

        button.Id        = "Favorites_Button";
        button.SortIndex = int.MaxValue - 2;
        button.ToggleClass("selected", _selectedExpansion == "Favorites");
        button.OnClick += _ => ActivateExpansion("Favorites");
        button.OnMouseEnter += _ => {
            if (OpenCategoryOnHover) {
                ActivateExpansion("Favorites");
            }
        };
        
        CondensedSidePanelNode.AppendChild(button);

        button.Style.IsVisible = Favorites.Count > 0;
 
        Node expansionNode = new() { ClassList = ["condensed-expansion"] };

        expansionNode.Style.IsVisible = false;
        expansionNode.Id              = "Favorites_Content";
        expansionNode.SortIndex       = int.MaxValue - 1;

        CondensedContentsNode.AppendChild(expansionNode);
        ExpansionNodes["Favorites"] = expansionNode;

        Node regionNode = Document.CreateNodeFromTemplate("condensed-region", new() { { "label", I18N.Translate("Widget.Teleport.Favorites") } });
        expansionNode.AppendChild(regionNode);

        foreach (var favorite in Favorites) {
            if (!_destinations.TryGetValue(favorite, out var destination)) continue;
            CondensedBuildFavoritesButton(destination);
        }
    }

    private void CondensedBuildFavoritesButton(TeleportDestination destination)
    {
        FavoritesButton.Style.IsVisible = true;

        Node node = Document.CreateNodeFromTemplate("condensed-teleport", new() {
            { "label", destination.Name },
            { "cost", $"{SeIconChar.Gil.ToIconChar()} {I18N.FormatNumber(destination.GilCost)}" }
        });

        node.Id        = $"SortableCondensed_{destination.NodeId}";
        node.SortIndex = Favorites.IndexOf(destination.NodeId);

        var icon = node.QuerySelector(".icon");
        if (icon != null) {
            icon.Style.UldPartId = destination.UldPartId;
        } else {
            Logger.Warning($"CondensedBuildFavoritesButton: Icon not found in {node.Id}");
        }

        node.OnClick      += _ => Teleport(destination);
        node.OnRightClick += _ => OpenContextMenu(destination, true);

        FavoritesList.AppendChild(node);
    }

    private void CondensedRemoveFavoritesButton(TeleportDestination destination)
    {
        FavoritesList.QuerySelector($"#SortableCondensed_{destination.NodeId}")?.Dispose();

        if (Favorites.Count == 0) {
            FavoritesButton.Style.IsVisible = false;
        }

        if (_selectedExpansion == "Favorites") {
            ActivateExpansion("Other");
        }
    }

    private void CondensedSortFavorites()
    {
        foreach (var favoriteId in Favorites) {
            if (_destinations.TryGetValue(favoriteId, out var destination)) {
                var node = FavoritesList.QuerySelector($"#SortableCondensed_{destination.NodeId}");
                if (node != null) {
                    node.SortIndex = Favorites.IndexOf(favoriteId);
                }
            }
        }
    }

    private void ActivateExpansion(string id)
    {
        _selectedExpansion = id;

        foreach (var node in CondensedContentsNode.QuerySelectorAll(".condensed-expansion")) {
            node.Style.IsVisible = node.Id == $"{id}_Content";
        }

        foreach (var node in CondensedSidePanelNode.QuerySelectorAll(".side-panel-button")) {
            node.ToggleClass("selected", node.Id == $"{id}_Button");
        }
    }
}
