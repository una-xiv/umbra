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
using ImGuiNET;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Windows.ConfigWindow;

internal sealed partial class ConfigWindow : Window
{
    protected override string Id => "ConfigWindow";

    private string _selectedCategory;

    public ConfigWindow()
    {
        Title       = I18N.Translate("ConfigWindow.Title");
        DefaultSize = new(800, 600);
        MinSize     = new(650, 480);
        MaxSize     = new(1200, 900);

        _selectedCategory = ConfigManager.GetCategories().First();
        ConfigManager.GetCategories().ForEach(AddCategory);

        BuildAppearanceButton();
        BuildAppearancePanel();
    }

    protected override void OnDraw(int instanceId)
    {
        var size = ImGui.GetWindowSize();
        var pos  = ImGui.GetWindowPos();

        _windowElement.Size                 = new((int)size.X, (int)size.Y);
        _windowElement.Get("NavPanel").Size = new(200, (int)size.Y);
        _windowElement.Get("Main").Size     = new((int)size.X - 200, (int)size.Y);

        _windowElement.Render(ImGui.GetWindowDrawList(), pos);
    }
}
