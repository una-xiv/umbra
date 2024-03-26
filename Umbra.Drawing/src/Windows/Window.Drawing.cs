/* Umbra.Drawing | (c) 2024 by Una      ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Drawing is free software: you can       \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Numerics;
using ImGuiNET;
using Umbra.Common;

namespace Umbra.Drawing;

public abstract partial class Window
{
    /// <summary>
    /// The current content size of the window.
    /// </summary>
    public Size ContentSize { get; private set; } = new(400, 300);

    /// <summary>
    /// Defines the spacing between the window frame and its contents.
    /// </summary>
    protected Spacing Padding { get; set; } = new(8, 8, 8, 8);

    [ConfigVariable("window.enableClipping", "Window Settings", "Enable window clipping", "Allows windows to render behind native game windows. This may have a slight performance cost and may not work properly for all native game windows.")]
    private static bool EnableClipping { get; set; } = true;

    public void Draw(ClipRectSolver clipRectSolver)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);

        ImGui.SetNextWindowSize(new(MinSize.Width + 2, MinSize.Height + 33), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowSizeConstraints(new(MinSize.Width + 2, MinSize.Height + 33), new(MaxSize.Width + 2, MaxSize.Height + 33));
        ImGui.Begin($"Window_{Id}", DefaultWindowFlags);

        IsFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows);
        IsHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows);

        ContentSize = new((int)ImGui.GetWindowSize().X - 2, (int)ImGui.GetWindowSize().Y - 33);

        if (!EnableClipping || IsFocused) {
            DrawWindowBody(0);
            ImGui.End();
            ImGui.PopStyleVar(6);
            return;
        }

        var ownClipRect = new ClipRect(ImGui.GetWindowPos(), ImGui.GetWindowPos() + ImGui.GetWindowSize());
        var solvedRects = clipRectSolver.Solve(ownClipRect);

        if (solvedRects.SolvedRects.Count == 0) {
            DrawWindowBody(0);
            ImGui.End();
            ImGui.PopStyleVar(6);
            return;
        }

        // Disable mouse input when it's hovering native rects.
        solvedRects.NativeRects.ForEach(rect => {
            if (rect.Contains(ImGui.GetMousePos()))
                ImGui.SetNextFrameWantCaptureMouse(false);
        });

        // Nothing to render if the window is completely overlapped.
        if (solvedRects.IsOverlapped) {
            ImGui.End();
            ImGui.PopStyleVar(6);
            return;
        }

        for (int i = 0; i < solvedRects.SolvedRects.Count; i++) {
            ContentSize = new((int)ImGui.GetWindowSize().X - 2, (int)ImGui.GetWindowSize().Y - 33);
            var rect = solvedRects.SolvedRects[i];
            ImGui.PushClipRect(rect.Min, rect.Max, false);
            DrawWindowBody(i + 1);
            ImGui.PopClipRect();
        }

        ImGui.End();
        ImGui.PopStyleVar(6);
    }

    private void DrawWindowBody(int instanceId)
    {
        Backdrop.Size = new(ContentSize.Width + 1, ContentSize.Height + 32);
        Backdrop.GetNode<ShadowNode>().IsVisible = IsFocused;
        Backdrop.Render(ImGui.GetWindowDrawList(), new(0, 0));

        try {
            DrawTitlebar(instanceId);
            DrawContents(instanceId);
        } catch (Exception e) {
            Logger.Error($"An error occurred while rendering the window: {e}");
        }
    }

    private void DrawTitlebar(int instanceId)
    {
        _titlebar.Size = new Size(ImGui.GetWindowWidth(), 32);
        _titlebar.GetNode<RectNode>("Highlight").Color = IsFocused ? ImGui.GetColorU32(ImGuiCol.TabHovered) : ImGui.GetColorU32(ImGuiCol.TabUnfocused);
        _titlebar.Get("Title").GetNode<TextNode>().Text = Title;

        ImGui.SetCursorPos(new(0, 1));
        ImGui.BeginChild($"Window_{Id}##{instanceId}_Titlebar", new(ContentSize.Width, 32), false);
        _titlebar.Render(ImGui.GetWindowDrawList(), new(1, 1));
        ImGui.EndChild();
    }

    private void DrawContents(int instanceId)
    {
        ImGui.SetCursorPos(new(0, 32));
        ImGui.BeginChild($"Window_{Id}##{instanceId}_Contents", new(ContentSize.Width, ContentSize.Height), false, ImGuiWindowFlags.NoMove);

        ContentSize      = new(ContentSize.Width - Padding.Left - Padding.Right, ContentSize.Height - Padding.Top - Padding.Bottom);
        _contents.Size    = ContentSize;
        _contents.Padding = Padding;
        _contents.GetNode<RectNode>().Margin = new(-Padding.Top, -Padding.Right + 1, -Padding.Bottom + 1, -Padding.Left);

        OnBeforeDraw();

        _contents.Render(ImGui.GetWindowDrawList(), new(1, 0));

        OnAfterDraw();

        ImGui.EndChild();
    }

    private static ImGuiWindowFlags DefaultWindowFlags =>
        ImGuiWindowFlags.NoTitleBar |
        ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoCollapse |
        ImGuiWindowFlags.NoDocking |
        ImGuiWindowFlags.NoScrollWithMouse |
        ImGuiWindowFlags.NoBackground;
}
