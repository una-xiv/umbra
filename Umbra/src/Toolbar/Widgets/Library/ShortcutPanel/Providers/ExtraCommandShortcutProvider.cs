﻿using Lumina.Excel.Sheets;
using System.Globalization;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class ExtraCommandShortcutProvider(IDataManager dataManager) : AbstractShortcutProvider
{
    public override string ShortcutType          => "EX"; // Extra
    public override string PickerWindowTitle     => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Extra.Title");
    public override string ContextMenuEntryName  => I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickExtraCommand");
    public override uint?  ContextMenuEntryIcon  => 9;
    public override int    ContextMenuEntryOrder => -797;

    /// <inheritdoc/>
    public override unsafe IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        List<ExtraCommand> commands  = dataManager.GetExcelSheet<ExtraCommand>().ToList();
        List<Shortcut>     shortcuts = [];

        foreach (var command in commands) {
            if (command.RowId == 0 || string.IsNullOrEmpty(command.Name.ExtractText())) continue;

            shortcuts.Add(
                new() {
                    Id          = command.RowId,
                    Name        = command.Name.ExtractText(),
                    Description = command.Description.ExtractText().Split(".")[0],
                    IconId      = (uint)command.Icon,
                }
            );
        }

        return shortcuts;
    }

    /// <inheritdoc/>
    public override Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        if (id == 0u) return null;

        var command = dataManager.GetExcelSheet<ExtraCommand>().FindRow(id);
        if (command == null) return null;

        return new() {
            Id     = id,
            Name   = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(command.Value.Name.ExtractText()),
            IconId = (uint)command.Value.Icon,
        };
    }

    /// <inheritdoc/>
    public override void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        // This is silly, but I can't figure out how to invoke these otherwise...
        string? chatCmd = id switch {
            1 => "/gpose",
            2 => "/idlingcamera",
            3 => "/alarm",
            _ => null
        };

        if (chatCmd == null) return;
        Framework.Service<IChatSender>().Send(chatCmd);
    }
}
