using System;
using Umbra.Common;
using Umbra.Windows.Library.VariableEditor;

namespace Umbra.Widgets;

public abstract class WidgetConfigVariable<T>(string id, string name, string? description, T defaultValue, string? group = null)
    : IWidgetConfigVariable, IUntypedWidgetConfigVariable, IConfigurableWidgetConfigVariable, IDisposable
{
    public event Action<T>? ValueChanged;

    /// <inheritdoc/>
    public event Action<object>? UntypedValueChanged;

    /// <summary>
    /// Specifies the internal name of this variable.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// Whether this variable should be hidden from the configuration interface.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Specifies the display name of this variable.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Specifies the category of this variable.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The name of the variable group this one belongs to.
    /// When multiple variables within the same category share the same group
    /// name, they are rendered in a collapsible group.
    /// </summary>
    public string? Group { get; set; } = group;
    
    /// <summary>
    /// A description of this variable that is visible in the configuration
    /// interface for the widget this variable belongs to.
    /// </summary>
    public string? Description { get; } = description;

    /// <summary>
    /// The default (initial) value of this variable.
    /// </summary>
    public T DefaultValue { get; } = defaultValue;

    /// <summary>
    /// Determines whether this variable should be displayed in the
    /// configuration window.
    /// </summary>
    public Variable.DisplayIfDelegate? DisplayIf { get; set; }
    
    /// <summary>
    /// The current value of this variable.
    /// </summary>
    public T Value {
        get => _value;
        set {
            T sanitized = Sanitize(value);
            if (sanitized!.Equals(_value)) return;

            _value = sanitized;
            ValueChanged?.Invoke(_value);
            UntypedValueChanged?.Invoke(_value);
        }
    }

    private T _value = defaultValue;

    /// <inheritdoc/>
    public void SetValue(object value)
    {
        _value = Sanitize(value);
        ValueChanged?.Invoke(_value);
        UntypedValueChanged?.Invoke(value);
    }

    /// <inheritdoc/>
    public object? GetDefaultValue() => DefaultValue;

    /// <summary>
    /// Sanitizes the given value to ensure it is valid for this variable.
    /// </summary>
    /// <remarks>
    /// The returned value MUST be of the same type as the generic type and
    /// MUST NOT be null.
    /// </remarks>
    /// <param name="value"></param>
    /// <returns></returns>
    protected abstract T Sanitize(object? value);

    public virtual void Dispose()
    {
        if (null != UntypedValueChanged) {
            foreach (var delegateHandler in UntypedValueChanged.GetInvocationList()) {
                UntypedValueChanged -= (Action<object>)delegateHandler;
            }
        }

        if (null != ValueChanged) {
            foreach (var delegateHandler in ValueChanged.GetInvocationList()) {
                ValueChanged -= (Action<T>)delegateHandler;
            }
        }
    }
}
