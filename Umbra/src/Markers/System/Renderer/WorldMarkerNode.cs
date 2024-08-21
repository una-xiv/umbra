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

using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Umbra.Common;
using Umbra.Game;
using Umbra.Windows.Clipping;
using Una.Drawing;

namespace Umbra.Markers.System.Renderer;

internal class WorldMarkerNode : Node
{
    public float AggregateDistance { get; set; } = 1.0f;
    public int   MaxWidth          { get; set; } = 150;
    public int   NodeId            { get; }

    public WorldMarkerNode(int id)
    {
        NodeId     = id;
        Stylesheet = WorldMarkerStylesheet;
        Id         = $"WorldMarker_{id}";
        ClassList  = ["world-marker"];

        ChildNodes = [
            new() {
                ClassList = ["icon-list"],
                ChildNodes = [
                    new() { ClassList = ["icon"], Id = "Icon0" },
                    new() { ClassList = ["icon"], Id = "Icon1" },
                    new() { ClassList = ["icon"], Id = "Icon2" },
                ]
            },
            new() {
                ClassList = ["label-list"],
                ChildNodes = [
                    new() { ClassList = ["label"], Id     = "Label0" },
                    new() { ClassList = ["sub-label"], Id = "SubLabel0" },
                    new() { ClassList = ["label"], Id     = "Label1" },
                    new() { ClassList = ["sub-label"], Id = "SubLabel1" },
                    new() { ClassList = ["label"], Id     = "Label2" },
                    new() { ClassList = ["sub-label"], Id = "SubLabel2" },
                ]
            },
            new() {
                ClassList = ["distance-label"],
                NodeValue = "0.0 yalms",
            }
        ];
    }

    private readonly Dictionary<string, WorldMarker> _markers = new();

    private readonly IGameCamera      _gameCamera = Framework.Service<IGameCamera>();
    private readonly IPlayer          _player     = Framework.Service<IPlayer>();
    private readonly ClipRectProvider _clipRects  = Framework.Service<ClipRectProvider>();

    public void AddMarker(WorldMarker marker)
    {
        if (_markers.Count >= 3) return;

        _markers.TryAdd(marker.Key, marker);
    }

    public void RemoveMarker(string id)
    {
        var index = _markers.Keys.ToList().IndexOf(id);
        if (index == -1) return;

        _markers.Remove(id);

        ClearState(index);
    }

    public void UpdateMarker(WorldMarker marker)
    {
        if (_markers.ContainsKey(marker.Key)) {
            _markers[marker.Key] = marker;
        }
    }

    public bool IsMarkerStillValidForThisNode(WorldMarker marker)
    {
        if (!_markers.ContainsKey(marker.Key)) return false;

        return _markers.Count != 0
            && _markers.Values.All(t => !(Vector3.Distance(t.WorldPosition, marker.WorldPosition) > AggregateDistance));
    }

    public Vector3? WorldPosition => _markers.Count > 0 ? _markers.Values.First().WorldPosition : null;

    public void Update()
    {
        if (_markers.Count == 0 || WorldPosition == null) {
            Style.IsVisible = false;
            return;
        }

        Style.IsVisible = true;

        var markers = _markers.Values.ToArray();

        float minDist = 0.1f;
        float maxDist = 0.25f;

        for (var i = 0; i < 3; i++) {
            if (i < markers.Length) {
                if (markers[i].IsVisible == false) {
                    ClearState(i);
                } else {
                    UpdateState(i, markers[i].Label, markers[i].SubLabel, markers[i].IconId);

                    minDist = Math.Max(minDist, markers[i].FadeDistance.X);
                    maxDist = Math.Max(maxDist, markers[i].FadeDistance.Y);
                }
            } else {
                ClearState(i);
            }
        }

        float distance = Vector3.Distance(_player.Position, WorldPosition.Value);
        float opacity  = Math.Clamp((distance - minDist) / (maxDist - minDist), 0, 1);

        QuerySelector(".distance-label")!.NodeValue = $"{distance:F1} yalms";

        UpdateNodeSizes();
        Style.Opacity = opacity;

        if (opacity < 0.05f) return;

        if (_gameCamera.WorldToScreen(WorldPosition.Value, out var screenPos)) {
            Vector2 workPos = ImGui.GetMainViewport().WorkPos;

            int x = (int)(screenPos.X + workPos.X);
            int y = (int)(screenPos.Y + workPos.Y);

            int                   halfSize = MaxWidth / 2;
            Windows.Clipping.Rect rect     = new(x - halfSize, y - halfSize, x + halfSize, y + halfSize);

            if (_clipRects.FindClipRectsIntersectingWith(rect).Count > 0) return;

            Render(ImGui.GetBackgroundDrawList(), new(x, y));
        }
    }

