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
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Interface.Animation.EasingFunctions;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

internal unsafe partial class FlagWidget
{
    private IZoneManager ZoneManager { get; } = Framework.Service<IZoneManager>();

    private IAetheryteEntry? _aetheryteEntry;
    private string?          _aetheryteKey;
    private string?          _zoneName;
    private string?          _flagCoords;
    private string?          _aetheryteName;

    /// <summary>
    /// Returns true if a flag marker has been set.
    /// </summary>
    /// <returns></returns>
    private static bool IsFlagMarkerSet()
    {
        AgentMap* agentMap = AgentMap.Instance();

        return null != agentMap
            && 0 != agentMap->IsFlagMarkerSet;
    }

    private void UpdateWidgetInfoState()
    {
        if (!ZoneManager.HasCurrentZone) return;
        if (Player.IsBetweenAreas) return;

        AgentMap* map = AgentMap.Instance();
        if (map == null) return;

        var cacheKey =
            $"{ZoneManager.CurrentZone.Id}_{map->FlagMapMarker.MapId}_{map->FlagMapMarker.XFloat}_{map->FlagMapMarker.YFloat}";

        if (_aetheryteKey == cacheKey) return;

        IZone   zone = ZoneManager.GetZone(map->FlagMapMarker.MapId);
        Vector2 pos  = MapUtil.WorldToMap(new(map->FlagMapMarker.XFloat, map->FlagMapMarker.YFloat), zone.MapSheet);

        _aetheryteKey = cacheKey;
        _zoneName     = $"{zone.Name}";
        _flagCoords   = $"{pos.X:F1}, {pos.Y:F1}";

        var flagPos2D = new Vector2(map->FlagMapMarker.XFloat, map->FlagMapMarker.YFloat);

        // Find all Aetheryte markers in the current zone.
        List<ZoneMarker> aetherytes = zone
            .StaticMarkers
            .Where(m => m.Type == ZoneMarkerType.Aetheryte)
            .ToList();

        // Find the nearest Aetheryte marker to the flag marker, or null if none are nearby.
        ZoneMarker? marker = aetherytes.Count != 0
            ? aetherytes.MinBy(m => Vector2.Distance(new(m.WorldPosition.X, m.WorldPosition.Z), flagPos2D))
            : null;

        // Find the AetheryteEntry that the player has actually unlocked and is able to use.
        _aetheryteEntry = marker != null
            ? AetheryteList.FirstOrDefault(a => a.AetheryteId == marker.Value.DataId)
            : null;

        // Abort if there is none.
        if (_aetheryteEntry == null) {
            _aetheryteName = null;
            return;
        }

        var placeName = _aetheryteEntry.AetheryteData.GameData?.PlaceName.Value?.Name.ToString() ?? "???";
        var gilCost   = _aetheryteEntry.GilCost.ToString("D");

        _aetheryteName = placeName == zone.Name
            ? I18N.Translate("Widget.Flag.TeleportNearbyForGil",  gilCost)
            : I18N.Translate("Widget.Flag.TeleportToPlaceForGil", placeName, gilCost);
    }
}
