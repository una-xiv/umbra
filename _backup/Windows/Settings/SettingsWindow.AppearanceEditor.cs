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
                            size: new(200, 0),
                            gap: 6,
                            children: [
                                new SelectInputElement(
                                    id: "PresetSelect",
                                    value: "",
                                    anchor: Anchor.MiddleLeft,
                                    options: ThemePresets
                                        .Presets
                                        .Select(p => p.Name)
                                        .ToList()
                                ) {
                                    Padding = new(top: 5),
                                },
                            ]
                        ),
                        new ButtonElement(
                            id: "ApplyPresetButton",
                            label: "Apply"
                        ) { Anchor = Anchor.MiddleLeft, Tooltip = I18N.Translate("Config.AppearanceImport") },
                        new(
                            id: "ImportExportButtons",
                            flow: Flow.Horizontal,
                            anchor: Anchor.MiddleRight,
                            gap: 6,
                            children: [
                                new ButtonElement(
                                    id: "ImportButton",
                                    icon: FontAwesomeIcon.FileImport,
                                    onClick: Theme.ImportFromClipboard
                                ) { Tooltip = I18N.Translate("Config.AppearanceImport") },
                                new ButtonElement(
                                    id: "ExportButton",
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
        Element applyPresetBtn = presetSelector.Get("ApplyPresetButton");

        presetSelector.Get("List").Get<SelectInputElement>().OnValueChanged += value => {
            _selectedPreset = ThemePresets.Presets.FirstOrDefault(p => p.Name == value);
        };

        applyPresetBtn.OnBeforeCompute += () => applyPresetBtn.IsDisabled = _selectedPreset == null;
        applyPresetBtn.OnClick += () => {
            if (_selectedPreset == null) return;
            Theme.ApplyFromPreset(_selectedPreset);
        };

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

    private IThemePreset? _selectedPreset = null;
}
