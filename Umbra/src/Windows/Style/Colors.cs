/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows;

public class Colors
{
    [WhenFrameworkCompiling]
    private static void RegisterColors()
    {
        Color.AssignByName("Window.Background",                  0xFF212021);
        Color.AssignByName("Window.BackgroundLight",             0xFF292829);
        Color.AssignByName("Window.Border",                      0xFF484848);
        Color.AssignByName("Window.TitlebarBackground",          0xFF101010);
        Color.AssignByName("Window.TitlebarBorder",              0xFF404040);
        Color.AssignByName("Window.TitlebarGradient1",           0xFF2F2E2F);
        Color.AssignByName("Window.TitlebarGradient2",           0xFF1A1A1A);
        Color.AssignByName("Window.TitlebarText",                0xFFD0D0D0);
        Color.AssignByName("Window.TitlebarTextOutline",         0xC0000000);
        Color.AssignByName("Window.TitlebarCloseButton",         0xFF101010);
        Color.AssignByName("Window.TitlebarCloseButtonBorder",   0xFF404040);
        Color.AssignByName("Window.TitlebarCloseButtonHover",    0xFF904030);
        Color.AssignByName("Window.TitlebarCloseButtonX",        0xFFD0D0D0);
        Color.AssignByName("Window.TitlebarCloseButtonXHover",   0xFFFFFFFF);
        Color.AssignByName("Window.TitlebarCloseButtonXOutline", 0xFF000000);
        Color.AssignByName("Window.ScrollbarTrack",              0xFF212021);
        Color.AssignByName("Window.ScrollbarThumb",              0xFF484848);
        Color.AssignByName("Window.ScrollbarThumbHover",         0xFF808080);
        Color.AssignByName("Window.ScrollbarThumbActive",        0xFF909090);
        Color.AssignByName("Window.Text",                        0xFFD0D0D0);
        Color.AssignByName("Window.TextLight",                   0xFFFFFFFF);
        Color.AssignByName("Window.TextMuted",                   0xB0C0C0C0);
        Color.AssignByName("Window.TextOutline",                 0xC0000000);
        Color.AssignByName("Window.TextDisabled",                0xA0A0A0A0);
        Color.AssignByName("Window.AccentColor",                 0xFFb98e4c);

        Color.AssignByName("Input.Background",          0xFF151515);
        Color.AssignByName("Input.Border",              0xFF404040);
        Color.AssignByName("Input.Text",                0xFFD0D0D0);
        Color.AssignByName("Input.TextMuted",           0xA0D0D0D0);
        Color.AssignByName("Input.TextOutline",         0xC0000000);
        Color.AssignByName("Input.BackgroundHover",     0xFF212021);
        Color.AssignByName("Input.BorderHover",         0xFF707070);
        Color.AssignByName("Input.TextHover",           0xFFFFFFFF);
        Color.AssignByName("Input.TextOutlineHover",    0xFF000000);
        Color.AssignByName("Input.BackgroundDisabled",  0xE0212021);
        Color.AssignByName("Input.BorderDisabled",      0xC0404040);
        Color.AssignByName("Input.TextDisabled",        0xA0A0A0A0);
        Color.AssignByName("Input.TextOutlineDisabled", 0xC0000000);
    }
}
