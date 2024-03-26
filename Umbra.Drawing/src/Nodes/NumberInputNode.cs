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
using ImGuiNET;

namespace Umbra.Drawing;

public sealed class NumberInputNode(
    string? id = null,
    uint color = 0xFFFFFFFF,
    Font font = Font.Default,
    Spacing margin = new(),
    Vector2 offset = new(),
    int value = 0,
    int min = int.MinValue,
    int max = int.MaxValue,
    int step = 1
) : INode
{
    public event Action<int>? OnValueChanged;

    /// <summary>
    /// The ID of the node.
    /// </summary>
    public string? Id { get; set; } = id;

    /// <summary>
    /// Determines whether the node is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// The text color of the input box.
    /// </summary>
    public uint Color { get; set; } = color;

    /// <summary>
    /// The font to use for the input box.
    /// </summary>
    public Font Font { get; set; } = font;

    /// <summary>
    /// The margin around the input box.
    /// </summary>
    public Spacing Margin { get; set; } = margin;

    /// <summary>
    /// The position offset of the input box.
    /// </summary>
    public Vector2 Offset { get; set; } = offset;

    /// <summary>
    /// The minimum value.
    /// </summary>
    public int Min { get; set; } = min;

    /// <summary>
    /// The maximum value.
    /// </summary>
    public int Max { get; set; } = max;

    /// <summary>
    /// The step value.
    /// </summary>
    public int Step { get; set; } = step;

    /// <summary>
    /// The current value.
    /// </summary>
    public int Value { get; set; } = value;

    private static uint _lastId;
    private readonly uint _id = _lastId++;

    public void Render(ImDrawListPtr drawList, Rect rect, float elementOpacity)
    {
        var start = new Vector2(rect.X + Margin.Left + Offset.X, rect.Y + Margin.Top + Offset.Y);
        var end   = new Vector2(rect.X + rect.Width - Margin.Right, rect.Y + rect.Height - Margin.Bottom);
        var pos   = ImGui.GetCursorScreenPos();

        ImGui.SetCursorScreenPos(start);
        FontRepository.PushFont(Font);

        ImGui.PushStyleColor(ImGuiCol.FrameBg, 0x00000000);
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, 0x00000000);
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive, 0x00000000);
        ImGui.PushStyleColor(ImGuiCol.Text, Color);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, new Vector2(0, 0));
        ImGui.BeginChild($"##NumberInput_{Id}_{_id}", end - start, false);
        ImGui.SetNextItemWidth(rect.Width - Margin.Left - Margin.Right);

        var v = Value;
        if (ImGui.InputInt("", ref v, Step, Step)) {
            var newValue = Math.Clamp(v, Min, Max);

            if (newValue != Value) {
                Value = newValue;
                OnValueChanged?.Invoke(Value);
            }
        }

        ImGui.EndChild();
        ImGui.PopStyleVar(4);
        ImGui.PopStyleColor(4);
        FontRepository.PopFont(Font);

        ImGui.SetCursorScreenPos(pos);
    }

    public Size? GetComputedSize()
    {
        return null;
    }
}
