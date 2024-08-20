using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets2;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class CollectionItemShortcutProvider(IDataManager dataManager) : AbstractShortcutProvider
{
    public override string ShortcutType      => "CO"; // Collection
    public override string PickerWindowTitle => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Item.Title");

    public override string ContextMenuEntryName =>
        I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickCollectionItem");

    public override uint? ContextMenuEntryIcon  => 3;
    public override int   ContextMenuEntryOrder => -998;

    /// <inheritdoc/>
    public override IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        return GetMcGuffins()
            .Select(
                i => new Shortcut {
                    Id          = i.RowId,
                    Name        = i.UIData.Value!.Name.ToDalamudString().TextValue,
                    Description = "",
                    IconId      = i.UIData.Value!.Icon,
                }
            )
            .Where(i => i.Name.Contains(searchFilter ?? "", StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <inheritdoc/>
    public override unsafe Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        if (id == 0u) return null;

        var item = dataManager.GetExcelSheet<McGuffin>()!.GetRow(id);
        if (item == null) return null;

        return new() {
            Id         = id,
            Name       = item.UIData.Value!.Name.ToDalamudString().TextValue,
            IconId     = item.UIData.Value!.Icon,
            IsDisabled = !PlayerState.Instance()->IsMcGuffinUnlocked((ushort)id),
        };
    }

    /// <inheritdoc/>
    public override unsafe void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        var result = stackalloc AtkValue[1];
        var values = stackalloc AtkValue[2];
        values[0].SetInt(1);
        values[1].SetUInt(id);

        AgentModule.Instance()->GetAgentByInternalId(AgentId.McGuffin)->ReceiveEvent(result, values, 2, 0);
    }

    private unsafe List<McGuffin> GetMcGuffins()
    {
        List<McGuffin> list        = dataManager.GetExcelSheet<McGuffin>()!.ToList();
        PlayerState*   playerState = PlayerState.Instance();

        list.Sort(
            (a, b) => String.Compare(
                a.UIData.Value!.Name.ToDalamudString().TextValue,
                b.UIData.Value!.Name.ToDalamudString().TextValue,
                StringComparison.OrdinalIgnoreCase
            )
        );

        return list.Where(i => playerState->IsMcGuffinUnlocked(i.RowId)).ToList();
    }
}
