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
using Umbra.Common;

namespace Umbra.Interface;

public partial class Element
{
    public void Render(ImDrawListPtr drawList, Vector2? position)
    {
        if (null != position) ComputeLayout(position.Value);

        try {
            SetupInteractive(drawList);

            RenderShadow(drawList);
            RenderBackground(drawList);
            RenderImage(drawList);
            RenderText(drawList);

            foreach (var child in Children) {
                child.Render(drawList, null);
            }

            EndInteractive(drawList);

        } catch (Exception e) {
            Logger.Warning($"Rendering of element '{FullyQualifiedName}' failed: {e.Message}");
        }

        RenderDebugger();
    }

    private void RenderBackground(ImDrawListPtr drawList)
    {
        var rect = ContentBox;

        if (Style.GetBackgroundColor(this) > 0) {
            drawList.AddRectFilled(
                rect.Min,
                rect.Max,
                Style.GetBackgroundColor(this),
                Style.GetBorderRadius(this),
                (ImDrawFlags)Style.GetRoundedCorners(this)
            );
        }

        if (Style.GetGradient(this).HasValue) {
            Gradient gradient = Style.GetGradient(this);

            drawList.AddRectFilledMultiColor(
                rect.Min,
                rect.Max,
                gradient.TopLeft,
                gradient.TopRight,
                gradient.BottomRight,
                gradient.BottomLeft
            );
        }

        if (Style.BorderColor != 0 && Style.BorderWidth > 0) {
            drawList.AddRect(
                rect.Min,
                rect.Max,
                Style.GetBorderColor(this),
                Style.GetBorderRadius(this),
                (ImDrawFlags)Style.GetRoundedCorners(this),
                Style.GetBorderWidth(this)
            );
        }
    }

    private void RenderText(ImDrawListPtr drawList)
    {
        if (Text == null) return;

        Font font = Style.GetFont(this);
        FontRepository.PushFont(font);

        Rect    rect     = ContentBox;
        Vector2 textSize = ImGui.CalcTextSize(Text);
        Vector2 pos      = new(rect.X1, rect.Y1);
        Anchor  align    = Style.GetTextAlign(this);

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

        sbyte outlineWidth = Style.GetOutlineWidth(this);

        if (outlineWidth > 0) {
            uint outlineColor = Style.GetOutlineColor(this);

            for (int i = -outlineWidth; i <= outlineWidth; i++) {
                for (int j = -outlineWidth; j <= outlineWidth; j++) {
                    if (i == 0 && j == 0) continue;
                    drawList.AddText(pos + new Vector2(i, j), outlineColor, Text);
                }
            }
        }

        drawList.AddText(pos, Style.GetForegroundColor(this), Text);

        FontRepository.PopFont(font);
    }

    private void RenderImage(ImDrawListPtr drawList)
    {
        IntPtr? img = GetImagePtr();
        if (null == img) return;

        Rect rect = ContentBox;

        drawList.AddImage(
            img.Value,
            rect.Min,
            rect.Max
        );
    }

    private static IDalamudTextureWrap? _shadowTexture;

    private void RenderShadow(ImDrawListPtr drawList)
    {
        if (null == Style.Shadow) return;
        _shadowTexture ??= ImageRepository.GetLocalTexture("images\\shadow.png");

        Rect    rect   = ContentBox;
        Spacing offset = Style.Shadow.Inset;
        Side    side   = Style.Shadow.Side;
        uint    color  = Style.Shadow.Color;
        int     size   = Style.Shadow.Size;

        const float uv0 = 0.0f;
        const float uv1 = 0.333333f;
        const float uv2 = 0.666666f;
        const float uv3 = 1.0f;

        ImDrawListPtr dl = drawList;
        IntPtr        id = _shadowTexture.ImGuiHandle;
        Vector2       p  = rect.Min  + offset.TopLeft;
        Vector2       s  = rect.Size - offset.TopLeft - offset.BottomRight;
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

    private IntPtr? GetImagePtr()
    {
        if (Style.Image == null) return null;

        uint? iconId = Style.Image.Value.IconId;

        if (iconId is > 0) {
            return ImageRepository.GetIcon(iconId.Value).ImGuiHandle;
        }

        string? path = Style.Image.Value.Path;
        return path is { Length: > 0 } ? ImageRepository.GetLocalTexture(path).ImGuiHandle : IntPtr.Zero;
    }
}
