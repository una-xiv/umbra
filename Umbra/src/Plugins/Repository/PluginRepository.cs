using Newtonsoft.Json;

namespace Umbra.Plugins.Repository;

internal static class PluginRepository
{
    internal static event Action<PluginEntry>? EntryAdded;
    internal static event Action<PluginEntry>? EntryRemoved;
    
    /// <summary>
    /// True if a restart is required to apply the changes made to the list of plugins.
    /// </summary>
    public static bool IsRestartRequired { get; private set; } = false;

    [ConfigVariable("PluginEntries")] private static string PluginEntriesRaw { get; set; } = string.Empty;

    private static List<PluginEntry> PluginEntries { get; } = [];
    
    public static PluginEntry? FindEntryFromRepository(string repositoryOwner, string repositoryName)
    {
        return PluginEntries.FirstOrDefault(e =>
            e.RepositoryOwner.Equals(repositoryOwner, StringComparison.OrdinalIgnoreCase) &&
            e.RepositoryName.Equals(repositoryName, StringComparison.OrdinalIgnoreCase));
    }

    public static List<PluginEntry> Entries => PluginEntries.OrderBy(p => p.FilePath).ToList();
    
    public static void AddEntry(PluginEntry entry, bool isInitialLoad = false)
    {
        // Check if an entry with the same data already exists.
        foreach (var existingEntry in PluginEntries) {
            if (existingEntry.FilePath == entry.FilePath && existingEntry.RepositoryOwner == entry.RepositoryOwner) {
                Logger.Warning($"Plugin entry already exists: {entry.FilePath}");
                return;
            }
        }

        PluginEntries.Add(entry);

        if (!isInitialLoad) {
            PersistEntries();
            IsRestartRequired = true;
        }
        
        EntryAdded?.Invoke(entry);
    }

    public static void AddUpdatedEntry(PluginEntry updatedEntry)
    {
        PluginEntry? oldEntry = PluginEntries.FirstOrDefault(e =>
            e.Type == updatedEntry.Type &&
            e.RepositoryOwner == updatedEntry.RepositoryOwner &&
            e.RepositoryName == updatedEntry.RepositoryName &&
            e.Name == updatedEntry.Name
        );

        if (oldEntry == null) return;
        
        RemoveEntry(oldEntry);
        AddEntry(updatedEntry);
    }
    
    public static void RemoveEntry(PluginEntry entry)
    {
        PluginEntries.Remove(entry);
        PersistEntries();
        
        EntryRemoved?.Invoke(entry);
    }
    
    [WhenFrameworkCompiling(executionOrder: int.MinValue)]
    public static void LoadPluginEntries()
    {
        // Don't enable plugins if the user hasn't explicitly enabled them.
        if (!PluginManager.CustomPluginsEnabled) return;
        if (string.IsNullOrWhiteSpace(PluginEntriesRaw)) return;

        try {
            var entries = JsonConvert.DeserializeObject<List<PluginEntry>>(PluginEntriesRaw);
            if (entries != null) {
                foreach (var entry in entries) AddEntry(entry, true);
            }
        } catch (Exception e) {
            Logger.Warning($"Failed to load plugin entries: {e.Message}");
        }
    }

    private static void PersistEntries()
    {
        ConfigManager.Set("PluginEntries", JsonConvert.SerializeObject(PluginEntries));
    }

    internal class AddResult
    {
        
    }
}
