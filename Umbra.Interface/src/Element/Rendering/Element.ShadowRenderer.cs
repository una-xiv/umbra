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

using System;
using System.Numerics;
using Dalamud.Interface.Internal;
using ImGuiNET;

namespace Umbra.Interface;

public partial class Element
{
    private static IDalamudTextureWrap? _shadowTexture;

    private void RenderShadow(ImDrawListPtr drawList)
    {
        if (null == _computedStyle.Shadow) return;
        _shadowTexture ??= ImageRepository.GetEmbeddedTexture("Shadow.png");

        Rect    rect   = ContentBox;
        Spacing offset = _computedStyle.Shadow.Inset;
        Side    side   = _computedStyle.Shadow.Side;
        uint    color  = _computedStyle.Shadow.Color.ApplyAlphaComponent(_computedStyle.Opacity ?? 1);
        int     size   = _computedStyle.Shadow.Size;

        const float uv0 = 0.0f;
        const float uv1 = 0.333333f;
        const float uv2 = 0.666666f;
        const float uv3 = 1.0f;

        ImDrawListPtr dl = drawList;
        IntPtr        id = _shadowTexture.ImGuiHandle;
        Vector2       p  = rect.Min              + offset.TopLeft;
        Vector2       s  = rect.Size.ToVector2() - offset.TopLeft - offset.BottomRight;
        Vector2       m  = new(p.X + s.X, p.Y + s.Y);

        if (IsInWindowDrawList(dl)) dl.PushClipRectFullScreen();

        if (side.HasFlag(Side.Top) || side.HasFlag(Side.Left))
            dl.AddImage(id, new(p.X - size, p.Y - size), new(p.X, p.Y), new(uv0, uv0), new(uv1, uv1), color);

        if (side.HasFlag(Side.Top))
            dl.AddImage(id, p with { Y = p.Y - size }, new(m.X, p.Y), new(uv1, uv0), new(uv2, uv1), color);

        if (side.HasFlag(Side.Top) || side.HasFlag(Side.Right))
            dl.AddImage(id, m with { Y = p.Y - size }, p with { X = m.X + size }, new(uv2, uv0), new(uv3, uv1), color);

        if (side.HasFlag(Side.Left))
            dl.AddImage(id, p with { X = p.X - size }, new(p.X, m.Y), new(uv0, uv1), new(uv1, uv2), color);

        if (side.HasFlag(Side.Right))
            dl.AddImage(id, new(m.X, p.Y), m with { X = m.X + size }, new(uv2, uv1), new(uv3, uv2), color);

        if (side.HasFlag(Side.Bottom) || side.HasFlag(Side.Left))
            dl.AddImage(id, m with { X = p.X - size }, p with { Y = m.Y + size }, new(uv0, uv2), new(uv1, uv3), color);

        if (side.HasFlag(Side.Bottom))
            dl.AddImage(id, new(p.X, m.Y), m with { Y = m.Y + size }, new(uv1, uv2), new(uv2, uv3), color);

        if (side.HasFlag(Side.Bottom) || side.HasFlag(Side.Right))
            dl.AddImage(id, new(m.X, m.Y), new(m.X + size, m.Y + size), new(uv2, uv2), new(uv3, uv3), color);

        if (IsInWindowDrawList(drawList)) dl.PopClipRect();
    }
}
