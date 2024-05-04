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

using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar;

[Service]
internal partial class ToolbarLayout
{
    public readonly Dictionary<string, IToolbarWidget> Widgets = [];

    public ToolbarLayout(IToolbarWidget[] widgets)
    {
        foreach (var widget in widgets) {
            Widgets.Add(widget.Element.Id, widget);
            LayoutConfig.Anchors[widget.Element.Id] = widget.Element.Anchor;
            LayoutConfig.Indices[widget.Element.Id] = widget.Element.SortIndex;
        }

        LoadWidgetLayout();
        NormalizeIndices();
    }

    public int GetLastSortIndexOf(Anchor anchor)
    {
        return Widgets.Values.Where(w => w.Element.Anchor == anchor).Max(w => w.Element.SortIndex);
    }

    public void MoveWidgetUp(string id)
    {
        if (!LayoutConfig.Indices.TryGetValue(id, out int oldIndex)) return;
        if (oldIndex == 0) return;

        var widget   = Widgets[id];
        int newIndex = oldIndex - 1;

        foreach (IToolbarWidget other in Widgets.Values) {
            if (widget.Element.Anchor == other.Element.Anchor && other.Element.SortIndex == newIndex) {
                other.Element.SortIndex                = oldIndex;
                LayoutConfig.Indices[other.Element.Id] = oldIndex;
                break;
            }
        }

        widget.Element.SortIndex = newIndex;
        LayoutConfig.Indices[id] = newIndex;

        SaveWidgetLayout();
    }

    public void MoveWidgetDown(string id)
    {
        if (!LayoutConfig.Indices.TryGetValue(id, out int oldIndex)) return;

        var widget   = Widgets[id];
        int newIndex = oldIndex + 1;

        if (oldIndex == GetLastSortIndexOf(widget.Element.Anchor)) return;

        foreach (IToolbarWidget other in Widgets.Values) {
            if (widget.Element.Anchor == other.Element.Anchor && other.Element.SortIndex == newIndex) {
                other.Element.SortIndex                = oldIndex;
                LayoutConfig.Indices[other.Element.Id] = oldIndex;
                break;
            }
        }

        widget.Element.SortIndex = newIndex;
        LayoutConfig.Indices[id] = newIndex;

        SaveWidgetLayout();
    }

    public void SwapAnchors(string id)
    {
        if (!LayoutConfig.Anchors.ContainsKey(id)) return;

        var widget = Widgets[id];

        List<string> sourceIds = Widgets.Values
            .Where(w => w.Element.Anchor == widget.Element.Anchor && w.Element.Id != id)
            .OrderBy(w => w.Element.SortIndex)
            .Select(w => w.Element.Id)
            .ToList();

        widget.Element.Anchor = widget.Element.Anchor switch {
            Anchor.MiddleLeft  => Anchor.MiddleRight,
            Anchor.MiddleRight => Anchor.MiddleLeft,
            _                  => widget.Element.Anchor
        };

        LayoutConfig.Anchors[id] = widget.Element.Anchor;

        List<string> targetIds = Widgets.Values
            .Where(w => w.Element.Anchor == widget.Element.Anchor)
            .OrderBy(w => w.Element.SortIndex)
            .Select(w => w.Element.Id)
            .ToList();

        for (var i = 0; i < sourceIds.Count; i++) {
            LayoutConfig.Indices[sourceIds[i]]      = i;
            Widgets[sourceIds[i]].Element.SortIndex = i;
        }

        for (var i = 0; i < targetIds.Count; i++) {
            LayoutConfig.Indices[targetIds[i]]      = i;
            Widgets[targetIds[i]].Element.SortIndex = i;
        }

        SaveWidgetLayout();
    }

    private void NormalizeIndices()
    {
        List<string> leftIds = Widgets.Values
            .Where(w => w.Element.Anchor == Anchor.MiddleLeft)
            .OrderBy(w => w.Element.SortIndex)
            .Select(w => w.Element.Id)
            .ToList();

        List<string> rightIds = Widgets.Values
            .Where(w => w.Element.Anchor == Anchor.MiddleRight)
            .OrderBy(w => w.Element.SortIndex)
            .Select(w => w.Element.Id)
            .ToList();

        for (var i = 0; i < leftIds.Count; i++) {
            LayoutConfig.Indices[leftIds[i]]      = i;
            Widgets[leftIds[i]].Element.SortIndex = i;
        }

        for (var i = 0; i < rightIds.Count; i++) {
            LayoutConfig.Indices[rightIds[i]]      = i;
            Widgets[rightIds[i]].Element.SortIndex = i;
        }

        SaveWidgetLayout();
    }
}
