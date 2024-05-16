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
using Umbra.Windows.Clipping;
using Una.Drawing;
using Rect = Umbra.Windows.Clipping.Rect;

namespace Umbra.Windows;

public abstract partial class Window
{
    [ConfigVariable("Window.EnableClipping", "General", "WindowSettings")]
    private static bool EnableClipping { get; set; } = true;

    protected abstract Vector2 MinSize     { get; }
    protected abstract Vector2 MaxSize     { get; }
    protected abstract Vector2 DefaultSize { get; }

    public bool IsFocused { get; private set; }
    public bool IsHovered { get; private set; }

    private void RenderWindow(string id)
    {
        ImGui.SetNextWindowViewport(ImGui.GetMainViewport().ID);
        ImGui.SetNextWindowSizeConstraints(MinSize * Node.ScaleFactor, MaxSize * Node.ScaleFactor);
        ImGui.SetNextWindowSize(DefaultSize, ImGuiCond.FirstUseEver);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding,   0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,     new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding,    0);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize,  0);
        ImGui.Begin($"{id}", ImGuiWindowFlags);

        IsFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows);
        IsHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows);

        if (IsFocused || !EnableClipping) {
            RenderFinal(id);
            return;
        }

        var ownClipRect = new Rect(ImGui.GetWindowPos(), ImGui.GetWindowPos() + ImGui.GetWindowSize());
        var result      = Framework.Service<ClipRectSolver>().Solve(ownClipRect);

        // If there are no solved rects, meaning nothing is intersecting with our window.
        if (result.SolvedRects.Count == 0) {
            RenderFinal(id);
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
            RenderFinal(id, false);
            return;
        }

        // Render the window contents for each solved rect.
        for (var i = 0; i < result.SolvedRects.Count; i++) {
            var rect = result.SolvedRects[i];
            ImGui.PushClipRect(rect.Min, rect.Max, false);
            RenderWindowInstance(id, i + 1);
            ImGui.PopClipRect();
        }

        ImGui.End();
        ImGui.PopStyleVar(6);
    }

    private void RenderFinal(string id, bool render = true)
    {
        switch (render) {
            case true:
                RenderWindowInstance(id);
                break;
            case false:
                ImGui.SetNextFrameWantCaptureKeyboard(false);
                ImGui.SetNextFrameWantCaptureMouse(false);
                break;
        }

        ImGui.End();
        ImGui.PopStyleVar(6);
    }

    private void RenderWindowInstance(string id, int instanceId = 0)
    {
        ImGui.SetCursorPos(new(0, 0));
        ImGui.BeginChild($"Window_{id}##{instanceId}", ImGui.GetWindowSize(), false);

        OnUpdate(instanceId);

        Vector2 ps = ImGui.GetWindowPos();
        Point   pt = new((int)ps.X, (int)ps.Y);

        _windowNode.Render(ImGui.GetWindowDrawList(), pt);

        ImGui.EndChild();
        ImGui.EndChild();
    }

    private static ImGuiWindowFlags ImGuiWindowFlags =>
        ImGuiWindowFlags.NoTitleBar
        | ImGuiWindowFlags.NoCollapse
        | ImGuiWindowFlags.NoDocking
        | ImGuiWindowFlags.NoScrollbar
        | ImGuiWindowFlags.NoScrollWithMouse
        | ImGuiWindowFlags.NoBackground;
}
