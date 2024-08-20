using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets2;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class KeyItemShortcutProvider(IDataManager dataManager) : AbstractShortcutProvider
{
    public override string ShortcutType          => "EI"; // Event Item
    public override string PickerWindowTitle     => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Item.Title");
    public override string ContextMenuEntryName  => I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickKeyItem");
    public override uint?  ContextMenuEntryIcon  => 3;
    public override int    ContextMenuEntryOrder => -999;

    /// <inheritdoc/>
    public override IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        return GetEventItemsFromInventory()
            .Where(
                i => i.Name.ToDalamudString().TextValue.Contains(searchFilter ?? "", StringComparison.OrdinalIgnoreCase)
            )
            .Select(
                i => new Shortcut {
                    Id          = i.RowId,
                    Name        = i.Name.ToString(),
                    Description = $"Item ID: {i.RowId}",
                    IconId      = i.Icon,
                }
            )
            .ToList();
    }

    /// <inheritdoc/>
    public override unsafe Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        if (id == 0u) return null;

        var item = dataManager.GetExcelSheet<EventItem>()!.GetRow(id);
        if (item == null) return null;

        InventoryContainer* container = InventoryManager.Instance()->GetInventoryContainer(InventoryType.KeyItems);

        bool found = false;

        if (container != null) {
            for (var i = 0; i < container->Size; i++) {
                if (container->GetInventorySlot(i)->ItemId == id) {
                    found = true;
                    break;
                }
            }
        }

        return new() {
            Id         = id,
            Name       = item.Name.ToDalamudString().TextValue,
            IconId     = item.Icon,
            IsDisabled = found,
        };
    }

    /// <inheritdoc/>
    public override unsafe void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        EventItem? item = dataManager.GetExcelSheet<EventItem>()!.GetRow(id);
        if (item == null) return;

        ActionManager* am = ActionManager.Instance();

        if (am->GetActionStatus(ActionType.KeyItem, id) == 0) am->UseAction(ActionType.KeyItem, id);
    }

    private unsafe List<EventItem> GetEventItemsFromInventory()
    {
        InventoryContainer* container = InventoryManager.Instance()->GetInventoryContainer(InventoryType.KeyItems);
        if (container == null) return [];

        var items = new List<EventItem>();
        var sheet = dataManager.GetExcelSheet<EventItem>()!;

        for (var i = 0; i < container->Size; i++) {
            var slot = container->GetInventorySlot(i);
            if (slot == null || slot->ItemId == 0) continue;
            var item = sheet.GetRow(slot->ItemId);

            if (item == null) {
                Logger.Warning($"Failed to find item for ID: {slot->ItemId}");
                continue;
            }

            items.Add(item);
        }

        items.Sort(
            (a, b) => string.Compare(a.Name.ToString(), b.Name.ToString(), StringComparison.OrdinalIgnoreCase)
        );

        return items;
    }
}
