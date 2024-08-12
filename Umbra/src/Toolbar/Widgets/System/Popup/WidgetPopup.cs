﻿/* Umbra | (c) 2024 by Una              ____ ___        ___.
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
using System;
using Umbra.Common;
using Umbra.Style;
using Umbra.Widgets.System;
using Una.Drawing;

namespace Umbra.Widgets;

public abstract class WidgetPopup : IDisposable
{
    [ConfigVariable("Toolbar.PopupAlignmentMethod", "General", "Toolbar", options: ["Center", "Aligned"])]
    public static string PopupAlignmentMethod { get; set; } = "Aligned";

    public event Action? OnPopupOpen;
    public event Action? OnPopupClose;

    internal Vector2 Position { get; set; }
    internal Vector2 Size     { get; set; }

    private readonly Node _popupNode = new() {
        Stylesheet = PopupStyles.WidgetPopupStylesheet,
        ClassList  = ["widget-popup"],
    };

    /// <summary>
    /// The content node of this popup.
    /// </summary>
    protected abstract Node Node { get; }

    /// <summary>
    /// Forces the popup to be placed within the main viewport.
    /// </summary>
    protected bool ForcePopupInMainViewport { get; set; }

    public void Dispose()
    {
        OnDisposed();
        ContextMenu?.Dispose();
        _popupNode.Dispose();

        foreach (var handler in OnPopupOpen?.GetInvocationList() ?? []) OnPopupOpen   -= (Action)handler;
        foreach (var handler in OnPopupClose?.GetInvocationList() ?? []) OnPopupClose -= (Action)handler;
    }

    /// <summary>
    /// Closes the popup.
    /// </summary>
    protected void Close()
    {
        _shouldClose = true;
    }

    protected virtual bool CanOpen()
    {
        return true;
    }

    protected virtual void OnUpdate() { }

    protected virtual void OnOpen() { }

    protected virtual void OnClose() { }

    protected virtual void OnDisposed() { }

    protected ContextMenu? ContextMenu { get; set; }

    private float _opacityDest;
    private float _opacity;
    private float _yOffsetDest;
    private float _yOffset;
    private bool  _shouldClose;
    private bool  _isOpen;

    private readonly ContextMenuManager _contextMenuManager = Framework.Service<ContextMenuManager>();

    /// <summary>
    /// True if the popup is currently open.
    /// </summary>
    public bool IsOpen => _isOpen;

    public bool Render(ToolbarWidget activator)
    {
        if (!CanOpen()) {
            return false;
        }

        if (_popupNode.ChildNodes.Count == 0) {
            _popupNode.ChildNodes.Add(Node);
        }

        if (!_isOpen) {
            _isOpen      = true;
            _shouldClose = false;
            OnPopupOpen?.Invoke();
            OnOpen();
        }

        switch (IsPopupOpenDownward(activator)) {
            case true when !_popupNode.TagsList.Contains("top"):
                _popupNode.TagsList.Remove("bottom");
                _popupNode.TagsList.Add("top");
                break;
            case false when !_popupNode.TagsList.Contains("bottom"):
                _popupNode.TagsList.Remove("top");
                _popupNode.TagsList.Add("bottom");
                break;
        }

        switch (!Toolbar.IsStretched || WidgetManager.EnforceFloatingPopups) {
            case true when !_popupNode.TagsList.Contains("floating"):
                _popupNode.TagsList.Add("floating");
                break;
            case false when _popupNode.TagsList.Contains("floating"):
                _popupNode.TagsList.Remove("floating");
                break;
        }

        OnUpdate();

        // Reflow to pre-calculate bounds.
        _popupNode.Reflow();

        Size = new(_popupNode.OuterWidth, _popupNode.OuterHeight);

        Position = PopupAlignmentMethod == "Aligned"
            ? GetPopupPositionAligned(activator, Size)
            : GetPopupPositionCentered(activator, Size);

        // Animate the popup.
        float deltaTime = ImGui.GetIO().DeltaTime * 10;

        _opacityDest = 1;
        _yOffsetDest = 0;

        _opacity = _opacity += (_opacityDest - _opacity) * deltaTime;
        _yOffset = _yOffset += (_yOffsetDest - _yOffset) * deltaTime;

        switch (IsPopupOpenDownward(activator)) {
            case true when _yOffset > -1:
            case false when _yOffset < 1:
                _yOffset = 0;
                break;
        }

        if (_opacity > 0.9) _opacity = 1;

        _popupNode.Style.Opacity     = _opacity;
        _popupNode.Style.ShadowInset = (int)(_yOffset + 8);
        _popupNode.Style.ShadowSize  = WidgetManager.EnableWidgetPopupShadow ? null : new(0);

        // Draw the popup window.
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding,   0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);

        if (ForcePopupInMainViewport) {
            // NOTE: This line breaks multi-monitor support, but omitting it
            //       seems to crash the game when the gearset switcher popup
            //       is opened. Issue has been narrowed down to the fact that
            //       the popup contains ImGui scrolling child frames.
            ImGui.SetNextWindowViewport(ImGui.GetMainViewport().ID);
        }

        ImGui.SetNextWindowPos(new(Position.X, Position.Y));
        ImGui.SetNextWindowSize(new(Size.X, Size.Y));
        ImGui.Begin($"###Popup_{activator.Id.GetHashCode():X}", PopupWindowFlags);

        bool hasFocus = ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows);

        _popupNode.Render(ImGui.GetWindowDrawList(), new((int)Position.X, (int)(Position.Y + _yOffset)));

        if (ContextMenu is { ShouldRender: true }) {
            _contextMenuManager.Draw();
        }

        ImGui.End();
        ImGui.PopStyleVar(4);

        bool keepOpen = !_shouldClose && hasFocus;

        if (!keepOpen) {
            _isOpen      = false;
            _shouldClose = false;
            OnClose();
            OnPopupClose?.Invoke();
        }

        return keepOpen;
    }

    public void Reset()
    {
        if (_isOpen) {
            _isOpen      = false;
            _shouldClose = false;
            OnPopupClose?.Invoke();
            OnClose();
        }

        _opacity     = 0;
        _opacityDest = 0;
        _yOffset     = Toolbar.IsTopAligned ? -32 : 32;
        _yOffsetDest = 0;
    }

    private static Vector2 GetPopupPositionAligned(ToolbarWidget activator, Vector2 size)
    {
        string toolbarColumnId      = activator.Node.ParentNode!.Id!;
        Rect   activatorBoundingBox = activator.Node.Bounds.MarginRect;
        Rect   toolbarBoundingBox   = activator.Node.ParentNode!.ParentNode!.Bounds.MarginRect;

        float popupX = GetXOffsetsOf(activator, size);

        float popupY = IsPopupOpenDownward(activator)
            ? toolbarBoundingBox.Y2 - 2
            : (toolbarBoundingBox.Y1 + 2) - size.Y;

        var viewportPos  = ImGui.GetMainViewport().WorkPos;
        var viewportSize = ImGui.GetMainViewport().WorkSize;

        if (popupX < viewportPos.X) {
            popupX = viewportPos.X + 8;
        } else if (popupX + size.X > viewportPos.X + viewportSize.X) {
            popupX = viewportPos.X + (viewportSize.X - size.X - 8);
        }

        return new(popupX, popupY);
    }

    private static float GetXOffsetsOf(ToolbarWidget activator, Vector2 size)
    {
        string toolbarColumnId      = activator.Node.ParentNode!.Id!;
        Rect   activatorBoundingBox = activator.Node.Bounds.MarginRect;

        if (toolbarColumnId == "aux") {
            if (activator.Node.ParentNode.ParentNode!.ClassList.Contains("left-aligned")) {
                toolbarColumnId = "Left";
            } else if (activator.Node.ParentNode.ParentNode!.ClassList.Contains("right-aligned")) {
                toolbarColumnId = "Right";
            } else {
                toolbarColumnId = "Center";
            }
        }

        float originX = toolbarColumnId switch {
            "Left"   => activatorBoundingBox.X1,
            "Center" => activatorBoundingBox.X1 + (activatorBoundingBox.Width / 2),
            "Right"  => activatorBoundingBox.X2,
            _        => 0
        };

        float popupX = toolbarColumnId switch {
            "Left"   => originX - 32,
            "Center" => originX - (size.X / 2),
            "Right"  => originX - size.X + 32,
            _        => 0
        };

        return popupX;
    }

    private static Vector2 GetPopupPositionCentered(ToolbarWidget activator, Vector2 size)
    {
        float actX    = activator.Node.Bounds.MarginRect.X1 + (activator.Node.OuterWidth / 2);
        Rect  tBounds = activator.Node.ParentNode!.ParentNode!.Bounds.MarginRect;

        float windowX     = actX - (size.X / 2);
        float windowY     = IsPopupOpenDownward(activator) ? tBounds.Y2 - 2 : tBounds.Y1 + 2;
        float screenWidth = ImGui.GetMainViewport().WorkSize.X;
        float screenPosX  = ImGui.GetMainViewport().WorkPos.X;

        if (!IsPopupOpenDownward(activator)) {
            windowY -= size.Y;
        }

        if (windowX < screenPosX + 8) {
            windowX = screenPosX + 8;
        } else if (screenPosX + windowX + size.X > screenWidth - 8) {
            windowX = screenPosX + screenWidth - size.X - 8;
        }

        return new(windowX, windowY);
    }

    /// <summary>
    /// Returns true if the popup associated with the given widget should be opened downward.
    /// </summary>
    private static bool IsPopupOpenDownward(ToolbarWidget activator)
    {
        if (activator.Node.ParentNode!.Id == "aux") {
            var h = ImGui.GetMainViewport().WorkPos.Y + (ImGui.GetMainViewport().WorkSize.Y / 2);
            var y = activator.Node.Bounds.MarginRect.Y1 + activator.Node.OuterHeight;

            return y < h;
        }

        return Toolbar.IsTopAligned;
    }

    private static ImGuiWindowFlags PopupWindowFlags =>
        ImGuiWindowFlags.NoTitleBar
        | ImGuiWindowFlags.NoResize
        | ImGuiWindowFlags.NoMove
        | ImGuiWindowFlags.NoScrollbar
        | ImGuiWindowFlags.NoSavedSettings
        | ImGuiWindowFlags.NoDecoration
        | ImGuiWindowFlags.NoBackground
        | ImGuiWindowFlags.NoDocking
        | ImGuiWindowFlags.NoNav
        | ImGuiWindowFlags.NoNavInputs
        | ImGuiWindowFlags.NoNavFocus;
}
