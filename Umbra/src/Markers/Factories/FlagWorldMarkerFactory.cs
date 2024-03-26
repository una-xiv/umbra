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
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers;

[Service]
internal sealed class FlagWorldMarkerFactory(IZoneManager ZoneManager) : IWorldMarkerFactory
{
    [ConfigVariable("Markers.Flag.Enabled", "Enabled Markers", "Show flag marker")]
    private static bool Enabled { get; set; } = true;

    public unsafe List<WorldMarker> GetMarkers()
    {
        if (false == Enabled) return [];

        var agentMap = AgentMap.Instance();

        if (agentMap                   == null
         || agentMap->IsFlagMarkerSet  == 0
         || ZoneManager.CurrentZone.Id != agentMap->FlagMapMarker.MapId
           ) {
            return [];
        }

        var marker = agentMap->FlagMapMarker;

        return [
            new WorldMarker {
                IconId          = marker.MapMarker.IconId,
                Position        = new(marker.XFloat, 0, marker.YFloat),
                Label           = null,
                SubLabel        = null,
                MinFadeDistance = 30,
                MaxFadeDistance = 50
            },
        ];
    }
}
