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

public class LightPreset : IThemePreset
{
    public string Name                => "Light";
    public uint   Background          => 0xFFE0E0E0;
    public uint   BackgroundDark      => 0xFFD0D0D0;
    public uint   BackgroundLight     => 0xFFF0F0F0;
    public uint   BackgroundActive    => 0xFFFFFFFF;
    public uint   Border              => 0xFFA0A0A0;
    public uint   BorderDark          => 0xFF787778;
    public uint   BorderLight         => 0xFF282828;
    public uint   Text                => 0xFF252525;
    public uint   TextLight           => 0xFF000000;
    public uint   TextMuted           => 0xFF404040;
    public uint   TextOutline         => 0x10FFFFFF;
    public uint   TextOutlineLight    => 0x20FFFFFF;
    public uint   HighlightBackground => 0xA0405079;
    public uint   HighlightForeground => 0xFFFFFFFF;
    public uint   HighlightOutline    => 0x40000000;
    public uint   Accent              => 0x307FCFFF;
}
