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
using Umbra.Windows.Clipping;
using Una.Drawing;
using Rect = Umbra.Windows.Clipping.Rect;

namespace Umbra.Windows;

internal abstract partial class Window
{
    [ConfigVariable("Window.EnableClipping", "General", "WindowSettings")]
    private static bool EnableClipping { get; set; } = true;

    protected abstract Vector2 MinSize     { get; }
    protected abstract Vector2 MaxSize     { get; }
    protected abstract Vector2 DefaultSize { get; }

    protected Size ContentSize { get; private set; } = new();

    public bool IsFocused { get; private set; }
    public bool IsHovered { get; private set; }
    public bool IsMinimized { get; private set; }

    private void RenderWindow(string id)
    {
        ImGui.SetNextWindowViewport(ImGui.GetMainViewport().ID);
        ImGui.SetNextWindowSize(DefaultSize, ImGuiCond.FirstUseEver);

        if (!IsMinimized) {
            ImGui.SetNextWindowSizeConstraints(MinSize * Node.ScaleFactor, MaxSize * Node.ScaleFactor);

            if (CurrentWindowSize.Y > 35 * Node.ScaleFactor) {
                ImGui.SetNextWindowSize(CurrentWindowSize, ImGuiCond.Always);
            }
        } else {
            ImGui.SetNextWindowSizeConstraints(
                MinSize with { Y = 35 * Node.ScaleFactor },
                MaxSize with { Y = 35 * Node.ScaleFactor }
            );
        }

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding,   0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,     new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding,    0);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize,  0);
        ImGui.Begin($"{id}", ImGuiWindowFlags | (IsMinimized ? ImGuiWindowFlags.NoSavedSettings : ImGuiWindowFlags.None));

        IsFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows);
        IsHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows);

        if (!IsMinimized) {
            CurrentWindowSize = ImGui.GetWindowSize();
        }

        Vector2 size = ImGui.GetWindowSize() / Node.ScaleFactor;
        size.X = (float)Math.Floor(size.X);
        size.Y = (float)Math.Floor(size.Y);

        _windowNode.Style.Size      = new((int)size.X - 2, (int)size.Y - 2);
        TitlebarNode.Style.Size     = new((int)size.X - 7, 32);
        TitlebarTextNode.Style.Size = new((int)size.X - 64, 32);

        ContentNode.Style.Size = new((int)size.X - 7, (int)size.Y - 39);
        Node.Style.Margin      = new(1);
        ContentSize            = new(ContentNode.Style.Size.Width - 2, ContentNode.Style.Size.Height - 2);

        // Only enable shadow if the window has focus.
        _windowNode.Style.ShadowSize = IsFocused ? new(64) : new(0);

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
        var drawList = ImGui.GetWindowDrawList();

        ImGui.SetCursorPos(new(0, 0));
        ImGui.BeginChild($"Window_{id}##{instanceId}", ImGui.GetWindowSize(), false);

        if (!IsMinimized) {
            OnUpdate(instanceId);
        }

        Vector2 ps = ImGui.GetWindowPos();
        Point   pt = new((int)ps.X + 2, (int)ps.Y + 2);

        _windowNode.Render(drawList, pt);

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
