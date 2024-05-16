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

namespace Umbra.Style;

internal static class Colors
{
    [WhenFrameworkCompiling]
    private static void RegisterColors()
    {
        // Toolbar-specific colors.
        Color.AssignByName("Toolbar.ShadowOpaque",      0xA0000000);
        Color.AssignByName("Toolbar.ShadowTransparent", 0x00000000);
        Color.AssignByName("Toolbar.Background1",       0xFF2F2E2F);
        Color.AssignByName("Toolbar.Background2",       0xFF1A1A1A);
        Color.AssignByName("Toolbar.Border",            0xFF484848);

        // Generic widget colors.
        Color.AssignByName("Widget.Background",      0xFF101010);
        Color.AssignByName("Widget.BackgroundHover", 0xFF2F2F2F);
        Color.AssignByName("Widget.Border",          0xFF484848);
        Color.AssignByName("Widget.BorderHover",     0xFF8A8A8A);
        Color.AssignByName("Widget.Text",            0xFFD0D0D0);
        Color.AssignByName("Widget.TextHover",       0xFFFFFFFF);
        Color.AssignByName("Widget.TextMuted",       0xFF909090);
        Color.AssignByName("Widget.TextOutline",     0xC0000000);

        // Generic popup colors.
        Color.AssignByName("Widget.PopupBackground",           0xFF101010);
        Color.AssignByName("Widget.PopupBackground.Gradient1", 0xFF2F2E2F);
        Color.AssignByName("Widget.PopupBackground.Gradient2", 0xFF1A1A1A);
        Color.AssignByName("Widget.PopupBorder",               0xFF484848);

        // Menu-popup colors.
        Color.AssignByName("Widget.PopupMenuText",             0xFFD0D0D0);
        Color.AssignByName("Widget.PopupMenuTextMuted",        0xFFB0B0B0);
        Color.AssignByName("Widget.PopupMenuTextHover",        0xFFFFFFFF);
        Color.AssignByName("Widget.PopupMenuBackgroundHover",  0x802F5FFF);
        Color.AssignByName("Widget.PopupMenuTextOutline",      0xA0000000);
        Color.AssignByName("Widget.PopupMenuTextOutlineHover", 0xA0000000);
    }
}
