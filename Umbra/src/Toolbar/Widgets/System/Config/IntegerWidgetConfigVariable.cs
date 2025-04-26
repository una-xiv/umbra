﻿using System;
using Umbra.Common;

namespace Umbra.Widgets;

public class IntegerWidgetConfigVariable(
    string  id,
    string  name,
    string? description,
    int     defaultValue,
    int     minValue = int.MinValue,
    int     maxValue = int.MaxValue
)
    : WidgetConfigVariable<int>(id, name, description, defaultValue)
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
