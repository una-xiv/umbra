using Umbra.Game.Script;

namespace Umbra.Widgets;

public class StringWidgetConfigVariable(
    string  id,
    string  name,
    string? description,
    string  defaultValue,
    short   maxLength         = 255,
    bool    supportsScripting = false
)
    : WidgetConfigVariable<string>(id, name, description, defaultValue)
{
    public short MaxLength { get; set; } = maxLength;
    
    public bool SupportsScripting { get; set; } = supportsScripting;

    private UmbraScript? _script;
    private string?      _scriptValue;
    
    /// <inheritdoc/>
    protected override string Sanitize(object? value)
    {
        if (value is not string str) return string.Empty;

        return MaxLength > 0 && str.Length > MaxLength ? str[..MaxLength] : str;
    }
    
    public string EvaluatedValue
    {
        get
        {
            if (!SupportsScripting) return Value;
            
            if (_script is null || _scriptValue != Value) {
                try {
                    _script?.Dispose();
                    _script      = UmbraScript.Parse(Value);
                    _scriptValue = Value;
                } catch {
                    return Value;
                }
            }
            
            return _script.Value;
        }
    }

    public override void Dispose()
    {
        _script?.Dispose();
        
        base.Dispose();
    }
}
