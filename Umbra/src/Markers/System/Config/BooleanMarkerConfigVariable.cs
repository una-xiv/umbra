namespace Umbra.Markers;

public class BooleanMarkerConfigVariable(string id, string name, string? description, bool defaultValue)
    : MarkerConfigVariable<bool>(id, name, description, defaultValue)
{
    /// <inheritdoc/>
    protected override bool Sanitize(object? value)
    {
        if (value is null) return false;

        return (bool)value;
    }
}
