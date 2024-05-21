/* Umbra.Interface | (c) 2024 by Una    ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Interface is free software: you can    \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Interface is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Numerics;
using ImGuiNET;

namespace Umbra.Interface;

public abstract partial class Window
{
    public void Render(ClipRectSolver clipRectSolver)
    {
        if (!IsVisible) {
            return;
        }

        ImGui.SetNextWindowViewport(ImGui.GetMainViewport().ID);
        ImGui.SetNextWindowSizeConstraints(MinSize.ToVector2() * Element.ScaleFactor, MaxSize.ToVector2() * Element.ScaleFactor);
        ImGui.SetNextWindowSize(DefaultSize.ToVector2(), ImGuiCond.FirstUseEver);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding,   0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,     new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding,    0);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize,  0);
        ImGui.Begin($"{Id}", WindowFlags);

        IsFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows);
        IsHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows);

        if (IsFocused || !EnableClipping) {
            RenderFinal();
            return;
        }

        var ownClipRect = new Rect(ImGui.GetWindowPos(), ImGui.GetWindowPos() + ImGui.GetWindowSize());
        var result      = clipRectSolver.Solve(ownClipRect);

        // If there are no solved rects, meaning nothing is intersecting with our window.
        if (result.SolvedRects.Count == 0) {
            RenderFinal();
            return;
        }

        // Disable mouse input when it's hovering native rects.
        result.NativeRects.ForEach(
            rect => {
                if (rect.Contains(ImGui.GetMousePos())) ImGui.SetNextFrameWantCaptureMouse(false);
            }
        );

        // Our window is fully overlapped by native game windows.
        if (result.IsOverlapped) {
            RenderFinal(false);
            return;
        }

        // Render the window contents for each solved rect.
        for (var i = 0; i < result.SolvedRects.Count; i++) {
            var rect = result.SolvedRects[i];
            ImGui.PushClipRect(rect.Min, rect.Max, false);
            RenderWindow(i + 1);
            ImGui.PopClipRect();
        }

        ImGui.End();
        ImGui.PopStyleVar(6);
    }

    private void RenderFinal(bool render = true)
    {
        switch (render) {
            case true:
                RenderWindow();
                break;
            case false:
                ImGui.SetNextFrameWantCaptureKeyboard(false);
                ImGui.SetNextFrameWantCaptureMouse(false);
                break;
        }

        ImGui.End();
        ImGui.PopStyleVar(6);
    }

    private void RenderWindow(int instanceId = 0)
    {
        ImGui.SetCursorPos(new(0, 0));
        ImGui.BeginChild($"Window_{Id}##{instanceId}", ImGui.GetWindowSize(), false);

        RenderWindowBackground();
        RenderTitlebar();

        ImGui.SetCursorPos(new Vector2(2, 34) * Element.ScaleFactor);
        ImGui.BeginChild($"Window_{Id}##{instanceId}_Body", ImGui.GetWindowSize() - (new Vector2(4, 36) * Element.ScaleFactor), false);

        OnDraw(instanceId);

        ImGui.EndChild();
        ImGui.EndChild();
    }

    /// <summary>
    /// Draws the window.
    /// </summary>
    /// <param name="instanceId">The instance ID of the window in case of clipped segments.</param>
    protected virtual void OnDraw(int instanceId) { }

    private static ImGuiWindowFlags WindowFlags =>
        ImGuiWindowFlags.NoTitleBar
      | ImGuiWindowFlags.NoCollapse
      | ImGuiWindowFlags.NoDocking
      | ImGuiWindowFlags.NoScrollbar
      | ImGuiWindowFlags.NoScrollWithMouse
      | ImGuiWindowFlags.NoBackground;
}
