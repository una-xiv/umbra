using System;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

internal sealed partial class UnifiedMainMenuPopup : WidgetPopup
{
    private void CreateContextMenu()
    {
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
        if (null != node) node.Dispose();

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
}
