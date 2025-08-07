

namespace Umbra.Windows.Components;

public class CheckboxNode : BoxedInputNode
{
    public event Action<bool>? OnValueChanged;

    public bool Value {
        get => _value;
        set
        {
            if (_value == value) return;
            _value            = value;
            BoxNode.NodeValue = value ? "☑" : "☐";
            OnValueChanged?.Invoke(value);
        }
    }

    private bool _value;

    public CheckboxNode(string id, bool value, string label, string? description = null)
    {
        _value      = value;
        Id          = id;
        Label       = label;
        Description = description;

        OnClick += _ => { Value = !Value; };
    }

    public CheckboxNode()
    {
        OnClick += _ => { Value = !Value; };
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        base.OnDraw(drawList);
        BoxNode.NodeValue = Value ? FontAwesomeIcon.Check.ToIconString() : null;
    }

    public void SetValueInternal(bool c) => _value = c;

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? []) OnValueChanged -= (Action<bool>)handler;
        OnValueChanged = null;

        base.OnDisposed();
    }
}
