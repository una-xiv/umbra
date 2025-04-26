using System;
using System.Collections.Generic;

namespace Umbra.Windows.Library.VariableEditor;

public sealed class StringSelectVariable(string id) : Variable(id), IDisposable
{
    public Dictionary<string, string> Choices { get; set; } = [];

    public string Value {
        get => _value;
        set
        {
            if (_value == value) return;

            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    private string _value = string.Empty;

    public event Action<string>? ValueChanged;

    public override void Dispose()
    {
        foreach (Delegate handler in ValueChanged?.GetInvocationList() ?? []) {
            ValueChanged -= (Action<string>)handler;
        }

        ValueChanged = null;
    }
}
