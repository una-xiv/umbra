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
using Umbra.Plugins;

namespace Umbra.Windows.Settings.Modules;

internal sealed partial class PluginsModule : SettingsModule
{
    public override string Id   { get; } = "SettingsModule";
    public override string Name { get; } = I18N.Translate("Settings.PluginsModule.Title");

    public PluginsModule()
    {
        Node.FindById("AddPlugin")!.OnMouseUp     += _ => ShowOpenFileDialog();
        Node.FindById("RestartButton")!.OnMouseUp += _ => Framework.Restart();

        foreach (Plugins.Plugin plugin in PluginManager.Plugins) {
            AddPluginNode(plugin);
        }
    }

    public override void OnOpen()
    {
        FileDialogManager = new();
    }

    public override void OnClose()
    {
        FileDialogManager = null;
    }

    public override void OnUpdate()
    {
        UpdateNodeSizes();
        UpdateOpenFileDialog();
    }
}
