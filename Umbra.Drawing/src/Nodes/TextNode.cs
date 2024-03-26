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
/// A node that renders text.
/// </summary>
/// <param name="id">The ID of this node.</param>
/// <param name="text">The text to render.</param>
/// <param name="color">The color of the text.</param>
/// <param name="outlineColor">The color of the text outline.</param>
/// <param name="outlineSize">The thickness of the text outline.</param>
/// <param name="opacity">The opacity of the text.</param>
/// <param name="autoSize">Whether this node should expose a bounding box based on the text content.</param>
/// <param name="font">The font to use for the text.</param>
/// <param name="align">The alignment of the text.</param>
/// <param name="margin">The margin of the text.</param>
/// <param name="offset">The offset of the text.</param>
public sealed class TextNode(
    string? id = null,
    string text = "",
    uint color = 0xFFFFFFFF,
    uint outlineColor = 0xA0000000,
    int outlineSize = 0,
    float opacity = 1f,
    bool autoSize = true,
    Font font = Font.Default,
    Align align = Align.TopLeft,
    Spacing margin = new(),
    Vector2 offset = new()
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
    /// The color of the text outline.
    /// </summary>
    public uint OutlineColor { get; set; } = outlineColor;

    /// <summary>
    /// The thickness of the text outline.
    /// </summary>
    public int OutlineThickness { get; set; } = outlineSize;

    /// <summary>
    /// The opacity of the text.
    /// </summary>
    public float Opacity { get; set; } = opacity;

    /// <summary>
    /// Whether this node should expose a bounding box based on the text content.
    /// This should be set to false if the parent element has a fixed size.
    /// </summary>
    public bool AutoSize { get; set; } = autoSize;

    /// <summary>
    /// The font to use for the text.
    /// </summary>
    public Font Font { get; set; } = font;

    /// <summary>
    /// The alignment of the text.
    /// </summary>
    public Align Align { get; set; } = align;

    /// <summary>
    /// The margin of the text.
    /// </summary>
    public Spacing Margin { get; set; } = margin;

    /// <summary>
    /// The offset of the text.
    /// </summary>
    public Vector2 Offset { get; set; } = offset;

    public void Render(ImDrawListPtr drawList, Rect rect, float elementOpacity)
    {
        Vector2 pos = rect.Position + GetAlignedPosition(rect.Size) + Offset;

        FontRepository.PushFont(Font);

        if (OutlineThickness > 0) {
            for (int x = -OutlineThickness; x <= OutlineThickness; x++) {
                for (int y = -OutlineThickness; y <= OutlineThickness; y++) {
                    if (x == 0 && y == 0) continue;
                    drawList.AddText(pos + new Vector2(x, y), OutlineColor.ApplyAlphaComponent(Opacity * elementOpacity), Text);
                }
            }
        }

        drawList.AddText(pos, Color.ApplyAlphaComponent(Opacity * elementOpacity), Text);
        FontRepository.PopFont(Font);
    }

    public Size? GetComputedSize()
    {
        if (!AutoSize) return null;

        FontRepository.PushFont(Font);
        var textSize = ImGui.CalcTextSize(Text);
        FontRepository.PopFont(Font);

        return new Size(textSize.X + Margin.Left + Margin.Right, textSize.Y + Margin.Top + Margin.Bottom);
    }

    private Vector2 GetAlignedPosition(Size size)
    {
        Vector2 align = new (0, 0);
        
        FontRepository.PushFont(Font);
        var temp = ImGui.CalcTextSize(Text);
        var textSize = new Size(temp.X, temp.Y);
        FontRepository.PopFont(Font);

        switch (Align)
        {
            case Align.TopLeft:
                align = new (Margin.Left, Margin.Top);
                break;
            case Align.TopCenter:
                align = new ((size.Width / 2) - (textSize.Width / 2), Margin.Top);
                break;
            case Align.TopRight:
                align = new (size.Width - textSize.Width - Margin.Right, Margin.Top);
                break;
            case Align.MiddleLeft:
                align = new (Margin.Left, (size.Height / 2) - (textSize.Height / 2));
                break;
            case Align.MiddleCenter:
                align = new ((size.Width / 2) - (textSize.Width / 2), (size.Height / 2) - (textSize.Height / 2));
                break;
            case Align.MiddleRight:
                align = new (size.Width - textSize.Width - Margin.Right, (size.Height / 2) - (textSize.Height / 2));
                break;
            case Align.BottomLeft:
                align = new (Margin.Left, size.Height - textSize.Height - Margin.Bottom);
                break;
            case Align.BottomCenter:
                align = new ((size.Width / 2) - (textSize.Width / 2), size.Height - textSize.Height - Margin.Bottom);
                break;
            case Align.BottomRight:
                align = new (size.Width - textSize.Width - Margin.Right, size.Height - textSize.Height - Margin.Bottom);
                break;
        }

        return align;
    }
}