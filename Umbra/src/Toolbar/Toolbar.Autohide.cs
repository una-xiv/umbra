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

using ImGuiNET;
using Una.Drawing;

namespace Umbra;

internal partial class Toolbar
{
    public bool AllowAutoHide { get; set; } = true;

    private float _autoHideYOffset;
    private float _autoHideYTarget;
    private bool  _isVisible       = true;
    private float _autoHideOpacity = 1.0f;

    private void UpdateToolbarAutoHideOffset()
    {
        if (!ShouldAutoHide()) {
            _autoHideYOffset = 0;
            _autoHideYTarget = 0;
            _autoHideOpacity = 1;
            _isVisible       = true;

            _toolbarNode.Style.Opacity = _autoHideOpacity;
            return;
        }

        float height    = _toolbarNode.Height;
        float deltaTime = ImGui.GetIO().DeltaTime * 10f;

        if (!_isVisible && IsCursorNearToolbar()) {
            _isVisible       = true;
            _autoHideYTarget = 0;
        }

        if (_isVisible && !IsCursorNearToolbar()) {
            _isVisible       = false;
            _autoHideYTarget = IsTopAligned ? -(height + 2) : height + 2;
        }

        if (IsMultiMonitorSupportEnabled()) {
            // Don't slide the toolbar off the screen when multi-monitor
            // support is enabled, since it may cause significant drops
            // in framerate.
            _autoHideOpacity += ((_isVisible ? 1 : 0) - _autoHideOpacity) * deltaTime;
            _autoHideYOffset =  0;
            return;
        }

        _autoHideYTarget = IsTopAligned switch {
            // Correct the Y offset if the alignment changes.
            true when _autoHideYTarget > 0  => -(height + 2),
            false when _autoHideYTarget < 0 => height + 2,
            _                               => _autoHideYTarget
        };

        _autoHideOpacity += ((_isVisible ? 1 : 0) - _autoHideOpacity) * deltaTime;
        _autoHideYOffset += (_autoHideYTarget - _autoHideYOffset) * deltaTime;
    }

    private bool IsCursorNearToolbar()
    {
        if (!AllowAutoHide) return true;

        int nodeWidth  = _toolbarNode.Width;
        int nodeHeight = _toolbarNode.Height;

        int y = ToolbarYPosition - (IsTopAligned ? 0 :nodeHeight);
        int x = (nodeWidth / 2) + (ToolbarLeftMargin / 2) + (ToolbarRightMargin / 2);

        var bounds = new Rect(ToolbarXPosition - x, y, ToolbarXPosition + x, y + nodeHeight);
        bounds.Expand(new(_toolbarNode.Height / 2, 0));

        // Apply margins.
        bounds.X1 += ToolbarLeftMargin;
        bounds.X2 -= ToolbarRightMargin;

        // Change the hover area height based on visibility. This requires the user to nearly touch the edge of the
        // screen to show the toolbar, but allows for a larger area to hide it.
        if (IsTopAligned) {
            bounds.Y2 = _isVisible ? bounds.Y2 : bounds.Y2 - (bounds.Height - (int)(24 * Node.ScaleFactor));
        } else {
            bounds.Y1 = _isVisible ? bounds.Y1 : bounds.Y1 + (bounds.Height - (int)(24 * Node.ScaleFactor));
        }

        return bounds.Contains(ImGui.GetMousePos());
    }

    private bool ShouldAutoHide()
    {
        return IsAutoHideEnabled ||
               (AutoHideDuringCutscenes && player.IsInCutscene)
               || (AutoHideDuringDuty && player.IsBoundByDuty)
               || (AutoHideDuringPvp && player.IsInPvP);
    }

    private static bool IsMultiMonitorSupportEnabled()
    {
        return (ImGui.GetIO().ConfigFlags & ImGuiConfigFlags.ViewportsEnable) == ImGuiConfigFlags.ViewportsEnable;
    }
}
