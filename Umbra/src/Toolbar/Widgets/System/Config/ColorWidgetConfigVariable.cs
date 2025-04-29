using System;

namespace Umbra.Widgets;

public class ColorWidgetConfigVariable(string id, string name, string? description, uint defaultValue)
    : WidgetConfigVariable<uint>(id, name, description, defaultValue)
{
    /// <inheritdoc/>
    protected override uint Sanitize(object? value)
    {
        if (value is null) return 0;

        try {
            return Convert.ToUInt32(value);
        } catch {
            return 0xFFFFFFFF;
        }
    }
}
