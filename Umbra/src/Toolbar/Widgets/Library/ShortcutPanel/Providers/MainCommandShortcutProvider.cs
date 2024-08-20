using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class MainCommandShortcutProvider(IDataManager dataManager) : AbstractShortcutProvider
{
    public override string ShortcutType => "MC"; // Main Command
    public override string PickerWindowTitle => I18N.Translate("Widget.ShortcutPanel.PickerWindow.MainCommand.Title");
    public override string ContextMenuEntryName => I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickMainCommand");
    public override uint?  ContextMenuEntryIcon => 29;
    public override int    ContextMenuEntryOrder => -798;

    /// <inheritdoc/>
    public override unsafe IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        List<Shortcut> shortcuts = [];
        var            actions   = dataManager.GetExcelSheet<MainCommand>()!.ToList();

        actions.Sort(
            (a, b) => string.Compare(
                a.Name.ToDalamudString().TextValue,
                b.Name.ToDalamudString().TextValue,
                StringComparison.OrdinalIgnoreCase
            )
        );

        UIModule* ui = UIModule.Instance();

        foreach (var action in actions) {
            if (!ui->IsMainCommandUnlocked(action.RowId) || 0 == action.Icon) continue;

            if (searchFilter != null
                && !action.Name.ToDalamudString().TextValue.Contains(searchFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            shortcuts.Add(
                new() {
                    Id          = action.RowId,
                    Name        = action.Name.ToDalamudString().TextValue,
                    Description = action.Description.ToDalamudString().TextValue,
                    IconId      = (uint)action.Icon
                }
            );
        }

        return shortcuts;
    }

    /// <inheritdoc/>
    public override unsafe Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        var command = dataManager.GetExcelSheet<MainCommand>()!.GetRow(id);
        if (command == null) return null;

        return new() {
            Id         = id,
            Name       = command.Name.ToDalamudString().TextValue,
            IconId     = (uint)command.Icon,
            IsDisabled = !UIModule.Instance()->IsMainCommandUnlocked(id)
                || ActionManager.Instance()->GetActionStatus(ActionType.MainCommand, id) != 0
        };
    }

    /// <inheritdoc/>
    public override unsafe void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        UIModule* uiModule = UIModule.Instance();
        if (uiModule == null || !uiModule->IsMainCommandUnlocked(id)) return;

        uiModule->ExecuteMainCommand(id);
    }
}
