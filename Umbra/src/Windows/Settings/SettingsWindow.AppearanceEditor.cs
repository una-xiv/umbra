/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Linq;
using Dalamud.Interface;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow
{
    private void BuildAppearanceButton()
    {
        CreateCategory("AppearancePanel", I18N.Translate("Config.Appearance"));
    }

    private void BuildAppearancePanel()
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
