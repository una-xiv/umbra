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

using Umbra.Widgets.System;

namespace Umbra;

internal partial class Toolbar
{
    private readonly Node _toolbarNode = UmbraDrawing.DocumentFrom("umbra.toolbar.xml").RootNode!;

    public Node? GetPanel(string id) =>
        id.StartsWith("aux")
            ? auxBars.FindOrMigrate(id)
            : _toolbarNode.FindById(id)
              ?? throw new InvalidOperationException($"Panel '{id}' not found.");

    public Node LeftPanel   => GetPanel("Left")!;
    public Node CenterPanel => GetPanel("Center")!;
    public Node RightPanel  => GetPanel("Right")!;

    private void RenderAuxBarNodes(ImDrawListPtr drawList)
    {
        foreach (var (auxBarNode, config) in auxBars.VisibleAuxBarPanels) {
            auxBarNode.ToggleClass("left-aligned", config.XAlign == "Left");
            auxBarNode.ToggleClass("right-aligned", config.XAlign == "Right");
            auxBarNode.ToggleClass("center-aligned", config.XAlign == "Center");
            auxBarNode.ToggleClass("top-aligned", config.YAlign == "Top");
            auxBarNode.ToggleClass("middle-aligned", config.YAlign == "Center");
            auxBarNode.ToggleClass("bottom-aligned", config.YAlign == "Bottom");

            var     viewport = ImGui.GetMainViewport();
            Vector2 workPos  = viewport.WorkPos;
            Vector2 workSize = viewport.WorkSize;

            // Render() 内の Reflow() で ComputeBounds() が走るため事前呼び出しは不要。
            // 前フレームのキャッシュ済みサイズを使用（1フレーム遅延は視覚的に無問題）。
            float xPos = config.XAlign switch {
                "Center" => (ToolbarXPosition - (auxBarNode.Bounds.MarginSize.Width / 2f)) + config.XPos,
                "Left"   => config.XPos,
                "Right"  => (int)(workPos.X + workSize.X - config.XPos - auxBarNode.Bounds.MarginSize.Width),
                _        => config.XPos,
            };

            float yPos = config.YAlign switch {
                "Center" => (workPos.Y + workSize.Y - (workSize.Y / 2f) - (auxBarNode.Bounds.MarginSize.Height / 2f)) + config.YPos,
                "Top"    => config.YPos,
                "Bottom" => (int)(workPos.Y + workSize.Y - config.YPos - auxBarNode.Bounds.MarginSize.Height),
                _        => config.YPos,
            };

            auxBarNode.Render(drawList, new(xPos, yPos));
        }
    }

    /// <summary>
    /// Renders the toolbar.
    /// </summary>
    private void RenderToolbarNode(ImDrawListPtr drawList)
    {
        _toolbarNode.Style.Gap     = ItemSpacing * 2;
        _toolbarNode.Style.Opacity = player.IsEditingHud ? 0.66f : _autoHideOpacity;
        LeftPanel.Style.Gap        = ItemSpacing;
        CenterPanel.Style.Gap      = ItemSpacing;
        RightPanel.Style.Gap       = ItemSpacing;

        LeftPanel.Style.Size   = new();
        CenterPanel.Style.Size = new();
        RightPanel.Style.Size  = new();

        if (_toolbarNode.ClassList.Contains("stretched")) {
            LeftPanel.ComputeBoundingSize();
            CenterPanel.ComputeBoundingSize();
            RightPanel.ComputeBoundingSize();

            float centerX1 = (ToolbarXPosition - (CenterPanel.Bounds.MarginSize.Width / 2f));
            float centerX2 = (ToolbarXPosition + (CenterPanel.Bounds.MarginSize.Width / 2f));

            LeftPanel.Style.Size.Width  = (int)centerX1 - 1;
            LeftPanel.Style.AutoSize    = (AutoSize.Fit, AutoSize.Grow);
            RightPanel.Style.Size.Width = (int)(ImGui.GetMainViewport().WorkSize.X - centerX2) - 1;
            RightPanel.Style.AutoSize   = (AutoSize.Fit, AutoSize.Grow);
        } else {
            LeftPanel.Style.AutoSize   = (AutoSize.Fit, AutoSize.Grow);
            CenterPanel.Style.AutoSize = (AutoSize.Fit, AutoSize.Grow);
            RightPanel.Style.AutoSize  = (AutoSize.Fit, AutoSize.Grow);
        }

        _toolbarNode.Render(drawList, new(ToolbarXPosition, (int)(ToolbarYPosition + _autoHideYOffset)));
    }

