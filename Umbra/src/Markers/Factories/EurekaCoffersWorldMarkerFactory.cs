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
using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers;

[Service]
internal sealed partial class EurekaCoffersWorldMarkerFactory : IWorldMarkerFactory, IDisposable
{
    [ConfigVariable(
        "Markers.EurekaCoffers.Enabled",
        "Enabled Markers",
        "Show eureka coffers",
        "Shows possible Eureka Bunny Coffers after using the Lucky Carrot based on the direction and distance in the chat message that appears after using the item."
    )]
    private static bool Enabled { get; set; } = true;

    private const uint LuckyCarrotItemId = 2002482;
    private const int  MinFateRespawn    = 530;
    private const int  MaxFateRespawn    = 1000;

    private long _lastBunnyFateSpawnTime;

    private static readonly Dictionary<uint, Vector3> BunnyFateSpawnPositions = new() {
        { 763, new Vector3(-166.60582f, -737.8845f, 295.7869f) },   // Pagos
        { 795, new Vector3(143.62721f,  707.92615f, 230.7047f) },   // Pyros
        { 827, new Vector3(-381.98996f, 499.92233f, -470.78052f) }, // Hydatos.
    };

    private static readonly Dictionary<uint, uint> BunnyFateIds = new() {
        { 763, 1367 }, // Pagos
        { 795, 1407 }, // Pyros
        { 827, 1425 }, // Hydatos
    };

    private readonly IZoneManager _zoneManager;
    private readonly IChatGui     _chatGui;
    private readonly IFateTable   _fateTable;
    private readonly Player       _player;

    private List<Vector3> _detectedCofferPositions = [];

    public EurekaCoffersWorldMarkerFactory(
        IZoneManager zoneManager, IChatGui chatGui, IFateTable fateTable, Player player
    )
    {
        _zoneManager = zoneManager;
        _chatGui     = chatGui;
        _fateTable   = fateTable;
        _player      = player;

        _chatGui.ChatMessageUnhandled += OnChatMessage;
    }

    public void Dispose()
    {
        _chatGui.ChatMessageUnhandled -= OnChatMessage;
    }

    public List<WorldMarker> GetMarkers()
    {
        if (false == Enabled
         || !CofferPositions.ContainsKey(_zoneManager.CurrentZone.TerritoryId)) {
            _lastBunnyFateSpawnTime = 0;
            return [];
        }

        bool isFateRunning                        = IsBunnyFateActive();
        if (isFateRunning) _lastBunnyFateSpawnTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        // If the player doesn't have the Lucky Carrot, but is in Eureka, show the bunny fate spawn marker.
        if (false == _player.HasItemInInventory(LuckyCarrotItemId)) {
            _detectedCofferPositions.Clear();
            return GetBunnyFateSpawnMarker();
        }

        return _detectedCofferPositions
            .Select(
                position => new WorldMarker {
                    Position        = position,
                    IconId          = 60356,
                    MinOpacity      = 0f,
                    MaxOpacity      = 1,
                    MinFadeDistance = 1,
                    MaxFadeDistance = 5,
                }
            )
            .ToList();
    }

    private List<WorldMarker> GetBunnyFateSpawnMarker()
    {
        if (!BunnyFateSpawnPositions.TryGetValue(_zoneManager.CurrentZone.TerritoryId, out Vector3 position)) {
            return [];
        }

        string? subLabel = null;

        if (_lastBunnyFateSpawnTime > 0) {
            var currentTime    = DateTimeOffset.Now.ToUnixTimeSeconds();
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

        return [
            new WorldMarker {
                Position        = position,
                IconId          = 60723,
                Label           = "Bunny Fate",
                SubLabel        = subLabel,
                MinOpacity      = 0f,
                MaxOpacity      = 0.8f,
                MinFadeDistance = 10,
                MaxFadeDistance = 40,
            },
        ];
    }

    private bool IsBunnyFateActive()
    {
        if (!BunnyFateIds.TryGetValue(_zoneManager.CurrentZone.TerritoryId, out uint fateId)) {
            return false;
        }

        return _fateTable.Any(fate => fate.FateId == fateId);
    }
}
