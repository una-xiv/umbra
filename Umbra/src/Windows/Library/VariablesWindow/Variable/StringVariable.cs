using System;

namespace Umbra.Windows.Library.VariablesWindow;

public sealed class StringVariable(string id) : Variable(id), IDisposable
{
    public uint MaxLength { get; set; } = 255;
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
