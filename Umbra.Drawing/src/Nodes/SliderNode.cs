/* Umbra.Drawing | (c) 2024 by Una      ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Drawing is free software: you can       \/     \/             \/ 
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Numerics;
using Dalamud.Interface.Internal;
using ImGuiNET;

namespace Umbra.Drawing;

public sealed class SliderNode(string? id, float value = 0, float minValue = 0, float maxValue = 1.0f, Size? size = null) : INode
{
    public string? Id { get; set; } = id;
    public bool IsVisible { get; set; } = true;
    public float Value { get; set; } = value;
    public float MinValue { get; set; } = minValue;
    public float MaxValue { get; set; } = maxValue;
    public Size? Size { get; set; } = size;

    public event Action<float>? OnValueChanged;

    public void Render(ImDrawListPtr drawList, Rect rect, float elementOpacity)
    {
        var p = ImGui.GetCursorScreenPos();

        drawList.AddLine(new Vector2(rect.X, rect.Y + (rect.Height / 2)), new Vector2(rect.X + rect.Width, rect.Y + (rect.Height / 2)), 0xFF505050);

        ImGui.SetCursorScreenPos(new Vector2(rect.X, rect.Y));
        ImGui.PushStyleColor(ImGuiCol.FrameBg, 0);
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, 0);
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive, 0);
        ImGui.PushStyleColor(ImGuiCol.SliderGrab, 0xFFC0C0C0);
        ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, 0xFFFFFFFF);

        ImGui.PushItemWidth(rect.Width);
        ImGui.PushID(Id);
        var value = Value;
        if (ImGui.SliderFloat("", ref value, MinValue, MaxValue, "", ImGuiSliderFlags.AlwaysClamp)) {
            Value = value;
            OnValueChanged?.Invoke(value);
        }

        ImGui.PopID();
        ImGui.PopItemWidth();
        ImGui.PopStyleColor(5);
        ImGui.SetCursorScreenPos(p);
    }

    public Size? GetComputedSize()
    {
        return Size ?? null;
    }
}