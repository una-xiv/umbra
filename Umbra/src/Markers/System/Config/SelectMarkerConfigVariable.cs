using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Markers;

public class SelectMarkerConfigVariable(
    string                     id,
    string                     name,
    string?                    description,
    string                     defaultValue,
    Dictionary<string, string> options,
    bool                       allowCustom = false
) : MarkerConfigVariable<string>(id, name, description, defaultValue)
{
    public Dictionary<string, string> Options { get; set; } = options;

    /// <inheritdoc/>
    protected override string Sanitize(object? value)
    {
        if (value is not string str) {
            return string.Empty;
        }

        if (!allowCustom && !Options.ContainsKey(str)) {
            Logger.Warning($"Invalid value for {Id} in {Name} ({value}) Available options: {string.Join(", ", Options.Keys)}");
            return DefaultValue;
        }

        return str;
    }
}
