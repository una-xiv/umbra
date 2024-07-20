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

using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class AppearanceModule : SettingsModule
{
    public override string Id   { get; } = "AppearanceModule";
    public override string Name { get; } = I18N.Translate("Settings.AppearanceModule.Name");

    public AppearanceModule()
    {
        CreateFontControlNodes();
        CreatePopupAppearancePanel();
        CreateColorProfileNodes();
        CreateColorControlNodes();

        OnColorProfileChanged();
    }

    public override void OnOpen()
    {
        UmbraColors.OnColorProfileChanged += OnColorProfileChanged;
        ConfigManager.CurrentProfileChanged += OnConfigProfileChanged;

        OnConfigProfileChanged("");
    }

    public override void OnClose()
    {
        UmbraColors.OnColorProfileChanged -= OnColorProfileChanged;
        ConfigManager.CurrentProfileChanged -= OnConfigProfileChanged;
    }

    public override void OnUpdate()
    {
        UpdateNodeSizes();
        UpdateProfileInputs();
    }

    private void OnColorProfileChanged()
    {
        _selectedProfile = UmbraColors.GetCurrentProfileName();

        foreach ((string id, ColorInputNode node) in _colorPickers) {
            node.Value = Color.GetNamedColor(id);
        }

        UpdateColorPickerVisibilityState();
    }

    private void OnConfigProfileChanged(string _)
    {
        var defaultFontNameNode = FontPanel.QuerySelector<SelectNode>("#DefaultFont");
        var defaultFontSizeNode = FontPanel.QuerySelector<FloatInputNode>("#DefaultFontSize");
        var monospaceFontNameNode = FontPanel.QuerySelector<SelectNode>("MonospaceFont");
        var monospaceFontSizeNode = FontPanel.QuerySelector<FloatInputNode>("MonospaceFontSize");
        var emphasisFontNameNode = FontPanel.QuerySelector<SelectNode>("EmphasisFont");
        var emphasisFontSizeNode = FontPanel.QuerySelector<FloatInputNode>("EmphasisFontSize");

        if (defaultFontNameNode is null || defaultFontSizeNode is null ||
            monospaceFontNameNode is null || monospaceFontSizeNode is null ||
            emphasisFontNameNode is null || emphasisFontSizeNode is null) return;

        defaultFontNameNode.Value   = ConfigManager.Get<string>("Font.Default.Name") ?? "Dalamud Default";
        defaultFontSizeNode.Value   = ConfigManager.Get<float>("Font.Default.Size");
        monospaceFontNameNode.Value = ConfigManager.Get<string>("Font.Monospace.Name") ?? "Dalamud Default";
        monospaceFontSizeNode.Value = ConfigManager.Get<float>("Font.Monospace.Size");
        emphasisFontNameNode.Value  = ConfigManager.Get<string>("Font.Emphasis.Name") ?? "Dalamud Default";
        emphasisFontSizeNode.Value  = ConfigManager.Get<float>("Font.Emphasis.Size");

        _activeProfileNode.Choices = UmbraColors.GetColorProfileNames();
        _activeProfileNode.Value   = UmbraColors.GetCurrentProfileName();
        _selectedProfile           = UmbraColors.GetCurrentProfileName();

        UpdateColorPickerVisibilityState();
    }

    private void UpdateColorPickerVisibilityState()
    {
        bool isBuiltInProfile = UmbraColors.IsBuiltInProfile(_selectedProfile);

        Node.FindById("ColorPickerDisabledText")!.Style.IsVisible = isBuiltInProfile;

        foreach (string id in Color.GetAssignedNames()) {
            string[] parts    = id.Split('.');
            string   category = parts[0];

            if (!_categoryNodes.TryGetValue(category, out Node? categoryNode)) {
                continue;
            }

            categoryNode.Style.IsVisible = !isBuiltInProfile;
        }
    }
}
