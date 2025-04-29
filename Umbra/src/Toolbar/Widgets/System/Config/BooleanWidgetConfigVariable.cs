namespace Umbra.Widgets;

public class BooleanWidgetConfigVariable(string id, string name, string? description, bool defaultValue)
    : WidgetConfigVariable<bool>(id, name, description, defaultValue)
{
    /// <inheritdoc/>
    protected override bool Sanitize(object? value)
    {
        if (value is null) return false;

        return (bool)value;
    }
}
