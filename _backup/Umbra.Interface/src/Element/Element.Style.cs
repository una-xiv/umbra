/* Umbra.Interface | (c) 2024 by Una    ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Interface is free software: you can    \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Interface is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Interface;

public partial class Element
{
    /// <summary>
    /// A dictionary of theme colors.
    /// </summary>
    /// <remarks>
    /// Before rendering an element, the computed style of an element is
    /// scanned for named colors. If a named color is found, and an entry for
    /// it exists in this dictionary, the value of the color is replaced by
    /// the value in the dictionary.
    /// </remarks>
    public static readonly Dictionary<string, uint> ThemeColors = [];

    /// <summary>
    /// The visual style to apply to this element.
    /// </summary>
    public Style Style;

    private Style _computedStyle = new();

    private void ComputeStyle()
    {
        if (null == Parent) {
            _computedStyle = Style;
            UpdateThemeColors();
            return;
        }

        _computedStyle = new(Style);
        _computedStyle.Merge(Parent._computedStyle);

        UpdateThemeColors();
    }

    private void UpdateThemeColors()
    {
        ApplyThemedColor(_computedStyle.BackgroundColor);
        ApplyThemedColor(_computedStyle.Gradient?.TopLeft);
        ApplyThemedColor(_computedStyle.Gradient?.TopRight);
        ApplyThemedColor(_computedStyle.Gradient?.BottomLeft);
        ApplyThemedColor(_computedStyle.Gradient?.BottomRight);
        ApplyThemedColor(_computedStyle.BackgroundBorderColor);
        ApplyThemedColor(_computedStyle.TextColor);
        ApplyThemedColor(_computedStyle.OutlineColor);
        ApplyThemedColor(_computedStyle.BorderColor?.Top);
        ApplyThemedColor(_computedStyle.BorderColor?.Right);
        ApplyThemedColor(_computedStyle.BorderColor?.Bottom);
        ApplyThemedColor(_computedStyle.BorderColor?.Left);
    }

    private static void ApplyThemedColor(object? color)
    {
        if (color is Color clr && ThemeColors.TryGetValue(clr.Name ?? "", out uint value)) {
            clr.Set(value);
        }
    }
}
