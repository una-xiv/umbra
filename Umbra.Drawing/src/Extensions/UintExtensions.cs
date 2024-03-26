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

namespace Umbra.Drawing;

public static class UintExtensions
{
    /// <summary>
    /// Apply the given alpha component to the color value.
    /// </summary>
    public static uint ApplyAlphaComponent(this uint value, float a)
    {
        byte alpha = (byte)(((value >> 24) & 0xFF) * Math.Clamp(a, 0f, 1f));
        byte red   = (byte)((value >> 16) & 0xFF);
        byte green = (byte)((value >> 8) & 0xFF);
        byte blue  = (byte)(value & 0xFF);

        return ((uint)alpha << 24) | ((uint)red << 16) | ((uint)green << 8) | blue;
    }

    public static uint ApplyBrightness(this uint value, float brightness)
    {
        byte red   = (byte)(((value >> 16) & 0xFF) * Math.Clamp(brightness, 0f, 2f));
        byte green = (byte)(((value >> 8) & 0xFF) * Math.Clamp(brightness, 0f, 2f));
        byte blue  = (byte)((value & 0xFF) * Math.Clamp(brightness, 0f, 1f));

        return (value & 0xFF000000) | ((uint)red << 16) | ((uint)green << 8) | blue;
    }
}
