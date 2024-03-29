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

namespace Umbra.Interface;

public class Style
{
    /// <summary>
    /// Defines the filled background color of the element.
    /// </summary>
    public uint? BackgroundColor;

    /// <summary>
    /// Defines the gradient color settings of this element. These colors are
    /// overlaid on top of the background color.
    /// </summary>
    public Gradient? Gradient;

    /// <summary>
    /// Defines the border color of the background rectangle. This border
    /// follows the same shape as the background rectangle.
    /// </summary>
    public uint? BackgroundBorderColor;

    /// <summary>
    /// Defines the line thickness of the background border.
    /// </summary>
    public int? BackgroundBorderWidth;

    /// <summary>
    /// Defines the corner rounding of the background rectangle.
    /// </summary>
    public int? BackgroundRounding;

    /// <summary>
    /// Defines which corners of the background rectangle should be rounded.
    /// </summary>
    public RoundedCorners? RoundedCorners;

    /// <summary>
    /// Specifies individual border colors for each side of the element. Note
    /// that this border is separate from the background border and does not
    /// have rounded corners.
    /// </summary>
    public SideValue<uint>? BorderColor;

    /// <summary>
    /// Defines the line thickness of the border. This can be set for each edge
    /// individually. Note that this border is separate from the background
    /// border and does not have rounded corners.
    /// </summary>
    public SideValue<int>? BorderWidth;

    /// <summary>
    /// Defines the color of the text.
    /// </summary>
    public uint? TextColor;

    /// <summary>
    /// Defines "box shadow" properties for the element. Set to NULL to disable
    /// the shadow effect.
    /// </summary>
    public Shadow? Shadow;

    /// <summary>
    /// The font face to use for rendering text.
    /// </summary>
    public Font? Font;

    /// <summary>
    /// Defines the alignment of the text within the element.
    /// </summary>
    public Anchor? TextAlign;

    /// <summary>
    /// Defines the offset of the text within the element. This is useful for
    /// fonts that don't align correctly when centered.
    /// </summary>
    public Vector2? TextOffset;

    /// <summary>
    /// Defines the outline color of the text.
    /// </summary>
    public uint? OutlineColor;

    /// <summary>
    /// Defines the outline width of the text.
    /// </summary>
    public sbyte? OutlineWidth;

    /// <summary>
    /// A reference to an image to render within the element. Images are drawn
    /// on top of any background color or gradient and below text.
    /// </summary>
    public ImageReference? Image;

    /// <summary>
    /// Defines the corner rounding of the image.
    /// </summary>
    public float? ImageRounding;

    /// <summary>
    /// Defines the grayscale factor of the image. A value of 0.0 is full color
    /// and 1.0 is full grayscale.
    /// </summary>
    public float? ImageGrayscale;

    /// <summary>
    /// Enables or disables black and white rendering of the image.
    /// </summary>
    public bool? ImageBlackAndWhite;

    /// <summary>
    /// Adjusts the brightness of the image.
    /// </summary>
    public float? ImageBrightness;

    /// <summary>
    /// Adjusts the contrast of the image.
    /// </summary>
    public float? ImageContrast;

    /// <summary>
    /// Defines the opacity of this element.
    /// </summary>
    public float? Opacity;

    public Style() { }

    public Style(Style other)
    {
        TextColor             = other.TextColor;
        BackgroundColor       = other.BackgroundColor;
        Gradient              = other.Gradient;
        BackgroundBorderColor = other.BackgroundBorderColor;
        BackgroundBorderWidth = other.BackgroundBorderWidth;
        BackgroundRounding    = other.BackgroundRounding;
        BorderColor           = other.BorderColor;
        BorderWidth           = other.BorderWidth;
        RoundedCorners        = other.RoundedCorners;
        TextAlign             = other.TextAlign;
        TextOffset            = other.TextOffset;
        Font                  = other.Font;
        Shadow                = other.Shadow;
        OutlineColor          = other.OutlineColor;
        OutlineWidth          = other.OutlineWidth;
        Image                 = other.Image;
        ImageRounding         = other.ImageRounding;
        ImageGrayscale        = other.ImageGrayscale;
        ImageBlackAndWhite    = other.ImageBlackAndWhite;
        ImageBrightness       = other.ImageBrightness;
        ImageContrast         = other.ImageContrast;
        Opacity               = other.Opacity;
    }

    /// <summary>
    /// Merges inheritable properties from another style into this style.
    /// </summary>
    /// <param name="other"></param>
    internal void Merge(Style other)
    {
        TextColor             ??= other.TextColor;
        BackgroundBorderColor ??= other.BackgroundBorderColor;
        TextAlign             ??= other.TextAlign;
        Font                  ??= other.Font;
        OutlineColor          ??= other.OutlineColor;
        OutlineWidth          ??= other.OutlineWidth;

        if (Opacity != null && other.Opacity != null) {
            Opacity *= other.Opacity.Value;
        }
    }
}
