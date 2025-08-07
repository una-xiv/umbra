using System.Collections.Immutable;
using System.Reflection;

namespace Umbra.Game.Script.Filters;

public static class FilterFunctionRegistry
{
    private static Dictionary<string, MethodInfo> FilterFunctions    { get; } = [];
    private static Dictionary<string, string>     FilterDescriptions { get; } = [];

    [WhenFrameworkCompiling(executionOrder: Int32.MaxValue)]
    public static void RegisterFilterFunctions()
    {
        List<MethodInfo> methods =
            Framework.Assemblies
                     .SelectMany(asm => asm.GetTypes())
                     .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<ServiceAttribute>() != null)
                     .SelectMany(t => t.GetMethods())
                     .Where(m => m.GetCustomAttribute<ScriptFilterAttribute>() != null)
                     .ToList();

        foreach (MethodInfo method in methods) {
            ScriptFilterAttribute? filterAttribute = method.GetCustomAttribute<ScriptFilterAttribute>();

            if (filterAttribute == null) {
                continue;
            }

            if (!FilterFunctions.TryAdd(filterAttribute.Name, method)) {
                throw new InvalidOperationException($"Filter function with name '{filterAttribute.Name}' is already registered.");
            }

            FilterDescriptions[filterAttribute.Name] = filterAttribute.Description;
        }
    }
    
    public static ImmutableDictionary<string, string> All => 
        FilterDescriptions.ToImmutableDictionary();

    public static bool Has(string name) => FilterFunctions.ContainsKey(name);

    public static string Invoke(string name, string input)
    {
        if (!FilterFunctions.TryGetValue(name, out MethodInfo? method)) {
            throw new KeyNotFoundException($"Filter function '{name}' not found.");
        }
        
        object? instance = method.IsStatic ? null : Framework.Service<object>(method.DeclaringType!);
        object? result = method.Invoke(instance, [input]);
        
        if (result is not string strResult) {
            throw new InvalidOperationException($"Filter function '{name}' did not return a string.");
        }
        
        return strResult;
    }
    
    [WhenFrameworkDisposing]
    public static void ClearFilterFunctions()
    {
        FilterFunctions.Clear();
        FilterDescriptions.Clear();
    }
}
