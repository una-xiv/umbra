using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class EmoteShortcutProvider(IDataManager dataManager) : AbstractShortcutProvider
{
    public override string ShortcutType          => "EM"; // Emote
    public override string PickerWindowTitle     => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Emote.Title");
    public override string ContextMenuEntryName  => I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickEmote");
    public override uint?  ContextMenuEntryIcon  => 9;
    public override int    ContextMenuEntryOrder => -899;

    /// <inheritdoc/>
    public override unsafe IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        var emoteList = dataManager.GetExcelSheet<Emote>()!.ToList();

        emoteList.Sort(
            (a, b) => string.Compare(a.Name.ToString(), b.Name.ToString(), StringComparison.OrdinalIgnoreCase)
        );

        List<Shortcut> shortcuts = [];

        foreach (var emote in emoteList) {
            if (emote.TextCommand.Value == null) continue;
            if (!UIState.Instance()->IsEmoteUnlocked((ushort)emote.RowId)) continue;

            var name = emote.Name.ToDalamudString().TextValue;
            if (!string.IsNullOrEmpty(searchFilter) && !name.Contains(searchFilter, StringComparison.OrdinalIgnoreCase)) continue;

            shortcuts.Add(
                new() {
                    Id          = emote.RowId,
                    Name        = name,
                    Description = emote.TextCommand.Value.ToString(),
                    IconId      = emote.Icon,
                }
            );
        }

        return shortcuts;
    }

    /// <inheritdoc/>
    public override unsafe Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        if (id == 0u) return null;

        var emote = dataManager.GetExcelSheet<Emote>()!.GetRow(id);
        if (emote == null) return null;

        return new() {
            Id         = emote.RowId,
            Name       = emote.Name.ToDalamudString().TextValue,
            IconId     = emote.Icon,
            IsDisabled = !UIState.Instance()->IsEmoteUnlocked((ushort)id),
        };
    }

    /// <inheritdoc/>
    public override void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        Emote? emote = dataManager.GetExcelSheet<Emote>()!.GetRow(id);
        if (emote?.TextCommand.Value == null) return;

        Framework.Service<IChatSender>().Send(emote.TextCommand.Value.Command.ToDalamudString().TextValue);
    }
}
