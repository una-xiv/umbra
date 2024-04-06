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
using Umbra.Game;

namespace Umbra.Markers;

[Service]
internal sealed class QuestWorldMarkerFactory(IZoneManager zoneManager) : IWorldMarkerFactory
{
    [ConfigVariable("Markers.Quest.Enabled", "EnabledMarkers")]
    private static bool Enabled { get; set; } = true;

    public List<WorldMarker> GetMarkers()
    {
        if (false == Enabled) return [];

        var questObjectives = zoneManager
            .CurrentZone.DynamicMarkers.Where(marker => marker.Type == ZoneMarkerType.QuestObjective)
            .Select(
                marker => new WorldMarker {
                    IconId          = marker.IconId,
                    Position        = marker.WorldPosition,
                    Label           = marker.Name,
                    SubLabel        = null,
                    MinFadeDistance = 40,
                    MaxFadeDistance = 50
                }
            )
            .ToList();

        var battleObjectives = zoneManager
            .CurrentZone.DynamicMarkers.Where(marker => marker.Type == ZoneMarkerType.ObjectiveArea)
            .Select(
                marker => new WorldMarker {
                    IconId          = marker.IconId,
                    Position        = marker.WorldPosition,
                    Label           = marker.Name,
                    SubLabel        = null,
                    MinFadeDistance = 40,
                    MaxFadeDistance = 50
                }
            )
            .ToList();

        return [
            ..questObjectives,
            ..battleObjectives
        ];
    }
}
