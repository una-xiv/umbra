using ImGuiNET;
using System;
using Umbra.Common.Extensions;
using Una.Drawing;

namespace Umbra.Windows.Components;

public class StringInputNode : ImGuiInputNode
{
    public event Action<string>? OnValueChanged;

    public string Value {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public uint MaxLength { get; set; }

    public bool Immediate { get; set; }

    private string _value;

    public StringInputNode(
        string  id,
        string  value,
        uint    maxLength,
        string? label       = null,
        string? description = null,
        int     leftMargin  = 36,
        bool    immediate   = false
    )
    {
        Id          = id;
        MaxLength   = maxLength;
        LeftMargin  = leftMargin;
        Label       = label;
        Description = description;
        _value      = value;
        Immediate   = immediate;
    }

    public StringInputNode()
    {
        _value    = string.Empty;
        MaxLength = 255;
    }

    protected override void DrawImGuiInput(Rect bounds)
    {
        ImGui.SetNextItemWidth(bounds.Width);

        if (ImGui.InputText($"##{InternalId.Slugify()}", ref _value, MaxLength,
            !Immediate ? ImGuiInputTextFlags.EnterReturnsTrue : ImGuiInputTextFlags.None)) {
            OnValueChanged?.Invoke(_value);
        }

        if (ImGui.IsItemDeactivatedAfterEdit()) {
            OnValueChanged?.Invoke(_value);
        }
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? []) OnValueChanged -= (Action<string>)handler;

        OnValueChanged = null;

        base.OnDisposed();
    }
}
