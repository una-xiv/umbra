

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

        float nodeWidth  = RightPanel.Bounds.PaddingRect.X2 - LeftPanel.Bounds.PaddingRect.X1;
        float nodeHeight = _toolbarNode.Height;

        float y = ToolbarYPosition - (IsTopAligned ? 0 :nodeHeight);
        float x = LeftPanel.Bounds.PaddingRect.X1;

        var bounds = new Rect(x, y, x + nodeWidth, y + nodeHeight);
        bounds.Expand(new(_toolbarNode.Height / 2f, 0));

        // Change the hover area height based on visibility. This requires the user to nearly touch the edge of the
        // screen to show the toolbar, but allows for a larger area to hide it.
        int offset = (int)(Math.Min(Height - 8, (Height * .9f)));
        if (IsTopAligned) {
            bounds.Y2 = _isVisible ? bounds.Y2 : bounds.Y2 - offset;
        } else {
            bounds.Y1 = _isVisible ? bounds.Y1 : bounds.Y1 + offset;
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
