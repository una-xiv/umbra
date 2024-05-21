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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Umbra.Common;

public static partial class ConfigManager
{
    private static readonly Dictionary<string, Cvar> Cvars = [];

    private static Timer? _debounceTimer;
    private static bool   _isInitialLoad;

    internal static void Initialize()
    {
        CollectVariablesFromAssemblies();
        RestoreProfileAssociations();

        _isInitialLoad = true;
        Load();
        _isInitialLoad = false;
    }

    internal static void Dispose()
    {
        _debounceTimer?.Dispose();
        Cvars.Clear();
    }

    internal static void Persist()
    {
        Persist(null);
    }

    private static void Persist(object? _)
    {
        // Safety net to prevent overwriting the config file with an empty object.
        if (Cvars.Count == 0) return;

        var data = new Dictionary<string, object?>();

        foreach (var cvar in Cvars.Values) {
            data[cvar.Id] = cvar.Value;
        }

        WriteFile(GetCurrentConfigFileName(), data);
    }

    private static void LoadWithoutRestart()
    {
        _isInitialLoad = true;
        Load();
        _isInitialLoad = false;
    }

    private static void Load()
    {
        if (!FileExists(GetCurrentConfigFileName())) {
            return;
        }

        var data = ReadFile<Dictionary<string, object?>>(GetCurrentConfigFileName());

        if (null == data) return;

        foreach (var cvar in Cvars.Values) {
            if (data.TryGetValue(cvar.Id, out object? value)) {
                Set(cvar.Id, ConvertValue(value, cvar.Default!.GetType()), false);
            }
        }
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null) return null;

        if (targetType.IsEnum) {
            return Enum.Parse(targetType, value.ToString()!);
        }

        if (targetType == typeof(int)
         && value is long longValue) {
            return (int)longValue;
        }

        return value;
    }
}

public class Cvar(string id, object? defaultValue)
{
    public readonly   string             Id      = id;
    public readonly   object?            Default = defaultValue;
    public            List<string>?      Options = [];
    public            string?            Category;
    public            string?            SubCategory;
    public            object?            Value = defaultValue;
    public            float?             Min;
    public            float?             Max;
    public            float?             Step;
    internal readonly List<PropertyInfo> Properties = [];
    public            bool               RequiresRestart;

    public string Slug => Id.Replace(".", "_");
}
