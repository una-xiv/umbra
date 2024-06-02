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
                if (info is null) return;

                string name = I18N.Has(info.Name) ? I18N.Translate(info.Name) : info.Name;
                string desc = I18N.Has(info.Description) ? I18N.Translate(info.Description) : info.Description;

                RegisteredWidgets.Add((type, new(info.Id, name, desc)));
            });
    }

    [WhenFrameworkDisposing]
    private static void WhenFrameworkDisposing()
    {
        RegisteredWidgets.Clear();
    }
}
