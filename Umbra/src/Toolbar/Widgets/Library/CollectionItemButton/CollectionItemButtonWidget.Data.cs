using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets.Library.CollectionItemButton;

internal partial class CollectionItemButtonWidget
{
    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();

    private Dictionary<string, CollectionItem> Items { get; } = [];

    private void LoadCollectionItems()
    {
        foreach (var entry in DataManager.GetExcelSheet<McGuffin>().ToList()) {
            var uiData = entry.UIData.ValueNullable;
            if (uiData == null || uiData.Value.Icon == 0) continue;

            Items.TryAdd(entry.RowId.ToString(), new(entry.RowId, uiData.Value.Icon, uiData.Value.Name.ToString()));
        }
    }

    private unsafe bool IsItemUnlocked(CollectionItem item)
    {
        PlayerState* ps = PlayerState.Instance();
        if (ps == null || ps->IsLoaded == 0) return false;

        return ps->IsMcGuffinUnlocked(item.Id);
    }

    private readonly struct CollectionItem(uint id, uint icon, string name)
    {
        public readonly uint   Id   = id;
        public readonly uint   Icon = icon;
        public readonly string Name = name;
    }
}
