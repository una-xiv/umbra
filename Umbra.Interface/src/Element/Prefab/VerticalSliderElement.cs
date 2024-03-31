/* Umbra.Interface | (c) 2024 by Una    ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Interface is free software: you can    \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Interface is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Numerics;
using ImGuiNET;

namespace Umbra.Interface;

public class VerticalSliderElement : Element
{
    public int Value    { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }

    public event Action<int>? OnValueChanged;

    public VerticalSliderElement(
        string id, Size size, Anchor anchor, int value = 50, int minValue = 0, int maxValue = 100
    ) : base(id)
    {
        Size     = size;
        Anchor   = anchor;
        Value    = value;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    protected override void AfterCompute()
    {
        ComputedSize = new(Parent!.ComputedSize.Width - Padding.Horizontal, 1);
        ComputeBoundingBox();
    }

    protected override void Draw(ImDrawListPtr drawList)
    {
        int value = Value;

        ImGui.PushID($"VS_{Id}");
        ImGui.SetCursorScreenPos(ContentBox.Min);

        ImGui.PushStyleVar(ImGuiStyleVar.GrabRounding,  8);
        ImGui.PushStyleVar(ImGuiStyleVar.GrabMinSize,   16);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 8);
        ImGui.PushStyleColor(ImGuiCol.SliderGrab,       0xFFA0A0A0);
        ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, 0xFFFFFFFF);
        ImGui.PushStyleColor(ImGuiCol.FrameBg,          0x80101010);
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered,   0x80151515);
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive,    0xD0101010);

        if (ImGui.VSliderInt(
                "",
                Size.ToVector2() - new Vector2(0, Padding.Vertical),
                ref value,
                MinValue,
                MaxValue,
                ""
            )) {
            if (Value == value) return;
            Value = value;
            OnValueChanged?.Invoke(Value);
        }

        ImGui.PopStyleColor(5);
        ImGui.PopStyleVar(3);
        ImGui.PopID();
    }
}