    private void UpdateState(int index, string? label, string? subLabel, uint iconId)
    {
        Node iconNode     = QuerySelector($"#Icon{index}")!;
        Node labelNode    = QuerySelector($"#Label{index}")!;
        Node subLabelNode = QuerySelector($"#SubLabel{index}")!;

        bool hasLabel    = !string.IsNullOrEmpty(label);
        bool hasSubLabel = !string.IsNullOrEmpty(subLabel);

        iconNode.Style.IsVisible = iconId > 0;
        iconNode.Style.IconId    = iconId;

        labelNode.Style.IsVisible    = hasLabel;
        labelNode.NodeValue          = label;
        subLabelNode.Style.IsVisible = hasSubLabel;
        subLabelNode.NodeValue       = subLabel;
    }

    private void ClearState(int index)
    {
        Node iconNode     = QuerySelector($"#Icon{index}")!;
        Node labelNode    = QuerySelector($"#Label{index}")!;
        Node subLabelNode = QuerySelector($"#SubLabel{index}")!;

        iconNode.Style.IsVisible     = false;
        labelNode.Style.IsVisible    = false;
        subLabelNode.Style.IsVisible = false;
    }

    private void UpdateNodeSizes()
    {
        Style.Size = new(MaxWidth, 0);

        for (var i = 0; i < 3; i++) {
            QuerySelector($"#Label{i}")!.Style.Size    = new(MaxWidth, 18);
            QuerySelector($"#SubLabel{i}")!.Style.Size = new(MaxWidth, 20);
        }

        QuerySelector(".icon-list")!.Style.Size      = new(MaxWidth, 32);
        QuerySelector(".label-list")!.Style.Size     = new(MaxWidth, 0);
        QuerySelector(".distance-label")!.Style.Size = new(MaxWidth, 20);
    }

    private static Stylesheet WorldMarkerStylesheet { get; } = new(
        [
            new(
                ".world-marker",
                new() {
                    Anchor = Anchor.BottomCenter,
                    Flow   = Flow.Vertical,
                    Size   = new(150, 0),
                    Gap    = 2,
                }
            ),
            new(
                ".icon-list",
                new() {
                    Anchor = Anchor.TopLeft,
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
                    Anchor = Anchor.TopLeft,
                    Flow   = Flow.Vertical,
                    Size   = new(150, 0),
                }
            ),
            new(
                ".label",
                new() {
                    Anchor          = Anchor.TopLeft,
                    Font            = (uint)FontId.WorldMarkers,
                    FontSize        = 13,
                    Color           = new(0xFFFFFFFF),
                    OutlineColor    = new(0xFF000000),
                    OutlineSize     = 1,
                    TextAlign       = Anchor.TopCenter,
                    Size            = new(150, 18),
                    TextOverflow    = false,
                    TextShadowColor = new(0xFF000000),
                    TextShadowSize  = 8,
                    WordWrap        = false,
                }
            ),
            new(
                ".sub-label",
                new() {
                    Anchor          = Anchor.TopLeft,
                    Font            = (uint)FontId.WorldMarkers,
                    FontSize        = 12,
                    Color           = new(0xD0FFFFFF),
                    OutlineColor    = new(0x90000000),
                    OutlineSize     = 1,
                    TextAlign       = Anchor.TopCenter,
                    Size            = new(150, 20),
                    TextOverflow    = false,
                    WordWrap        = false,
                    TextShadowColor = new(0xFF000000),
                    TextShadowSize  = 8,
                    TextOffset      = new(0, -1),
                }
            ),
            new(
                ".distance-label",
                new() {
                    Font         = (uint)FontId.WorldMarkers,
                    Anchor       = Anchor.TopLeft,
                    FontSize     = 12,
                    Color        = new(0xD0FFFFFF),
                    OutlineColor = new(0x90000000),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.TopCenter,
                    Size         = new(150, 20),
                    TextOffset   = new(0, -1),
                }
            )
        ]
    );
}
