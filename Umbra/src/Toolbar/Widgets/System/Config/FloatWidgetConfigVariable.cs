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

namespace Umbra.Widgets;

public class FloatWidgetConfigVariable(
    string  id,
    string  name,
    string? description,
    float   defaultValue,
    float   minValue = float.MinValue,
    float   maxValue = float.MaxValue
)
    : WidgetConfigVariable<float>(id, name, description, defaultValue)
{
    public float MinValue { get; set; } = minValue;
    public float MaxValue { get; set; } = maxValue;

    /// <inheritdoc/>
    protected override float Sanitize(object? value)
    {
        try {
            float res = value switch {
                null       => 0,
                string str => !float.TryParse(str, out float result) ? 0 : result,
                _          => Convert.ToSingle(value)
            };

            return Math.Clamp(res, MinValue, MaxValue);
        } catch {
            return 0;
        }
    }
}
