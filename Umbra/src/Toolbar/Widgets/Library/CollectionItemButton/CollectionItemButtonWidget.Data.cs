using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace Umbra.Widgets;

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
        if (ps == null || !ps->IsLoaded) return false;

        return ps->IsMcGuffinUnlocked(item.Id);
    }

    private readonly struct CollectionItem(uint id, uint icon, string name)
    {
        public readonly uint   Id   = id;
        public readonly uint   Icon = icon;
        public readonly string Name = name;
    }
    
    private unsafe void Invoke(Node _)
    {
        if (!Items.TryGetValue(GetConfigValue<string>("Item"), out var item)) return;

        var result = stackalloc AtkValue[1];
        var values = stackalloc AtkValue[2];
        values[0].SetInt(1);
        values[1].SetUInt(item.Id);

        AgentModule.Instance()->GetAgentByInternalId(AgentId.McGuffin)->ReceiveEvent(result, values, 2, 0);
    }
}
