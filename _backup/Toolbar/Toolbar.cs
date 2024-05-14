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
using System.Threading;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using Umbra.Common;
using Umbra.Game;
using Umbra.Interface;

namespace Umbra.Toolbar;

[Service]
internal partial class Toolbar(
    IPlayer             player,
    IToolbarWidget[]    widgets,
    ToolbarPopupContext popupContext,
    UmbraVisibility     visibility
)
{
    public const int Height = 32;

    [ConfigVariable("Toolbar.Enabled", "ToolbarSettings")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.IsTopAligned", "ToolbarSettings")]
    public static bool IsTopAligned { get; set; } = false;

    [ConfigVariable("Toolbar.IsAutoHideEnabled", "ToolbarSettings")]
    public static bool IsAutoHideEnabled { get; set; } = false;

    private readonly List<IToolbarWidget> _widgets = [..widgets];

    // Auto-hide state.
    private bool _isVisible = true;

    [OnDraw(executionOrder: 10)]
    public void OnDraw()
    {
        if (!visibility.IsVisible()) return;

        if (!Enabled) {
            _element.IsVisible = false;
            return;
        }

        _element.IsVisible = true;
        UpdateToolbar();

        foreach (var widget in _widgets) {
            AssignWidgetContainer(widget);
            widget.OnDraw();
        }

        // Auto-hide stuff.
        if (IsAutoHideEnabled) {
            float height = Height * Element.ScaleFactor * 1.25f;

            if (!_isVisible && IsCursorNearToolbar()) {
                _isVisible       = true;
                _autoHideYTarget = 0;
            }

            if (_isVisible && !IsCursorNearToolbar()) {
                _isVisible       = false;
                _autoHideYTarget = IsTopAligned ? -height : height;
            }

            _autoHideYTarget = IsTopAligned switch {
                // Correct the Y offset if the alignment changes.
                true when _autoHideYTarget  > 0 => -height,
                false when _autoHideYTarget < 0 => height,
                _                               => _autoHideYTarget
            };
        } else {
            _isVisible       = true;
            _autoHideYOffset = 0;
            _autoHideYTarget = 0;
        }

        RenderToolbar();
    }

    [OnTick(interval: 23)]
    public void OnTick()
    {
        if (!Enabled || player.IsEditingHud) return;

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

    private bool IsCursorNearToolbar()
    {
        if (popupContext.HasActiveElement()) return true;

        var cursorPos = ImGui.GetMousePos();

        var height = (int)(Height * Element.ScaleFactor * 1.5f);
        int minY   = IsTopAligned ? 0 : (int)(ImGui.GetIO().DisplaySize.Y - height);
        int maxY   = IsTopAligned ? height : (int)ImGui.GetIO().DisplaySize.Y;

        return cursorPos.Y >= minY && cursorPos.Y <= maxY;
    }
}
