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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ImGuiNET;
using Umbra.Common;
using Umbra.Drawing;

namespace Umbra.Windows.Config;

[ChatCommandInvokable("config",   "Show the configuration window.")]
[ChatCommandInvokable("settings", "Alias of config.")]
internal sealed partial class ConfigWindow : Window
{
    protected override string Id => "UmbraConfig";

    private readonly List<ConfigCategoryButton>  _categoryButtons = [];
    private readonly Dictionary<string, Element> _categoryPanels  = [];
    private          string                      _activeCategory  = "";

    public ConfigWindow()
    {
        Title   = "Umbra Configuration";
        MinSize = new(550, 380);
        MaxSize = new(640, 480);
        Padding = new(1, 1);

        AddElement(_root);

        BuildCategoryButtons();
        BuildCategoryPanels();
    }

    protected override void OnBeforeDraw()
    {
        var height = ImGui.GetWindowHeight() - 2;

        _root.Get("Menu").Size      = new(150, height);
        _root.Get("Separator").Size = new(2, height);
        _root.Get("Panels").Size    = new(ContentSize.Width - 156, height);

        _categoryButtons.ForEach(button => button.IsActive = button.Id == _activeCategory);

        foreach (var key in _categoryPanels.Keys) {
            var panel = _categoryPanels[key];

            panel.IsVisible = key.Equals(_activeCategory);
            if (!panel.IsVisible) continue;

            UpdatePanel(panel);
        }
    }

    protected override void OnAfterDraw() { }

    /// <summary>
    /// Converts a category name into a slug.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string Slugify(string input)
    {
        // Replace any non-alphanumeric character with an underscore.
        return SlugifyRegex().Replace(input.ToLower(), "_");
    }

    private void UpdatePanel(Element panel)
    {
        // Update the width of the panel to match the window.
        panel.Size = new(ImGui.GetWindowSize().X - 156, panel.Size.Height);

        panel.Children.ForEach(
            child => {
                if (child is ConfigVariableCheckbox checkbox) {
                    var desc = checkbox.Get("Label.Description").GetNode<WrappedTextNode>();

                    checkbox.Size                          = new(panel.Size.Width, 0);
                    checkbox.Get("Label").Size             = new(panel.Size.Width - 50, 0);
                    checkbox.Get("Label.Name").Size        = new(panel.Size.Width - 50, 0);
                    checkbox.Get("Label.Description").Size = new(panel.Size.Width - 50, desc.ComputedHeight);
                    return;
                }

                if (child is ConfigVariableNumber number) {
                    var desc = number.Get("Label.Description").GetNode<WrappedTextNode>();

                    number.Size                          = new(panel.Size.Width, 0);
                    number.Get("Label").Size             = new(panel.Size.Width - 50, 0);
                    number.Get("Label.Name").Size        = new(panel.Size.Width - 50, 0);
                    number.Get("Label.Description").Size = new(panel.Size.Width - 50, desc.ComputedHeight);
                    number.Get("Label.Input").Size       = new(panel.Size.Width - 50, 32);
                    return;
                }

                if (child is ConfigVariableEnum @enum) {
                    var desc = @enum.Get("Label.Description").GetNode<WrappedTextNode>();

                    @enum.Size                          = new(panel.Size.Width, 0);
                    @enum.Get("Label").Size             = new(panel.Size.Width - 50, 0);
                    @enum.Get("Label.Name").Size        = new(panel.Size.Width - 50, 0);
                    @enum.Get("Label.Description").Size = new(panel.Size.Width - 50, desc.ComputedHeight);
                    @enum.Get("Label.Buttons").Size     = new(panel.Size.Width - 50, 32);
                }
            }
        );
    }

    /// <summary>
    /// Builds the category buttons that show up in the left panel.
    /// </summary>
    private void BuildCategoryButtons()
    {
        string? firstCategory = null;

        ConfigManager
            .GetCategories()
            .ForEach(
                category => {
                    firstCategory ??= category;

                    var button = new ConfigCategoryButton(Slugify(category), category)
                        { IsActive = category == firstCategory };

                    button.OnClick += () => _activeCategory = Slugify(category);

                    _categoryButtons.Add(button);
                    _root.Get("Menu.Items").AddChild(button);
                }
            );

        if (null != firstCategory) _activeCategory = Slugify(firstCategory);
    }

    private void BuildCategoryPanels()
    {
        ConfigManager
            .GetCategories()
            .ForEach(
                category => {
                    var panel = new Element(
                        id: Slugify(category),
                        direction: Direction.Vertical,
                        anchor: Anchor.Top | Anchor.Left,
                        gap: 12,
                        padding: new(6, 6, 0, 6)
                    ) { IsVisible = false };

                    ConfigManager
                        .GetVariablesFromCategory(category)
                        .ForEach(
                            cvar => {
                                // If cvar.DefaultValue is a boolean...
                                if (cvar.Default is bool) {
                                    var checkbox = new ConfigVariableCheckbox(cvar);
                                    panel.AddChild(checkbox);
                                }

                                // If cvar.DefaultValue is a number...
                                if (cvar.Default is int
                                 || cvar.Default is uint) {
                                    var numberInput = new ConfigVariableNumber(cvar);
                                    panel.AddChild(numberInput);
                                }

                                // If cvar.DefaultValue is an enum...
                                if (cvar.Default is Enum) {
                                    var enumInput = new ConfigVariableEnum(cvar);
                                    panel.AddChild(enumInput);
                                }
                            }
                        );

                    _categoryPanels.Add(Slugify(category), panel);
                    _root.Get("Panels").AddChild(panel);
                }
            );
    }

    [GeneratedRegex("[^a-z0-9]")]
    private static partial Regex SlugifyRegex();
}
