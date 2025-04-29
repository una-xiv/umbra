using Dalamud.Game.Text.SeStringHandling;
using System;

namespace Umbra.Windows.Library.VariableEditor;

public sealed class BitmapIconVariable(string id) : Variable(id), IDisposable
{
    public BitmapFontIcon Value {
        get => _value;
        set
        {
            if (_value == value) return;

            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    private BitmapFontIcon _value = BitmapFontIcon.None;

    public event Action<BitmapFontIcon>? ValueChanged;

    public override void Dispose()
    {
        foreach (Delegate handler in ValueChanged?.GetInvocationList() ?? []) {
            ValueChanged -= (Action<BitmapFontIcon>)handler;
        }

        ValueChanged = null;
    }
}
