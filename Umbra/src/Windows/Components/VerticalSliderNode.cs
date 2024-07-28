/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using ImGuiNET;
using Una.Drawing;

namespace Umbra.Windows.Components;

internal class VerticalSliderNode : Node
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
    public int Step     { get; set; } = 1;

    private int _value;

    /// <summary>
    /// Sets the value of the slider without triggering the <see cref="OnValueChanged"/> event.
    /// </summary>
    /// <param name="value"></param>
    public void SetValue(int value)
    {
        _value = Math.Clamp(value, MinValue, MaxValue);
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        Rect rect = Bounds.ContentRect;
        Size size = Bounds.ContentSize;

        ImGui.SetCursorScreenPos(rect.TopLeft);
        ImGui.PushID($"VS##{Id}");

        int value = _value;

        ImGui.PushStyleVar(ImGuiStyleVar.GrabRounding, 8);
        ImGui.PushStyleVar(ImGuiStyleVar.GrabMinSize,  14);

        ImGui.PushStyleColor(ImGuiCol.FrameBg,          0);
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered,   0x10000000);
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive,    0x30000000);
        ImGui.PushStyleColor(ImGuiCol.SliderGrab,       Color.GetNamedColor("Widget.PopupMenuTextDisabled"));
        ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, Color.GetNamedColor("Widget.PopupMenuTextHover"));

        if (ImGui.VSliderInt("##VS", size.ToVector2(), ref value, MinValue, MaxValue, "")) {
            Value = (value / Step) * Step;
        }

        ImGui.PopStyleColor(5);
        ImGui.PopStyleVar(2);
        ImGui.PopID();
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? [])  OnValueChanged -= (Action<int>)handler;
    }
}
