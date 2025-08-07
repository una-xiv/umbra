namespace Umbra.Windows.Library.VariableEditor;

public sealed class FaIconVariable(string id) : Variable(id), IDisposable
{
    public FontAwesomeIcon Value {
        get => _value;
        set
        {
            if (_value == value) return;

            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    private FontAwesomeIcon _value = FontAwesomeIcon.None;

    public event Action<FontAwesomeIcon>? ValueChanged;

    public override void Dispose()
    {
        foreach (Delegate handler in ValueChanged?.GetInvocationList() ?? []) {
            ValueChanged -= (Action<FontAwesomeIcon>)handler;
        }

        ValueChanged = null;
    }
}
