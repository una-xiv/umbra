namespace Umbra.Widgets;

public class StringWidgetConfigVariable(
    string  id,
    string  name,
    string? description,
    string  defaultValue,
    short   maxLength = 255
)
    : WidgetConfigVariable<string>(id, name, description, defaultValue)
{
    public short MaxLength { get; set; } = maxLength;

    /// <inheritdoc/>
    protected override string Sanitize(object? value)
    {
        if (value is not string str) return string.Empty;

        return MaxLength > 0 && str.Length > MaxLength ? str[..MaxLength] : str;
    }
}
