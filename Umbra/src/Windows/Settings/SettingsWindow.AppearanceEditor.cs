using System.Linq;
using Dalamud.Interface;
using ImGuiNET;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow
{
    private static void BuildAppearanceButton()
    {
        CreateCategory("AppearancePanel", I18N.Translate("Config.Appearance"));
    }

    private static void BuildAppearancePanel()
    {
        Element el = BuildCategoryPanelWrapper("AppearancePanel", I18N.Translate("Config.Appearance"));
        el.Parent!.Get("Header").IsVisible = false;

        Element panel = new(
            id: "AppearancePanel",
            flow: Flow.Vertical,
            gap: 16,
            padding: new(16, 0),
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
                new DropdownSeparatorElement() { Padding = new(0) },
                new(
                    id: "ColorList",
                    flow: Flow.Vertical,
                    anchor: Anchor.TopLeft,
                    gap: 10,
                    children: Theme.ColorNames.Select(CreateThemeColorOption).ToList()
                ),
            ]
        );

        Element presetSelector = panel.Get("PresetSelector");
        Element colorList      = panel.Get("ColorList");

        panel.OnBeforeCompute += () => {
            panel.Size          = new(WindowWidth - 248, 0);
            presetSelector.Size = panel.Size with { Height = 16 };
            colorList.Size      = panel.Size;

            for (var i = 0; i < colorList.Children.Count(); i++) {
                colorList.Children.ElementAt(i).SortIndex = i;
                colorList.Children.ElementAt(i).Size      = panel.Size;
            }
        };

        el.AddChild(panel);
    }
}
