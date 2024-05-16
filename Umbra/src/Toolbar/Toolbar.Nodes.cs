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
using Umbra.Common;
using Umbra.Style;
using Una.Drawing;

namespace Umbra;

internal partial class Toolbar
{
    private readonly Node _toolbarNode = new() {
        Stylesheet = ToolbarStyles.ToolbarStylesheet,
        Id         = "Toolbar",
        ClassList  = ["toolbar"],
        Style = new() {
            Size = new(1920, 32)
        },
        ChildNodes = [
            new() {
                Id        = "Left",
                ClassList = ["toolbar-panel"],
                Style     = new() { Anchor = Anchor.MiddleLeft }
            },
            new() {
                Id        = "Center",
                ClassList = ["toolbar-panel"],
                Style     = new() { Anchor = Anchor.MiddleCenter }
            },
            new() {
                Id        = "Right",
                ClassList = ["toolbar-panel"],
                Style     = new() { Anchor = Anchor.MiddleRight }
            }
        ]
    };

    public Node GetPanel(string id) =>
        _toolbarNode.QuerySelector(id) ?? throw new InvalidOperationException($"Panel '{id}' not found.");

    public Node LeftPanel   => GetPanel("Left");
    public Node CenterPanel => GetPanel("Center");
    public Node RightPanel  => GetPanel("Right");

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
        switch (IsTopAligned) {
            case true when !_toolbarNode.TagsList.Contains("top"):
                _toolbarNode.TagsList.Remove("bottom");
                _toolbarNode.TagsList.Add("top");
                break;
            case false when !_toolbarNode.TagsList.Contains("bottom"):
                _toolbarNode.TagsList.Remove("top");
                _toolbarNode.TagsList.Add("bottom");
                break;
        }

        switch (IsStretched) {
            case true when !_toolbarNode.TagsList.Contains("stretched"):
                _toolbarNode.TagsList.Add("stretched");
                _toolbarNode.TagsList.Remove("floating");
                break;
            case false when !_toolbarNode.TagsList.Contains("floating"):
                _toolbarNode.TagsList.Add("floating");
                _toolbarNode.TagsList.Remove("stretched");
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
}
