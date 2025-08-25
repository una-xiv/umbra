using Dalamud.Plugin;

namespace Umbra.Widgets;

internal partial class DtrSingleWidget
{
    private Dictionary<string, string> AllEntries { get; set; } = [];

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        AllEntries = new() { { "", "" } };

        foreach (var entry in DtrBar.Entries.OrderBy(e => e.Title)) {
            AllEntries[entry.Title] = entry.Title;
        }

        return [
            new SelectWidgetConfigVariable(
                "SelectedEntry",
                I18N.Translate("Widget.DtrSingle.Config.SelectedEntry.Name"),
                I18N.Translate("Widget.DtrSingle.Config.SelectedEntry.Description"),
                "",
                AllEntries
            ),
            ..base.GetConfigVariables(),
        ];
    }
}
