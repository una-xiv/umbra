using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Widgets.Library.ShortcutPanel.Providers;
using Umbra.Windows;
using Umbra.Windows.Library.VariablesWindow;
using Una.Drawing;

namespace Umbra.Widgets.Library.DynamicMenu;

internal sealed partial class DynamicMenuPopup : WidgetPopup
{
    private void OpenCustomItemEditor()
    {
        if (_selectedItemIndex == null) return;
        DynamicMenuEntry entry = Entries[_selectedItemIndex.Value];

        StringVariable labelVar = new("Label") {
            Name        = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Label.Name"),
            Description = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Label.Description"),
            Value       = entry.Cl ?? "",
            MaxLength   = 100,
        };

        StringVariable altLabelVar = new("SubLabel") {
            Name        = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.AltLabel.Name"),
            Description = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.AltLabel.Description"),
            Value       = entry.Cm ?? "",
            MaxLength   = 100,
        };

        IconIdVariable iconVar = new("Icon") {
            Name        = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Icon.Name"),
            Description = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Icon.Description"),
            Value       = (entry.Ci ?? 0),
        };

        ColorVariable iconColorVar = new("IconColor") {
            Name        = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.IconColor.Name"),
            Description = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.IconColor.Description"),
            Value       = entry.Cj,
        };

        StringSelectVariable typeVar = new("Type") {
            Name        = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Type.Name"),
            Description = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Type.Description"),
            Value       = entry.Ct ?? "Chat",
            Choices = new() {
                { "Chat", I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Type.Option.ChatCommand") },
                { "URL", I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Type.Option.WebLink") },
            },
        };

        StringVariable commandVar = new("Command") {
            Name        = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Command.Name"),
            Description = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Command.Description"),
            Value       = entry.Cc ?? "",
            MaxLength   = 100,
        };

        List<Variable> variables = [labelVar, altLabelVar, iconVar, iconColorVar, typeVar, commandVar];

        VariablesWindow window = new(I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Title"), variables);

        Framework
            .Service<WindowManager>()
            .Present(
                "CustomItemEditor",
                window,
                _ => {
                    entry.Cl = labelVar.Value;
                    entry.Cm = altLabelVar.Value;
                    entry.Ci = iconVar.Value;
                    entry.Cj = iconColorVar.Value;
                    entry.Ct = typeVar.Value;
                    entry.Cc = commandVar.Value;

                    RebuildMenu();
                    OnEntriesChanged?.Invoke();
                }
            );
    }
}
