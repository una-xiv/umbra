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

using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Umbra.Common;

namespace Umbra.Windows.Clipping;

[Service]
internal sealed class ClipRectProvider
{
    private readonly List<Rect> _rects = [];

    [OnDraw(executionOrder: int.MaxValue)]
    public unsafe void UpdateRects()
    {
        _rects.Clear();

        ForEachAtkUnit(
            (uPtr, _) => {
                var unitBase = (AtkUnitBase*)uPtr;
                var rootNode = unitBase->GetNodeById(1);

                if (rootNode == null) {
                    return;
                }

                // var node = GetWindowBoundsNode(unitBase, rootNode);

                FFXIVClientStructs.FFXIV.Common.Math.Bounds bounds;
                unitBase->GetWindowBounds(&bounds);

                var start = new Vector2(bounds.Pos1.X + 1, bounds.Pos1.Y);
                var end   = new Vector2(bounds.Pos2.X - 1, bounds.Pos2.Y - 1);

                var rect = new Rect(start, end);
                if (rect.IsValid()) _rects.Add(rect);
            }
        );
    }

    /// <summary>
    /// Finds a clip rect that overlap with the given area.
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public Rect? FindOverlappingRect(Rect area)
    {
        return _rects.FirstOrDefault(rect => rect.Overlaps(area));
    }

    public List<Rect> FindClipRectsIntersectingWith(Rect rect)
    {
        return _rects.Where(other => other.IntersectsWith(rect)).ToList();
    }

    /// <summary>
    /// Returns the AtkUnitManager instance.
    /// </summary>
    /// <returns></returns>
    private unsafe AtkUnitManager* GetAtkUnitManager()
    {
        AtkStage* stage = AtkStage.Instance();

        if (stage == null) {
            return null;
        }

        RaptureAtkUnitManager* manager = stage->RaptureAtkUnitManager;

        if (manager == null) {
            return null;
        }

        return &manager->AtkUnitManager;
    }

    /// <summary>
    /// Iterates over all visible ATK units and calls the given iterator for each.
    /// </summary>
    /// <param name="iterator"></param>
    /// <param name="focusedOnly"></param>
    private unsafe void ForEachAtkUnit(Action<IntPtr, string> iterator, bool focusedOnly = false)
    {
        var unitManager = GetAtkUnitManager();
        var unitList    = focusedOnly ? unitManager->FocusedUnitsList : unitManager->AllLoadedUnitsList;

        for (var i = 0; i < unitList.Count; i++) {
            AtkUnitBase* unit = *(AtkUnitBase**)Unsafe.AsPointer(ref unitList.Entries[i]);

            if (null == unit || !unit->IsVisible || null == unit->WindowNode) continue;

            string? name = unit->NameString;
            if (name is "_FocusTargetInfo" or "JobHudNotice") continue;

            iterator((IntPtr)unit, name ?? string.Empty);
        }
    }
}
