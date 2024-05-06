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

public class StringInputElement : Element
{
    public string Value     { get; set; }
    public uint   MinLength { get; set; }
    public uint   MaxLength { get; set; }

    public event Action<string>? OnValueChanged;

    public StringInputElement(
        string id, Anchor anchor, string value = "", uint minLength = 0, uint maxLength = 100
    ) : base(id)
    {
        Anchor    = anchor;
        Value     = value;
        MinLength = minLength;
        MaxLength = maxLength;
        Size      = new(0, 32);
    }

    protected override void Draw(ImDrawListPtr drawList)
    {
        string value = Value;

        ImGui.PushID($"SI_{Id}");
        ImGui.SetCursorScreenPos(ContentBox.Min);

        FontRepository.PushFont(Font.Axis);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,  new Vector2(8, 4));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5);
        ImGui.PushStyleColor(ImGuiCol.Text,           Theme.Color(ThemeColor.Text));
        ImGui.PushStyleColor(ImGuiCol.FrameBg,        Theme.Color(ThemeColor.BackgroundActive));
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, Theme.Color(ThemeColor.BackgroundLight));
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive,  Theme.Color(ThemeColor.BackgroundActive));

        ImGui.SetNextItemWidth(
            (Parent!.ContentBox.Width / ImGui.GetIO().FontGlobalScale) - (Parent!.Padding.Horizontal * ScaleFactor)
        );

        if (ImGui.InputText("", ref value, MaxLength, ImGuiInputTextFlags.None | ImGuiInputTextFlags.EnterReturnsTrue)) {
            if (value.Length < MinLength) return;
            if (value.Length > MaxLength) return;

            if (Value != value) {
                Value = value;
                OnValueChanged?.Invoke(Value);
            }
        }

        ImGui.PopStyleColor(4);
        ImGui.PopStyleVar(2);
        ImGui.PopID();
        FontRepository.PopFont(Font.Axis);
    }
}
