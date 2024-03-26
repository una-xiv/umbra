/* Umbra.Drawing | (c) 2024 by Una      ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Drawing is free software: you can       \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Numerics;
using Dalamud.Interface.Internal;
using ImGuiNET;

namespace Umbra.Drawing;

/// <summary>
/// A node that draws a "box shadow" around the edges of the attached element.
/// </summary>
/// <param name="id">The ID of this node.</param>
/// <param name="size">The size of the shadow.</param>
/// <param name="opacity">The opacity of the shadow.</param>
/// <param name="inset">The inset margin of the shadow.</param>
/// <param name="side">The sides to draw the shadow on.</param>
/// <param name="requiresFullscreenClipRect">Whether the shadow requires a full screen clip rect.</param>
public sealed class ShadowNode(
    string? id = null,
    float size = 32f,
    float opacity = 1f,
    Spacing inset = new(),
    ShadowSide side = ShadowSide.All,
    bool requiresFullscreenClipRect = false
) : INode
{
    /// <summary>
    /// The ID of this node.
    /// </summary>
    public string? Id { get; set; } = id;

    /// <summary>
    /// Determines whether this node is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// The size of the shadow.
    /// </summary>
    public float Size { get; set; } = size;

    /// <summary>
    /// The opacity of the shadow.
    /// </summary>
    public float Opacity { get; set; } = opacity;

    /// <summary>
    /// The inset margin of the shadow.
    /// </summary>
    public Spacing Inset { get; set; } = inset;

    /// <summary>
    /// The sides to draw the shadow on.
    /// </summary>
    public ShadowSide Side { get; set; } = side;

    /// <summary>
    /// Whether the shadow requires a full screen clip rect.
    /// </summary>
    public bool RequiresFullscreenClipRect { get; set; } = requiresFullscreenClipRect;

    private static IDalamudTextureWrap? _shadowTexture = null;

    public void Render(ImDrawListPtr drawList, Rect rect, float elementOpacity)
    {
        if (null == _shadowTexture) _shadowTexture = AssetManager.GetLocalTexture("images\\shadow.png");

        Vector2 p = rect.Min + Inset.TopLeft;
        Vector2 s = rect.Size.XY - Inset.TopLeft - Inset.BottomRight;
        Vector2 m = new(p.X + s.X, p.Y + s.Y);

        float uv0 = 0.0f;
        float uv1 = 0.333333f;
        float uv2 = 0.666666f;
        float uv3 = 1.0f;

        var cl = 0xFFFFFFFF.ApplyAlphaComponent(Opacity * elementOpacity);
        var sz = Size;
        var dl = drawList;
        var id = _shadowTexture.ImGuiHandle;

        if (RequiresFullscreenClipRect) dl.PushClipRectFullScreen();
        if (Side.HasFlag(ShadowSide.Top) || Side.HasFlag(ShadowSide.Left))     dl.AddImage(id, new Vector2(p.X - sz, p.Y - sz), new Vector2(p.X,      p.Y     ), new Vector2(uv0, uv0), new Vector2(uv1, uv1), cl); // Top left.
        if (Side.HasFlag(ShadowSide.Top))                                      dl.AddImage(id, new Vector2(p.X,      p.Y - sz), new Vector2(m.X,      p.Y     ), new Vector2(uv1, uv0), new Vector2(uv2, uv1), cl); // Top.
        if (Side.HasFlag(ShadowSide.Top) || Side.HasFlag(ShadowSide.Right))    dl.AddImage(id, new Vector2(m.X,      p.Y - sz), new Vector2(m.X + sz, p.Y     ), new Vector2(uv2, uv0), new Vector2(uv3, uv1), cl); // Top right.
        if (Side.HasFlag(ShadowSide.Left))                                     dl.AddImage(id, new Vector2(p.X - sz, p.Y     ), new Vector2(p.X,      m.Y     ), new Vector2(uv0, uv1), new Vector2(uv1, uv2), cl); // Left.
        if (Side.HasFlag(ShadowSide.Right))                                    dl.AddImage(id, new Vector2(m.X,      p.Y     ), new Vector2(m.X + sz, m.Y     ), new Vector2(uv2, uv1), new Vector2(uv3, uv2), cl); // Right.
        if (Side.HasFlag(ShadowSide.Bottom) || Side.HasFlag(ShadowSide.Left))  dl.AddImage(id, new Vector2(p.X - sz, m.Y     ), new Vector2(p.X,      m.Y + sz), new Vector2(uv0, uv2), new Vector2(uv1, uv3), cl); // Bottom left.
        if (Side.HasFlag(ShadowSide.Bottom))                                   dl.AddImage(id, new Vector2(p.X,      m.Y     ), new Vector2(m.X,      m.Y + sz), new Vector2(uv1, uv2), new Vector2(uv2, uv3), cl); // Bottom.
        if (Side.HasFlag(ShadowSide.Bottom) || Side.HasFlag(ShadowSide.Right)) dl.AddImage(id, new Vector2(m.X,      m.Y     ), new Vector2(m.X + sz, m.Y + sz), new Vector2(uv2, uv2), new Vector2(uv3, uv3), cl); // Bottom right.
        if (RequiresFullscreenClipRect) dl.PopClipRect();
    }

    public Size? GetComputedSize()
    {
        return null;
    }
}

[Flags]
public enum ShadowSide
{
    Top = 1,
    Bottom = 2,
    Left = 4,
    Right = 8,
    All = Top | Bottom | Left | Right
}
