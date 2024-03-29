﻿/* Umbra.Interface | (c) 2024 by Una    ____ ___        ___.
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
using Dalamud.Interface.Internal;
using ImGuiNET;
using SixLabors.ImageSharp.Processing;

namespace Umbra.Interface;

public partial class Element
{
    private static readonly Dictionary<string, IDalamudTextureWrap> ProcessedImageCache = [];

    private string ProcessedImageCacheKey =>
        $"{_computedStyle.Image!.Value.Path ?? _computedStyle.Image!.Value.IconId!.Value.ToString()}_{_computedStyle.ImageRounding}_{_computedStyle.ImageBlackAndWhite}_{_computedStyle.ImageGrayscale}_{_computedStyle.ImageBrightness}_{_computedStyle.ImageContrast}";

    private void RenderImage(ImDrawListPtr drawList)
    {
        IntPtr? img = GetImageToRender();
        if (null == img) return;

        Rect rect = ContentBox;

        drawList.AddImage(
            img.Value,
            rect.Min,
            rect.Max
        );
    }

    private IntPtr? GetImageToRender()
    {
        if (_computedStyle.Image == null) return null;
        if (false       == RequiresImageProcessing()) return GetImagePtr();

        if (ProcessedImageCache.TryGetValue(ProcessedImageCacheKey, out IDalamudTextureWrap? processedImage)) {
            return processedImage.ImGuiHandle;
        }

        using var image = _computedStyle.Image.Value.IconId is > 0
            ? ImageRepository.GetIconFile(_computedStyle.Image.Value.IconId!.Value).GetImage()
            : ImageRepository.GetLocalImage(_computedStyle.Image.Value.Path!);

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

        uint? iconId = _computedStyle.Image.Value.IconId;

        if (iconId is > 0) {
            return ImageRepository.GetIcon(iconId.Value).ImGuiHandle;
        }

        string? path = _computedStyle.Image.Value.Path;
        return path is { Length: > 0 } ? ImageRepository.GetLocalTexture(path).ImGuiHandle : IntPtr.Zero;
    }

    private bool RequiresImageProcessing()
    {
        return _computedStyle.ImageRounding > 0 || _computedStyle.ImageBlackAndWhite == true || _computedStyle.ImageGrayscale > 0 || _computedStyle.ImageBrightness > 0 || _computedStyle.ImageContrast > 0;
    }
}
