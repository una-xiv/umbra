using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Aetherytes;
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

    private static bool IsFlagMarkerSet()
    {
        AgentMap* agentMap = AgentMap.Instance();

        return null != agentMap && agentMap->IsFlagMarkerSet;
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
        _flagCoords   = $"{I18N.FormatNumber(pos.X)}, {I18N.FormatNumber(pos.Y)}";

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

        var placeName = _aetheryteEntry.AetheryteData.Value.PlaceName.ValueNullable?.Name.ToString() ?? "???";
        var gilCost   = _aetheryteEntry.GilCost.ToString("D");

        _aetheryteName = placeName == zone.Name
            ? I18N.Translate("Widget.Flag.TeleportNearbyForGil",  gilCost)
            : I18N.Translate("Widget.Flag.TeleportToPlaceForGil", placeName, gilCost);
    }
}
