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
using Umbra.Interface;

namespace Umbra;

[Service]
internal sealed class ToolbarPopupContext
{
    private DropdownElement? _activeElement;
    private Element?         _activator;

    public void RegisterDropdownActivator(Element activator, DropdownElement dropdownElement)
    {
        dropdownElement.IsVisible = false;

        activator.OnDelayedMouseEnter += () => {
            if (_activeElement != null) {
                Activate(dropdownElement, activator);
            }
        };

        activator.OnClick += () => {
            if (_activeElement == dropdownElement) {
                return;
            }

            Activate(dropdownElement, activator);
        };
    }

    public void Activate(DropdownElement dropdownElement, Element activator)
    {
        if (_activeElement == dropdownElement) return;

        _activeElement = dropdownElement;
        _activator     = activator;

        float toolbarHeight = Toolbar.Toolbar.Height * Element.ScaleFactor;

        _activeElement.Offset        = Toolbar.Toolbar.IsTopAligned ? new(0, -toolbarHeight) : new(0, toolbarHeight);
        _activeElement.Style.Opacity = 0;

        _activeElement.IsVisible = true;
        _activeElement.ComputeLayout(new(0, 0));

        _activeElement.Animate(new Animation<OutCirc>(300) {
            Offset  = new(0),
            Opacity = 1
        });
    }

    public void Clear()
    {
        if (_activeElement != null) {
            _activeElement.IsVisible = false;
        }

        _activeElement = null;
        _activator     = null;
    }

    [OnDraw]
    public void OnDraw()
    {
        if (_activeElement == null || _activator == null) {
            return;
        }

        UpdatePopupAnchor();
        Vector2 position = GetActivatorPosition() + GetPopupDrawPositionOffset();
        Vector2 winSize  = new(_activeElement.BoundingBox.Width, _activeElement.BoundingBox.Height);
        Vector2 offset   = new();

        if (_activeElement.Anchor.IsRight()) {
            offset.X = _activeElement.BoundingBox.Width / 2;

            // Clamp to the edge of the screen in case the dropdown is too close to the right edge of the screen.
            if (position.X + offset.X + _activeElement.BoundingBox.Width > ImGui.GetIO().DisplaySize.X) {
                offset.X = ImGui.GetIO().DisplaySize.X - position.X - _activeElement.BoundingBox.Width - 16;
            }

            offset.X += ImGui.GetMainViewport().Pos.X;
        }

        ImGui.SetNextWindowPos(position + offset);
        ImGui.SetNextWindowSize(winSize);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding,   0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);

        ImGui.SetNextWindowViewport(ImGui.GetMainViewport().ID);
        ImGui.Begin("##Dropdown", DropdownWindowFlags);

        _activeElement.Render(
            ImGui.GetWindowDrawList(),
            position - GetPopupDrawPositionOffset() + offset
        );

        if (!ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows)) {
            Clear();
        }

        ImGui.End();

        ImGui.PopStyleVar(3);
    }

    private Vector2 GetActivatorPosition()
    {
        if (_activator == null) return new();

        Vector2 displayPos = ImGui.GetMainViewport().Pos;

        float y = displayPos.Y
          + (
                Toolbar.Toolbar.IsTopAligned
                    ? ImGui.GetMainViewport().WorkPos.Y + ((Toolbar.Toolbar.Height + Toolbar.Toolbar.YOffset) * Element.ScaleFactor)
                    : ImGui.GetIO().DisplaySize.Y - ((Toolbar.Toolbar.Height + Toolbar.Toolbar.YOffset) * Element.ScaleFactor)
            );

        if (_activator.Anchor.IsLeft()) {
            return new(_activator.BoundingBox.X1, y);
        }

        if (_activator.Anchor.IsCenter()) {
            return new(_activator.BoundingBox.X1 + (_activator.BoundingBox.Width / 2), y);
        }

        return new(_activator.BoundingBox.X2, y);
    }

    private Vector2 GetPopupDrawPositionOffset()
    {
        if (_activeElement == null) return new();

        return _activeElement.Anchor switch {
            Anchor.TopLeft      => new(),
            Anchor.TopCenter    => new(-_activeElement.BoundingBox.Width / 2, 0),
            Anchor.TopRight     => new(-_activeElement.BoundingBox.Width, 0),
            Anchor.BottomLeft   => new(0, -_activeElement.BoundingBox.Height),
            Anchor.BottomCenter => new(-_activeElement.BoundingBox.Width / 2, -_activeElement.BoundingBox.Height),
            Anchor.BottomRight  => new(-_activeElement.BoundingBox.Width, -_activeElement.BoundingBox.Height),
            _                   => new()
        };
    }

    private void UpdatePopupAnchor()
    {
        switch (Toolbar.Toolbar.IsTopAligned) {
            case true:
                if (_activator!.Anchor.IsLeft()) {
                    _activeElement!.Anchor = Anchor.TopLeft;
                } else if (_activator.Anchor.IsCenter()) {
                    _activeElement!.Anchor = Anchor.TopCenter;
                } else {
                    _activeElement!.Anchor = Anchor.TopRight;
                }

                break;
            case false:
                if (_activator!.Anchor.IsLeft()) {
                    _activeElement!.Anchor = Anchor.BottomLeft;
                } else if (_activator.Anchor.IsCenter()) {
                    _activeElement!.Anchor = Anchor.BottomCenter;
                } else {
                    _activeElement!.Anchor = Anchor.BottomRight;
                }

                break;
        }
    }

    private static ImGuiWindowFlags DropdownWindowFlags =>
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
