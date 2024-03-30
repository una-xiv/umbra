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

using System.Numerics;
using ImGuiNET;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar;

[Service]
internal partial class Toolbar
{
    [ConfigVariable(
        "Toolbar.IsTopAligned",
        "Toolbar Settings",
        "Place the toolbar at the top of the screen.",
        "Aligns the toolbar to the top of the screen rather than the bottom."
    )]
    private static bool IsTopAligned { get; set; } = false;

    [ConfigVariable(
        "Toolbar.Height",
        "Toolbar Settings",
        "Toolbar Height",
        "Defines the height of the toolbar in pixels.",
        min: 24,
        max: 48,
        step: 1
    )]
    private static int Height { get; set; } = 32;

    public Toolbar()
    {
        _element.Get("Left").AddChild(new ButtonElement("L1", "Left Button"));
        _element.Get("Middle").AddChild(new ButtonElement("M1", "Middle Button"));
        _element.Get("Right").AddChild(new ButtonElement("R1", "Right Button"));
    }

    [OnDraw]
    public void OnDraw()
    {
        Vector2 displaySize = ImGui.GetMainViewport().Size;
        float   yPosition   = IsTopAligned ? ImGui.GetMainViewport().WorkPos.Y : displaySize.Y;

        _element.Anchor = IsTopAligned ? Anchor.TopLeft : Anchor.BottomLeft;
        _element.Size   = new((int)displaySize.X, Height);

        _element.Get<GradientElement>().Gradient = Gradient.Vertical(
            IsTopAligned ? Color2 : Color1,
            IsTopAligned ? Color1 : Color2
        );

        _element.Render(ImGui.GetBackgroundDrawList(), new(0, yPosition));
    }
}
