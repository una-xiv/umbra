/* Umbra.Common | (c) 2024 by Una       ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Common is free software: you can        \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Globalization;

namespace Umbra.Interface;

public static class UintExtensions
{
    /// <summary>
    /// Apply the given alpha component to the color value.
    /// </summary>
    public static uint ApplyAlphaComponent(this uint value, float a)
    {
        var alpha = (byte)(((value >> 24) & 0xFF) * Math.Clamp(a, 0f, 1f));
        var blue  = (byte)((value >> 16) & 0xFF);
        var green = (byte)((value >> 8) & 0xFF);
        var red   = (byte)(value & 0xFF);

        return ((uint)alpha << 24) | ((uint)blue << 16) | ((uint)green << 8) | red;
    }

    /// <summary>
    /// Converts a large number to a human-readable string (e.g., 1.3M).
    /// Shortens numbers 10,000 or greater.
    /// </summary>
    public static string ToHumanReadable(this uint number)
    {
        if (number < 10000) {
            return number.ToString("N0", CultureInfo.InvariantCulture);
        }

        var    absoluteNumber = Math.Abs((double)number);
        string suffix;
        double scaledNumber;

        if (absoluteNumber >= 1_000_000_000_000) {
            suffix       = "T";
            scaledNumber = number / 1_000_000_000_000.0;
        } else if (absoluteNumber >= 1_000_000_000) {
            suffix       = "B";
            scaledNumber = number / 1_000_000_000.0;
        } else if (absoluteNumber >= 1_000_000) {
            suffix       = "M";
            scaledNumber = number / 1_000_000.0;
        } else {
            suffix       = "K";
            scaledNumber = number / 1_000.0;
        }

        // Format the number to one decimal place and append the suffix
        return scaledNumber.ToString("N1", CultureInfo.InvariantCulture) + suffix;
    }
}
