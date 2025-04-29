using System;

namespace Umbra.Widgets;

public interface IUntypedWidgetConfigVariable
{
    public event Action<object>? UntypedValueChanged;

    /// <summary>
    /// Returns an untyped version of the default value of this variable.
    /// </summary>
    public object? GetDefaultValue();

    /// <summary>
    /// Sets the value of this variable to the given untyped value.
    /// </summary>
    public void SetValue(object value);

    public void Dispose();
}
