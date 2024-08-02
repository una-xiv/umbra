/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using Umbra.Common;

namespace Umbra.Widgets;

public abstract class WidgetConfigVariable<T>(string id, string name, string? description, T defaultValue)
    : IWidgetConfigVariable, IUntypedWidgetConfigVariable, IDisposable
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
    /// A description of this variable that is visible in the configuration
    /// interface for the widget this variable belongs to.
    /// </summary>
    public string? Description { get; } = description;

    /// <summary>
    /// The default (initial) value of this variable.
    /// </summary>
    public T DefaultValue { get; } = defaultValue;

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

    public void Dispose()
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
