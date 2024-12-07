﻿using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
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
        var            actions   = dataManager.GetExcelSheet<MainCommand>().ToList();

        actions.Sort(
            (a, b) => string.Compare(
                a.Name.ExtractText(),
                b.Name.ExtractText(),
                StringComparison.OrdinalIgnoreCase
            )
        );

        UIModule* ui = UIModule.Instance();

        foreach (var action in actions) {
            if (!ui->IsMainCommandUnlocked(action.RowId) || 0 == action.Icon) continue;

            if (searchFilter != null
                && !action.Name.ExtractText().Contains(searchFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            shortcuts.Add(
                new() {
                    Id          = action.RowId,
                    Name        = action.Name.ExtractText(),
                    Description = action.Description.ExtractText(),
                    IconId      = (uint)action.Icon
                }
            );
        }

        return shortcuts;
    }

    /// <inheritdoc/>
    public override unsafe Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        var command = dataManager.GetExcelSheet<MainCommand>().FindRow(id);
        if (command == null) return null;

        UIModule* uiModule = UIModule.Instance();
        if (uiModule == null) return null;

        AgentHUD* agentHud = AgentHUD.Instance();
        if (agentHud == null) return null;

        return new() {
            Id     = id,
            Name   = command.Value.Name.ExtractText(),
            IconId = (uint)command.Value.Icon,
            IsDisabled = !uiModule->IsMainCommandUnlocked(id) || !agentHud->IsMainCommandEnabled(id)
        };
    }

    /// <inheritdoc/>
    public override unsafe void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        UIModule* uiModule = UIModule.Instance();
        if (uiModule == null || !uiModule->IsMainCommandUnlocked(id)) return;

        AgentHUD* agentHud = AgentHUD.Instance();
        if (agentHud == null || !agentHud->IsMainCommandEnabled(id)) return;

        uiModule->ExecuteMainCommand(id);
    }
}
