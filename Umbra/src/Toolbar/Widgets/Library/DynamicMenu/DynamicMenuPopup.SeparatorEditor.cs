using Umbra.Windows;
using Umbra.Windows.Library.VariableEditor;

namespace Umbra.Widgets;

internal sealed partial class DynamicMenuPopup
{
    private void OpenSeparatorEditor()
    {
        if (null == _selectedItemIndex) return;
        DynamicMenuEntry entry = Entries[_selectedItemIndex.Value];

        StringVariable labelVar = new("Label") {
            Name        = I18N.Translate("Widget.DynamicMenu.SeparatorEditor.Label.Name"),
            Description = I18N.Translate("Widget.DynamicMenu.SeparatorEditor.Label.Description"),
            Value       = entry.Sl ?? "",
            MaxLength   = 100,
        };

        List<Variable>        variables = [labelVar];
        VariablesEditorWindow window    = new(I18N.Translate("Widget.DynamicMenu.SeparatorEditor.Title"), variables, []);

        Framework
           .Service<WindowManager>()
           .Present(
                "SeparatorEditor",
                window,
                _ => {
                    entry.Sl = labelVar.Value;
                    NotifyEntriesChanged(true);
                }
            );
    }
}
