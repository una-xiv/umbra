using System;
using System.IO;
using System.Reflection;
using Umbra.Common;
using Umbra.Plugins.Repository;

namespace Umbra.Plugins;

internal class Plugin(PluginEntry entry) : IDisposable
{
    public PluginEntry Entry      { get; private set; } = entry;
    public FileInfo?   File       { get; private set; }
    public Assembly?   Assembly   { get; private set; }
    public bool        IsDisposed { get; private set; }

    private PluginLoadContext? _context;

    public void Load()
    {
        File = new(Entry.FilePath);

        if (!File.Exists) {
            Entry.LoadError = $"File not found: {File.FullName}";
            return;
        }

        try {
            _context = new(File.Directory!);
            Assembly = _context.LoadFromFile(File.FullName);

            Framework.Assemblies.Add(Assembly);
        } catch (Exception e) {
            Entry.LoadError = e.Message;
            Dispose();
            Logger.Error($"Failed to load plugin: {File.FullName}: {e.Message}");
        }
    }

    public void Dispose()
    {
        if (Assembly != null) {
            Framework.Assemblies.Remove(Assembly);
        }

        _context?.Unload();
        Assembly   = null;
        IsDisposed = true;
    }
}
