using System;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using Una.Drawing;

namespace Umbra.Windows.Components;

public class VerticalSliderNode : Node
{
    public event Action<int>? OnValueChanged;

    public int Value {
        get => _value;
        set {
            int newValue = Math.Clamp(value, MinValue, MaxValue);
            if (newValue == _value) return;

            _value = newValue;
            OnValueChanged?.Invoke(_value);
        }
    }

    public int MinValue { get; set; } = 0;
    public int MaxValue { get; set; } = 100;

    private int _value;

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        Rect rect = Bounds.ContentRect;
        Size size = Bounds.ContentSize;

        ImGui.SetCursorScreenPos(rect.TopLeft);
        ImGui.PushID($"VS##{Id}");

        int value = _value;

        ImGui.PushStyleVar(ImGuiStyleVar.GrabRounding, 8);
        ImGui.PushStyleVar(ImGuiStyleVar.GrabMinSize, 14);

        ImGui.PushStyleColor(ImGuiCol.FrameBg,          0);
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered,   0x10000000);
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive,    0x30000000);
        ImGui.PushStyleColor(ImGuiCol.SliderGrab,       Color.GetNamedColor("Widget.PopupMenuTextDisabled"));
        ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, Color.GetNamedColor("Widget.PopupMenuTextHover"));

        if (ImGui.VSliderInt("##VS", size.ToVector2(), ref value, MinValue, MaxValue, "")) {
            Value = value;
        }

        ImGui.PopStyleColor(3);
        ImGui.PopStyleVar(2);
        ImGui.PopID();
    }
}
