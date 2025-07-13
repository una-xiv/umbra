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

using Dalamud.Game.ClientState.Keys;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Umbra.Common;
using Umbra.Widgets.System;
using Una.Drawing;

namespace Umbra;

internal partial class Toolbar
{
    private readonly Node _toolbarNode = UmbraDrawing.DocumentFrom("umbra.toolbar.xml").RootNode!;
    
    public Node? GetPanel(string id) =>
        id.StartsWith("aux")
            ? auxBars.FindOrMigrate(id)
            : _toolbarNode.FindById(id)
              ?? throw new InvalidOperationException($"Panel '{id}' not found.");
    
    public Node LeftPanel   => GetPanel("Left")!;
    public Node CenterPanel => GetPanel("Center")!;
    public Node RightPanel  => GetPanel("Right")!;

    private void RenderAuxBarNodes()
    {
        foreach (var (auxBarNode, config) in auxBars.VisibleAuxBarPanels) {
            auxBarNode.ToggleClass("left-aligned", config.XAlign == "Left");
            auxBarNode.ToggleClass("right-aligned", config.XAlign == "Right");
            auxBarNode.ToggleClass("center-aligned", config.XAlign == "Center");

            Vector2 workPos = ImGui.GetMainViewport().WorkPos;

            float xPos = config.XAlign switch {
                "Center" => (ToolbarXPosition - (auxBarNode.Bounds.MarginSize.Width / 2f)) + config.XPos,
                "Left"   => config.XPos,
                "Right"  => (int)(ImGui.GetMainViewport().WorkPos.X + ImGui.GetMainViewport().WorkSize.X - config.XPos - auxBarNode.Bounds.MarginSize.Width),
                _        => config.XPos,
            };
            
            auxBarNode.Render(
                ImGui.GetBackgroundDrawList(ImGui.GetMainViewport()),
                new(xPos, (int)workPos.Y + config.YPos)
            );
        }
    }

    /// <summary>
    /// Renders the toolbar.
    /// </summary>
    private void RenderToolbarNode()
    {
        _toolbarNode.Style.Gap     = ItemSpacing * 2;
        _toolbarNode.Style.Opacity = player.IsEditingHud ? 0.66f : _autoHideOpacity;
        LeftPanel.Style.Gap        = ItemSpacing;
        CenterPanel.Style.Gap      = ItemSpacing;
        RightPanel.Style.Gap       = ItemSpacing;

        _toolbarNode.Render(
            ImGui.GetBackgroundDrawList(ImGui.GetMainViewport()),
            new(ToolbarXPosition, (int)(ToolbarYPosition + _autoHideYOffset))
        );
    }

    /// <summary>
    /// Updates the width of the toolbar.
    /// </summary>
    private void UpdateToolbarWidth()
    {
        _toolbarNode.ToggleClass("stretched", IsStretched);

        float width  = IsStretched ? ImGui.GetMainViewport().WorkSize.X : 0;

        _toolbarNode.Style.Size = new(width / Node.ScaleFactor, Height);
        _toolbarNode.Style.Padding = new() {
            Left  = (int)(ToolbarLeftMargin / Node.ScaleFactor),
            Right = (int)(ToolbarRightMargin / Node.ScaleFactor)
        };
    }

    /// <summary>
    /// Updates the class list of the toolbar node based on the current configuration.
    /// </summary>
    private void UpdateToolbarNodeClassList()
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

        _toolbarNode.ToggleClass("shadow", EnableShadow && (!EnableInactiveColors || IsCursorNearToolbar()));
        _toolbarNode.ToggleClass("top", IsTopAligned);
        _toolbarNode.ToggleClass("bottom", !IsTopAligned);
        _toolbarNode.ToggleClass("rounded", !IsStretched && RoundedCorners);
    }

    /// <summary>
    /// Returns the horizontal position of the toolbar.
    /// </summary>
    private static float ToolbarXPosition =>
        (ImGui.GetMainViewport().WorkPos.X + (ImGui.GetMainViewport().WorkSize.X / 2));

    /// <summary>
    /// Returns the vertical position of the toolbar.
    /// </summary>
    private static float ToolbarYPosition =>
        IsTopAligned
            ? ImGui.GetMainViewport().WorkPos.Y + YOffset
            : ImGui.GetMainViewport().WorkPos.Y + (int)ImGui.GetMainViewport().WorkSize.Y - YOffset;
}
