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

using ImGuiNET;

namespace Umbra.Interface;

public partial class Element
{
    private void RenderBackground(ImDrawListPtr drawList)
    {
        var rect = ContentBox;

        if (_computedStyle.BackgroundColor is > 0) {
            drawList.AddRectFilled(
                rect.Min,
                rect.Max,
                _computedStyle.BackgroundColor.Value,
                _computedStyle.BorderRadius ?? 0,
                _computedStyle.RoundedCorners is not null ? (ImDrawFlags)_computedStyle.RoundedCorners.Value : ImDrawFlags.None
            );
        }

        if (_computedStyle.Gradient?.HasValue ?? false) {
            Gradient gradient = _computedStyle.Gradient;

            drawList.AddRectFilledMultiColor(
                rect.Min,
                rect.Max,
                gradient.TopLeft,
                gradient.TopRight,
                gradient.BottomRight,
                gradient.BottomLeft
            );
        }

        if (Style is { BorderColor: > 0, BorderWidth: > 0 }) {
            drawList.AddRect(
                rect.Min,
                rect.Max,
                _computedStyle.BorderColor ?? 0,
                _computedStyle.BorderRadius ?? 0,
                _computedStyle.RoundedCorners is not null ? (ImDrawFlags)_computedStyle.RoundedCorners.Value : ImDrawFlags.None,
                _computedStyle.BorderWidth ?? 0
            );
        }
    }
}
