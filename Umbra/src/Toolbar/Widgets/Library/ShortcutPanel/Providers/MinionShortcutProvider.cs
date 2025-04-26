﻿using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

#pragma warning disable SeStringEvaluator

[Service]
internal sealed class MinionShortcutProvider(IDataManager dataManager, ISeStringEvaluator evaluator) : AbstractShortcutProvider
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

        var minions = dataManager.GetExcelSheet<Companion>().ToList();

        minions.Sort(
            (a, b) => string.Compare(
                a.Singular.ExtractText(),
                b.Singular.ExtractText(),
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
                    Name        = evaluator.EvaluateObjStr(ObjectKind.Companion, minion.RowId),
                    Description = $"{minion.MinionRace.ValueNullable?.Name.ExtractText()}",
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

        var minion = dataManager.GetExcelSheet<Companion>().FindRow(id);
        if (minion == null) return null;

        return new() {
            Id         = minion.Value.RowId,
            Name       = evaluator.EvaluateObjStr(ObjectKind.Companion, minion.Value.RowId),
            IconId     = minion.Value.Icon,
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
