/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using Umbra.Common;

namespace Umbra.Markers;

public class IntegerMarkerConfigVariable(
    string  id,
    string  name,
    string? description,
    int     defaultValue,
    int     minValue = int.MinValue,
    int     maxValue = int.MaxValue
)
    : MarkerConfigVariable<int>(id, name, description, defaultValue)
{
    public int MinValue { get; set; } = minValue;
    public int MaxValue { get; set; } = maxValue;

    /// <inheritdoc/>
    protected override int Sanitize(object? value)
    {
        try {
            int res = value switch {
                null       => 0,
                string str => !int.TryParse(str, out int result) ? 0 : result,
                _          => Convert.ToInt32(value),
            };

            return Math.Clamp(res, MinValue, MaxValue);
        } catch (Exception e) {
            Logger.Warning(e.Message);
            return 0;
        }
    }
}
