using System;

namespace Umbra.Windows.Library.VariablesWindow;

public sealed class IntegerVariable(string id) : Variable(id), IDisposable
{
    public int Min   { get; set; } = int.MinValue;
    public int Max   { get; set; } = int.MaxValue;
    public int Value {
        get => _value;
        set
        {
            if (_value == value) return;

            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    private int _value;

    public event Action<int>? ValueChanged;

    public override void Dispose()
    {
        foreach (Delegate handler in ValueChanged?.GetInvocationList() ?? []) {
            ValueChanged -= (Action<int>)handler;
        }

        ValueChanged = null;
    }
}
