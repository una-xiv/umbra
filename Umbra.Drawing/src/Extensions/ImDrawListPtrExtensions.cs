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

public static class ImDrawListPtrExtensions
{
    /// <summary>
    /// Adds a rotated image to the draw list.
    /// </summary>
    /// <param name="drawList">The <see cref="ImDrawListPtr"/> to draw to.</param>
    /// <param name="image">A pointer to the image to draw.</param>
    /// <param name="rotation">The rotation of the image in radians.</param>
    /// <param name="position">The position where to draw the image.</param>
    /// <param name="size">The size of the image. Defaults to 32x32 if omitted.</param>
    /// <param name="color">The color tint to apply to the image.</param>
    /// <param name="opacity">The opacity of the image.</param>
    public static void AddImageRotated(this ImDrawListPtr drawList, IntPtr image, float rotation, Vector2 position, Vector2? size = null, uint color = 0xFFFFFFFF, float opacity = 1.0f)
    {
        size ??= new Vector2(32, 32);
        color = color.ApplyAlphaComponent(opacity);

        var center = size.Value / 2;
        
        Matrix3x2 mat = Matrix3x2.CreateRotation(rotation, center);

        Vector2[] corners = [
            new Vector2(0, 0),
            new Vector2(size.Value.X, 0),
            new Vector2(size.Value.X, size.Value.Y),
            new Vector2(0, size.Value.Y)
        ];

        center -= size.Value / 2;

        for (byte i = 0; i < 4; i++)
        {
            corners[i] -= center;
            corners[i] = Vector2.Transform(corners[i], mat);
            corners[i] += center;
        }

        drawList.AddImageQuad(
            image,
            position + corners[0], position + corners[1], 
            position + corners[2], position + corners[3],
            Vector2.UnitY, Vector2.Zero, 
            Vector2.UnitX, Vector2.One,
            color
        );
    }
}
