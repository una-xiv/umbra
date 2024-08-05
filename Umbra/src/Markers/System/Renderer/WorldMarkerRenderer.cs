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
using System.Numerics;
using ImGuiNET;
using Umbra.Common;
using Umbra.Game;
using Umbra.Windows.Clipping;
using Una.Drawing;

namespace Umbra.Markers.System.Renderer;

[Service]
internal class WorldMarkerRenderer(
    IGameCamera         gameCamera,
    IPlayer             player,
    ClipRectProvider    clipRectProvider,
    WorldMarkerRegistry registry,
    IZoneManager        zoneManager,
    UmbraVisibility     visibility
)
{
    [ConfigVariable("Markers.Renderer.Enabled", "Markers", "MarkersRenderer")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Markers.Renderer.AggregateDistance", "Markers", "MarkersRenderer", 1, 30)]
    private static int AggregateDistance { get; set; } = 1;

    [ConfigVariable("Markers.Renderer.MaxWidth", "Markers", "MarkersRenderer", 64, 500)]
    private static int MaxWidth { get; set; } = 150;

    private readonly Dictionary<string, WorldMarkerNode> _nodes     = [];
    private readonly Dictionary<WorldMarkerNode, Point>  _positions = [];

    private uint _lastZoneId;

    [OnDraw]
    private void OnDraw()
    {
        if (!Enabled || !zoneManager.HasCurrentZone || !visibility.AreMarkersVisible()) return;

        if (_lastZoneId != zoneManager.CurrentZone.Id) {
            _lastZoneId = zoneManager.CurrentZone.Id;
            _nodes.Clear();
            _positions.Clear();
        }

        List<string> usedIds = [];

        uint mapId = zoneManager.CurrentZone.Id;

        foreach (var marker in registry.GetMarkers()) {
            if (marker.MapId != mapId || !marker.IsVisible) continue;

            string nodeId = GetNodeId(marker);

            Vector3 pos = registry.GetResolvedPosition(marker);

            if (!gameCamera.WorldToScreen(pos, out Vector2 screenPosition)) {
                continue;
            }

            usedIds.Add(nodeId);

            float distance = Vector3.Distance(player.Position, registry.GetResolvedPosition(marker));
            if (marker.FadeDistance.X > distance) continue;

            if (!_nodes.TryGetValue(nodeId, out WorldMarkerNode? node)) {
                node = new(nodeId) { Style = new() };
                node.SetMaxWidth(MaxWidth);
                _nodes[nodeId] = node;
            }

            node.AddMarker(marker);
            node.SetDistance(distance);

            _positions[node] = new((int)screenPosition.X, (int)screenPosition.Y);
        }

        foreach (WorldMarkerNode node in _nodes.Values.ToList()) {
            if (!usedIds.Contains(node.Id!)) {
                _nodes.Remove(node.Id!);
                _positions.Remove(node);
                continue;
            }

            node.Flush();
            node.SetMaxWidth(MaxWidth);

            if (node.Style.Opacity == 0 || !(node.Style.IsVisible ?? false)) continue;

            node.Reflow(_positions[node]);

            if (clipRectProvider.FindClipRectsIntersectingWith(
                        new(
                            node.Bounds.MarginRect.X1,
                            node.Bounds.MarginRect.Y1,
                            node.Bounds.MarginRect.X2,
                            node.Bounds.MarginRect.Y2
                        )
                    )
                    .Count
                > 0)
                continue;

            Point   pos     = _positions[node];
            Vector2 workPos = ImGui.GetMainViewport().WorkPos;

            node.Render(ImGui.GetBackgroundDrawList(), new((int)workPos.X + pos.X, (int)workPos.Y + pos.Y), true);
        }
    }

    /// <summary>
    /// Returns an ID for a <see cref="Node"/> based on its rounded position.
    /// </summary>
    private string GetNodeId(WorldMarker marker)
    {
        Vector3 p     = registry.GetResolvedPosition(marker);
        float   pDist = Vector3.Distance(player.Position, p);
        var     aDist = (int)Math.Max(1, Math.Min(pDist, AggregateDistance));

        int x = (int)Math.Floor(p.X) / aDist * aDist;
        int y = (int)Math.Ceiling(p.Y) / aDist * aDist;
        int z = (int)Math.Floor(p.Z) / aDist * aDist;

        return $"WM_{x}_{y}_{z}";
    }
}
