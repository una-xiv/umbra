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

namespace Umbra.Interface;

public class Style
{
    public uint?           ForegroundColor;
    public uint?           BackgroundColor;
    public Gradient?       Gradient;
    public uint?           BorderColor;
    public int?            BorderWidth;
    public int?            BorderRadius;
    public RoundedCorners? RoundedCorners;
    public Anchor?         TextAlign;
    public Font?           Font;
    public Shadow?         Shadow;
    public uint?           OutlineColor;
    public sbyte?          OutlineWidth;
    public ImageReference? Image;
    public float?          ImageRounding;
    public float?          ImageGrayscale;
    public bool?           ImageBlackAndWhite;
    public float?          ImageBrightness;
    public float?          ImageContrast;

    public Style()
    {
    }

    public Style(Style other)
    {
        ForegroundColor    = other.ForegroundColor;
        BackgroundColor    = other.BackgroundColor;
        Gradient           = other.Gradient;
        BorderColor        = other.BorderColor;
        BorderWidth        = other.BorderWidth;
        BorderRadius       = other.BorderRadius;
        RoundedCorners     = other.RoundedCorners;
        TextAlign          = other.TextAlign;
        Font               = other.Font;
        Shadow             = other.Shadow;
        OutlineColor       = other.OutlineColor;
        OutlineWidth       = other.OutlineWidth;
        Image              = other.Image;
        ImageRounding      = other.ImageRounding;
        ImageGrayscale     = other.ImageGrayscale;
        ImageBlackAndWhite = other.ImageBlackAndWhite;
        ImageBrightness    = other.ImageBrightness;
        ImageContrast      = other.ImageContrast;
    }

    /// <summary>
    /// Merges inheritable properties from another style into this style.
    /// </summary>
    /// <param name="other"></param>
    internal void Merge(Style other)
    {
        ForegroundColor    ??= other.ForegroundColor;
        BorderColor        ??= other.BorderColor;
        TextAlign          ??= other.TextAlign;
        Font               ??= other.Font;
        OutlineColor       ??= other.OutlineColor;
        OutlineWidth       ??= other.OutlineWidth;
    }
}
