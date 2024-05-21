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
    private void RenderBorders(ImDrawListPtr drawList)
    {
        if (_computedStyle.BorderWidth == null || _computedStyle.BorderColor == null) return;

        var rect  = ContentBox;
        var width = _computedStyle.BorderWidth;

        if (width.Top > 0) {
            drawList.AddLine(
                rect.Min,
                new (rect.Max.X, rect.Min.Y),
                _computedStyle.BorderColor.Top.ApplyAlpha(_computedStyle.Opacity ?? 1),
                width.Top * ScaleFactor
            );
        }

        if (width.Bottom > 0) {
            drawList.AddLine(
                new (rect.Min.X, rect.Max.Y),
                rect.Max,
                _computedStyle.BorderColor.Bottom.ApplyAlpha(_computedStyle.Opacity ?? 1),
                width.Bottom * ScaleFactor
            );
        }

        if (width.Left > 0) {
            drawList.AddLine(
                rect.Min,
                new (rect.Min.X, rect.Max.Y),
                _computedStyle.BorderColor.Left.ApplyAlpha(_computedStyle.Opacity ?? 1),
                width.Left * ScaleFactor
            );
        }

        if (width.Right > 0) {
            drawList.AddLine(
                new (rect.Max.X, rect.Min.Y),
                rect.Max,
                _computedStyle.BorderColor.Right.ApplyAlpha(_computedStyle.Opacity ?? 1),
                width.Right * ScaleFactor
            );
        }
    }
}
