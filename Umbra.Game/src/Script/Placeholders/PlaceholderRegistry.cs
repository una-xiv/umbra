using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Umbra.Common;

namespace Umbra.Game.Script;

internal static class PlaceholderRegistry
{
    public static event Action<string>? OnValueChanged;

    private static Dictionary<string, string>            PlaceholderValues   { get; } = [];
    private static Dictionary<string, ScriptPlaceholder> PlaceholderServices { get; } = [];

    /// <summary>
    /// Returns true if a placeholder with the given name exists.
    /// </summary>
    /// <remarks>
    /// The name of the placeholder should be passed in lowercase.
    /// </remarks>
    /// <param name="name">The name of the placeholder.</param>
    /// <returns></returns>
    public static bool Has(string name) => PlaceholderValues.ContainsKey(name);

    /// <summary>
    /// Returns the current value of the placeholder with the given name.
    /// </summary>
    /// <param name="name">The name of the placeholder.</param>
    /// <returns>The value of the placeholder.</returns>
    /// <exception cref="KeyNotFoundException">If no such placeholder exists.</exception>
    public static string Get(string name)
    {
        if (!PlaceholderValues.TryGetValue(name, out var value)) {
            throw new KeyNotFoundException($"Placeholder '{name}' not found.");
        }

        return value;
    }

    [WhenFrameworkCompiling(executionOrder: Int32.MaxValue)]
    private static void RegisterPlaceholders()
    {
        Type placeholderType = typeof(ScriptPlaceholder);

        List<ScriptPlaceholder> placeholderTypes =
            Framework.Assemblies
                     .SelectMany(asm => asm.GetTypes())
                     .Where(t => t.IsSubclassOf(placeholderType) && !t.IsAbstract)
                     .Where(t => t.GetCustomAttribute<ServiceAttribute>() != null)
                     .Select(Framework.Service<ScriptPlaceholder>)
                     .ToList();

        foreach (var p in placeholderTypes) {
            if (!PlaceholderServices.TryAdd(p.Name, p)) {
                throw new InvalidOperationException($"Placeholder with name '{p.Name}' is already registered.");
            }

            PlaceholderValues[p.Name] = p.Value;

            p.OnValueChanged += value => PlaceholderValues[p.Name] = value;
        }
    }

    [WhenFrameworkDisposing]
    private static void UnregisterPlaceholders()
    {
        foreach (var p in PlaceholderServices.Values) p.Dispose();

        if (OnValueChanged != null) {
            foreach (var p in OnValueChanged.GetInvocationList()) OnValueChanged -= (Action<string>)p;
            OnValueChanged = null;
        }

        PlaceholderValues.Clear();
        PlaceholderServices.Clear();
    }
}
