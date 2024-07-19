using System;
using System.IO;
using System.Reflection;
using Umbra.Common;

namespace Umbra.Plugins;

internal class Plugin(string path) : IDisposable
{
    public FileInfo  File       { get; } = new(path);
    public Assembly? Assembly   { get; private set; }
    public string?   LoadError  { get; private set; }
    public bool      IsDisposed { get; private set; }

    private PluginLoadContext? _context;

    public void Load()
    {
        FileInfo file = new(path);

        if (!file.Exists) {
            LoadError = $"File not found: {file.FullName}";
            return;
        }

        try {
            _context = new(file.Name, file.Directory!);
            Assembly = _context.LoadFromFile(file.FullName);
            Framework.Assemblies.Add(Assembly);
        } catch (Exception e) {
            LoadError = e.Message;
            Logger.Error($"Failed to load plugin: {file.FullName}: {e.Message}");
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

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}
