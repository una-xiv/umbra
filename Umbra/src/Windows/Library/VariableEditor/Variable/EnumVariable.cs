namespace Umbra.Windows.Library.VariableEditor;

public sealed class EnumVariable<T>(string id) : Variable(id), IEnumVariable, IDisposable where T : struct, Enum
{
    public T Value {
        get => _value;
        set
        {
            if (_value.Equals(value)) return;

            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    private T _value;

    public event Action<T>? ValueChanged;

    public override void Dispose()
    {
        foreach (Delegate handler in ValueChanged?.GetInvocationList() ?? []) {
            ValueChanged -= (Action<T>)handler;
        }

        ValueChanged = null;
    }
}

public interface IEnumVariable
{
    public string  Id          { get; }
    public string  Name        { get; }
    public string? Description { get; }
    public string? Group       { get; }
}
