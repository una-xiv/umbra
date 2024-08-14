using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class InventoryItemShortcutProvider(IDataManager dataManager, IPlayer player) : AbstractShortcutProvider
{
    public override string ShortcutType          => "I"; // Item
    public override string PickerWindowTitle     => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Item.Title");
    public override string ContextMenuEntryName  => I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickItem");
    public override uint?  ContextMenuEntryIcon  => 2;
    public override int    ContextMenuEntryOrder => -1000;

    /// <inheritdoc/>
    public override IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        List<Item> items = [
            ..GetInventoryItems(InventoryType.Inventory1),
            ..GetInventoryItems(InventoryType.Inventory2),
            ..GetInventoryItems(InventoryType.Inventory3),
            ..GetInventoryItems(InventoryType.Inventory4),
        ];

        if (searchFilter != null) {
            items = items.FindAll(
                item => item.Name.ToString().Contains(searchFilter, StringComparison.OrdinalIgnoreCase)
            );
        }

        items.Sort(
            (a, b) => string.Compare(a.Name.ToString(), b.Name.ToString(), StringComparison.OrdinalIgnoreCase)
        );

        return items
            .Select(
                item => new Shortcut {
                    Id   = item.RowId,
                    Name = item.Name.ToDalamudString().TextValue,
                    Description =
                        $"{player.GetItemCount(item.RowId, ItemUsage.NqOnly)} NQ / {player.GetItemCount(item.RowId, ItemUsage.HqOnly)} HQ - Item ID: {item.RowId}",
                    IconId = item.Icon,
                }
            )
            .ToImmutableList();
    }

    /// <inheritdoc/>
    public override Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        if (id == 0u) return null;

        var item = dataManager.GetExcelSheet<Item>()!.GetRow(id);
        if (item == null) return null;

        var count = player.GetItemCount(id);

        return new() {
            Id             = id,
            Name           = item.Name.ToDalamudString().TextValue,
            IconId         = item.Icon,
            Count          = (uint)count,
            IsConfigurable = false,
            IsDisabled     = count == 0,
        };
    }

    /// <inheritdoc/>
    public override void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        if (player.GetItemCount(id) == 0) return;

        // UseInventoryItem does all the required condition tests.
        player.UseInventoryItem(id);
    }

    private unsafe List<Item> GetInventoryItems(InventoryType type)
    {
        InventoryContainer* container = InventoryManager.Instance()->GetInventoryContainer(type);
        if (container == null) return [];

        var items = new List<Item>();
        var sheet = dataManager.GetExcelSheet<Item>()!;

        for (var i = 0; i < container->Size; i++) {
            var slot = container->GetInventorySlot(i);
            if (slot == null || slot->ItemId == 0) continue;

            var item = sheet.GetRow(slot->ItemId > 1000000 ? slot->ItemId - 1000000 : slot->ItemId);
            if (item == null) continue;

            if (!items.Contains(item)) items.Add(item);
        }

        return items;
    }
}
