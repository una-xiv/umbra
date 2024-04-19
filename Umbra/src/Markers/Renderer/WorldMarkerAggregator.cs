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
using System.Numerics;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers;

[Service]
public sealed class WorldMarkerAggregator(
    IWorldMarkerFactory[]        worldMarkerFactories,
    WorldMarkerRaycaster         raycaster,
    WorldMarkerRenderer          markerRenderer,
    WorldMarkerDirectionRenderer directionRenderer,
    IPlayer                      player
)
{
    private readonly List<WorldMarkerObject> _markers = [];

    [OnDraw]
    public void OnDraw()
    {
        // Don't draw anything if the player is occupied.
        if (player.IsOccupied) return;

        _markers.Clear();

        // Gather & Aggregate...
        foreach (var factory in worldMarkerFactories) {
            List<WorldMarker> markers = factory.GetMarkers();
            if (markers.Count == 0) continue;

            foreach (var marker in markers) {
                TryAggregate(marker);
            }
        }

        // Render to screen...
        markerRenderer.Render(_markers);
        directionRenderer.Render(_markers);
    }

    private void TryAggregate(WorldMarker marker)
    {
        var position = raycaster.Raycast(marker);

        WorldMarkerObject? markerObject = _markers.FirstOrDefault(m => Vector3.Distance(m.Position, position) < 2f);

        if (null == markerObject) {
            _markers.Add(
                new WorldMarkerObject {
                    Position        = position,
                    IconIds         = [marker.IconId],
                    ShowDirection   = marker.ShowDirection,
                    MinOpacity      = marker.MinOpacity,
                    MaxOpacity      = marker.MaxOpacity,
                    MinFadeDistance = marker.MinFadeDistance,
                    MaxFadeDistance = marker.MaxFadeDistance,
                    OnClick         = marker.OnClick,
                    Text = [
                        new MarkerText {
                            Label    = marker.Label,
                            SubLabel = marker.SubLabel
                        }
                    ],
                }
            );

            return;
        }

        markerObject.IconIds.Add(marker.IconId);

        markerObject.Text.Add(
            new MarkerText {
                Label    = marker.Label,
                SubLabel = marker.SubLabel
            }
        );

        if (marker.MinOpacity < markerObject.MinOpacity) markerObject.MinOpacity = marker.MinOpacity;
        if (marker.MaxOpacity > markerObject.MaxOpacity) markerObject.MaxOpacity = marker.MaxOpacity;

        if (marker.MinFadeDistance < markerObject.MinFadeDistance)
            markerObject.MinFadeDistance = marker.MinFadeDistance;

        if (marker.MaxFadeDistance > markerObject.MaxFadeDistance)
            markerObject.MaxFadeDistance = marker.MaxFadeDistance;

        if (!markerObject.ShowDirection
         && marker.ShowDirection)
            markerObject.ShowDirection = true;

        if (null == markerObject.OnClick
         && null != marker.OnClick)
            markerObject.OnClick = marker.OnClick;
    }
}
