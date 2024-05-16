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
using Umbra.Style;
using Una.Drawing;

namespace Umbra.Widgets;

public abstract class WidgetPopup
{
    private readonly Node _popupNode = new() {
        Stylesheet = PopupStyles.WidgetPopupStylesheet,
        ClassList  = ["widget-popup"],
    };

    /// <summary>
    /// The content node of this popup.
    /// </summary>
    protected abstract Node Node { get; }

    /// <summary>
    /// Closes the popup.
    /// </summary>
    protected void Close()
    {
        _shouldClose = true;
    }

    private float _opacityDest;
    private float _opacity;
    private float _yOffsetDest;
    private float _yOffset;
    private bool  _shouldClose;
    private bool  _isOpen;

    public bool Render(ToolbarWidget activator)
    {
        if (_popupNode.ChildNodes.Count == 0) {
            _popupNode.ChildNodes.Add(Node);
        }

        if (!_isOpen) {
            _isOpen      = true;
            _shouldClose = false;
        }

        switch (Toolbar.IsTopAligned) {
            case true when !_popupNode.TagsList.Contains("top"):
                _popupNode.TagsList.Remove("bottom");
                _popupNode.TagsList.Add("top");
                break;
            case false when !_popupNode.TagsList.Contains("bottom"):
                _popupNode.TagsList.Remove("top");
                _popupNode.TagsList.Add("bottom");
                break;
        }

        switch (!Toolbar.IsStretched) {
            case true when !_popupNode.TagsList.Contains("floating"):
                _popupNode.TagsList.Add("floating");
                break;
            case false when _popupNode.TagsList.Contains("floating"):
                _popupNode.TagsList.Remove("floating");
                break;
        }

        // Reflow to pre-calculate bounds.
        _popupNode.Reflow();

        float width   = _popupNode.OuterWidth;
        float height  = _popupNode.OuterHeight;
        float actX    = activator.Node.Bounds.MarginRect.X1 + (activator.Node.OuterWidth / 2);
        Rect  tBounds = activator.Node.ParentNode!.ParentNode!.Bounds.MarginRect;

        // Calculate the position based on the anchor.
        float windowX     = actX - (width / 2);
        float windowY     = Toolbar.IsTopAligned ? tBounds.Y2 - 2 : tBounds.Y1 + 2;
        float screenWidth = ImGui.GetMainViewport().WorkSize.X;
        float deltaTime   = ImGui.GetIO().DeltaTime * 10;

        if (!Toolbar.IsTopAligned) {
            windowY -= height;
        }

        if (windowX < 16) {
            windowX = 16;
        } else if (windowX + width > screenWidth - 16) {
            windowX = screenWidth - width - 16;
        }

        // Animate the popup.
        _opacityDest = 1;
        _yOffsetDest = 0;

        _opacity = _opacity += (_opacityDest - _opacity) * deltaTime;
        _yOffset = _yOffset += (_yOffsetDest - _yOffset) * deltaTime;

        switch (Toolbar.IsTopAligned) {
            case true when _yOffset > -1:
            case false when _yOffset < 1:
                _yOffset = 0;
                break;
        }

        if (_opacity > 0.9) _opacity = 1;

        _popupNode.Style.Opacity     = _opacity;
        _popupNode.Style.ShadowInset = (int)(_yOffset + 8);

        // Draw the popup window.
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding,   0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);

        ImGui.SetNextWindowViewport(ImGui.GetMainViewport().ID);
        ImGui.SetNextWindowPos(new(windowX, windowY));
        ImGui.SetNextWindowSize(new(width, height));
        ImGui.Begin($"###Popup_{activator.Id.GetHashCode():X}", PopupWindowFlags);

        bool hasFocus = ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows);

        _popupNode.Render(ImGui.GetWindowDrawList(), new((int)windowX, (int)(windowY + _yOffset)));

        ImGui.End();
        ImGui.PopStyleVar(4);

        bool keepOpen = !_shouldClose && hasFocus;

        if (!keepOpen) {
            _isOpen      = false;
            _shouldClose = false;
        }

        return keepOpen;
    }

    public void Reset()
    {
        _opacity     = 0;
        _opacityDest = 0;
        _yOffset     = Toolbar.IsTopAligned ? -32 : 32;
        _yOffsetDest = 0;
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
