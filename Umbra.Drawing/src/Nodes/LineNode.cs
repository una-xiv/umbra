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
/// A node that draws a line.
/// </summary>
/// <param name="id">The ID of the node.</param>
/// <param name="color">The color of the line.</param>
/// <param name="direction">The direction of the line.</param>
/// <param name="margin">The margin around the line.</param>
public sealed class LineNode(
    string? id = null,
    uint color = 0xFFFFFFFF,
    Direction direction = Direction.Horizontal,
    Spacing margin = new()
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
    /// The color of the line.
    /// </summary>
    public uint Color { get; set; } = color;

    /// <summary>
    /// The direction of the line.
    /// </summary>
    public Direction Direction { get; set; } = direction;

    /// <summary>
    /// The margin around the line.
    /// </summary>
    public Spacing Margin { get; set; } = margin;

    public void Render(ImDrawListPtr drawList, Rect rect, float elementOpacity)
    {
        var start = new Vector2(rect.X + Margin.Left, rect.Y + Margin.Top);
        var end = new Vector2(rect.X + rect.Width - Margin.Right, rect.Y + rect.Height - Margin.Bottom);

        if (Direction == Direction.Horizontal)
        {
            var y = start.Y + (end.Y - start.Y) / 2;
            drawList.AddLine(new(start.X, y), new(end.X, y), Color.ApplyAlphaComponent(elementOpacity));
        }
        else
        {
            var x = start.X + (end.X - start.X) / 2;
            drawList.AddLine(new(x, start.Y), new(x, end.Y), Color.ApplyAlphaComponent(elementOpacity));
        }
    }

    public Size? GetComputedSize()
    {
        return null;
    }
}