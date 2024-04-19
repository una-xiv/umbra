using System.Linq;
using Dalamud.Interface;
using ImGuiNET;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Windows.ConfigWindow;

internal partial class ConfigWindow
{
    private void BuildAppearanceButton()
    {
        CategoryList.AddChild(BuildCategoryButton("AppearancePanel", I18N.Translate("Config.Appearance")));
    }

    private void BuildAppearancePanel()
    {
        Element panel = new(
            id: "AppearancePanel",
            flow: Flow.Vertical,
            gap: 8,
            padding: new(bottom: 0, right: 8),
            children: [
                new(
                    id: "PresetSelector",
                    flow: Flow.Horizontal,
                    anchor: Anchor.TopLeft,
                    gap: 10,
                    children: [
                        new(
                            id: "Label",
                            anchor: Anchor.MiddleLeft,
                            text: $"{I18N.Translate("Config.AppearancePreset")}:",
                            style: new() {
                                TextColor    = Theme.Color(ThemeColor.Text),
                                TextOffset   = new(0, -1),
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1
                            }
                        ),
                        new(
                            id: "List",
                            flow: Flow.Horizontal,
                            anchor: Anchor.MiddleLeft,
                            gap: 6,
                            children: ThemePresets
                                .Presets
                                .Select(
                                    p => new ButtonElement(
                                        Slugify(p.Name),
                                        p.Name,
                                        isGhost: true,
                                        onClick: () => Theme.ApplyFromPreset(p)
                                    ) as Element
                                )
                                .ToList()
                        ),
                        new(
                            id: "ImportExportButtons",
                            flow: Flow.Horizontal,
                            anchor: Anchor.MiddleRight,
                            gap: 6,
                            children: [
                                new ButtonElement(
                                    "ImportButton",
                                    icon: FontAwesomeIcon.FileImport,
                                    onClick: Theme.ImportFromClipboard
                                ) { Tooltip = I18N.Translate("Config.AppearanceImport") },
                                new ButtonElement(
                                    "ExportButton",
                                    icon: FontAwesomeIcon.FileExport,
                                    onClick: Theme.ExportToClipboard
                                ) { Tooltip = I18N.Translate("Config.AppearanceExport") }
                            ]
                        ),
                    ]
                ),
                new DropdownSeparatorElement(),
                new(
                    id: "ColorList",
                    flow: Flow.Vertical,
                    anchor: Anchor.TopLeft,
                    gap: 10,
                    children: Theme.ColorNames.Select(CreateThemeColorOption).ToList()
                ),
                new DropdownSeparatorElement(),
            ]
        ) { IsVisible = false };

        panel.OnBeforeCompute += () => {
            panel.IsVisible                  = _selectedCategory == "AppearancePanel";
            panel.Size                       = new((int)ImGui.GetWindowSize().X - 24, 0);
            panel.Get("PresetSelector").Size = panel.Size;
            panel.Get("ColorList").Size      = panel.Size;

            for (var i = 0; i < panel.Get("ColorList").Children.Count(); i++) {
                panel.Get("ColorList").Children.ElementAt(i).SortIndex = i;
                panel.Get("ColorList").Children.ElementAt(i).Size      = panel.Size;
            }
        };

        Body.AddChild(panel);
    }
}
