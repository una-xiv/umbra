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

using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbra.Common;

namespace Umbra.Widgets.System;

internal static class WidgetRegistry
{
    internal static List<(Type, WidgetInfo)> RegisteredWidgets { get; } = [];

    [WhenFrameworkCompiling(executionOrder: 100)]
    private static void WhenFrameworkCompiling()
    {
        Framework.Assemblies.SelectMany(asm => asm.GetTypes())
            .Where(type => type is { IsClass: true, IsAbstract: false } && type.IsSubclassOf(typeof(ToolbarWidget)))
            .ToList()
            .ForEach(type =>
            {
                var info = type.GetCustomAttribute<ToolbarWidgetAttribute>();
                if (info != null) {
                    string name = I18N.Has(info.Name) ? I18N.Translate(info.Name) : info.Name;
                    string desc = I18N.Has(info.Description) ? I18N.Translate(info.Description) : info.Description;

                    RegisteredWidgets.Add((type, new(info.Id, name, desc)));
                    return;
                }

                var info2 = type.GetCustomAttribute<InteropToolbarWidgetAttribute>();
                if (info2 != null) {
                    // This code runs after the login event, so we can safely check for loaded plugins.
                    IExposedPlugin? plugin = Framework.DalamudPlugin.InstalledPlugins
                        .FirstOrDefault(p => p.InternalName == info2.PluginName && p.IsLoaded);

                    if (plugin == null) {
                        return;
                    }

                    string name = I18N.Has(info2.Name) ? I18N.Translate(info2.Name) : info2.Name;
                    string desc = I18N.Has(info2.Description) ? I18N.Translate(info2.Description) : info2.Description;

                    RegisteredWidgets.Add((type, new(info2.Id, name, desc)));
                }
            });
    }

    [WhenFrameworkDisposing]
    private static void WhenFrameworkDisposing()
    {
        RegisteredWidgets.Clear();
    }
}
