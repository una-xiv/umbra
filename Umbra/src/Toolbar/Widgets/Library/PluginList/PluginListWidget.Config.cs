﻿/* Umbra | (c) 2024 by Una              ____ ___        ___.
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
using Lumina.Misc;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets;

internal sealed partial class PluginListWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            
            new BooleanWidgetConfigVariable(
                "ShowTooltip",
                I18N.Translate("Widget.PluginList.Config.ShowTooltip.Name"),
                I18N.Translate("Widget.PluginList.Config.ShowTooltip.Description"),
                true
            ),
            
            ..GetPluginListItems()
        ];
    }

    private static List<IWidgetConfigVariable> GetPluginListItems()
    {
        List<IWidgetConfigVariable> pluginList = [];
        List<string>                usedNames  = [];

        IEnumerable<IExposedPlugin> plugins = Framework.DalamudPlugin.InstalledPlugins.ToList();
        plugins = plugins.OrderBy(p => p.Name);

        foreach (IExposedPlugin plugin in plugins) {
            if (usedNames.Contains(plugin.InternalName)) continue;
            
            usedNames.Add(plugin.InternalName);
            
            pluginList.Add(new BooleanWidgetConfigVariable(
                $"EnabledPlugin_{Crc32.Get(plugin.InternalName)}",
                plugin.Name,
                null,
                plugin.InternalName == "Umbra"
            ) { Category = I18N.Translate("Widget.PluginList.Config.EnabledCategory") });
        }

        return pluginList;
    }
}
