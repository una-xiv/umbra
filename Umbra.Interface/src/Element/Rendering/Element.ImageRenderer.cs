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
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Internal;
using ImGuiNET;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Umbra.Common;

namespace Umbra.Interface;

public partial class Element
{
    [ConfigVariable("General.EnableImageProcessing", "General", requiresRestart: true)]
    private static bool EnableImageProcessing { get; set; } = true;

    private static readonly Dictionary<string, IDalamudTextureWrap> ProcessedImageCache = [];

    private static bool _isImageProcessingEnabled = true;

    private string ProcessedImageCacheKey =>
        $"{_computedStyle.Image}_{_computedStyle.ImageRounding}_{_computedStyle.ImageBlackAndWhite}_{_computedStyle.ImageGrayscale}_{_computedStyle.ImageBrightness}_{_computedStyle.ImageContrast}";

    private void RenderImage(ImDrawListPtr drawList)
    {
        if (_isImageProcessingEnabled != EnableImageProcessing) {
            _isImageProcessingEnabled = EnableImageProcessing;
            return;
        }

        IntPtr? img = GetImageToRender();
        if (null == img) return;

        Rect    rect   = ContentBox;
        Vector2 offset = _computedStyle.ImageOffset ?? new();

        Vector2 uv1 = _computedStyle.ImageUVs != null
            ? new(_computedStyle.ImageUVs.Value.X, _computedStyle.ImageUVs.Value.Y)
            : Vector2.Zero;

        Vector2 uv2 = _computedStyle.ImageUVs != null
            ? new(_computedStyle.ImageUVs.Value.Z, _computedStyle.ImageUVs.Value.W)
            : Vector2.One;

        drawList.AddImage(
            img.Value,
            rect.Min + offset,
            rect.Max + offset,
            uv1,
            uv2,
            0xFFFFFFFF.ApplyAlphaComponent(_computedStyle.Opacity ?? 1)
        );
    }

    private IntPtr? GetImageToRender()
    {
        if (_computedStyle.Image == null) return null;

        if (false == RequiresImageProcessing()) return GetImagePtr();

        if (ProcessedImageCache.TryGetValue(ProcessedImageCacheKey, out IDalamudTextureWrap? processedImage)) {
            return processedImage.ImGuiHandle;
        }

        Image<Rgba32> image;

        switch (_computedStyle.Image) {
            case uint iconId and > 0:
                image = ImageRepository.GetIconFile(iconId).GetImage();
                break;
            case string { Length: > 0 } path:
                image = ImageRepository.GetLocalImage(path);
                break;
            default:
                return null;
        }

        image.Mutate(
            ctx => {
                if (_computedStyle.ImageBlackAndWhite != null && _computedStyle.ImageBlackAndWhite.Value) {
                    ctx.BlackWhite();
                }

                if (_computedStyle.ImageRounding is > 0) {
                    ctx.ApplyRoundedCorners(_computedStyle.ImageRounding.Value);
                }

                if (_computedStyle.ImageGrayscale is > 0) {
                    ctx.Grayscale(_computedStyle.ImageGrayscale.Value);
                }

                if (_computedStyle.ImageBrightness is > 0) {
                    ctx.Brightness(_computedStyle.ImageBrightness.Value);
                }

                if (_computedStyle.ImageContrast is > 0) {
                    ctx.Contrast(_computedStyle.ImageContrast.Value);
                }
            }
        );

        var texture = image.ToDalamudTextureWrap();

        ProcessedImageCache[ProcessedImageCacheKey] = texture;

        return texture.ImGuiHandle;
    }

    private IntPtr? GetImagePtr()
    {
        if (_computedStyle.Image == null) return null;

        return _computedStyle.Image switch {
            uint iconId and > 0         => ImageRepository.GetIcon(iconId).ImGuiHandle,
            string { Length: > 0 } path => ImageRepository.GetEmbeddedTexture(path).ImGuiHandle,
            _                           => null
        };
    }

    private bool RequiresImageProcessing()
    {
        return _computedStyle.ImageRounding   > 0
         || _computedStyle.ImageBlackAndWhite == true
         || _computedStyle.ImageGrayscale     > 0
         || _computedStyle.ImageBrightness    > 0
         || _computedStyle.ImageContrast      > 0;
    }
}
