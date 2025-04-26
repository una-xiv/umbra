using System;

namespace Umbra.Windows.Library.VariableEditor;

public sealed class FloatVariable(string id) : Variable(id), IDisposable
{
    public float Min   { get; set; } = float.MinValue;
    public float Max   { get; set; } = float.MaxValue;
    public float Value {
        get => _value;
        set
        {
            if (_value == value) return;

            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    private float _value;

    public event Action<float>? ValueChanged;

    public override void Dispose()
    {
        foreach (Delegate handler in ValueChanged?.GetInvocationList() ?? []) {
            ValueChanged -= (Action<float>)handler;
        }

        ValueChanged = null;
    }
}
