
using Umbra.Common.Extensions;

namespace Umbra.Windows.Components;

public class FloatInputNode : ImGuiInputNode
{
    public event Action<float>? OnValueChanged;

    public float Value {
        get => _value;
        set
        {
            if (Math.Abs(_value - value) < 0.01f) return;
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public float MinValue { get; set; }
    public float MaxValue { get; set; }

    private float _value;

    public FloatInputNode(
        string  id,
        float   value,
        float   min,
        float   max,
        string  label,
        string? description = null,
        int     leftMargin  = 36
    )
    {
        _value      = value;
        Id          = id;
        MinValue    = min;
        MaxValue    = max;
        LeftMargin  = leftMargin;
        Label       = label;
        Description = description;
    }

    public FloatInputNode()
    {
        MinValue = float.MinValue;
        MaxValue = float.MaxValue;
    }

    protected override void DrawImGuiInput(Rect bounds)
    {
        ImGui.SetNextItemWidth(bounds.Width);

        if (ImGui.InputFloat($"##{InternalId.Slugify()}", ref _value, 0.1f, 1.0f, "%.2f")) {
            _value = Math.Clamp(_value, MinValue, MaxValue);
        }

        if (ImGui.IsItemDeactivatedAfterEdit()) {
            OnValueChanged?.Invoke(_value);
        }
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? []) OnValueChanged -= (Action<float>)handler;

        OnValueChanged = null;

        base.OnDisposed();
    }
}
