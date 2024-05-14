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
using ImGuiNET;
using Una.Drawing;

namespace Umbra;

internal partial class Toolbar
{
    private readonly Node _toolbarNode = new() {
        Id        = "Toolbar",
        ClassList = ["toolbar", "toolbar-bottom"],
        Style = new() {
            Size = new(1920, 32)
        },
        ChildNodes = [
            new() {
                Id        = "LeftPanel",
                ClassList = ["toolbar-panel"],
                Style     = new() { Anchor = Anchor.MiddleLeft }
            },
            new() {
                Id        = "CenterPanel",
                ClassList = ["toolbar-panel"],
                Style     = new() { Anchor = Anchor.MiddleCenter }
            },
            new() {
                Id        = "RightPanel",
                ClassList = ["toolbar-panel"],
                Style     = new() { Anchor = Anchor.MiddleRight }
            }
        ]
    };

    /// <summary>
    /// Renders the toolbar.
    /// </summary>
    private void RenderToolbarNode()
    {
        _toolbarNode.Render(
            ImGui.GetBackgroundDrawList(),
            new(ToolbarXPosition, (int)(ToolbarYPosition + _autoHideYOffset))
        );
    }

    /// <summary>
    /// Updates the width of the toolbar.
    /// </summary>
    private void UpdateToolbarWidth()
    {
        if (IsStretched) {
            _toolbarNode.Style.Size!.Width = (int)Math.Ceiling(ImGui.GetMainViewport().Size.X);
            return;
        }

        _toolbarNode.Style.Size = new(
            LeftPanel.OuterWidth
            + CenterPanel.OuterWidth
            + RightPanel.OuterWidth
            + 12,
            32
        );
    }

    /// <summary>
    /// Updates the class list of the toolbar node based on the current configuration.
    /// </summary>
    private void UpdateToolbarNodeClassList()
    {
        string topClass    = IsStretched ? "toolbar-stretched-top" : "toolbar-floating-top";
        string bottomClass = IsStretched ? "toolbar-stretched-bottom" : "toolbar-floating-bottom";

        switch (IsTopAligned) {
            case true when !_toolbarNode.ClassList.Contains(topClass):
                _toolbarNode.ClassList.Remove("toolbar-stretched-bottom");
                _toolbarNode.ClassList.Remove("toolbar-floating-bottom");
                _toolbarNode.ClassList.Add(topClass);
                break;
            case false when !_toolbarNode.ClassList.Contains(bottomClass):
                _toolbarNode.ClassList.Remove("toolbar-stretched-top");
                _toolbarNode.ClassList.Remove("toolbar-floating-top");
                _toolbarNode.ClassList.Add(bottomClass);
                break;
        }
    }

    /// <summary>
    /// Returns the horizontal position of the toolbar.
    /// </summary>
    private static int ToolbarXPosition => (int)(ImGui.GetMainViewport().WorkSize.X / 2);

    /// <summary>
    /// Returns the vertical position of the toolbar.
    /// </summary>
    private static int ToolbarYPosition =>
        IsTopAligned
            ? (int)ImGui.GetMainViewport().WorkPos.Y + YOffset
            : (int)ImGui.GetIO().DisplaySize.Y - YOffset;

    private Node LeftPanel   => _toolbarNode.QuerySelector("LeftPanel")!;
    private Node CenterPanel => _toolbarNode.QuerySelector("CenterPanel")!;
    private Node RightPanel  => _toolbarNode.QuerySelector("RightPanel")!;
}
