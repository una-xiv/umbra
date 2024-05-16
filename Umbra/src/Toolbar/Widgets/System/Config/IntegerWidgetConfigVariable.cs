﻿/* Umbra | (c) 2024 by Una              ____ ___        ___.
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

namespace Umbra.Widgets;

public class IntegerWidgetConfigVariable(
    string  name,
    string? description,
    int     defaultValue,
    int     minValue = Int32.MinValue,
    int     maxValue = Int32.MaxValue
)
    : WidgetConfigVariable<int>(name, description, defaultValue)
{
    /// <inheritdoc/>
    protected override int Sanitize(object? value)
    {
        try {
            int res = value switch {
                null       => 0,
                string str => !int.TryParse(str, out int result) ? 0 : result,
                _          => (int)value,
            };

            return Math.Clamp(res, minValue, maxValue);
        } catch {
            return 0;
        }
    }
}
