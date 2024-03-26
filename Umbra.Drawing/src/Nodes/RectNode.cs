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
/// A node that renders a rectangle.
/// </summary>
/// <remarks>
/// This node is used to render a rectangle with a specified size, color, border, and rounding.
/// </remarks>
/// <param name="id">The ID of the node.</param>
/// <param name="size">The size of the rectangle. Defaults to the parent element size if omitted.</param>
/// <param name="color">The color of the rectangle. Defaults to 50% white if omitted.</param>
/// <param name="borderColor">The color of the border.</param>
/// <param name="borderSize">The size (thickness) of the border.</param>
/// <param name="rounding">The rounding factor of the rectangle.</param>
/// <param name="opacity">The opacity of the rectangle.</param>
/// <param name="margin">The margin of the rectangle.</param>
/// <param name="flags">The draw flags to apply to the rectangle.</param>
/// <param name="gradients">The gradients to apply to the rectangle.</param>
public sealed class RectNode(
    string? id = null,
    Size? size = null,
    uint color = 0xAAFFFFFF,
    uint borderColor = 0x00000000,
    float borderSize = 1,
    float rounding = 0,
    float opacity = 1f,
    bool overflow = false,
    Spacing margin = new(),
    ImDrawFlags flags = ImDrawFlags.RoundCornersAll,
    Gradient? gradients = null
) : INode
{
    /// <summary>
    /// The ID of the node.
    /// </summary>
    public string? Id { get; set; } = id;

    /// <summary>
    /// Determines whether the node is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// The size of the rectangle. Defaults to the parent element size if omitted.
    /// </summary>
    public Size? Size { get; set; } = size;

    /// <summary>
    /// The color of the rectangle. Defaults to 50% white if omitted.
    /// </summary>
    public uint Color { get; set; } = color;

    /// <summary>
    /// True to let the rect overflow its parent by not returning a bounding box.
    /// </summary>
    public bool Overflow { get; set; } = overflow;

    /// <summary>
    /// The color of the border.
    /// </summary>
    public uint BorderColor { get; set; } = borderColor;

    /// <summary>
    /// The size (thickness) of the border.
    /// </summary>
    public float BorderSize { get; set; } = borderSize;

    /// <summary>
    /// The rounding factor of the rectangle.
    /// </summary>
    public float Rounding { get; set; } = rounding;

    /// <summary>
    /// The opacity of the rectangle.
    /// </summary>
    public float Opacity { get; set; } = opacity;

    /// <summary>
    /// The margin of the rectangle.
    /// </summary>
    public Spacing Margin { get; set; } = margin;

    /// <summary>
    /// The draw flags to apply to the rectangle.
    /// </summary>
    public ImDrawFlags Flags { get; set; } = flags;

    /// <summary>
    /// The gradients to apply to the rectangle.
    /// </summary>
    public Gradient? Gradients { get; set; } = gradients;

    public void Render(ImDrawListPtr drawList, Rect rect, float elementOpacity)
    {
        var start = new Vector2(rect.X + Margin.Left, rect.Y + Margin.Top);
        var end = new Vector2(rect.X + rect.Width - Margin.Right, rect.Y + rect.Height - Margin.Bottom);

        drawList.AddRectFilled(
            start,
            end,
            Color.ApplyAlphaComponent(Opacity * elementOpacity),
            Rounding,
            Flags
        );

        if (Gradients != null) {
            drawList.AddRectFilledMultiColor(
                start,
                end,
                Gradients.Value.TopLeft.ApplyAlphaComponent(Opacity * elementOpacity),
                Gradients.Value.TopRight.ApplyAlphaComponent(Opacity * elementOpacity),
                Gradients.Value.BottomRight.ApplyAlphaComponent(Opacity * elementOpacity),
                Gradients.Value.BottomLeft.ApplyAlphaComponent(Opacity * elementOpacity)
            );
        }

        if (BorderSize > 0) {
            drawList.AddRect(
                start,
                end,
                BorderColor.ApplyAlphaComponent(Opacity * elementOpacity),
                Rounding,
                Flags,
                BorderSize
            );
        }    
    }

    public Size? GetComputedSize()
    {
        return Overflow ? null : Size;
    }
}
