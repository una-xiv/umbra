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
/// Represents a rectangle.
/// </summary>
public struct Rect(float x, float y, float width, float height)
{
    public          float X      = x;
    public          float Y      = y;
    public readonly float Width  = width;
    public readonly float Height = height;

    /// <summary>
    /// Returns the <see cref="Size"/> of this rectangle.
    /// </summary>
    public readonly Size Size => new(Width, Height);

    /// <summary>
    /// Returns the <see cref="Vector2"/> position of this rectangle.
    /// </summary>
    public readonly Vector2 Position => new(X, Y);

    /// <summary>
    /// Returns the top-left point of this rectangle.
    /// </summary>
    public readonly Vector2 Min => new(X, Y);

    /// <summary>
    /// Returns the lower-right point of this rectangle.
    /// </summary>
    public readonly Vector2 Max => new(X + Width, Y + Height);

    /// <summary>
    /// Returns true if the given point is within the bounds of the rectangle.
    /// </summary>
    public readonly bool Contains(Vector2 point) => point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;

    /// <summary>
    /// Renders the rectangle to the screen.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="label"></param>
    public readonly void RenderToScreen(uint color = 0xAAFFFFFF, string label = "")
    {
        var drawList = ImGui.GetForegroundDrawList();

        drawList.AddRect(
            new(X, Y),
            new(X + Width, Y + Height),
            color
        );

        drawList.AddRectFilled(
            new(X, Y),
            new(X + Width, Y + Height),
            0x40000000
        );

        drawList.AddText(new(X + 20, Y + 20), 0xAAFFFFFF, $"{X}, {Y} - {Width}, {Height}");

        if (!string.IsNullOrEmpty(label)) {
            var textSize = ImGui.CalcTextSize(label);

            drawList.AddText(
                new(X + Width / 2 - textSize.X / 2, Y + Height / 2 - textSize.Y / 2),
                color,
                label
            );
        }
    }
}
