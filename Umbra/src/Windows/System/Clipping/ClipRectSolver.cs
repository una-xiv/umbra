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
using Umbra.Common;

namespace Umbra.Windows.Clipping;

[Service]
internal sealed class ClipRectSolver(ClipRectProvider provider)
{
    // A smaller cell size will result in more accurate clipping when multiple
    // game windows are intersecting with the given area clip. However, having
    // smaller sized cells will result in more cells being created, which will
    // have a negative impact on performance.
    private const int CellSize = 48;

    public RectSolverResult Solve(Rect area)
    {
        List<Rect> rects  = provider.FindClipRectsIntersectingWith(area);
        List<Rect> result = [];

        if (rects.Count == 0)
            return new() {
                SolvedRects  = result,
                NativeRects  = rects,
                IsOverlapped = false,
            };

        // First find a clip rects that completely overlaps the designated area.
        Rect? overlappingRect = provider.FindOverlappingRect(area);

        if (overlappingRect != null) {
            return new() {
                SolvedRects  = [overlappingRect],
                NativeRects  = rects,
                IsOverlapped = true,
            };
        }

        for (var y = area.Y1; y < area.Y2; y += CellSize) {
            for (var x = area.X1; x < area.X2; x += CellSize) {
                var cell = new Rect(new Vector2(x, y), new Vector2(x + CellSize, y + CellSize));
                var skip = false;

                // Clamp to area max.
                cell.X2 = Math.Min(cell.X2, area.X2);
                cell.Y2 = Math.Min(cell.Y2, area.Y2);

                // Check if cell is inside any clip rect.
                foreach (var r in rects.Where(r => r.IntersectsWith(cell)).ToList()) {
                    // If this cell is completely inside a clip rect, we don't need to render it.
                    if (r.Overlaps(cell)) {
                        skip = true;
                        break;
                    }

                    float minX = cell.X1;
                    float minY = cell.Y1;
                    float maxX = cell.X2;
                    float maxY = cell.Y2;

                    // Expand this cell so that it does not touch the clip rect.
                    if (r.X1 < cell.X1) cell.X1 = Math.Min(cell.X1, r.X2);
                    if (r.X2 > cell.X2) cell.X2 = Math.Min(cell.X2, r.X1);
                    if (r.Y1 < cell.Y1) cell.Y1 = Math.Max(cell.Y1, r.Y2);
                    if (r.Y2 > cell.Y2) cell.Y2 = Math.Max(cell.Y2, r.Y1);

                    // If this cell overlaps with any other rects after adjustment, skip it.
                    if (rects.Any(rect => rect != r && rect.IntersectsWith(cell))) {
                        skip = true;
                        break;
                    }

                    var gapL = new Rect(new(minX, minY), new(Math.Min(maxX, r.X1), Math.Min(maxY, r.Y2))).Clamp(area);
                    var gapR = new Rect(new(Math.Max(minX, r.X2), minY), new(maxX, Math.Min(maxY, r.Y2))).Clamp(area);

                    var gapT = new Rect(
                        new(Math.Max(minX, r.X1), minY),
                        new(Math.Min(r.X2, maxX), Math.Min(maxY, r.Y1))
                    ).Clamp(area);

                    var gapB = new Rect(new(Math.Max(minX, r.X1), r.Y2), new(maxX, maxY)).Clamp(area);

                    // Ensure the gap fillers don't intersect with any rects other than the current one.
                    if (r.X1 > minX && !rects.Any(rect => rect != r && rect.IntersectsWith(gapL))) result.Add(gapL);
                    if (r.X2 < maxX && !rects.Any(rect => rect != r && rect.IntersectsWith(gapR))) result.Add(gapR);
                    if (r.Y1 > minY && !rects.Any(rect => rect != r && rect.IntersectsWith(gapT))) result.Add(gapT);
                    if (r.Y2 < maxY && !rects.Any(rect => rect != r && rect.IntersectsWith(gapB))) result.Add(gapB);
                }

                if (!skip && cell.IsValid() && !result.Any(r => cell.Overlaps(r))) {
                    result.Add(cell);
                }
            }
        }

        // Double-pass merge to ensure all gaps are filled.
        result = MergeRectangles(MergeRectangles(result.Where(r => r.IsValid()).ToList()));

        return new() {
            SolvedRects  = result,
            NativeRects  = rects,
            IsOverlapped = false,
        };
    }

    private static List<Rect> MergeRectangles(List<Rect> rectangles)
    {
        var mergedRectangles = new List<Rect>();

        foreach (var rect in rectangles) {
            var merged = false;

            foreach (Rect mergedRect in mergedRectangles.Where(mergedRect => CanMerge(rect, mergedRect))) {
                mergedRect.X1 = Math.Min(mergedRect.X1, rect.X1);
                mergedRect.Y1 = Math.Min(mergedRect.Y1, rect.Y1);
                mergedRect.X2 = Math.Max(mergedRect.X2, rect.X2);
                mergedRect.Y2 = Math.Max(mergedRect.Y2, rect.Y2);

                merged = true;
                break;
            }

            if (!merged) mergedRectangles.Add(rect);
        }

        mergedRectangles = mergedRectangles.Distinct().ToList();

        return mergedRectangles;
    }

    private static bool CanMerge(Rect rect1, Rect rect2)
    {
        return (rect1.X1 == rect2.X1 && rect1.X2 == rect2.X2 && (rect1.Y2 == rect2.Y1 || rect1.Y1 == rect2.Y2))
         || (rect1.Y1    == rect2.Y1 && rect1.Y2 == rect2.Y2 && (rect1.X2 == rect2.X1 || rect1.X1 == rect2.X2));
    }
}

internal struct RectSolverResult
{
    public List<Rect> SolvedRects;
    public List<Rect> NativeRects;
    public bool       IsOverlapped;
}
