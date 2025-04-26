using ImGuiNET;
using System;
using Umbra.Common.Extensions;
using Una.Drawing;

namespace Umbra.Windows.Components;

public class IntegerInputNode : ImGuiInputNode
{
    public event Action<int>? OnValueChanged;

    public int Value {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public int MinValue { get; set; }
    public int MaxValue { get; set; }

    private int _value;

    public IntegerInputNode(string id, int value, int min, int max, string label, string? description = null,
                            int    leftMargin = 36)
    {
        _value      = value;
        Id          = id;
        MinValue    = min;
        MaxValue    = max;
        Label       = label;
        Description = description;
        LeftMargin  = leftMargin;
    }

    public IntegerInputNode()
    {
        MinValue = int.MinValue;
        MaxValue = int.MaxValue;
    }

    protected override void DrawImGuiInput(Rect bounds)
    {
        ImGui.SetNextItemWidth(bounds.Width);

        if (ImGui.InputInt($"##{InternalId.Slugify()}", ref _value, 1, 10)) {
            _value = Math.Clamp(_value, MinValue, MaxValue);
        }

        if (ImGui.IsItemDeactivatedAfterEdit()) {
            OnValueChanged?.Invoke(_value);
        }
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? []) OnValueChanged -= (Action<int>)handler;

        OnValueChanged = null;

        base.OnDisposed();
    }
}
