using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets2;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class GeneralActionShortcutProvider(IDataManager dataManager, IPlayer player) : AbstractShortcutProvider
{
    public override string ShortcutType => "GA"; // General Action
    public override string PickerWindowTitle => I18N.Translate("Widget.ShortcutPanel.PickerWindow.GeneralAction.Title");
    public override string ContextMenuEntryName => I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickGeneralAction");
    public override uint?  ContextMenuEntryIcon => 4;
    public override int    ContextMenuEntryOrder => -799;

    /// <inheritdoc/>
    public override IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        List<Shortcut> shortcuts = [];

        var actions = dataManager.GetExcelSheet<GeneralAction>()!.ToList();

        actions.Sort(
            (a, b) => string.Compare(
                a.Name.ToDalamudString().TextValue,
                b.Name.ToDalamudString().TextValue,
                StringComparison.OrdinalIgnoreCase
            )
        );

        foreach (var action in actions) {
            if (!player.IsGeneralActionUnlocked(action.RowId)
                || action.Icon < 1
                || string.IsNullOrEmpty(action.Name.ToDalamudString().TextValue))
                continue;

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
        var action = dataManager.GetExcelSheet<GeneralAction>()!.GetRow(id);
        if (action == null) return null;

        return new() {
            Id         = id,
            Name       = action.Name.ToDalamudString().TextValue,
            IconId     = (uint)action.Icon,
            IsDisabled = !player.IsGeneralActionUnlocked(id)
                || ActionManager.Instance()->GetActionStatus(ActionType.GeneralAction, id) != 0
        };
    }

    /// <inheritdoc/>
    public override unsafe void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        ActionManager* am = ActionManager.Instance();

        if (!Framework.Service<IPlayer>().IsGeneralActionUnlocked(id)) return;
        if (am == null || am->GetActionStatus(ActionType.GeneralAction, id) != 0) return;

        Framework.Service<IPlayer>().UseGeneralAction(id);
    }
}
