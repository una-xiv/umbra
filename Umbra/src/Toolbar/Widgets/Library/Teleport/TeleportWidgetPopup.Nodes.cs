using Dalamud.Game.Text;
using System.Runtime.CompilerServices;
using System.Text;
using Umbra.Widgets.Popup;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    private string? _builtInterfaceKey;

    private readonly List<(Node Node, string DestinationKey)> _costNodes     = [];
    private readonly List<(Node Node, MainMenuItem Item)>     _menuItemNodes = [];

    private void UpdateInterface()
    {
        ExtendedInterfaceNode.Style.IsVisible  = DisplayMode == PopupDisplayMode.Extended;
        CondensedInterfaceNode.Style.IsVisible = DisplayMode == PopupDisplayMode.Condensed;

        if (DisplayMode == PopupDisplayMode.Condensed) {
            CondensedInterfaceNode.Style.FlowOrder = ReverseCondensedElements ? FlowOrder.Reverse : FlowOrder.Normal;
        }

        foreach (var node in (DisplayMode == PopupDisplayMode.Extended ? ExtendedInterfaceNode : CondensedInterfaceNode).QuerySelectorAll(".list .cost")) {
            node.Style.IsVisible = ShowTeleportCost;
        }
    }

    private void BuildInterfaces()
    {
        _costNodes.Clear();
        _menuItemNodes.Clear();

        ExtendedInterfaceNode.Clear();
        CondensedInterfaceNode.Clear();

        if (DisplayMode == PopupDisplayMode.Extended) {
            BuildExtendedInterface();
        } else {
            BuildCondensedInterface();
        }

        ApplyFontSize();
    }

    private void RefreshInterface()
    {
        _costNodes.RemoveAll(entry => entry.Node.IsDisposed);

        foreach ((Node node, string destinationKey) in _costNodes) {
            if (_destinations.TryGetValue(destinationKey, out var destination)) {
                node.NodeValue = FormatGilCost(destination);
            }
        }

        foreach ((Node node, MainMenuItem item) in _menuItemNodes) {
            ApplyMenuItem(node, item);
        }

        if (DisplayMode == PopupDisplayMode.Condensed) {
            ActivateDefaultExpansion();
        }

        ApplyFontSize();
    }

    private void ApplyFontSize()
    {
        foreach (var node in Node.QuerySelectorAll(".text, .cost")) {
            node.Style.FontSize = PopupFontSize;
        }
    }

    // Anything the build reads must be part of the key, or a stale interface is reused.
    private string GetInterfaceKey()
    {
        StringBuilder sb = new();

        sb.Append($"{DisplayMode}|{ShowMapNames}|{FixedPopupWidth}|{CustomPopupWidth}|{PopupHeight}|{Toolbar.IsTopAligned}|");
        sb.Append(string.Join(",", Favorites)).Append('|');

        foreach (var expansion in _expansions.Values) {
            sb.Append($"{expansion.NodeId}:{expansion.Name};");

            foreach (var region in expansion.Regions.Values) {
                sb.Append($"{region.NodeId}:{region.Name};");

                foreach (var map in region.Maps.Values) {
                    sb.Append($"{map.Name};");

                    foreach (var destination in map.Destinations.Values) {
                        sb.Append($"{destination.NodeId}:{destination.Name};");
                    }
                }
            }
        }

        // Click handlers capture the item instance.
        foreach (MainMenuItem item in GetTravelMenuItems()) {
            sb.Append($"{item.Id}:{RuntimeHelpers.GetHashCode(item)};");
        }

        return sb.ToString();
    }

    private IEnumerable<MainMenuItem> GetTravelMenuItems()
    {
        foreach (MainMenuItem item in Framework.Service<IMainMenuRepository>().GetCategory(MenuCategory.Travel).Items) {
            switch (item.Type) {
                case MainMenuItemType.MainCommand when item.CommandId != 36:
                case MainMenuItemType.Separator:
                    continue;
                default:
                    yield return item;
                    break;
            }
        }
    }

    private void RegisterCostNode(Node teleportNode, TeleportDestination destination)
    {
        var costNode = teleportNode.QuerySelector(".cost");
        if (costNode == null) return;

        _costNodes.Add((costNode, $"{destination.AetheryteId}:{destination.SubIndex}"));
    }

    private static string FormatGilCost(TeleportDestination destination)
    {
        return $"{SeIconChar.Gil.ToIconChar()} {I18N.FormatNumber(destination.GilCost)}";
    }

    private void ApplyMenuItem(Node menuItemNode, MainMenuItem item)
    {
        menuItemNode.QuerySelector(".text")!.NodeValue = item.Name;
        menuItemNode.QuerySelector(".cost")!.NodeValue = item.ShortKey;

        switch (item.Icon) {
            case uint iconId:
                menuItemNode.QuerySelector(".icon")!.Style.IconId = iconId;
                menuItemNode.QuerySelector(".icon")!.NodeValue    = default;
                break;
            case SeIconChar seIconChar:
                menuItemNode.QuerySelector(".icon")!.Style.IconId = null;
                menuItemNode.QuerySelector(".icon")!.Style.Color  = item.IconColor != null ? new(item.IconColor.Value) : null;
                menuItemNode.QuerySelector(".icon")!.NodeValue    = seIconChar.ToIconString();
                break;
        }
    }

    private void BuildFavoritesButton(TeleportDestination destination)
    {
        if (DisplayMode == PopupDisplayMode.Extended) ExtendedBuildFavoritesButton(destination);
        if (DisplayMode == PopupDisplayMode.Condensed) CondensedBuildFavoritesButton(destination);
    }

    private void RemoveFavoritesButton(TeleportDestination destination)
    {
        if (DisplayMode == PopupDisplayMode.Extended) ExtendedRemoveFavoritesButton(destination);
        if (DisplayMode == PopupDisplayMode.Condensed) CondensedRemoveFavoritesButton(destination);
    }

    private void UpdateFavoriteSortIndices()
    {
        if (DisplayMode == PopupDisplayMode.Extended) ExtendedSortFavorites();
        if (DisplayMode == PopupDisplayMode.Condensed) CondensedSortFavorites();
    }
}
