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
            Size = new(1920, Height),
            Gap  = ItemSpacing,
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
        _toolbarNode.FindById(id) ?? throw new InvalidOperationException($"Panel '{id}' not found.");

    public Node LeftPanel   => GetPanel("Left");
    public Node CenterPanel => GetPanel("Center");
    public Node RightPanel  => GetPanel("Right");

    /// <summary>
    /// Renders the toolbar.
    /// </summary>
    private void RenderToolbarNode()
    {
        _toolbarNode.Style.ShadowSize = new(EnableShadow && (
            !EnableInactiveColors || IsCursorNearToolbar()
        ) ? (Height * 2) : 0);

        _toolbarNode.Style.Gap     = ItemSpacing * 2;
        _toolbarNode.Style.Opacity = player.IsEditingHud ? 0.66f : null;
        LeftPanel.Style.Gap        = ItemSpacing;
        CenterPanel.Style.Gap      = ItemSpacing;
        RightPanel.Style.Gap       = ItemSpacing;

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
        if (EnableInactiveColors) {
            if (!IsCursorNearToolbar() && !_toolbarNode.TagsList.Contains("blur")) {
                _toolbarNode.TagsList.Add("blur");
            } else if (IsCursorNearToolbar() && _toolbarNode.TagsList.Contains("blur")) {
                _toolbarNode.TagsList.Remove("blur");
            }
        } else {
            _toolbarNode.TagsList.Remove("blur");
        }

        _toolbarNode.Style.Margin = new() {
            Left  = (int)(ToolbarLeftMargin / Node.ScaleFactor),
            Right = (int)(ToolbarRightMargin / Node.ScaleFactor)
        };

        if (IsStretched) {
            float sw = ImGui.GetMainViewport().Size.X;
            float pw = ToolbarLeftMargin + ToolbarRightMargin;

            LeftPanel.Style.Anchor   = Anchor.MiddleLeft;
            CenterPanel.Style.Anchor = Anchor.MiddleCenter;
            RightPanel.Style.Anchor  = Anchor.MiddleRight;

            _toolbarNode.Style.Size!.Width  = (int)Math.Ceiling((sw - pw) / Node.ScaleFactor);
            _toolbarNode.Style.Size!.Height = Height;
            return;
        }

        LeftPanel.Style.Anchor   = Anchor.MiddleCenter;
        CenterPanel.Style.Anchor = Anchor.MiddleCenter;
        RightPanel.Style.Anchor  = Anchor.MiddleCenter;

        _toolbarNode.Style.Size = new(
            (int)(LeftPanel.OuterWidth / Node.ScaleFactor)
            + (int)(CenterPanel.OuterWidth / Node.ScaleFactor)
            + (int)(RightPanel.OuterWidth / Node.ScaleFactor)
            + (ItemSpacing * 6),
            Height
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
    private static int ToolbarXPosition => (int)(ImGui.GetMainViewport().WorkPos.X + (ImGui.GetMainViewport().WorkSize.X / 2));

    /// <summary>
    /// Returns the vertical position of the toolbar.
    /// </summary>
    private static int ToolbarYPosition =>
        IsTopAligned
            ? (int)ImGui.GetMainViewport().WorkPos.Y + YOffset
            : (int)ImGui.GetMainViewport().WorkPos.Y + (int)ImGui.GetMainViewport().WorkSize.Y - YOffset;
}