    /// <summary>
    /// Updates the width of the toolbar.
    /// </summary>
    private void UpdateToolbarWidth()
    {
        _toolbarNode.ToggleClass("stretched", IsStretched);

        float width = IsStretched ? ImGui.GetMainViewport().WorkSize.X : 0;

        _toolbarNode.Style.Size = new(width / Node.ScaleFactor, Height);
        _toolbarNode.Style.Padding = new() {
            Left  = (int)(ToolbarLeftMargin / Node.ScaleFactor),
            Right = (int)(ToolbarRightMargin / Node.ScaleFactor)
        };
    }

    /// <summary>
    /// Updates the class list of the toolbar node based on the current configuration.
    /// </summary>
    private void UpdateToolbarNodeClassList()
    {
        if (EnableInactiveColors) {
            if (!_cachedIsCursorNear && !_toolbarNode.TagsList.Contains("blur")) {
                _toolbarNode.TagsList.Add("blur");
            } else if (_cachedIsCursorNear && _toolbarNode.TagsList.Contains("blur")) {
                _toolbarNode.TagsList.Remove("blur");
            }
        } else {
            _toolbarNode.TagsList.Remove("blur");
        }

        _toolbarNode.ToggleClass("shadow", EnableShadow && (!EnableInactiveColors || _cachedIsCursorNear));
        _toolbarNode.ToggleClass("top", IsTopAligned);
        _toolbarNode.ToggleClass("bottom", !IsTopAligned);
        _toolbarNode.ToggleClass("rounded", !IsStretched && RoundedCorners);
    }

    /// <summary>
    /// Opens the toolbar host window if UseWindowDrawList is enabled and returns its draw list.
    /// Returns false if using the background draw list (no window was opened).
    /// </summary>
    private static unsafe bool BeginWindowDrawList(out ImDrawListPtr drawList)
    {
        if (!UseWindowDrawList) {
            drawList = ImGui.GetBackgroundDrawList(ImGui.GetMainViewport());
            Node.PassthroughDrawListHandles.Clear();
            return false;
        }

        var vp    = ImGui.GetMainViewport();
        var flags = ImGuiWindowFlags.NoTitleBar    | ImGuiWindowFlags.NoResize     | ImGuiWindowFlags.NoScrollbar
                  | ImGuiWindowFlags.NoInputs      | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoFocusOnAppearing
                  | ImGuiWindowFlags.NoDecoration  | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoDocking
                  | ImGuiWindowFlags.NoNavFocus    | ImGuiWindowFlags.NoNavInputs   | ImGuiWindowFlags.NoCollapse
                  | ImGuiWindowFlags.NoMove        | ImGuiWindowFlags.NoScrollWithMouse;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
        ImGui.SetNextWindowPos(vp.WorkPos);
        ImGui.SetNextWindowSize(vp.WorkSize);
        ImGui.SetNextWindowViewport(vp.ID);
        ImGui.Begin("##UmbraToolbarHost", flags);
        ImGui.PopStyleVar(2);
        ImGui.SetCursorScreenPos(Vector2.Zero);
        drawList = ImGui.GetWindowDrawList();
        Node.PassthroughDrawListHandles.Clear();
        Node.PassthroughDrawListHandles.Add((nint)drawList.Handle);
        return true;
    }

    /// <summary>
    /// Returns the horizontal position of the toolbar.
    /// </summary>
    private static float ToolbarXPosition {
        get {
            var vp = ImGui.GetMainViewport();
            return vp.WorkPos.X + (vp.WorkSize.X / 2);
        }
    }

    /// <summary>
    /// Returns the vertical position of the toolbar.
    /// </summary>
    private static float ToolbarYPosition {
        get {
            var vp = ImGui.GetMainViewport();
            return IsTopAligned
                ? vp.WorkPos.Y + YOffset
                : vp.WorkPos.Y + (int)vp.WorkSize.Y - YOffset;
        }
    }
}
