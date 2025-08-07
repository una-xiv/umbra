namespace Umbra.Windows.Library.VariableEditor;

public sealed class BooleanVariable(string id) : Variable(id), IDisposable
{
    public bool Value {
        get => _value;
        set
        {
            if (_value == value) return;

            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    private bool _value = false;

    public event Action<bool>? ValueChanged;

    public override void Dispose()
    {
        foreach (Delegate handler in ValueChanged?.GetInvocationList() ?? []) {
            ValueChanged -= (Action<bool>)handler;
        }

        ValueChanged = null;
    }
}
