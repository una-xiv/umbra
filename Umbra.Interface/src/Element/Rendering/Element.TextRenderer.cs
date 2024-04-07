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
    private static uint _globalWrappedTextFrameId = 10_000_000;
    private uint _wrappedTextFrameId;

    private void RenderText(ImDrawListPtr drawList)
    {
        if (Text == null) return;

        Font font = _computedStyle.Font ?? Font.Default;
        FontRepository.PushFont(font);

        bool shouldWrap = Size.Width > 0 && _computedStyle.TextWrap == true;

        Vector2 textSize = shouldWrap
            ? ImGui.CalcTextSize(Text, (float)(Size.Width - Padding.Horizontal))
            : ImGui.CalcTextSize(Text);

        Rect    rect  = ContentBox;
        Vector2 pos   = new(rect.X1, rect.Y1);
        Anchor  align = _computedStyle.TextAlign ?? Anchor.MiddleLeft;

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

        if (_computedStyle.TextOffset.HasValue) {
            pos += _computedStyle.TextOffset.Value;
        }

        // Round position to prevent subpixel rendering.
        pos = new((int)pos.X, (int)pos.Y);

        if (shouldWrap) {
            if (0 == _wrappedTextFrameId) {
                _wrappedTextFrameId = _globalWrappedTextFrameId++;
            }

            ImGui.SetCursorScreenPos(ContentBox.Min);
            ImGui.BeginChildFrame(_wrappedTextFrameId, textSize, ImGuiWindowFlags.NoMouseInputs | ImGuiWindowFlags.NoInputs);
            ImGui.PushStyleColor(ImGuiCol.Text, _computedStyle.TextColor?.Value ?? 0xFFC0C0C0);
            ImGui.TextWrapped(Text);
            ImGui.PopStyleColor();
            ImGui.EndChildFrame();

            FontRepository.PopFont(font);
            return;
        }

        sbyte outlineWidth = _computedStyle.OutlineWidth ?? 0;

        if (outlineWidth > 0) {
            uint  outlineColor = _computedStyle.OutlineColor?.Value ?? 0xFF000000;
            float opacity      = _computedStyle.Opacity      ?? 1;

            if (opacity < 1) outlineColor = outlineColor.ApplyAlphaComponent(opacity / (outlineWidth * 3));

            for (int i = -outlineWidth; i <= outlineWidth; i++) {
                for (int j = -outlineWidth; j <= outlineWidth; j++) {
                    if (i == 0 && j == 0) continue;
                    drawList.AddText(pos + new Vector2(i, j), outlineColor, Text);
                }
            }
        }

        drawList.AddText(
            pos,
            (_computedStyle.TextColor?.Value ?? 0xFFFFFFFF).ApplyAlphaComponent(_computedStyle.Opacity ?? 1),
            Text
        );

        FontRepository.PopFont(font);
    }
}
