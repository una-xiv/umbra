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

using System.Numerics;
using ImGuiNET;

namespace Umbra.Drawing;

/// <summary>
/// A node that renders wrapped text.
/// </summary>
/// <param name="id">The ID of this node.</param>
/// <param name="text">The text to render.</param>
/// <param name="color">The color of the text.</param>
/// <param name="opacity">The opacity of the text.</param>
/// <param name="font">The font to use for the text.</param>
public sealed class WrappedTextNode(
    string? id = null,
    string text = "",
    uint color = 0xFFFFFFFF,
    float opacity = 1f,
    Font font = Font.Default
) : INode
{
    /// <summary>
    /// The ID of this node.
    /// </summary>
    public string? Id { get; set; } = id;

    /// <summary>
    /// Determines whether this node is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// The text to render.
    /// </summary>
    public string Text { get; set; } = text;

    /// <summary>
    /// The color of the text.
    /// </summary>
    public uint Color { get; set; } = color;

    /// <summary>
    /// The opacity of the text.
    /// </summary>
    public float Opacity { get; set; } = opacity;

    /// <summary>
    /// The font to use for the text.
    /// </summary>
    public Font Font { get; set; } = font;

    public float ComputedHeight { get; private set; }

    private static   uint _lastId;
    private readonly uint _id     = ++_lastId;

    private float? _lastKnownWidth;
    private string? _cacheKey;
    private Size? _cachedSize;

    public void Render(ImDrawListPtr drawList, Rect rect, float elementOpacity)
    {
        Vector2 pos     = rect.Position;
        Vector2 oldPos  = ImGui.GetCursorScreenPos();
        _lastKnownWidth = rect.Size.Width;

        FontRepository.PushFont(Font);

        ImGui.SetCursorScreenPos(pos);
        ImGui.PushStyleColor(ImGuiCol.Text, Color.ApplyAlphaComponent(elementOpacity * Opacity));
        ImGui.PushStyleColor(ImGuiCol.FrameBg, 0);
        ImGui.BeginChildFrame(_id, new(rect.Width, rect.Height), ImGuiWindowFlags.NoScrollbar);
        ImGui.SetCursorPos(new(0, 0));
        ImGui.TextWrapped(Text);
        ImGui.EndChildFrame();
        ImGui.PopStyleColor(2);
        ImGui.SetCursorScreenPos(oldPos);

        FontRepository.PopFont(Font);
    }

    public Size? GetComputedSize()
    {
        if (null == _lastKnownWidth || _lastKnownWidth < 1) return null;
        if (_cacheKey == NewCacheKey && null != _cachedSize) return _cachedSize;

        _cacheKey = NewCacheKey;

        FontRepository.PushFont(Font);
        var size = ImGui.CalcTextSize(Text, false, _lastKnownWidth.Value);
        FontRepository.PopFont(Font);
        ComputedHeight = size.Y;

        return _cachedSize = new(size.X, size.Y);
    }

    private string NewCacheKey => $"{Text}_{Font}_{_lastKnownWidth}";
}
