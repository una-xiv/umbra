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
using Umbra.Drawing;
using Umbra.Drawing.Prefabs;

namespace Umbra;

[Service]
internal sealed class ToolbarDropdownContext
{
    [ConfigVariable("Toolbar.IsTopAligned")]
    private static bool IsTopAligned { get; set; } = false;

    [ConfigVariable("Toolbar.Height")] private static int Height { get; set; } = 32;

    private DropdownElement? _activeElement;
    private Element?         _activator;

    public void RegisterDropdownActivator(Element activator, DropdownElement dropdownElement)
    {
        activator.OnMouseEnter += () => {
            if (_activeElement != null) {
                _activeElement = dropdownElement;
                _activator     = activator;
            }
        };

        activator.OnMouseDown += () => {
            if (_activeElement == dropdownElement) {
                return;
            }

            _activeElement = dropdownElement;
            _activator     = activator;
        };
    }

    public void Activate(DropdownElement dropdownElement, Element activator)
    {
        _activeElement = dropdownElement;
        _activator     = activator;
    }

    public void Clear()
    {
        _activeElement = null;
        _activator     = null;
    }

    [OnDraw]
    public void OnDraw()
    {
        if (null == _activeElement
         || null == _activator)
            return;

        if (_activeElement.BoundingBox.Width  == 0
         || _activeElement.BoundingBox.Height == 0)
            _activeElement.ComputeLayout();

        float y = UpdateY();
        float x = UpdateX();

        x -= 8;

        if (_activeElement.Anchor.HasFlag(Anchor.Right)) {
            x -= _activeElement.BoundingBox.Width;
        } else if (_activeElement.Anchor.HasFlag(Anchor.Center)) {
            x -= _activeElement.BoundingBox.Width / 2;
        }

        if (_activeElement.Anchor.HasFlag(Anchor.Bottom)) {
            y -= _activeElement.BoundingBox.Height + 15;
        }

        ImGui.SetNextWindowPos(new(x, y), ImGuiCond.Always);

        ImGui.SetNextWindowSize(
            new(_activeElement.BoundingBox.Width + 16, _activeElement.BoundingBox.Height + 16),
            ImGuiCond.Always
        );

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.Begin("ToolbarDropdown", DropdownWindowFlags);

        _activeElement.Anchor = Anchor.Left | (_activeElement.Anchor.HasFlag(Anchor.Top) ? Anchor.Top : Anchor.Bottom);

        _activeElement.Render(
            ImGui.GetWindowDrawList(),
            new(8, _activeElement.Anchor.HasFlag(Anchor.Top) ? 0 : _activeElement.BoundingBox.Height + 16)
        );

        if (!ImGui.IsWindowFocused()) {
            _activeElement = null;
            _activator     = null;
        }

        ImGui.End();
        ImGui.PopStyleVar(2);
    }

    private float UpdateY()
    {
        float y;

        if (IsTopAligned) {
            y                     = ImGui.GetMainViewport().WorkPos.Y + Height - 2;
            _activeElement!.Anchor = Anchor.Top;
        } else {
            y                     = ImGui.GetIO().DisplaySize.Y - Height;
            _activeElement!.Anchor = Anchor.Bottom;
        }

        return y;
    }

    private float UpdateX()
    {
        float x = 0;

        switch (FindActivatorWidgetContainerId(_activator!)) {
            case "ToolbarLeftWidgetContainer":
                x                     =  _activator!.BoundingBox.X;
                _activeElement!.Anchor |= Anchor.Left;
                break;
            case "ToolbarCenterWidgetContainer":
                x                     =  _activator!.BoundingBox.X + (_activator!.BoundingBox.Width / 2);
                _activeElement!.Anchor |= Anchor.Center;
                break;
            case "ToolbarRightWidgetContainer":
                x = _activator!.BoundingBox.X + _activeElement!.BoundingBox.Width;

                if (x >= ImGui.GetIO().DisplaySize.X - 16) {
                    x = _activator!.BoundingBox.X + _activator!.BoundingBox.Width;
                }

                _activeElement!.Anchor |= Anchor.Right;
                break;
        }

        return x;
    }

    private static string FindActivatorWidgetContainerId(Element element, int depth = 0)
    {
        if (element.Id == "ToolbarLeftWidgetContainer"
         || element.Id == "ToolbarCenterWidgetContainer"
         || element.Id == "ToolbarRightWidgetContainer"
           ) {
            return element.Id;
        }

        if (depth > 10) {
            throw new System.Exception("Could not find Toolbar Widget Container");
        }

        return FindActivatorWidgetContainerId(element.Parent!, depth + 1);
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
