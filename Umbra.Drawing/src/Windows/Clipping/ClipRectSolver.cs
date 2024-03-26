// FFXIV Empherial UI                       _____           _           _     _
//     An XIV-Launcher plugin              |   __|_____ ___| |_ ___ ___|_|___| |
//                                         |   __|     | . |   | -_|  _| | .'| |
// github.com/empherial/empherial-ui       |_____|_|_|_|  _|_|_|___|_| |_|__,|_|
// --------------------------------------------------- |_|

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Umbra.Common;

namespace Umbra.Drawing;

[Service]
public sealed class ClipRectSolver(ClipRectProvider provider)
{
    // A smaller cell size will result in more accurate clipping when multiple
    // game windows are intersecting with the given area clip. However, having
    // smaller sized cells will result in more cells being created, which will
    // have a negative impact on performance.
    private const int CellSize = 48;

    public ClipRectSolverResult Solve(ClipRect area)
    {
        List<ClipRect> rects = provider.FindClipRectsIntersectingWith(area);
        List<ClipRect> result = [];

        if (rects.Count == 0) return new() {
            SolvedRects  = result,
            NativeRects  = rects,
            IsOverlapped = false,
        };

        // First find a clip rects that completely overlaps the designated area.
        ClipRect? overlappingRect = provider.FindOverlappingClipRect(area);

        if (overlappingRect != null) {
            return new() {
                SolvedRects  = [overlappingRect],
                NativeRects  = rects,
                IsOverlapped = true,
            };
        }

        for (var y = area.Min.Y; y < area.Max.Y; y += CellSize)
        {
            for (var x = area.Min.X; x < area.Max.X; x += CellSize)
            {
                var cell = new ClipRect(new Vector2(x, y), new Vector2(x + CellSize, y + CellSize));
                var skip = false;

                // Clamp to area max.
                cell.Max.X = Math.Min(cell.Max.X, area.Max.X);
                cell.Max.Y = Math.Min(cell.Max.Y, area.Max.Y);

                // Check if cell is inside any clip rect.
                foreach (var r in rects.Where(r => r.IntersectsWith(cell)).ToList())
                {
                    // If this cell is completely inside a clip rect, we don't need to render it.
                    if (r.Overlaps(cell)) {
                        skip = true;
                        break;
                    }

                    float minX = cell.Min.X;
                    float minY = cell.Min.Y;
                    float maxX = cell.Max.X;
                    float maxY = cell.Max.Y;

                    // Expand this cell so that it does not touch the clip rect.
                    if (r.Min.X < cell.Min.X) cell.Min.X = Math.Min(cell.Min.X, r.Max.X);
                    if (r.Max.X > cell.Max.X) cell.Max.X = Math.Min(cell.Max.X, r.Min.X);
                    if (r.Min.Y < cell.Min.Y) cell.Min.Y = Math.Max(cell.Min.Y, r.Max.Y);
                    if (r.Max.Y > cell.Max.Y) cell.Max.Y = Math.Max(cell.Max.Y, r.Min.Y);

                    // If this cell overlaps with any other rects after adjustment, skip it.
                    if (rects.Any(rect => rect != r && rect.IntersectsWith(cell))) {
                        skip = true;
                        break;
                    }

                    var gapL = new ClipRect(new(minX, minY), new (Math.Min(maxX, r.Min.X), Math.Min(maxY, r.Max.Y))).Clamp(area);
                    var gapR = new ClipRect(new(Math.Max(minX, r.Max.X), minY), new (maxX, Math.Min(maxY, r.Max.Y))).Clamp(area);
                    var gapT = new ClipRect(new(Math.Max(minX, r.Min.X), minY), new (Math.Min(r.Max.X, maxX), Math.Min(maxY, r.Min.Y))).Clamp(area);
                    var gapB = new ClipRect(new(Math.Max(minX, r.Min.X), r.Max.Y), new (maxX, maxY)).Clamp(area);

                    // Ensure the gap fillers don't intersect with any rects other than the current one.
                    if (r.Min.X >= minX && !rects.Any(rect => rect != r && rect.IntersectsWith(gapL))) result.Add(gapL);
                    if (r.Max.X <= maxX && !rects.Any(rect => rect != r && rect.IntersectsWith(gapR))) result.Add(gapR);
                    if (r.Min.Y >= minY && !rects.Any(rect => rect != r && rect.IntersectsWith(gapT))) result.Add(gapT);
                    if (r.Max.Y <= maxY && !rects.Any(rect => rect != r && rect.IntersectsWith(gapB))) result.Add(gapB);
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

    private static List<ClipRect> MergeRectangles(List<ClipRect> rectangles)
    {
        var mergedRectangles = new List<ClipRect>();

        foreach (var rect in rectangles)
        {
            var merged = false;

            foreach (var mergedRect in mergedRectangles)
            {
                if (! CanMerge(rect, mergedRect)) continue;

                mergedRect.Min.X = Math.Min(mergedRect.Min.X, rect.Min.X);
                mergedRect.Min.Y = Math.Min(mergedRect.Min.Y, rect.Min.Y);
                mergedRect.Max.X = Math.Max(mergedRect.Max.X, rect.Max.X);
                mergedRect.Max.Y = Math.Max(mergedRect.Max.Y, rect.Max.Y);

                merged = true;
                break;
            }

            if (!merged) mergedRectangles.Add(rect);
        }

        mergedRectangles = mergedRectangles.Distinct().ToList();

        return mergedRectangles;
    }

    private static bool CanMerge(ClipRect rect1, ClipRect rect2)
    {
        return (rect1.Min.X == rect2.Min.X && rect1.Max.X == rect2.Max.X &&
                (rect1.Max.Y == rect2.Min.Y || rect1.Min.Y == rect2.Max.Y)) ||
               (rect1.Min.Y == rect2.Min.Y && rect1.Max.Y == rect2.Max.Y &&
                (rect1.Max.X == rect2.Min.X || rect1.Min.X == rect2.Max.X));
    }
}

public struct ClipRectSolverResult
{
    public List<ClipRect> SolvedRects;
    public List<ClipRect> NativeRects;
    public bool IsOverlapped;
}
