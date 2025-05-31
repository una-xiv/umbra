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
internal sealed partial class OccultCoffersMarkerFactory : WorldMarkerFactory
{
    public override string Id { get; } = "OccultCoffers";
    public override string Name { get; } = I18N.Translate("Markers.OccultCoffers.Name");
    public override string Description { get; } = I18N.Translate("Markers.OccultCoffers.Description");

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            new BooleanMarkerConfigVariable(
                "EnableMapMarkers",
                I18N.Translate("Markers.OccultCoffers.EnableMapMarkers.Name"),
                I18N.Translate("Markers.OccultCoffers.EnableMapMarkers.Description"),
                true
            ),
            ..DefaultFadeConfigVariables,
        ];
    }

    private const uint MagicalElixirItemId = 2003296;
    private const int MinFateRespawn = 530;
    private const int MaxFateRespawn = 1000;

    private long _lastPotFateSpawnTime;

    private static readonly Dictionary<uint, List<Vector3>> PotFateSpawnPositions = new() {
        { 1252, new List<Vector3> {
            new(200f, 111.7266f, -215f), // South Horn - Persistent Pots
            new(-481f, 75f, 528f)        // South Horn - Pleading Pots
        }}
    };

    private static readonly Dictionary<uint, List<uint>> PotFateIds = new() {
        { 1252, new List<uint> {
            1976, // South Horn - Persistent Pots
            1977  // South Horn - Pleading Pots
        }}
    };

    private readonly IZoneManager _zoneManager;
    private readonly IChatGui _chatGui;
    private readonly IFateTable _fateTable;
    private readonly IPlayer _player;

    private List<Vector3> _detectedOccultCofferPositions = [];
    private bool _hasPlacedMapMarkers;

    private bool isEnabled;

    public OccultCoffersMarkerFactory(
        IZoneManager zoneManager,
        IChatGui chatGui,
        IFateTable fateTable,
        IPlayer player
    )
    {
        _zoneManager = zoneManager;
        _chatGui = chatGui;
        _fateTable = fateTable;
        _player = player;

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
        bool zoneIsValidForChat = _zoneManager.HasCurrentZone &&
                                  OccultCofferPositions.ContainsKey(_zoneManager.CurrentZone.TerritoryId);

        bool previouslyEnabledForChat = isEnabled;
        isEnabled = zoneIsValidForChat;

        if (previouslyEnabledForChat && !isEnabled) {
            _detectedOccultCofferPositions.Clear();
            ResetMapMarkers();
        }

        if (false == GetConfigValue<bool>("Enabled")
            || !_zoneManager.HasCurrentZone
            || !OccultCofferPositions.ContainsKey(_zoneManager.CurrentZone.TerritoryId)) {
            _lastPotFateSpawnTime = 0;
            RemoveAllMarkers();
            return;
        }

        if (PotFate() is not null) _lastPotFateSpawnTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        // If the player doesn't have the Magical Elixir, but is in Occult, show the pot fate spawn marker.
        if (false == _player.HasItemInInventory(MagicalElixirItemId)) {
            _detectedOccultCofferPositions.Clear();
            RemoveAllMarkers();
            GetPotFateSpawnMarker();
            ResetMapMarkers();
            return;
        }

        List<string> activeIds = [];

        var showDirection = GetConfigValue<bool>("ShowOnCompass");
        var fadeDistance = GetConfigValue<int>("FadeDistance");
        var fadeAttenuation = GetConfigValue<int>("FadeAttenuation");

        foreach (Vector3 position in _detectedOccultCofferPositions) {
            string id = $"OccultCoffer_{position.X:N0}_{position.Y:N0}_{position.Z:N0}";
            activeIds.Add(id);

            SetMarker(
                new() {
                    Key = id,
                    MapId = _zoneManager.CurrentZone.Id,
                    Position = position,
                    IconId = 60356,
                    ShowOnCompass = showDirection,
                    FadeDistance = new(fadeDistance, fadeDistance + fadeAttenuation),
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }

    private void GetPotFateSpawnMarker()
    {
        if (!PotFateSpawnPositions.TryGetValue(_zoneManager.CurrentZone.TerritoryId, out List<Vector3> position)) {
            RemoveMarker("PotFate");
            return;
        }

        string? subLabel = null;
        IFate? potFate = PotFate();
        var showDirection = GetConfigValue<bool>("ShowOnCompass");
        var fadeDistance = GetConfigValue<int>("FadeDistance");
        var fadeAttenuation = GetConfigValue<int>("FadeAttenuation");
        var maxVisDistance = GetConfigValue<int>("MaxVisibleDistance");

        if (potFate is null && _lastPotFateSpawnTime > 0) {
            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            var minRespawnTime = TimeSpan.FromSeconds(_lastPotFateSpawnTime + MinFateRespawn - currentTime);
            var maxRespawnTime = TimeSpan.FromSeconds(_lastPotFateSpawnTime + MaxFateRespawn - currentTime);

            if (minRespawnTime.TotalSeconds > 0) {
                var min = minRespawnTime.ToString(minRespawnTime.TotalSeconds > 59 ? @"%m\ \m\i\n" : @"%s\ \s\e\c");
                var max = maxRespawnTime.ToString(maxRespawnTime.TotalSeconds > 59 ? @"%m\ \m\i\n" : @"%s\ \s\e\c");
                subLabel = $"Respawn between {min} and {max}";
            } else {
                subLabel = "Respawning soon";
            }
        }

        if (potFate is not null) {
            subLabel = $@"{potFate.Progress}% - {TimeSpan.FromSeconds(potFate.TimeRemaining):mm\:ss} remaining";
        }

        SetMarker(
            new() {
                Key = "PotFate",
                Position = position,
                MapId = _zoneManager.CurrentZone.Id,
                IconId = 60723,
                Label = "Pot Fate",
                SubLabel = subLabel,
                FadeDistance = new(fadeDistance, fadeDistance + fadeAttenuation),
                ShowOnCompass = showDirection,
                MaxVisibleDistance = maxVisDistance,
            }
        );
    }

    private IFate? PotFate()
    {
        return PotFateIds.TryGetValue(_zoneManager.CurrentZone.TerritoryId, out List<uint> fateIds)
            ? _fateTable.FirstOrDefault(fate => fateIds.Contains(fate.FateId))
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
                    InternalName: "EurekaTrackerAutoPopper",
                    IsLoaded: true
                }
            )) {
            // Don't show map markers if the Eureka Tracker Auto Popper plugin is loaded.
            return;
        }

        AgentMap* map = AgentMap.Instance();
        if (null == map) return;

        map->ResetMapMarkers();
        map->ResetMiniMapMarkers();

        foreach (var pos in _detectedOccultCofferPositions) {
            var mapPos = pos;

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
