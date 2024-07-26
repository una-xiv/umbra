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
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Umbra.Common;
using Framework = Umbra.Common.Framework;
using Task = System.Threading.Tasks.Task;

namespace Umbra.Plugins;

internal static class PluginManager
{
    [ConfigVariable("CustomPluginPaths")] private static string CustomPluginsRaw { get; set; } = string.Empty;

    public static List<Plugin> Plugins { get; private set; } = [];

    private static Dictionary<string, long> PluginTimestamps  { get; }      = [];
    private static List<string>             CustomPluginPaths { get; set; } = [];
    private static bool                     HasRemovedPlugins { get; set; }
    private static bool                     IsDisposing       { get; set; }

    public static Plugin? AddPlugin(string path, bool store = true)
    {
        FileInfo file = new(path);

        if (Plugins.Any(p => p.File.FullName == file.FullName)) {
            Logger.Warning($"Another plugin with the same path is already loaded: {file.Name}");
            return null;
        }

        var plugin = new Plugin(path);
        Plugins.Add(plugin);

        if (store) {
            CustomPluginPaths.Add(path);
            StoreCustomPluginsPaths();
        }

        return plugin;
    }

    public static void RemovePlugin(Plugin plugin)
    {
        if (null == plugin.Assembly) {
            Plugins.Remove(plugin);
        } else {
            try {
                Framework.Assemblies.Remove(plugin.Assembly!);
                plugin.Dispose();
            } catch (Exception e) {
                Logger.Warning($"Failed to dispose plugin: {plugin.File.Name} ({e.Message})");
            }

            HasRemovedPlugins = true;
        }

        CustomPluginPaths.Remove(plugin.File.FullName);
        StoreCustomPluginsPaths();
    }

    public static bool IsRestartRequired()
    {
        if (HasRemovedPlugins) return true;

        var restartRequired = false;

        foreach (Plugin plugin in Plugins) {
            if (string.IsNullOrEmpty(plugin.LoadError) && plugin.Assembly is null) {
                restartRequired = true;
                break;
            }
        }

        return restartRequired;
    }

    [WhenFrameworkCompiling(executionOrder: int.MinValue)]
    private static void LoadCustomPlugins()
    {
        if (string.IsNullOrEmpty(CustomPluginsRaw)) return;
        List<string>? customPluginPaths = JsonConvert.DeserializeObject<List<string>>(CustomPluginsRaw);

        if (customPluginPaths == null) return;
        CustomPluginPaths = customPluginPaths;

        foreach (string pluginPath in CustomPluginPaths) {
            if (string.IsNullOrEmpty(pluginPath)) continue;
            var plugin = AddPlugin(pluginPath, false);

            if (null == plugin) continue;

            try {
                plugin.Load();
            } catch (Exception) {
                Logger.Warning($"Failed to load plugin: {plugin.File.Name} (Incompatible version)");
                plugin.Dispose();
            }

            if (string.IsNullOrEmpty(plugin.LoadError) && plugin.Assembly != null) {
                Logger.Info($"Loaded plugin: {plugin.Assembly.FullName}");
                PluginTimestamps[plugin.File.FullName] = plugin.File.LastWriteTime.Ticks;
            } else {
                Logger.Warning($"Failed to load plugin: {plugin.File.Name} ({plugin.LoadError})");
            }
        }

        IsDisposing = false;
        Task.Run(WatchForPluginFileChanges);
    }

    [WhenFrameworkDisposing]
    private static void Dispose()
    {
        HasRemovedPlugins = false;
        IsDisposing       = true;

        foreach (Plugin plugin in Plugins) {
            if (plugin.IsDisposed) continue;

            try {
                plugin.Dispose();
                Logger.Info($"Disposed plugin: {plugin.File.Name}");
            } catch (Exception e) {
                Logger.Warning($"Failed to dispose plugin: {plugin.File.Name} ({e.Message})");
            }
        }

        Plugins.Clear();
        CustomPluginPaths.Clear();
        PluginTimestamps.Clear();
    }

    private static void StoreCustomPluginsPaths()
    {
        ConfigManager.Set("CustomPluginPaths", JsonConvert.SerializeObject(CustomPluginPaths));
    }

    private static async void WatchForPluginFileChanges()
    {
        if (IsDisposing) return;

        bool requiresRestart = false;

        foreach ((string path, long timestamp) in PluginTimestamps) {
            FileInfo file = new(path);

            if (file.LastWriteTime.Ticks != timestamp) {
                Logger.Info($"Plugin file changed: {file.Name}");
                requiresRestart = true;
                break;
            }
        }

        await Task.Delay(500);

        if (requiresRestart) {
            Framework.Restart();
            return;
        }

        WatchForPluginFileChanges();
    }
}
