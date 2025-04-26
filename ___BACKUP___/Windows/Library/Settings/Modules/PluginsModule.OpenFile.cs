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

using System.Numerics;
using System.Reflection;
using Dalamud.Interface.ImGuiFileDialog;
using ImGuiNET;
using Umbra.Common;
using Umbra.Plugins;

namespace Umbra.Windows.Settings.Modules;

internal sealed partial class PluginsModule : SettingsModule
{
    private FileDialogManager? FileDialogManager { get; set; }

    private void UpdateOpenFileDialog()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    new Vector2(8));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,     new Vector2(4));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding,   8);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1);
        ImGui.PushStyleColor(ImGuiCol.Border, 0xFF000000);

        FileDialogManager?.Draw();

        ImGui.PopStyleColor(1);
        ImGui.PopStyleVar(4);
    }

    private void ShowOpenFileDialog()
    {
        FileDialogManager!.OpenFileDialog(
            "Open Umbra Plugin File",
            ".dll",
            (isOk, fileName) => {
                if (isOk) {
                    var plugin = PluginManager.AddPlugin(fileName[0]);
                    if (null != plugin) AddPluginNode(plugin);
                }
            },
            1,
            startPath: Framework.DalamudPlugin.AssemblyLocation.Directory!.FullName
        );
    }

    private void TryLoad(string fileName)
    {
        Logger.Info("Try load: " + fileName);
    }
}
