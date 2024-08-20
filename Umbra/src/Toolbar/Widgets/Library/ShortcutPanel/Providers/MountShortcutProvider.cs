using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets2;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game.Localization;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class MountShortcutProvider(IDataManager dataManager, TextDecoder decoder) : AbstractShortcutProvider
{
    public override string ShortcutType          => "MO"; // Mount
    public override string PickerWindowTitle     => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Mount.Title");
    public override string ContextMenuEntryName  => I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickMount");
    public override uint?  ContextMenuEntryIcon  => 58;
    public override int    ContextMenuEntryOrder => -897;

    /// <inheritdoc/>
    public override unsafe IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        List<Shortcut> shortcuts = [];

        var mounts = dataManager.GetExcelSheet<Mount>()!.ToList();

        mounts.Sort(
            (a, b) => string.Compare(
                a.Singular.ToDalamudString().TextValue,
                b.Singular.ToDalamudString().TextValue,
                StringComparison.OrdinalIgnoreCase
            )
        );

        PlayerState* ps = PlayerState.Instance();

        foreach (var mount in mounts) {
            if (!ps->IsMountUnlocked(mount.RowId)) continue;

            if (searchFilter != null
                && !mount
                    .Singular.ToDalamudString()
                    .TextValue.Contains(searchFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            shortcuts.Add(
                new() {
                    Id          = mount.RowId,
                    Name        = decoder.ProcessNoun("Mount", mount.RowId),
                    Description = "",
                    IconId      = mount.Icon
                }
            );
        }

        return shortcuts;
    }

    /// <inheritdoc/>
    public override unsafe Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        if (id == 0u) return null;

        var mount = dataManager.GetExcelSheet<Mount>()!.GetRow(id);
        if (mount == null) return null;

        return new() {
            Id         = id,
            Name       = decoder.ProcessNoun("Mount", id),
            IconId     = mount.Icon,
            IsDisabled = !PlayerState.Instance()->IsMountUnlocked((ushort)id),
        };
    }

    /// <inheritdoc/>
    public override unsafe void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        ActionManager* am = ActionManager.Instance();
        if (am == null || am->GetActionStatus(ActionType.Mount, id) != 0) return;

        am->UseAction(ActionType.Mount, id);
    }
}
