using Umbra.Common;
using Umbra.Widgets.Popup;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    private void UpdateInterface()
    {
        ExtendedInterfaceNode.Style.IsVisible  = DisplayMode == PopupDisplayMode.Extended;
        CondensedInterfaceNode.Style.IsVisible = DisplayMode == PopupDisplayMode.Condensed;
    }

    private void BuildInterfaces()
    {
        ExtendedInterfaceNode.Clear();
        CondensedInterfaceNode.Clear();

        if (DisplayMode == PopupDisplayMode.Extended) {
            BuildExtendedInterface();
        } else {
            BuildCondensedInterface();
        }
        
        foreach (var node in Node.QuerySelectorAll(".text, .cost")) {
            node.Style.FontSize = PopupFontSize;
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
