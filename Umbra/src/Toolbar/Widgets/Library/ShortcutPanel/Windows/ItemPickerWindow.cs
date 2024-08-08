using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal sealed class ItemPickerWindow : PickerWindowBase
{
    protected override string Title  => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Item.Title");
    protected override string TypeId => "I";

    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();
    private IPlayer      Player      { get; } = Framework.Service<IPlayer>();

    public ItemPickerWindow()
    {
        List<Item> items = [
            ..GetInventoryItems(InventoryType.Inventory1),
            ..GetInventoryItems(InventoryType.Inventory2),
            ..GetInventoryItems(InventoryType.Inventory3),
            ..GetInventoryItems(InventoryType.Inventory4),
            ..GetInventoryItems(InventoryType.KeyItems)
        ];

        items.Sort(
            (a, b) => string.Compare(a.Name.ToString(), b.Name.ToString(), StringComparison.OrdinalIgnoreCase)
        );

        foreach (var item in items) {
            AddItem(
                item.Name.ToDalamudString().TextValue,
                $"{Player.GetItemCount(item.RowId, ItemUsage.NqOnly)} NQ / {Player.GetItemCount(item.RowId, ItemUsage.HqOnly)} HQ - Item ID: {item.RowId}",
                item.Icon,
                () => {
                    SetPickedItemId(item.RowId);
                    Close();
                }
            );
        }
    }

    private unsafe List<Item> GetInventoryItems(InventoryType type)
    {
        InventoryContainer* container = InventoryManager.Instance()->GetInventoryContainer(type);
        if (container == null) return [];

        var items = new List<Item>();
        var sheet = DataManager.GetExcelSheet<Item>()!;

        for (var i = 0; i < container->Size; i++) {
            var slot = container->GetInventorySlot(i);
            if (slot == null || slot->ItemId == 0) continue;

            var item = sheet.GetRow(slot->ItemId);
            if (item == null) continue;

            items.Add(item);
        }

        return items;
    }
}
