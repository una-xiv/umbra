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

using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;
using Lumina.Misc;
using System;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget("PluginList", "Widget.PluginList.Name", "Widget.PluginList.Description")]
internal sealed partial class PluginListWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : IconToolbarWidget(info, guid, configValues)
{
    public override MenuPopup Popup { get; } = new();

    private readonly List<string> _usedPluginIds = [];

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Popup.OnPopupOpen  += UpdatePluginList;
        Popup.OnPopupClose += ClearPluginList;

        Node.OnRightClick += _ => Framework.Service<IChatSender>().Send("/xlplugins");

        SetIcon(FontAwesomeIcon.Plug);
    }

    /// <inheritdoc/>
    protected override void OnDisposed()
    {
        Popup.OnPopupOpen  -= UpdatePluginList;
        Popup.OnPopupClose -= ClearPluginList;
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        base.OnUpdate();
        Node.Tooltip = GetConfigValue<bool>("ShowTooltip") ? I18N.Translate("Widget.PluginList.Tooltip") : null;
    }

    private void UpdatePluginList()
    {
        Framework.DalamudFramework.Run(UpdatePluginListInternal);
    }

    private void UpdatePluginListInternal()
    {
        List<IExposedPlugin> pluginList = [..Framework.DalamudPlugin.InstalledPlugins];
        pluginList.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));

        List<string> usedPluginIds = [];

        foreach (var plugin in pluginList) {
            if (!plugin.IsLoaded || plugin is { HasMainUi: false, HasConfigUi: false }) continue;

            string id = $"Plugin_{Crc32.Get(plugin.InternalName)}";

            if (HasConfigVariable($"Enabled{id}") && !GetConfigValue<bool>($"Enabled{id}")) continue;
            if (Popup.HasButton(id)) continue;

            usedPluginIds.Add(id);

            Popup.AddButton(
                id,
                plugin.Name,
                onClick: () => {
                    if ((ImGui.GetIO().KeyShift || ImGui.GetIO().KeyCtrl) && plugin.HasConfigUi) {
                        plugin.OpenConfigUi();
                        return;
                    }

                    if (plugin.HasMainUi) {
                        plugin.OpenMainUi();
                    } else {
                        plugin.OpenConfigUi();
                    }
                }
            );
        }

        // Remove buttons for plugins that are no longer installed.
        foreach (var id in _usedPluginIds) {
            if (!usedPluginIds.Contains(id)) {
                Popup.RemoveButton(id);
            }
        }

        _usedPluginIds.Clear();
        _usedPluginIds.AddRange(usedPluginIds);
    }

    private void ClearPluginList()
    {
        Framework.DalamudFramework.Run(
            () => {
                foreach (var id in _usedPluginIds) {
                    Popup.RemoveButton(id);
                }

                _usedPluginIds.Clear();
            }
        );
    }
}
