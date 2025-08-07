using Umbra.Windows.Library.VariableEditor;

namespace Umbra.Widgets;

public class EnumWidgetConfigVariable<T>(
    string  id,
    string  name,
    string? description,
    T       defaultValue)
    : WidgetConfigVariable<T>(id, name, description, defaultValue), IEnumWidgetConfigVariable
    where T : struct, Enum
{
    protected override T Sanitize(object? value)
    {
        if (value is T t) return t;
        if (value is not string str) return DefaultValue;

        return Enum.TryParse(str, out T result) ? result : DefaultValue;
    }

    public Variable CreateEnumVariable(ToolbarWidget widget)
    {
        EnumVariable<T> enumVar = new(Id) {
            Name        = Name,
            Category    = Category,
            Description = Description,
            Value       = Value,
            Group       = Group,
            DisplayIf   = DisplayIf
        };

        enumVar.ValueChanged += v => widget.SetConfigValue(enumVar.Id, v);

        return enumVar;
    }
}

public interface IEnumWidgetConfigVariable
{
    public Variable CreateEnumVariable(ToolbarWidget widget);
}
