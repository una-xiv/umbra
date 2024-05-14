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

using System;
using ImGuiNET;
using Umbra.Common;
using Umbra.Widgets.Library.Test1;
using Una.Drawing;

namespace Umbra;

[Service]
internal partial class Toolbar
{
    public Toolbar()
    {
        for (var i = 0; i < 10; i++) {
            var w1 = new Test1Widget();
            RightPanel.AppendChild(w1.Node);
        }

        for (var i = 0; i < 10; i++) {
            var w1 = new Test1Widget();
            LeftPanel.AppendChild(w1.Node);
        }
    }

    [OnDraw(executionOrder: -1)]
    private void DrawToolbar()
    {
        if (!Enabled) return;

        UpdateToolbarWidth();
        UpdateToolbarNodeClassList();
        UpdateToolbarAutoHideOffset();

        RenderToolbarNode();
    }
}
