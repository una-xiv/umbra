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

using System.Numerics;
using ImGuiNET;

namespace Umbra.Interface;

public partial class Element
{
    private void RenderText(ImDrawListPtr drawList)
    {
        if (Text == null) return;

        Font font = _computedStyle.Font ?? Font.Default;
        FontRepository.PushFont(font);

        Rect    rect     = ContentBox;
        Vector2 textSize = ImGui.CalcTextSize(Text);
        Vector2 pos      = new(rect.X1, rect.Y1);
        Anchor  align    = _computedStyle.TextAlign ?? Anchor.MiddleLeft;

        if (align.IsCenter()) {
            pos.X += (rect.Width - textSize.X) / 2;
        } else if (align.IsRight()) {
            pos.X += rect.Width - textSize.X;
        }

        if (align.IsMiddle()) {
            pos.Y += (rect.Height - textSize.Y) / 2;
        } else if (align.IsBottom()) {
            pos.Y += rect.Height - textSize.Y;
        }

        sbyte outlineWidth = _computedStyle.OutlineWidth ?? 0;

        if (outlineWidth > 0) {
            uint outlineColor = _computedStyle.OutlineColor ?? 0x80000000;

            for (int i = -outlineWidth; i <= outlineWidth; i++) {
                for (int j = -outlineWidth; j <= outlineWidth; j++) {
                    if (i == 0 && j == 0) continue;
                    drawList.AddText(pos + new Vector2(i, j), outlineColor, Text);
                }
            }
        }

        drawList.AddText(pos, _computedStyle.ForegroundColor ?? 0xFFFFFFFF, Text);

        FontRepository.PopFont(font);
    }
}
