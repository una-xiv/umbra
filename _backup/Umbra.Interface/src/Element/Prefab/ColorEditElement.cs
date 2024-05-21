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
using Dalamud.Interface;
using ImGuiNET;

namespace Umbra.Interface;

public class ColorEditElement : Element
{
    public uint Value { get; set; }

    public event Action<uint>? OnValueChanged;

    public ColorEditElement(string id, Anchor anchor, uint value = 0xFF0000FF) : base(id)
    {
        Anchor = anchor;
        Value  = value;
        Size   = new(18, 18);
    }

    protected override void Draw(ImDrawListPtr drawList)
    {
        Vector4 value = ImGui.ColorConvertU32ToFloat4(Value);

        ImGui.PushID($"CP_{Id}");
        ImGui.SetCursorScreenPos(ContentBox.Min);

        ImGui.PushStyleVar(ImGuiStyleVar.GrabRounding,  8);
        ImGui.PushStyleVar(ImGuiStyleVar.GrabMinSize,   16);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 8);
        ImGui.PushStyleColor(ImGuiCol.SliderGrab,       0xFFA0A0A0);
        ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, 0xFFFFFFFF);
        ImGui.PushStyleColor(ImGuiCol.FrameBg,          0x80101010);
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered,   0x80151515);
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive,    0xD0101010);

        if (ImGui.ColorEdit4("", ref value, ImGuiColorEditFlags.NoLabel | ImGuiColorEditFlags.NoBorder | ImGuiColorEditFlags.NoInputs)) {
            Value = ImGui.ColorConvertFloat4ToU32(value);
            OnValueChanged?.Invoke(Value);
        }

        ImGui.PopStyleColor(5);
        ImGui.PopStyleVar(3);
        ImGui.PopID();
    }
}
