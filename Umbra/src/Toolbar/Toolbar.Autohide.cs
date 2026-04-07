

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

        Vector2 mousePos = ImGui.GetMousePos();

        // Check main toolbar bounds
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

        if (bounds.Contains(mousePos)) return true;

        // Check autohide-enabled auxbars
        Vector2 workPos  = ImGui.GetMainViewport().WorkPos;
        Vector2 workSize = ImGui.GetMainViewport().WorkSize;

        foreach (var (auxBarNode, config) in auxBars.VisibleAuxBarPanels) {
            if (!config.EnableAutoHide) continue;

            if (config.IsConditionallyVisible) return true;

            float auxHeight = auxBarNode.Bounds.MarginSize.Height;
            float auxWidth  = auxBarNode.Bounds.MarginSize.Width;

            float auxXPos = config.XAlign switch {
                "Center" => (ToolbarXPosition - (auxWidth / 2f)) + config.XPos,
                "Left"   => config.XPos,
                "Right"  => (int)(workPos.X + workSize.X - config.XPos - auxWidth),
                _        => config.XPos,
            };

            float auxYPos = config.YAlign switch {
                "Center" => (workPos.Y + workSize.Y - (workSize.Y / 2f) - (auxHeight / 2f)) + config.YPos,
                "Top"    => config.YPos,
                "Bottom" => (int)(workPos.Y + workSize.Y - config.YPos - auxHeight),
                _        => config.YPos,
            };

            var auxBounds = new Rect(auxXPos, auxYPos, auxXPos + auxWidth, auxYPos + auxHeight);
            auxBounds.Expand(new(auxHeight / 2f, 0));

            // Apply same hover logic to auxbars
            int auxOffset = (int)(Math.Min(auxHeight - 2, (auxHeight * .9f)));
            if (config.YAlign == "Top") {
                auxBounds.Y2 = _isVisible ? auxBounds.Y2 : auxBounds.Y2 - auxOffset;
            } else if (config.YAlign == "Bottom") {
                auxBounds.Y1 = _isVisible ? auxBounds.Y1 : auxBounds.Y1 + auxOffset;
            }

            if (auxBounds.Contains(mousePos)) return true;
        }

        return false;
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
