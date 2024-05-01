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

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;

namespace Umbra.Interface;

/// <summary>
/// Renders a container that shows a scrollbar when the children exceed the
/// height of the container. This element REQUIRES a single child element with
/// a vertical flow to function properly.
/// </summary>
public class OverflowContainer : Element
{
    private static uint _childIdCounter = 10000;

    private readonly uint _currentId;

    public OverflowContainer(string id, Size? size = null, Anchor anchor = Anchor.TopLeft, List<Element>? children = null) : base(
        id,
        size: size ?? new(),
        anchor: anchor,
        children: children,
        flow: Flow.Vertical
    )
    {
        _currentId = _childIdCounter++;
    }

    protected override void BeginDraw(ImDrawListPtr drawList)
    {
        ImGui.SetCursorScreenPos(ContentBox.Min);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,    new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding,   0);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, 0);
        ImGui.BeginChildFrame(_currentId, ContentBox.Size.ToVector2(), ImGuiWindowFlags.None);

        // Reset the cursor pos to the top left of the child frame.
        ImGui.SetCursorPos(Vector2.Zero);
        PushDrawList(ImGui.GetWindowDrawList());

        // Recompute the layout for the children.
        foreach (var child in Children) {
            child.ComputeLayout(ImGui.GetCursorScreenPos());
        }
    }

    protected override void EndDraw(ImDrawListPtr drawList)
    {
        // Grab the combined height of all children.
        float totalHeight = Children.Aggregate(0f, (current, child) => current + child.ComputedSize.Height);

        ImGui.SetCursorPos(new(0, totalHeight));
        ImGui.EndChildFrame();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar(3);
        PopDrawList();
    }
}
