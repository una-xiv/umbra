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

internal partial class Toolbar
{
    private const uint Color1      = 0xFF2F2F2F;
    private const uint Color2      = 0xFF212021;
    private const int  ItemSpacing = 6;

    private float _yPosition;

    private readonly Element _element = new(
        id: "Toolbar",
        anchor: Anchor.TopLeft,
        size: new(0, Height),
        flow: Flow.Horizontal,
        children: [
            new GradientElement(gradient: Gradient.Vertical(Color1, Color2)),
            new BorderElement(padding: new(-1), color: 0xFF505050),
            new("Left", anchor: Anchor.MiddleLeft, gap: ItemSpacing, padding: new(left: ItemSpacing)),
            new("Middle", anchor: Anchor.MiddleCenter, gap: ItemSpacing),
            new("Right", anchor: Anchor.MiddleRight, gap: ItemSpacing, padding: new(right: ItemSpacing)),
        ]
    );

    private void UpdateToolbar()
    {
        Vector2 displaySize = ImGui.GetMainViewport().Size;
        _yPosition = IsTopAligned ? ImGui.GetMainViewport().WorkPos.Y : displaySize.Y;

        _element.Anchor = IsTopAligned ? Anchor.TopLeft : Anchor.BottomLeft;
        _element.Size   = new((int)displaySize.X, Height);

        _element.Get<GradientElement>().Gradient = Gradient.Vertical(
            IsTopAligned ? Color2 : Color1,
            IsTopAligned ? Color1 : Color2
        );
    }

    private void RenderToolbar()
    {
        _element.Render(ImGui.GetBackgroundDrawList(), new(0, _yPosition));
    }
}
