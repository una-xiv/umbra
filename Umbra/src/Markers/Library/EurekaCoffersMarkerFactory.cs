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
using Dalamud.Game.ClientState.Fates;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.Library;

[Service]
internal sealed partial class EurekaCoffersMarkerFactory : WorldMarkerFactory
{
    public override string Id          { get; } = "EurekaCoffers";
    public override string Name        { get; } = I18N.Translate("Markers.EurekaCoffers.Name");
    public override string Description { get; } = I18N.Translate("Markers.EurekaCoffers.Description");

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            new BooleanMarkerConfigVariable(
                "EnableMapMarkers",
                I18N.Translate("Markers.EurekaCoffers.EnableMapMarkers.Name"),
                I18N.Translate("Markers.EurekaCoffers.EnableMapMarkers.Description"),
                true
            ),
            ..DefaultFadeConfigVariables,
        ];
    }

    private const uint LuckyCarrotItemId = 2002482;
    private const int  MinFateRespawn    = 530;
    private const int  MaxFateRespawn    = 1000;

    private long _lastBunnyFateSpawnTime;

    private static readonly Dictionary<uint, Vector3> BunnyFateSpawnPositions = new() {
        { 763, new(-167.355f, -737.6541f, 303.0736f) },  // Pagos
        { 795, new(122.938f, 706.2372f, 236.9468f) },    // Pyros
        { 827, new(-370.0691f, 499.4355f, -477.2511f) }, // Hydatos.
    };

    private static readonly Dictionary<uint, uint> BunnyFateIds = new() {
        { 763, 1367 }, // Pagos
        { 795, 1407 }, // Pyros
        { 827, 1425 }, // Hydatos
    };

    private readonly IZoneManager _zoneManager;
    private readonly IChatGui     _chatGui;
    private readonly IFateTable   _fateTable;
    private readonly IPlayer      _player;

    private List<Vector3> _detectedCofferPositions = [];
    private bool          _hasPlacedMapMarkers;

    public EurekaCoffersMarkerFactory(
        IZoneManager zoneManager,
        IChatGui     chatGui,
        IFateTable   fateTable,
        IPlayer      player
    )
    {
        _zoneManager = zoneManager;
        _chatGui     = chatGui;
        _fateTable   = fateTable;
        _player      = player;

        _chatGui.CheckMessageHandled += OnChatMessage;
    }

    public override void Dispose()
    {
        _chatGui.CheckMessageHandled -= OnChatMessage;

        base.Dispose();
    }

    [OnTick(interval: 500)]
    public void GetMarkers()
    {
        if (false == GetConfigValue<bool>("Enabled")
            || !_zoneManager.HasCurrentZone
            || !CofferPositions.ContainsKey(_zoneManager.CurrentZone.TerritoryId)) {
            _lastBunnyFateSpawnTime = 0;
            RemoveAllMarkers();
            return;
        }

        if (BunnyFate() is not null) _lastBunnyFateSpawnTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        // If the player doesn't have the Lucky Carrot, but is in Eureka, show the bunny fate spawn marker.
        if (false == _player.HasItemInInventory(LuckyCarrotItemId)) {
            _detectedCofferPositions.Clear();
            RemoveAllMarkers();
            GetBunnyFateSpawnMarker();
            ResetMapMarkers();
            return;
        }

        List<string> activeIds = [];

        var showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var fadeDistance    = GetConfigValue<int>("FadeDistance");
        var fadeAttenuation = GetConfigValue<int>("FadeAttenuation");

        foreach (Vector3 position in _detectedCofferPositions) {
            string id = $"EurekaCoffer_{position.X:N0}_{position.Y:N0}_{position.Z:N0}";
            activeIds.Add(id);

            SetMarker(
                new() {
                    Key           = id,
                    MapId         = _zoneManager.CurrentZone.Id,
                    Position      = position,
                    IconId        = 60356,
                    ShowOnCompass = showDirection,
                    FadeDistance  = new(fadeDistance, fadeDistance + fadeAttenuation),
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }

    private void GetBunnyFateSpawnMarker()
    {
        if (!BunnyFateSpawnPositions.TryGetValue(_zoneManager.CurrentZone.TerritoryId, out Vector3 position)) {
            RemoveMarker("BunnyFate");
            return;
        }

        string? subLabel        = null;
        IFate?  bunnyFate       = BunnyFate();
        var     showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var     fadeDistance    = GetConfigValue<int>("FadeDistance");
        var     fadeAttenuation = GetConfigValue<int>("FadeAttenuation");

        if (bunnyFate is null && _lastBunnyFateSpawnTime > 0) {
            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            var minRespawnTime = TimeSpan.FromSeconds(_lastBunnyFateSpawnTime + MinFateRespawn - currentTime);
            var maxRespawnTime = TimeSpan.FromSeconds(_lastBunnyFateSpawnTime + MaxFateRespawn - currentTime);

            if (minRespawnTime.TotalSeconds > 0) {
                var min = minRespawnTime.ToString(minRespawnTime.TotalSeconds > 59 ? @"%m\ \m\i\n" : @"%s\ \s\e\c");
                var max = maxRespawnTime.ToString(maxRespawnTime.TotalSeconds > 59 ? @"%m\ \m\i\n" : @"%s\ \s\e\c");
                subLabel = $"Respawn between {min} and {max}";
            } else {
                subLabel = "Respawning soon";
            }
        }

        if (bunnyFate is not null) {
            subLabel = $@"{bunnyFate.Progress}% - {TimeSpan.FromSeconds(bunnyFate.TimeRemaining):mm\:ss} remaining";
        }

        SetMarker(
            new() {
                Key           = "BunnyFate",
                Position      = position,
                MapId         = _zoneManager.CurrentZone.Id,
                IconId        = 60723,
                Label         = "Bunny Fate",
                SubLabel      = subLabel,
                FadeDistance  = new(fadeDistance, fadeDistance + fadeAttenuation),
                ShowOnCompass = showDirection,
            }
        );
    }

    private IFate? BunnyFate()
    {
        return BunnyFateIds.TryGetValue(_zoneManager.CurrentZone.TerritoryId, out uint fateId)
            ? _fateTable.FirstOrDefault(fate => fate.FateId == fateId)
            : null;
    }

    private unsafe void AddMapMarkers()
    {
        if (!GetConfigValue<bool>("EnableMapMarkers")) {
            ResetMapMarkers();
            return;
        }

        if (Framework.DalamudPlugin.InstalledPlugins.Any(
                plugin => plugin is {
                    InternalName: "eurekaTrackerAutoPopper",
                    IsLoaded    : true
                }
            )) {
            // Don't show map markers if the Eureka Tracker Auto Popper plugin is loaded.
            return;
        }

        AgentMap* map = AgentMap.Instance();
        if (null == map) return;

        map->ResetMapMarkers();
        map->ResetMiniMapMarkers();

        foreach (var pos in _detectedCofferPositions) {
            var mapPos = pos;

            if (_zoneManager.CurrentZone.TerritoryId == 827) {
                mapPos.Z += 475; // Offset for Hydatos.
            }

            map->AddMapMarker(mapPos, 60356);
            map->AddMiniMapMarker(pos, 60356);
        }

        _hasPlacedMapMarkers = true;
    }

    private unsafe void ResetMapMarkers()
    {
        if (false == _hasPlacedMapMarkers) return;

        AgentMap* map = AgentMap.Instance();
        if (null == map) return;

        map->ResetMapMarkers();
        map->ResetMiniMapMarkers();

        _hasPlacedMapMarkers = false;
    }
}
