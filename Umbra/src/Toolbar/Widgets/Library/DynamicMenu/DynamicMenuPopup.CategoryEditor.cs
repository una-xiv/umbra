using Umbra.Windows;
using Umbra.Windows.Library.VariableEditor;

namespace Umbra.Widgets;

internal sealed partial class DynamicMenuPopup
{
    private void OpenCategoryEditor()
    {
        if (null == _selectedItemIndex) return;
        DynamicMenuEntry entry = Entries[_selectedItemIndex.Value];

        StringVariable labelVar = new("Label") {
            Name = I18N.Translate("Widget.DynamicMenu.CategoryEditor.Label.Name"),
            Description = I18N.Translate("Widget.DynamicMenu.CategoryEditor.Label.Description"),
            Value = entry.Cl ?? "",
            MaxLength = 100,
        };

        List<Variable> variables = [labelVar];
        VariablesEditorWindow window = new(I18N.Translate("Widget.DynamicMenu.CategoryEditor.Title"), variables, []);

        Framework
            .Service<WindowManager>()
            .Present(
                "CategoryEditor",
                window,
                _ => {
                    entry.Cl = labelVar.Value;
                    RebuildMenu();
                    OnEntriesChanged?.Invoke();
                }
            );
    }
}
