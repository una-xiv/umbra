using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game.Localization;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class MinionShortcutProvider(IDataManager dataManager, TextDecoder decoder) : AbstractShortcutProvider
{
    public override string ShortcutType          => "MI"; // Minion
    public override string PickerWindowTitle     => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Minion.Title");
    public override string ContextMenuEntryName  => I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickMinion");
    public override uint?  ContextMenuEntryIcon  => 59;
    public override int    ContextMenuEntryOrder => -898;

    /// <inheritdoc/>
    public override unsafe IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        List<Shortcut> shortcuts = [];

        var minions = dataManager.GetExcelSheet<Companion>()!.ToList();

        minions.Sort(
            (a, b) => string.Compare(
                a.Singular.ToDalamudString().TextValue,
                b.Singular.ToDalamudString().TextValue,
                StringComparison.OrdinalIgnoreCase
            )
        );

        UIState* ui = UIState.Instance();

        foreach (var minion in minions) {
            if (!ui->IsCompanionUnlocked(minion.RowId)) continue;

            if (searchFilter != null
                && !minion
                    .Singular.ToDalamudString()
                    .TextValue.Contains(searchFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            shortcuts.Add(
                new() {
                    Id          = minion.RowId,
                    Name        = decoder.ProcessNoun("Companion", minion.RowId),
                    Description = $"{minion.MinionRace.Value?.Name.ToDalamudString().TextValue}",
                    IconId      = minion.Icon
                }
            );
        }

        return shortcuts;
    }

    /// <inheritdoc/>
    public override unsafe Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        if (id == 0u) return null;

        var minion = dataManager.GetExcelSheet<Companion>()!.GetRow(id);
        if (minion == null) return null;

        return new() {
            Id         = minion.RowId,
            Name       = decoder.ProcessNoun("Companion", minion.RowId),
            IconId     = minion.Icon,
            IsDisabled = !UIState.Instance()->IsCompanionUnlocked((ushort)id),
        };
    }

    /// <inheritdoc/>
    public override unsafe void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        ActionManager* am = ActionManager.Instance();
        if (am == null || am->GetActionStatus(ActionType.Companion, id) != 0) return;

        am->UseAction(ActionType.Companion, id);
    }
}
