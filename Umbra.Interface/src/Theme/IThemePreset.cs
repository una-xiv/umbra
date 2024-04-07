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

public interface IThemePreset
{
    /// <summary>
    /// The name of this theme preset.
    /// </summary>
    public string Name { get; }

    public uint Background          { get; }
    public uint BackgroundDark      { get; }
    public uint BackgroundLight     { get; }
    public uint BackgroundActive    { get; }
    public uint Border              { get; }
    public uint BorderDark          { get; }
    public uint BorderLight         { get; }
    public uint Text                { get; }
    public uint TextLight           { get; }
    public uint TextMuted           { get; }
    public uint TextOutline         { get; }
    public uint TextOutlineLight    { get; }
    public uint HighlightBackground { get; }
    public uint HighlightForeground { get; }
    public uint HighlightOutline    { get; }
    public uint Accent              { get; }
}
