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
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.System.Renderer;

[Service]
internal class WorldMarkerRenderer : IDisposable
{
    [ConfigVariable("Markers.Renderer.Enabled", "Markers", "MarkersRenderer")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Markers.Renderer.AggregateDistance", "Markers", "MarkersRenderer", 1, 30)]
    private static int AggregateDistance { get; set; } = 1;

    [ConfigVariable("Markers.Renderer.MaxWidth", "Markers", "MarkersRenderer", 64, 500)]
    private static int MaxWidth { get; set; } = 150;

    private IGameCamera         GameCamera       { get; init; }
    private WorldMarkerRegistry Registry         { get; init; }
    private IZoneManager        ZoneManager      { get; init; }
    private UmbraVisibility     Visibility       { get; init; }

    private Dictionary<int, WorldMarkerNode> NodePool          { get; } = new();
    private Dictionary<string, int>          MarkerAssignments { get; } = new();

    public WorldMarkerRenderer(
        IGameCamera         gameCamera,
        WorldMarkerRegistry registry,
        IZoneManager        zoneManager,
        UmbraVisibility     visibility
    )
    {
        GameCamera       = gameCamera;
        Registry         = registry;
        ZoneManager      = zoneManager;
        Visibility       = visibility;

        for (var i = 0; i < 255; i++) {
            NodePool.Add(i, new(i));
        }
    }

    public void Dispose()
    {
        foreach (var node in NodePool.Values) {
            node.Dispose();
        }

        NodePool.Clear();
        MarkerAssignments.Clear();
    }

    [OnDraw]
    private void OnDraw()
    {
        if (!Enabled || !Visibility.AreMarkersVisible()) return;

        HashSet<string> usedMarkerIds = [];

        foreach (var marker in GetMarkersToRender()) {
            usedMarkerIds.Add(marker.Key);

            Vector3 position = marker.Position;
            if (position == Vector3.Zero) continue;

            if (MarkerAssignments.TryGetValue(marker.Key, out int nodeId)) {
                WorldMarkerNode node = NodePool[nodeId];

                if (node.IsMarkerStillValidForThisNode(marker)) {
                    node.UpdateMarker(marker);
                    continue;
                }

                node.RemoveMarker(marker.Key);
                MarkerAssignments.Remove(marker.Key);
            }

            WorldMarkerNode? targetNode = FindNodeForNewMarker(marker);
            if (targetNode == null) continue;

            targetNode.AddMarker(marker);
            MarkerAssignments.Add(marker.Key, targetNode.NodeId);
        }

        RemoveMarkersExcept(usedMarkerIds);

        foreach (var node in NodePool.Values) {
            node.MaxWidth          = MaxWidth;
            node.AggregateDistance = AggregateDistance;

            node.Update();
        }
    }

    private WorldMarker[] GetMarkersToRender()
    {
        return Registry
            .GetMarkers()
            .Where(
                marker => ZoneManager.HasCurrentZone
                    && marker.MapId == ZoneManager.CurrentZone.Id
                    && GameCamera.WorldToScreen(marker.WorldPosition, out _)
            )
            .ToArray();
    }

    /// <summary>
    /// Returns a <see cref="WorldMarkerNode"/> that can accommodate the given
    /// marker or NULL if there are no more nodes available.
    /// </summary>
    private WorldMarkerNode? FindNodeForNewMarker(WorldMarker marker)
    {
        WorldMarkerNode? emptyNode = null;

        // Find a node that can still accommodate this marker based on the aggregate distance.
        foreach (var node in NodePool.Values) {
            Vector3? worldPosition = node.WorldPosition;

            if (worldPosition is null) {
                emptyNode ??= node;
                continue;
            }

            if (Vector3.Distance(worldPosition.Value, marker.Position) <= AggregateDistance) {
                return node;
            }
        }

        if (emptyNode == null) {
            Logger.Warning($"All nodes are in use. Cannot render marker: {marker.Key}");
        }

        return emptyNode;
    }

    private void RemoveMarkersExcept(HashSet<string> usedMarkerIds)
    {
        List<string> idsToRemove = [];

        foreach ((string id, int nodeId) in MarkerAssignments) {
            if (!usedMarkerIds.Contains(id)) {
                idsToRemove.Add(id);
                NodePool[nodeId].RemoveMarker(id);
            }
        }

        foreach (string id in idsToRemove) {
            MarkerAssignments.Remove(id);
        }
    }
}
