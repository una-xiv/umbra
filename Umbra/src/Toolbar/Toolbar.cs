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

using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;
using Umbra.Interface;

namespace Umbra.Toolbar;

[Service]
internal partial class Toolbar(Player player, IToolbarWidget[] widgets)
{
    public const int Height = 32;

    [ConfigVariable(
        "Toolbar.Enabled",
        "Toolbar Settings",
        "Enable the toolbar",
        "Whether to display the toolbar. Use the \"/umbra\" command to open the settings window if you decide to disable the toolbar."
    )]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable(
        "Toolbar.IsTopAligned",
        "Toolbar Settings",
        "Place the toolbar at the top of the screen.",
        "Aligns the toolbar to the top of the screen rather than the bottom."
    )]
    public static bool IsTopAligned { get; set; } = false;

    private readonly List<IToolbarWidget> _widgets = [..widgets];

    [OnDraw]
    public void OnDraw()
    {
        if (!Enabled || player.IsInCutscene) {
            _element.IsVisible = false;
            return;
        }

        _element.IsVisible = true;
        UpdateToolbar();

        foreach (var widget in _widgets) {
            AssignWidgetContainer(widget);
            widget.OnDraw();
        }

        RenderToolbar();
    }

    [OnTick(interval: 23)]
    public void OnTick()
    {
        foreach (var widget in _widgets) {
            widget.OnUpdate();
        }
    }

    private void AssignWidgetContainer(IToolbarWidget widget)
    {
        Element left   = _element.Get("Left"),
                middle = _element.Get("Middle"),
                right  = _element.Get("Right");

        if (widget.Element.Anchor.IsLeft() && widget.Element.Parent != left) {
            left.AddChild(widget.Element);
        } else if (widget.Element.Anchor.IsCenter() && widget.Element.Parent != middle) {
            middle.AddChild(widget.Element);
        } else if (widget.Element.Anchor.IsRight() && widget.Element.Parent != right) {
            right.AddChild(widget.Element);
        }
    }
}
