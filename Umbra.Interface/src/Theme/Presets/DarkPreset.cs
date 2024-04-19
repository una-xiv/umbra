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

namespace Umbra.Interface.Presets;

public class DarkPreset : IThemePreset
{
    public string Name                => "Dark";
    public uint   Background          => 0xFF212021;
    public uint   BackgroundDark      => 0xFF1A1A1A;
    public uint   BackgroundLight     => 0xFF2F2F2F;
    public uint   BackgroundActive    => 0xFF3F3F3F;
    public uint   Border              => 0xFF3F3F3F;
    public uint   BorderDark          => 0xFF101010;
    public uint   BorderLight         => 0xFF5A5A5A;
    public uint   Text                => 0xFFC0C0C0;
    public uint   TextLight           => 0xFFFFFFFF;
    public uint   TextMuted           => 0xFF9A9A9A;
    public uint   TextOutline         => 0x80000000;
    public uint   TextOutlineLight    => 0xCC000000;
    public uint   HighlightBackground => 0x305FCFFF;
    public uint   HighlightForeground => 0xFFFFFFFF;
    public uint   HighlightOutline    => 0x80000000;
    public uint   Accent              => 0x155FCFFF;
    public uint   ToolbarLight        => 0xFF212021;
    public uint   ToolbarDark         => 0xFF1A1A1A;
}
