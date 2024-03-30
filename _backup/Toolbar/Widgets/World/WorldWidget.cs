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
using Umbra.Drawing;
using Umbra.Game;

namespace Umbra;

[Service]
internal sealed partial class WorldWidget(Player player) : IToolbarWidget
{
    [ConfigVariable(
        "Toolbar.Widget.World.Enabled",
        "Toolbar Widgets",
        "Show visiting world name",
        "Shows the name of the current world when you are visiting a world that is not your home world."
    )]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.Height")] private static int Height { get; set; } = 32;

    [OnDraw]
    public void OnDraw()
    {
        Element.IsVisible = Enabled && player.HomeWorldName != player.CurrentWorldName;
        if (!Enabled) return;

        Element.Size                                                 = new(0, Height - 6);
        Element.Get("Inner.Text.WorldName").GetNode<TextNode>().Text = player.CurrentWorldName;
    }
}
