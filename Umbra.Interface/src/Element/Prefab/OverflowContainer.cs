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
    public Color FadeColor          { get; set; }         = new(0);
    public bool  IsScrolledToTop    { get; private set; } = false;
    public bool  IsScrolledToBottom { get; private set; } = false;

    private static uint _childIdCounter = 10000;

    private readonly uint  _currentId;
    private readonly Color _transparent = new();

    private readonly Element _fader = new(
        id: "_Fader",
        anchor: Anchor.None,
        children: [
            new(
                id: "Top",
                size: new(0, 32),
                anchor: Anchor.TopLeft,
                style: new() {
                    Gradient = Gradient.Vertical(0, 0)
                }
            ),
            new(
                id: "Bottom",
                size: new(0, 32),
                anchor: Anchor.BottomLeft,
                style: new() {
                    Gradient = Gradient.Vertical(0, 0)
                }
            )
        ]
    );

    public OverflowContainer(
        string         id,
        Size?          size      = null,
        Anchor         anchor    = Anchor.TopLeft,
        List<Element>? children  = null,
        Color?         fadeColor = null
    ) : base(
        id,
        size: size ?? new(),
        anchor: anchor,
        children: children,
        flow: Flow.Vertical
    )
    {
        _currentId = _childIdCounter++;
        FadeColor  = fadeColor ?? new(0);
    }

    protected override void BeginDraw(ImDrawListPtr drawList)
    {
        // if (!Has("_Fader")) AddChild(_fader);

        if (TopFader.Style.Gradient?.TopLeft != FadeColor) {
            TopFader.Style.Gradient = Gradient.Vertical(FadeColor, new(FadeColor.Value.ApplyAlphaComponent(0)));
        }

        if (BottomFader.Style.Gradient?.BottomLeft != FadeColor) {
            BottomFader.Style.Gradient = Gradient.Vertical(new(FadeColor.Value.ApplyAlphaComponent(0)), FadeColor);
        }

        ImGui.SetCursorScreenPos(ContentBox.Min);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,    new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding,   0);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, 0);
        ImGui.BeginChildFrame(_currentId, ContentBox.Size.ToVector2(), ImGuiWindowFlags.None);

        // Reset the cursor pos to the top left of the child frame.
        ImGui.SetCursorPos(Vector2.Zero);
        PushDrawList(ImGui.GetWindowDrawList());

        IsScrolledToTop    = ImGui.GetScrollY()                         <= 0;
        IsScrolledToBottom = ImGui.GetScrollMaxY() - ImGui.GetScrollY() <= 0;

        // Recompute the layout for the children.
        foreach (var child in Children) {
            child.ComputeLayout(ImGui.GetCursorScreenPos());
        }

        // Draw the fade effect at the top and bottom of the container.
        _fader.Size               = new((int)(ContentBox.Width / ScaleFactor), (int)(ContentBox.Height / ScaleFactor));
        TopFader.Size             = new((int)(ContentBox.Width / ScaleFactor), 64);
        BottomFader.Size          = new((int)(ContentBox.Width / ScaleFactor), 64);
        TopFader.Style.Opacity    = Math.Min(ImGui.GetScrollY()                           / 64, 1);
        BottomFader.Style.Opacity = Math.Min((ImGui.GetScrollMaxY() - ImGui.GetScrollY()) / 64, 1);
    }

    protected override void EndDraw(ImDrawListPtr drawList)
    {
        // Grab the combined height of all children.
        float totalHeight =
            Children.Except([_fader]).Aggregate(0f, (current, child) => current + child.ComputedSize.Height);

        ImGui.SetCursorPos(new(0, totalHeight));
        ImGui.EndChildFrame();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar(3);
        PopDrawList();

        // Draw the fade effect at the top and bottom of the container.
        if (ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows)) {
            _fader.Render(ImGui.GetForegroundDrawList(), ContentBox.Min);
        }
    }

    private Element TopFader    => _fader.Get("Top");
    private Element BottomFader => _fader.Get("Bottom");
}
