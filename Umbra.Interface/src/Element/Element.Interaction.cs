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

using System;
using System.Numerics;
using ImGuiNET;
using Umbra.Common;

namespace Umbra.Interface;

public partial class Element
{
    public event Action? OnClick;
    public event Action? OnMiddleClick;
    public event Action? OnRightClick;
    public event Action? OnMouseEnter;
    public event Action? OnMouseLeave;

    /// <summary>
    /// True if the element has any interactive event listeners attached to it.
    /// </summary>
    public bool IsInteractive =>
        null != OnClick
     || null != OnMiddleClick
     || null != OnRightClick
     || null != OnMouseEnter
     || null != OnMouseLeave;

    /// <summary>
    /// True if the mouse cursor is currently inside the bounding box of the element.
    /// </summary>
    public bool IsMouseOver { get; private set; }

    /// <summary>
    /// True if this element currently has focus.
    /// </summary>
    public bool IsFocused { get; private set; }

    private bool _isInWindowOrInteractiveParent = false;

    private void SetupInteractive(ImDrawListPtr drawList)
    {
        if (!IsVisible || !IsInteractive) return;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,     Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize,  0);

        Vector2  boundingBoxSize   = BoundingBox.Size.ToVector2();
        Element? interactiveParent = GetInteractiveParent();
        _isInWindowOrInteractiveParent = IsInWindowDrawList(drawList) || interactiveParent != null;

        if (_isInWindowOrInteractiveParent) {
            ImGui.SetCursorScreenPos(BoundingBox.Min);
            ImGui.BeginChild(FullyQualifiedName, boundingBoxSize, false, InteractiveWindowFlags);
            ImGui.SetCursorScreenPos(ContentBox.Min);
        } else {
            ImGui.SetNextWindowPos(BoundingBox.Min, ImGuiCond.Always);
            ImGui.SetNextWindowSize(boundingBoxSize, ImGuiCond.Always);
            ImGui.Begin(FullyQualifiedName, InteractiveWindowFlags);
        }

        bool wasHovered = IsMouseOver;

        ImGui.SetCursorScreenPos(BoundingBox.Min);
        ImGui.InvisibleButton($"{FullyQualifiedName}##Button", ContentBox.Size.ToVector2());
        IsMouseOver = ImGui.IsItemHovered();
        IsFocused   = ImGui.IsItemFocused();

        switch (wasHovered) {
            case false when IsMouseOver:
                OnMouseEnter?.Invoke();
                break;
            case true when !IsMouseOver:
                OnMouseLeave?.Invoke();
                break;
        }

        if (IsMouseOver) {
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {
                OnClick?.Invoke();
            }

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Middle)) {
                OnMiddleClick?.Invoke();
            }

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right)) {
                OnRightClick?.Invoke();
            }
        }
    }

    private void EndInteractive()
    {
        if (!IsVisible || !IsInteractive) return;

        if (_isInWindowOrInteractiveParent) {
            ImGui.EndChild();
        } else {
            ImGui.End();
        }

        ImGui.PopStyleVar(4);
    }

    /// <summary>
    /// Returns true if the given drawList is not the foreground or background drawList.
    /// </summary>
    /// <param name="drawList"></param>
    /// <returns></returns>
    private static unsafe bool IsInWindowDrawList(ImDrawListPtr drawList)
    {
        return
            drawList.NativePtr != ImGui.GetForegroundDrawList().NativePtr
         && drawList.NativePtr != ImGui.GetBackgroundDrawList().NativePtr;
    }

    /// <summary>
    /// Returns the top-most interactive parent element.
    /// </summary>
    /// <returns></returns>
    private Element? GetInteractiveParent()
    {
        var parent = Parent;

        Element? interactiveParent = null;

        while (parent != null) {
            if (parent.IsInteractive) interactiveParent = parent;
            parent = parent.Parent;
        }

        return interactiveParent;
    }

    private static ImGuiWindowFlags InteractiveWindowFlags =>
        ImGuiWindowFlags.NoTitleBar
      | ImGuiWindowFlags.NoBackground
      | ImGuiWindowFlags.NoDecoration
      | ImGuiWindowFlags.NoResize
      | ImGuiWindowFlags.NoMove
      | ImGuiWindowFlags.NoScrollbar
      | ImGuiWindowFlags.NoScrollWithMouse
      | ImGuiWindowFlags.NoCollapse
      | ImGuiWindowFlags.NoSavedSettings
      | ImGuiWindowFlags.NoDocking
      | ImGuiWindowFlags.NoNavFocus
      | ImGuiWindowFlags.NoNavInputs
      | ImGuiWindowFlags.NoFocusOnAppearing;
}
