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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Markers.System.Renderer;

internal class WorldMarkerNode : Node
{
    private readonly List<WorldMarker> _enqueuedMarkers  = [];
    private readonly List<WorldMarker> _committedMarkers = [];

    public WorldMarkerNode(string id)
    {
        Id         = id;
        Stylesheet = WorldMarkerStylesheet;
        ClassList  = ["world-marker"];

        ChildNodes = [
            new() {
                ClassList = ["icon-list"],
            },
            new() {
                ClassList = ["label-list"],
            },
            new() {
                ClassList = ["distance-label"],
                NodeValue = "0.0 yalms",
            }
        ];
    }

    public void AddMarker(WorldMarker marker)
    {
        // Don't aggregate more than 3 markers at once.
        if (_enqueuedMarkers.Count > 3) return;

        _enqueuedMarkers.Add(marker);
    }

    public void SetMaxWidth(int width)
    {
        Style.Size                                   = new(width, 0);
        QuerySelector(".icon-list")!.Style.Size      = new(width, 0);
        QuerySelector(".label-list")!.Style.Size     = new(width, 0);
        QuerySelector(".distance-label")!.Style.Size = new(width, 14);

        foreach (Node childNode in QuerySelectorAll(".label")) childNode.Style.Size = new(width, 16);
        foreach (Node childNode in QuerySelectorAll(".sub-label")) childNode.Style.Size = new(width, 14);
    }

    public void SetDistance(float? distance)
    {
        Node node = QuerySelector(".distance-label")!;

        node.NodeValue       = distance.HasValue ? $"{distance:F1} yalms" : null;
        node.Style.IsVisible = distance.HasValue;

        WorldMarker? marker = _committedMarkers.FirstOrDefault();
        if (null == marker) return;

        float fadeStart = marker.FadeDistance.Y;
        float fadeEnd   = marker.FadeDistance.X;

        if (distance.HasValue) {
            Style.Opacity = 1.0f - Math.Clamp((distance.Value - fadeStart) / (fadeEnd - fadeStart), 0f, 1f);
        } else {
            Style.Opacity = 1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Flush()
    {
        Node iconList  = QuerySelector(".icon-list")!;
        Node labelList = QuerySelector(".label-list")!;

        foreach (WorldMarker marker in _committedMarkers) {
            string id = GetWorldMarkerId(marker);

            if (_enqueuedMarkers.Contains(marker)) {
                // Update the marker info.
                Node iconNode     = QuerySelector($".icon-{id}")!;
                Node labelNode    = QuerySelector($".label-{id}")!;
                Node subLabelNode = QuerySelector($".sub-label-{id}")!;

                iconNode.Style.IconId        = marker.IconId;
                iconNode.Style.IsVisible     = marker.IconId > 0;
                labelNode.NodeValue          = marker.Label;
                labelNode.Style.IsVisible    = !string.IsNullOrEmpty(marker.Label);
                subLabelNode.NodeValue       = marker.SubLabel;
                subLabelNode.Style.IsVisible = !string.IsNullOrEmpty(marker.SubLabel);
                continue;
            }

            // Remove marker from the node.
            QuerySelector($".icon-{id}")?.Remove();
            QuerySelector($".label-{id}")?.Remove();
            QuerySelector($".sub-label-{id}")?.Remove();
        }

        foreach (WorldMarker marker in _enqueuedMarkers) {
            if (_committedMarkers.Contains(marker)) continue;

            string id = GetWorldMarkerId(marker);

            iconList.AppendChild(
                new() {
                    ClassList = ["icon", $"icon-{id}"],
                    SortIndex = (int)marker.IconId,
                    Style = new() {
                        IconId    = marker.IconId,
                        IsVisible = marker.IconId > 0,
                    }
                }
            );

            labelList.AppendChild(
                new() {
                    ClassList = ["label", $"label-{id}"],
                    NodeValue = marker.Label,
                    Style = new() {
                        IsVisible = !string.IsNullOrEmpty(marker.Label),
                    }
                }
            );

            labelList.AppendChild(
                new() {
                    ClassList = ["sub-label", $"sub-label-{id}"],
                    NodeValue = marker.SubLabel,
                    Style = new() {
                        IsVisible = !string.IsNullOrEmpty(marker.SubLabel)
                    }
                }
            );
        }

        _committedMarkers.Clear();
        _committedMarkers.AddRange(_enqueuedMarkers);
        _enqueuedMarkers.Clear();

        Style.IsVisible = _committedMarkers.Count > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static string GetWorldMarkerId(WorldMarker marker)
    {
        char[]     charsToReplace  = ['.', '#', '>', '<'];
        const char replacementChar = '_';
        string     input           = marker.Key;

        foreach (char c in charsToReplace) {
            input = input.Replace(c, replacementChar);
        }

        return $"wm--{input}";
    }

    private static Stylesheet WorldMarkerStylesheet { get; } = new(
        [
            new(
                ".world-marker",
                new() {
                    Anchor = Anchor.BottomCenter,
                    Flow   = Flow.Vertical,
                    Gap    = 2,
                    Size   = new(150, 0),
                }
            ),
            new(
                ".icon-list",
                new() {
                    Anchor = Anchor.TopCenter,
                    Flow   = Flow.Horizontal,
                    Gap    = 8,
                    Size   = new(150, 0),
                }
            ),
            new(
                ".icon",
                new() {
                    Anchor = Anchor.TopCenter,
                    Size   = new(32, 32),
                }
            ),
            new(
                ".label-list",
                new() {
                    Anchor = Anchor.TopCenter,
                    Flow   = Flow.Vertical,
                    Size   = new(150, 0),
                }
            ),
            new(
                ".label",
                new() {
                    Anchor          = Anchor.TopCenter,
                    FontSize        = 13,
                    Color           = new(0xFFFFFFFF),
                    OutlineColor    = new(0xFF000000),
                    OutlineSize     = 1,
                    TextAlign       = Anchor.TopCenter,
                    Size            = new(150, 16),
                    TextOverflow    = false,
                    TextShadowColor = new(0xFF000000),
                    TextShadowSize  = 8,
                    TextOffset      = new(0, 1),
                    WordWrap        = false,
                }
            ),
            new(
                ".sub-label",
                new() {
                    Anchor          = Anchor.TopCenter,
                    FontSize        = 12,
                    Color           = new(0xD0FFFFFF),
                    OutlineColor    = new(0x90000000),
                    OutlineSize     = 1,
                    TextAlign       = Anchor.TopCenter,
                    Size            = new(150, 14),
                    TextOverflow    = false,
                    WordWrap        = false,
                    TextShadowColor = new(0xFF000000),
                    TextShadowSize  = 8,
                    TextOffset      = new(0, 1),
                }
            ),
            new(
                ".distance-label",
                new() {
                    Anchor       = Anchor.TopCenter,
                    FontSize     = 12,
                    Color        = new(0xD0FFFFFF),
                    OutlineColor = new(0x90000000),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.TopCenter,
                    Size         = new(150, 14),
                    TextOffset   = new(0, 1),
                }
            )
        ]
    );
}
