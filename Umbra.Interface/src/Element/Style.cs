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
    public uint?           ForegroundColor { get; set; }
    public uint?           BackgroundColor { get; set; }
    public Gradient?       Gradient        { get; set; }
    public uint?           BorderColor     { get; set; }
    public int?            BorderWidth     { get; set; }
    public int?            BorderRadius    { get; set; }
    public RoundedCorners? RoundedCorners  { get; set; }
    public Anchor?         TextAlign       { get; set; }
    public Font?           Font            { get; set; }
    public Shadow?         Shadow          { get; set; }
    public uint?           OutlineColor    { get; set; }
    public sbyte?          OutlineWidth    { get; set; }
    public ImageReference? Image           { get; set; }

    public uint GetForegroundColor(Element element) =>
        ForegroundColor ?? element.Parent?.Style.GetForegroundColor(element.Parent) ?? 0xFFFFFFFF;

    public uint GetBackgroundColor(Element element) =>
        BackgroundColor ?? element.Parent?.Style.GetBackgroundColor(element.Parent) ?? 0;

    public Gradient GetGradient(Element element) =>
        Gradient ?? element.Parent?.Style.GetGradient(element.Parent) ?? new Gradient(0);

    public uint GetBorderColor(Element element) =>
        BorderColor ?? element.Parent?.Style.GetBorderColor(element.Parent) ?? 0xFFFFFFFF;

    public int GetBorderWidth(Element element) =>
        BorderWidth ?? element.Parent?.Style.GetBorderWidth(element.Parent) ?? 0;

    public int GetBorderRadius(Element element) =>
        BorderRadius ?? element.Parent?.Style.GetBorderRadius(element.Parent) ?? 0;

    public RoundedCorners GetRoundedCorners(Element element) =>
        RoundedCorners ?? element.Parent?.Style.GetRoundedCorners(element.Parent) ?? Interface.RoundedCorners.All;

    public Anchor GetTextAlign(Element element) =>
        TextAlign ?? element.Parent?.Style.GetTextAlign(element.Parent) ?? Anchor.TopLeft;

    public Font GetFont(Element element) =>
        Font ?? element.Parent?.Style.GetFont(element.Parent) ?? Interface.Font.Default;

    public uint GetOutlineColor(Element element) =>
        OutlineColor ?? element.Parent?.Style.GetOutlineColor(element.Parent) ?? 0x80000000;

    public sbyte GetOutlineWidth(Element element) =>
        OutlineWidth ?? element.Parent?.Style.GetOutlineWidth(element.Parent) ?? 0;
}
