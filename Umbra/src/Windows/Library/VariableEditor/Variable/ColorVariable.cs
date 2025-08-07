namespace Umbra.Windows.Library.VariableEditor;

public sealed class ColorVariable(string id) : Variable(id), IDisposable
{
    public uint Min   { get; set; } = 0;
    public uint Max   { get; set; } = 0xFFFFFFFF;
    public uint Value {
        get => _value;
        set
        {
            if (_value == value) return;

            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    private uint _value;

    public event Action<uint>? ValueChanged;

    public override void Dispose()
    {
        foreach (Delegate handler in ValueChanged?.GetInvocationList() ?? []) {
            ValueChanged -= (Action<uint>)handler;
        }

        ValueChanged = null;
    }
}
