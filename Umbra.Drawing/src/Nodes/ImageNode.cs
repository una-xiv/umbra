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

using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Internal;
using ImGuiNET;
using SixLabors.ImageSharp.Processing;

namespace Umbra.Drawing;

/// <summary>
/// A node that renders a Game Icon.
/// </summary>
/// <param name="id">The ID of this node.</param>
/// <param name="path">The path to the image file to render.</param>
/// <param name="color">The color tint to apply to the final image.</param>
/// <param name="rounding">The rounding to apply to the image in pixels.</param>
/// <param name="opacity">The opacity to apply to the final image.</param>
/// <param name="grayscale">The grayscale factor to apply to the final image.</param>
/// <param name="margin">The spacing around the image.</param>
public sealed class ImageNode(
    string? id = null,
    string? path = null,
    uint color = 0xFFFFFFFF,
    float rounding = 0,
    float opacity = 1f,
    float grayscale = 0f,
    Spacing margin = new(),
    Vector2? uv1 = null,
    Vector2? uv2 = null
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
    /// The path to the image file to render.
    /// </summary>
    public string? ImagePath { get; set; } = path;

    /// <summary>
    /// The color tint to apply to the final image.
    /// </summary>
    public uint Color { get; set; } = color;

    /// <summary>
    /// The rounding to apply to the image in pixels.
    /// </summary>
    public float Rounding { get; set; } = rounding;

    /// <summary>
    /// The opacity to apply to the final image.
    /// </summary>
    public float Opacity { get; set; } = opacity;

    /// <summary>
    /// The grayscale factor to apply to the final image. A value of 0 is no grayscale, 1 is full grayscale.
    /// </summary>
    public float Grayscale { get; set; } = grayscale;

    /// <summary>
    /// The spacing around the image.
    /// </summary>
    public Spacing Margin { get; set; } = margin;

    /// <summary>
    /// The UV coordinates of the top-left corner of the image.
    /// </summary>
    public Vector2 Uv1 { get; set; } = uv1 ?? new(0, 0);

    /// <summary>
    /// The UV coordinates of the bottom-right corner of the image.
    /// </summary>
    public Vector2 Uv2 { get; set; } = uv2 ?? new(1, 1);

    private static readonly Dictionary<string, IDalamudTextureWrap> ImageCache = [];
    private string CacheKey => $"{ImagePath}_{Grayscale}_{Rounding}";

    public void Render(ImDrawListPtr drawList, Rect rect, float elementOpacity)
    {
        var start = new Vector2(rect.X + Margin.Left, rect.Y + Margin.Top);
        var end   = new Vector2(rect.X + rect.Width - Margin.Right, rect.Y + rect.Height - Margin.Bottom);
        var crc   = CacheKey;

        if (ImageCache.TryGetValue(crc, out var cachedIcon)) {
            drawList.AddImage(cachedIcon.ImGuiHandle, start, end, Uv1, Uv2, Color.ApplyAlphaComponent(elementOpacity * Opacity));
            return;
        }

        var icon = GetTexture();
        ImageCache[crc] = icon;

        drawList.AddImage(icon.ImGuiHandle, start, end, Uv1, Uv2, Color.ApplyAlphaComponent(elementOpacity * Opacity));
    }

    public Size? GetComputedSize()
    {
        return null;
    }

    private IDalamudTextureWrap GetTexture()
    {
        if (Rounding == 0 && Grayscale == 0) {
            return AssetManager.GetLocalTexture(ImagePath!);
        }

        using var image = AssetManager.GetLocalImage(ImagePath!);

        image.Mutate(ctx => {
            if (Grayscale > 0) ctx.Grayscale(Grayscale);
            if (Rounding > 0) ctx.ApplyRoundedCorners(Rounding);
        });

        return image.ToDalamudTextureWrap();
    }
}
