using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class OrnamentShortcutProvider(IDataManager dataManager) : AbstractShortcutProvider
{
    public override string ShortcutType          => "OR"; // Ornament
    public override string PickerWindowTitle     => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Ornament.Title");
    public override string ContextMenuEntryName  => I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickOrnament");
    public override uint?  ContextMenuEntryIcon  => 86;
    public override int    ContextMenuEntryOrder => -896;

    /// <inheritdoc/>
    public override unsafe IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        List<Ornament> ornaments = dataManager.GetExcelSheet<Ornament>()!.ToList();
        List<Shortcut> shortcuts = [];

        foreach (var ornament in ornaments) {
            if (!PlayerState.Instance()->IsOrnamentUnlocked(ornament.RowId)) continue;

            // Skip the following ornaments, they are the old spectacles that are no longer available but
            // still in the sheets and "unlocked" for some reason.
            if (ornament.RowId is 22 or 25 or 26 or 32) continue;

            var name = ornament.Singular.ToDalamudString().TextValue;
            name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);

            if (!string.IsNullOrEmpty(searchFilter) && !name.Contains(searchFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            shortcuts.Add(
                new() {
                    Id     = ornament.RowId,
                    Name   = name,
                    IconId = ornament.Icon,
                }
            );
        }

        return shortcuts;
    }

    /// <inheritdoc/>
    public override unsafe Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        if (id == 0u) return null;

        var ornament = dataManager.GetExcelSheet<Ornament>()!.GetRow(id);
        if (ornament == null) return null;

        var name = ornament.Singular.ToDalamudString().TextValue;

        return new() {
            Id         = ornament.RowId,
            Name       = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name),
            IconId     = ornament.Icon,
            IsDisabled = !PlayerState.Instance()->IsOrnamentUnlocked(id),
        };
    }

    /// <inheritdoc/>
    public override unsafe void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        ActionManager* am = ActionManager.Instance();
        if (am == null) return;

        if (am->GetActionStatus(ActionType.Ornament, id) != 0) return;
        am->UseAction(ActionType.Ornament, id);
    }
}
