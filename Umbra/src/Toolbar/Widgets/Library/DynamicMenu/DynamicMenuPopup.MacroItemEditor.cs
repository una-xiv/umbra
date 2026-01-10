using Umbra.Windows;
using Umbra.Windows.Library.VariableEditor;

namespace Umbra.Widgets;

internal sealed partial class DynamicMenuPopup
{
    private void OpenMacroItemEditor()
    {
        if (_selectedItemIndex == null) return;
        DynamicMenuEntry entry = Entries[_selectedItemIndex.Value];

        StringVariable altLabelVar = new("SubLabel") {
            Name        = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.AltLabel.Name"),
            Description = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.AltLabel.Description"),
            Value       = entry.Cm ?? "",
            MaxLength   = 100,
        };

        GameIconVariable iconVar = new("Icon") {
            Name        = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Icon.Name"),
            Description = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.Icon.Description"),
            Value       = (entry.Ci ?? 0),
        };

        ColorVariable iconColorVar = new("IconColor") {
            Name        = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.IconColor.Name"),
            Description = I18N.Translate("Widget.DynamicMenu.CustomItemEditor.IconColor.Description"),
            Value       = entry.Cj,
        };

        List<Variable> variables = [altLabelVar, iconVar, iconColorVar];

        VariablesEditorWindow window = new(I18N.Translate("Widget.DynamicMenu.MacroItemEditor.Title"), variables, []);

        Framework
           .Service<WindowManager>()
           .Present(
                "CustomItemEditor",
                window,
                _ => {
                    entry.Cm = altLabelVar.Value;
                    entry.Ci = iconVar.Value;
                    entry.Cj = iconColorVar.Value;

                    NotifyEntriesChanged(true);
                }
            );
    }
}
