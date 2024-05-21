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

        if (null != _computedStyle.BackgroundColor && _computedStyle.BackgroundColor.Value != 0u) {
            drawList.AddRectFilled(
                rect.Min,
                rect.Max,
                _computedStyle.BackgroundColor.Value.ApplyAlphaComponent(_computedStyle.Opacity ?? 1),
                _computedStyle.BackgroundRounding ?? 0,
                _computedStyle.RoundedCorners is not null
                    ? (ImDrawFlags)_computedStyle.RoundedCorners.Value
                    : ImDrawFlags.None
            );
        }

        if (_computedStyle.Gradient?.HasValue ?? false) {
            Gradient gradient = _computedStyle.Gradient;

            drawList.AddRectFilledMultiColor(
                rect.Min,
                rect.Max,
                gradient.TopLeft.ApplyAlpha(_computedStyle.Opacity     ?? 1),
                gradient.TopRight.ApplyAlpha(_computedStyle.Opacity    ?? 1),
                gradient.BottomRight.ApplyAlpha(_computedStyle.Opacity ?? 1),
                gradient.BottomLeft.ApplyAlpha(_computedStyle.Opacity  ?? 1)
            );
        }

        if (Style is { BackgroundBorderColor.Value: > 0, BackgroundBorderWidth: > 0 }) {
            drawList.AddRect(
                rect.Min,
                rect.Max,
                (_computedStyle.BackgroundBorderColor ?? 0u).ApplyAlpha(_computedStyle.Opacity ?? 1),
                _computedStyle.BackgroundRounding ?? 0,
                _computedStyle.RoundedCorners is not null
                    ? (ImDrawFlags)_computedStyle.RoundedCorners.Value
                    : ImDrawFlags.None,
                _computedStyle.BackgroundBorderWidth ?? 0
            );
        }
    }
}
