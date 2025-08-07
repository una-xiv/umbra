using Dalamud.Game.Text;

namespace Umbra.Windows.Library.VariableEditor;

public sealed class GameGlyphVariable(string id) : Variable(id), IDisposable
{
    public SeIconChar Value {
        get => _value;
        set
        {
            if (_value == value) return;

            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    private SeIconChar _value = SeIconChar.Cross;

    public event Action<SeIconChar>? ValueChanged;

    public override void Dispose()
    {
        foreach (Delegate handler in ValueChanged?.GetInvocationList() ?? []) {
            ValueChanged -= (Action<SeIconChar>)handler;
        }

        ValueChanged = null;
    }
}
