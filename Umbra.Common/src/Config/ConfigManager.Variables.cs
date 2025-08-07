/* Umbra.Common | (c) 2024 by Una       ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Common is free software: you can       \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

namespace Umbra.Common;

public static partial class ConfigManager
{
    private static void CollectVariablesFromAssemblies()
    {
        var props = Framework
            .Assemblies.SelectMany(asm => asm.GetTypes())
            .SelectMany(type => type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(prop => prop.GetCustomAttribute(typeof(ConfigVariableAttribute)) != null)
            .ToList();

        foreach (var prop in props) {
            var     attr         = prop.GetCustomAttribute<ConfigVariableAttribute>()!;
            string  id           = attr.Id;
            object? defaultValue = prop.GetValue(null);

            if (!Cvars.TryGetValue(id, out Cvar? cvar)) {
                cvar      = new(id, defaultValue);
                Cvars[id] = cvar;
            }

            if (attr.Category != null) cvar.Category       = attr.Category;
            if (attr.SubCategory != null) cvar.SubCategory = attr.SubCategory;
            if (attr.Min != float.MinValue) cvar.Min       = attr.Min;
            if (attr.Max != float.MaxValue) cvar.Max       = attr.Max;
            if (attr.Step != 0) cvar.Step                  = attr.Step;
            if (attr.Options.Count > 0) cvar.Options       = attr.Options;
            if (attr.RequiresRestart) cvar.RequiresRestart = true;

            if (!EqualityComparer<object>.Default.Equals(cvar.Default, defaultValue)) {
                throw new($"Config variable {id} has conflicting default values.");
            }

            cvar.Properties.Add(prop);
        }
    }
}
