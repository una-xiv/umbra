/* Umbra.Drawing | (c) 2024 by Una      ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Drawing is free software: you can       \/     \/             \/
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
using Newtonsoft.Json;

namespace Umbra.Common;

public static class ConfigManager
{
    private static readonly Dictionary<string, Cvar> Cvars = [];

    private static Timer? _debounceTimer;
    private static bool   _isInitialLoad;

    [WhenFrameworkCompiling(executionOrder: int.MinValue)]
    public static void GatherConfigVariableUsages()
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

            if (attr.Category != null) {
                cvar.Category = attr.Category;
            }

            if (attr.Min != float.MinValue) {
                cvar.Min = attr.Min;
            }

            if (attr.Max != float.MaxValue) {
                cvar.Max = attr.Max;
            }

            if (attr.Step != 0) {
                cvar.Step = attr.Step;
            }

            if (attr.Options.Count > 0) {
                cvar.Options = attr.Options;
            }

            if (attr.RequiresRestart) {
                cvar.RequiresRestart = true;
            }

            if (!EqualityComparer<object>.Default.Equals(cvar.Default, defaultValue)) {
                throw new($"Config variable {id} has conflicting default values.");
            }

            cvar.Properties.Add(prop);
        }

        _isInitialLoad = true;
        Load();
        _isInitialLoad = false;
    }

    [WhenFrameworkDisposing]
    public static void WhenFrameworkDisposing()
    {
        Cvars.Clear();
    }

    public static List<string> GetCategories()
    {
        return Cvars
            .Values
            .Select(cvar => cvar.Category)
            .Where(c => c != null && I18N.Has($"CVAR.Group.{c}"))
            .Distinct()
            .OrderBy(c => c)
            .ToList()!;
    }

    public static Cvar? GetCvar(string id)
    {
        return Cvars.GetValueOrDefault(id);
    }

    public static List<Cvar> GetVariablesFromCategory(string category)
    {
        return Cvars
            .Values
            .Where(cvar => cvar.Category == category && I18N.Has($"CVAR.{cvar.Id}.Name") && cvar.Category != null)
            .ToList();
    }

    public static void Set(string id, object? value, bool persist = true)
    {
        if (!Cvars.TryGetValue(id, out Cvar? cvar)) {
            throw new($"Config variable {id} does not exist.");
        }

        foreach (var prop in cvar.Properties) {
            if (prop.PropertyType.IsEnum) {
                value = Enum.Parse(prop.PropertyType, value!.ToString()!);
            } else if (prop.PropertyType == typeof(int)
                    && value is long longValue) {
                value = (int)longValue;
            } else if (prop.PropertyType == typeof(float)
                    && value is double doubleValue) {
                value = (float)doubleValue;
            } else if (prop.PropertyType == typeof(bool)
                    && value is int intValue) {
                value = intValue != 0;
            } else if (prop.PropertyType == typeof(uint)
                    && value is not uint) {
                value = Convert.ToUInt32(value);
            }

            prop.SetValue(null, value);
        }

        if (cvar.Value == value) return;

        cvar.Value = value;

        if (!_isInitialLoad && cvar.RequiresRestart) {
            _debounceTimer?.Dispose();
            Persist();
            Framework.Restart();
            return;
        }

        if (persist) {
            _debounceTimer?.Dispose();
            _debounceTimer = new Timer(Persist, null, 1000, Timeout.Infinite);
        }
    }

    public static T? Get<T>(string id)
    {
        if (!Cvars.TryGetValue(id, out Cvar? cvar)) {
            throw new($"Config variable {id} does not exist.");
        }

        return (T?)cvar.Value;
    }

    internal static void Persist()
    {
        Persist(null);
    }

    private static void Persist(object? _)
    {
        var data = new Dictionary<string, object?>();

        foreach (var cvar in Cvars.Values) {
            data[cvar.Id] = cvar.Value;
        }

        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(GetConfigFile().FullName, json);
    }

    private static void Load()
    {
        if (!GetConfigFile().Exists) {
            return;
        }

        var json = File.ReadAllText(GetConfigFile().FullName);
        var data = JsonConvert.DeserializeObject<Dictionary<string, object?>>(json);

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

    private static FileInfo GetConfigFile()
    {
        if (!Framework.DalamudPlugin.ConfigDirectory.Exists) {
            Framework.DalamudPlugin.ConfigDirectory.Create();
        }

        return new(Path.Combine(Framework.DalamudPlugin.ConfigDirectory.FullName, "cvars.json"));
    }
}

public class Cvar(string id, object? defaultValue)
{
    public readonly   string             Id      = id;
    public readonly   object?            Default = defaultValue;
    public            List<string>?      Options = [];
    public            string?            Category;
    public            object?            Value = defaultValue;
    public            float?             Min;
    public            float?             Max;
    public            float?             Step;
    internal readonly List<PropertyInfo> Properties = [];
    public            bool               RequiresRestart;
}
