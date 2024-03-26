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
using Dalamud.Utility;
using Lumina.Data.Files;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace Umbra.Drawing;

public static class TexFileExtensions
{
    private static readonly Dictionary<TexFile, Rgba32> DominantColorsCache = [];

    /// <summary>
    /// Returns an instance of <see cref="Image{Rgba32}"/> from the texture file.
    /// </summary>
    public static Image<Rgba32> GetImage(this TexFile file)
    {
        return Image.LoadPixelData<Rgba32>(file.GetRgbaImageData(), file.Header.Width, file.Header.Height);
    }

    /// <summary>
    /// Get the dominant color of the image.
    /// </summary>
    /// <param name="file">The texture file.</param>
    /// <returns>The dominant color.</returns>
    public static Rgba32 GetDominantColor(this TexFile file)
    {
        if (DominantColorsCache.TryGetValue(file, out var cachedColor)) return cachedColor;

        using var image = file.GetImage();

        image.Mutate(ctx => ctx
            .Resize(new ResizeOptions() { Sampler = KnownResamplers.NearestNeighbor, Size = new SixLabors.ImageSharp.Size(100, 0) })
            .Quantize(new OctreeQuantizer(new QuantizerOptions() { MaxColors = 1, Dither = null })));

        return DominantColorsCache[file] = image[0, 0];
    }
}
