using System;

namespace Umbra.Game.Script;

abstract class ScriptPlaceholder(string name, string description) : IDisposable
{
    public event Action<string>? OnValueChanged;

    public string Name        { get; } = name;
    public string Description { get; } = description;

    public string Value {
        get => _value;
        set
        {
            if (!string.Equals(value, _value, StringComparison.OrdinalIgnoreCase)) {
                _value = value;
                OnValueChanged?.Invoke(value);
            }
        }
    }

    private string _value = string.Empty;

    public void Dispose()
    {
        if (null == OnValueChanged) return;
        
        foreach (var d in OnValueChanged.GetInvocationList()) {
            OnValueChanged -= (Action<string>)d;
        }
        
        OnValueChanged = null;
        _value = string.Empty;
    }
}
