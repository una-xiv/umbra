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
using System.Numerics;
using ImGuiNET;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar;

internal partial class Toolbar
{
    private readonly Color _color1 = Theme.Color(ThemeColor.ToolbarLight);
    private readonly Color _color2 = Theme.Color(ThemeColor.ToolbarDark);

    [ConfigVariable("Toolbar.ItemSpacing", "ToolbarSettings", min: 1, max: 32)]
    private static int ItemSpacing { get; set; } = 6;

    private float _xPosition;
    private float _yPosition;

    private readonly Element _element = new(
        id: "Toolbar",
        anchor: Anchor.TopLeft,
        size: new(0, Height),
        flow: Flow.Horizontal,
        children: [
            new GradientElement(gradient: new(0u)),
            new BorderElement(padding: new(-1), color: Theme.Color(ThemeColor.Border)),
            new("Left", anchor: Anchor.MiddleLeft, gap: ItemSpacing, padding: new(left: ItemSpacing)),
            new("Middle", anchor: Anchor.MiddleCenter, gap: ItemSpacing),
            new("Right", anchor: Anchor.MiddleRight, gap: ItemSpacing, padding: new(right: ItemSpacing)),
        ]
    );

    private void UpdateToolbar()
    {
        var     viewport    = ImGui.GetMainViewport();
        Vector2 displaySize = viewport.Size;
        Vector2 displayPos  = viewport.Pos;

        Element left  = _element.Get("Left");
        Element mid   = _element.Get("Middle");
        Element right = _element.Get("Right");

        left.Gap  = ItemSpacing;
        mid.Gap   = ItemSpacing;
        right.Gap = ItemSpacing;

        left.IsVisible  = !player.IsEditingHud;
        mid.IsVisible   = !player.IsEditingHud;
        right.IsVisible = !player.IsEditingHud;

        _element.Style.Opacity = player.IsEditingHud ? 0.80f : 1.0f;

        _xPosition = displayPos.X;
        _yPosition = displayPos.Y + (IsTopAligned ? ImGui.GetMainViewport().WorkPos.Y : displaySize.Y);

        _element.Anchor = IsTopAligned ? Anchor.TopLeft : Anchor.BottomLeft;
        _element.Size   = new((int)(MathF.Ceiling(displaySize.X / Element.ScaleFactor)), Height);

        _element.Get<GradientElement>().Gradient = Gradient.Vertical(
            IsTopAligned ? _color2 : _color1,
            IsTopAligned ? _color1 : _color2
        );
    }

    private void RenderToolbar()
    {
        _element.Render(ImGui.GetBackgroundDrawList(), new(_xPosition, _yPosition));
    }
}
