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

using Dalamud.Interface.Internal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Umbra.Interface;

public static class ImageSharpExtensions
{
    /// <summary>
    /// Converts the image to raw pixel data.
    /// </summary>
    public static byte[] ImageToRaw(this Image<Rgba32> image)
    {
        var data = new byte[4 * image.Width * image.Height];

        image.CopyPixelDataTo(data);

        return data;
    }

    /// <summary>
    /// Converts the image to an instance of <see cref="IDalamudTextureWrap"/>.
    /// </summary>
    public static IDalamudTextureWrap ToDalamudTextureWrap(this Image<Rgba32> image)
    {
        return ImageRepository.LoadImageRaw(image.ImageToRaw(), image.Width, image.Height, 4);
    }

    /// <summary>
    /// Applies rounded corners to the image.
    /// </summary>
    public static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext context, float cornerRadius)
    {
        SixLabors.ImageSharp.Size size = context.GetCurrentSize();
        IPathCollection corners = BuildCorners(size.Width, size.Height, cornerRadius);

        context.SetGraphicsOptions(new GraphicsOptions() {
            Antialias            = true,
            AlphaCompositionMode = PixelAlphaCompositionMode.DestOut
        });

        foreach (IPath path in corners) context = context.Fill(SixLabors.ImageSharp.Color.Red, path);

        return context;
    }

    private static PathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
    {
        IPath cornerTopLeft = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius)
            .Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

        float rightPos  = imageWidth - cornerTopLeft.Bounds.Width + 1;
        float bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;

        IPath cornerTopRight    = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
        IPath cornerBottomLeft  = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
        IPath cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

        return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
    }
}
