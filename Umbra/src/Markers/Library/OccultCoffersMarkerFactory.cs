using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Umbra.Markers.Library;

[Service]
internal sealed partial class OccultCoffersMarkerFactory : WorldMarkerFactory
{
    public override string Id          { get; } = "OccultCoffers";
    public override string Name        { get; } = I18N.Translate("Markers.OccultCoffers.Name");
    public override string Description { get; } = I18N.Translate("Markers.OccultCoffers.Description");

    private const uint MagicalElixirItemId = 2003296;
    private const int  PotFateRespawn      = 1800;

    private readonly IZoneManager _zoneManager;
    private readonly IChatGui     _chatGui;
    private readonly IFateTable   _fateTable;
    private readonly IPlayer      _player;

    private List<Vector3> _detectedOccultCofferPositions = [];
    private long          _lastPotFateSpawnTime;
    private uint          _lastPotFateId;
    private bool          _hasPlacedMapMarkers;
    private bool          _isListeningForNotifications;

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


    private static readonly Dictionary<uint, Dictionary<uint, Vector3>> PotFates = new() {
        {
            1252, new() {
                // South Horn
                { 1976, new(200f, 111.7266f, -215f) }, // Persistent Pots
                { 1977, new(-481f, 75f, 528f) },       // Pleading Pots
            }
        }
    };

    public OccultCoffersMarkerFactory(
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
            || !OccultCofferPositions.ContainsKey(_zoneManager.CurrentZone.TerritoryId)) {
            _lastPotFateSpawnTime        = 0;
            _isListeningForNotifications = false;
            RemoveAllMarkers();
            return;
        }

        if (GetActivePotFate() is not null) _lastPotFateSpawnTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        // If the player doesn't have the Magical Elixir, but is in Occult, show the pot fate spawn marker.
        if (false == _player.HasItemInInventory(MagicalElixirItemId)) {
            _detectedOccultCofferPositions.Clear();
            RemoveAllMarkers();
            GetPotFateSpawnMarker();
            ResetMapMarkers();
            _isListeningForNotifications = false;
            return;
        }

        // Start listening for occult coffer notifications.
        _isListeningForNotifications = true;

        List<string> activeIds = [];

        var showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var fadeDistance    = GetConfigValue<int>("FadeDistance");
        var fadeAttenuation = GetConfigValue<int>("FadeAttenuation");

        foreach (Vector3 position in _detectedOccultCofferPositions) {
            string id = $"OccultCoffer_{position.X:N0}_{position.Y:N0}_{position.Z:N0}";
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

    private void GetPotFateSpawnMarker()
    {
        if (!PotFates.TryGetValue(_zoneManager.CurrentZone.TerritoryId, out var fates)) {
            RemoveMarker("PotFate");
            return;
        }

        string? subLabel        = null;
        IFate?  potFate         = GetActivePotFate();
        var     showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var     fadeDistance    = GetConfigValue<int>("FadeDistance");
        var     fadeAttenuation = GetConfigValue<int>("FadeAttenuation");
        var     maxVisDistance  = GetConfigValue<int>("MaxVisibleDistance");

        if (null == potFate && 0 == _lastPotFateSpawnTime) {
            // No pot fate is active, and we have no spawn time recorded.
            RemoveMarker("PotFate");
            return;
        }

        if (potFate is null && _lastPotFateSpawnTime > 0) {
            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            var potRespawnTime = TimeSpan.FromSeconds(_lastPotFateSpawnTime + PotFateRespawn - currentTime);

            if (potRespawnTime.TotalSeconds > 0) {
                var min = potRespawnTime.ToString(potRespawnTime.TotalSeconds > 59 ? @"%m\ \m\i\n" : @"%s\ \s\e\c");
                subLabel = $"Respawn in {min}";
            } else {
                subLabel = "Respawning soon";
            }
        }

        if (potFate is not null) {
            _lastPotFateId = potFate.FateId;
            subLabel       = $@"{potFate.Progress}% - {TimeSpan.FromSeconds(potFate.TimeRemaining):mm\:ss} remaining";
        }

        if (_lastPotFateId == 0) return;

        // Track the _other_ pot fate in the zone if the current one is not active.
        var position = potFate?.Position ?? fates.FirstOrDefault(pos => pos.Value != fates[_lastPotFateId]).Value;

        SetMarker(
            new() {
                Key                = "PotFate",
                Position           = position,
                MapId              = _zoneManager.CurrentZone.Id,
                IconId             = 60723,
                Label              = "Pot Fate",
                SubLabel           = subLabel,
                FadeDistance       = new(fadeDistance, fadeDistance + fadeAttenuation),
                ShowOnCompass      = showDirection,
                MaxVisibleDistance = maxVisDistance,
            }
        );
    }

    private IFate? GetActivePotFate()
    {
        return PotFates.TryGetValue(_zoneManager.CurrentZone.TerritoryId, out var fates)
            ? _fateTable.FirstOrDefault(fate => fates.ContainsKey(fate.FateId) && fate.TimeRemaining > 0)
            : null;
    }

    private unsafe void AddMapMarkers()
    {
        if (!GetConfigValue<bool>("EnableMapMarkers")) {
            ResetMapMarkers();
            return;
        }

        if (Framework.DalamudPlugin.InstalledPlugins.Any(plugin => plugin is {
                    InternalName: "EurekaTrackerAutoPopper",
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

    private void OnChatMessage(
        XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled
    )
    {
        if (!_isListeningForNotifications) return;

        var positions = ChatLocationFilter.FilterPositions(message.TextValue.Trim(), _player.Position, OccultCofferPositions[_zoneManager.CurrentZone.TerritoryId]);

        if (positions != null) _detectedOccultCofferPositions = positions;

        AddMapMarkers();
    }
}
