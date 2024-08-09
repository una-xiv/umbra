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
using Umbra.Game.Localization;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal sealed class KeyItemPickerWindow : PickerWindowBase
{
    protected override string Title  => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Item.Title");
    protected override string TypeId => "EI";

    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();
    private TextDecoder  TextDecoder { get; } = Framework.Service<TextDecoder>();

    public KeyItemPickerWindow()
    {
        List<EventItem> items = GetInventoryItems(InventoryType.KeyItems);

        items.Sort(
            (a, b) => string.Compare(a.Name.ToString(), b.Name.ToString(), StringComparison.OrdinalIgnoreCase)
        );

        foreach (var item in items) {
            AddItem(
                TextDecoder.ProcessNoun("EventItem", item.RowId),
                $"Item ID: {item.RowId}",
                item.Icon,
                () => {
                    SetPickedItemId(item.RowId);
                    Close();
                }
            );
        }
    }

    private unsafe List<EventItem> GetInventoryItems(InventoryType type)
    {
        InventoryContainer* container = InventoryManager.Instance()->GetInventoryContainer(type);
        if (container == null) return [];

        var items = new List<EventItem>();
        var sheet = DataManager.GetExcelSheet<EventItem>()!;

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

        return items;
    }
}
