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

using Dalamud.Utility;
using ImGuiNET;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow : Window
{
    private int WindowWidth  { get; set; }
    private int WindowHeight { get; set; }

    public SettingsWindow()
    {
        Title   = I18N.Translate("ConfigWindow.Title");
        MinSize = new(800, 600);
        MaxSize = new(1200, 950);

        BuildElements();

        ConfigManager
            .GetCategories()
            .ForEach(
                category => {
                    CreateCategory(category, I18N.Translate($"CVAR.Group.{category}"));
                    BuildCategoryPanel(category, I18N.Translate($"CVAR.Group.{category}"));
                }
            );

        BuildLayoutEditorButton();
        BuildAppearanceButton();
        BuildLayoutEditorPanel();
        BuildAppearancePanel();

        _footerElement.Get("Buttons.Close").OnClick   += Close;
        _footerElement.Get("Buttons.Restart").OnClick += Framework.Restart;
        _footerElement.Get("Buttons.KoFi").OnClick    += () => Util.OpenLink("https://ko-fi.com/una_xiv");

        _currentCategory = "LayoutEditorPanel";
    }

    public override void Dispose()
    {
        _currentCategory   = "";
        _categorySortIndex = 0;

        _navButtonsElement.Clear();
        _mainElement.Clear();
        _panels.Clear();
    }

    protected override string Id => "settings";

    protected override void OnDraw(int instanceId)
    {
        var size = ImGui.GetWindowSize() / Element.ScaleFactor;
        var pos  = ImGui.GetWindowPos();

        WindowWidth  = (int)size.X;
        WindowHeight = (int)size.Y;

        _windowElement.Size    = new(WindowWidth, WindowHeight);
        _footerElement.Size    = new(WindowWidth, 41);
        _workspaceElement.Size = new(WindowWidth, WindowHeight - 42);
        _navPanelElement.Size  = new(220, WindowHeight         - 42);
        _mainElement.Size      = new(WindowWidth               - 220, (int)size.Y - 42);

        _navPanelElement.Get("NavButtonsContainer").Size = new(220, (int)size.Y - (200 + 60));

        _windowElement.Render(ImGui.GetWindowDrawList(), pos);

        var currentPanelId = $"Panel:{_currentCategory}";

        foreach (var panel in _panels.Values) {
            panel.IsVisible = panel.Id == currentPanelId;

            if (panel.IsVisible)
                UpdatePanelDimensions(panel.Get("Panel"), _mainElement.Size.Width, _mainElement.Size.Height);
        }
    }
}
