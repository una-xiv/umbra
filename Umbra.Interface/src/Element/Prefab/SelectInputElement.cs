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
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace Umbra.Interface;

public class SelectInputElement : Element
{
    public string       Value   { get; set; }
    public List<string> Options { get; set; }

    public event Action<string>? OnValueChanged;

    public SelectInputElement(
        string id, Anchor anchor, string value, List<string> options
    ) : base(id)
    {
        Anchor  = anchor;
        Value   = value;
        Options = options;
        Size    = new(0, 32);
    }

    protected override void Draw(ImDrawListPtr drawList)
    {
        string value = Value;

        ImGui.PushID($"II_{Id}");
        ImGui.SetCursorScreenPos(ContentBox.Min);

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,  new Vector2(8, 4));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5);
        ImGui.PushStyleColor(ImGuiCol.Text,           Theme.Color(ThemeColor.Text));
        ImGui.PushStyleColor(ImGuiCol.FrameBg,        Theme.Color(ThemeColor.BackgroundActive));
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, Theme.Color(ThemeColor.BackgroundLight));
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive,  Theme.Color(ThemeColor.BackgroundActive));

        ImGui.SetNextItemWidth((Parent!.ContentBox.Width / ScaleFactor) - (Parent!.Padding.Horizontal * ScaleFactor));

        if (ImGui.BeginCombo("", value)) {
            foreach (string option in Options) {
                bool isSelected = option == value;

                if (ImGui.Selectable(option, isSelected)) {
                    if (Value != option) {
                        Value = option;
                        OnValueChanged?.Invoke(Value);
                    }
                }

                if (isSelected) {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndCombo();
        }

        ImGui.PopStyleColor(4);
        ImGui.PopStyleVar(2);
        ImGui.PopID();
    }
}
