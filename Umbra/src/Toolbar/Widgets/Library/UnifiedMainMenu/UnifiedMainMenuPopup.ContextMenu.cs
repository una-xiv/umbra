using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.Sheets;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

internal sealed partial class UnifiedMainMenuPopup : WidgetPopup
{
    private unsafe void CreateContextMenu()
    {
        var dm = Framework.Service<IDataManager>();
        
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
                new("Logout") {
                    Label   = dm.GetExcelSheet<MainCommand>().GetRow(23).Name.ExtractText().StripSoftHyphen(),
                    OnClick = () => UIModule.Instance()->ExecuteMainCommand(23),
                    IconId  = 27u,
                },
                new("Shutdown") {
                    Label   = dm.GetExcelSheet<MainCommand>().GetRow(24).Name.ExtractText().StripSoftHyphen(),
                    OnClick = () => UIModule.Instance()->ExecuteMainCommand(24),
                    IconId  = 26u,
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
