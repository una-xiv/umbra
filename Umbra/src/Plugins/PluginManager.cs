using Umbra.Plugins.Repository;

namespace Umbra.Plugins;

internal static class PluginManager
{
    [ConfigVariable("CustomPlugins.Enabled")]
    public static bool CustomPluginsEnabled { get; set; } = false;

    private static List<Plugin> Plugins { get; } = [];
    private static bool HasRemovedPlugins { get; set; }
    private static bool IsDisposing       { get; set; }

    public static bool IsLoaded(PluginEntry entry)
    {
        return Plugins.FirstOrDefault(p => p.Entry == entry) != null;
    }

    public static bool IsRestartRequired()
    {
        // Check against newly added plugins.
        foreach (var entry in PluginRepository.Entries) {
            if (Plugins.FirstOrDefault(p => p.Entry == entry) == null) {
                return true;
            }
        }

        // Check against removed plugins.
        if (HasRemovedPlugins) {
            foreach (var plugin in Plugins) {
                if (PluginRepository.Entries.FirstOrDefault(e => e == plugin.Entry) == null) {
                    return true;
                }
            }
        }

        return false;
    }

    [WhenFrameworkCompiling(executionOrder: int.MinValue + 1)]
    private static void LoadCustomPlugins()
    {
        // Don't load custom plugins if the user has not agreed to the EULA.
        if (!CustomPluginsEnabled) return;

        foreach (PluginEntry entry in PluginRepository.Entries) {
            var plugin = new Plugin(entry);
            
            try {
                plugin.Load();
                Plugins.Add(plugin);
            } catch (Exception) {
                Logger.Warning($"Failed to load plugin {entry.FilePath}: (Incompatible version)");
                plugin.Dispose();
            }

            if (string.IsNullOrEmpty(entry.LoadError) && plugin.Assembly != null) {
                Logger.Info($"Loaded plugin: {entry.FilePath}");
            } else {
                Logger.Warning($"Failed to load plugin: {entry.FilePath} ({entry.LoadError})");
            }
        }

        IsDisposing = false;
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
                Logger.Info($"Disposed plugin: {plugin.File?.Name}");
            } catch (Exception e) {
                Logger.Warning($"Failed to dispose plugin: {plugin.File?.Name} ({e.Message})");
            }
        }

        Plugins.Clear();
    }
}
