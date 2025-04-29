using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Umbra.Common;
using Umbra.Game;
using TerritoryType = Lumina.Excel.Sheets.TerritoryType;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    private IDataManager   DataManager   { get; } = Framework.Service<IDataManager>();
    private IAetheryteList AetheryteList { get; } = Framework.Service<IAetheryteList>();

    private readonly Dictionary<string, TeleportExpansion>   _expansions   = [];
    private readonly Dictionary<string, TeleportDestination> _destinations = [];

    private List<uint>? EstateAetheryteIds { get; set; }
    private string?     _selectedExpansion;

    /// <summary>
    /// Hydrates the aetheryte points into the expansion/region/destination structure that can be used to build
    /// the graphical nodes in the popup menu.
    /// </summary>
    private void HydrateAetherytePoints()
    {
        IZone currentZone   = Framework.Service<IZoneManager>().CurrentZone;
        var   territoryType = DataManager.GetExcelSheet<TerritoryType>().FindRow(currentZone.TerritoryId);

        if (territoryType == null) return;
        var currentExNodeId = $"Ex_{territoryType.Value.ExVersion.ValueNullable?.RowId}";

        foreach (var aetheryte in AetheryteList) {
            if (IsAetherytePlayerHousing(aetheryte)) continue;

            var gameData = aetheryte.AetheryteData.ValueNullable;
            if (gameData == null) continue;

            if (gameData.Value.Invisible || !gameData.Value.IsAetheryte) continue;

            var territory = gameData.Value.Territory.ValueNullable;
            if (territory == null) continue;

            var expansion = territory.Value.ExVersion.ValueNullable;
            if (expansion == null) continue;

            var   regionName    = territory.Value.Map.ValueNullable?.PlaceNameRegion.ValueNullable?.Name.ToString();
            var   mapName       = territory.Value.Map.ValueNullable?.PlaceName.ValueNullable?.Name.ToString();
            var   aetheryteName = gameData.Value.PlaceName.ValueNullable?.Name.ToString();
            uint? mapId         = territory.Value.Map.ValueNullable?.RowId;

            if (regionName == null || mapName == null || aetheryteName == null || mapId == null) continue;

            var regionNameRowId = territory.Value.Map.Value.PlaceNameRegion.RowId;
            var territoryRowId  = territory.Value.RowId;

            var expansionNodeId = $"Ex_{expansion.Value.RowId}";
            var regionNodeId    = $"Ex_{expansion.Value.RowId}_{regionNameRowId}";
            var aetheryteNodeId = $"Ex_{expansion.Value.RowId}_{territoryRowId}_{gameData.Value.RowId}";

            if (currentExNodeId == expansionNodeId) {
                _selectedExpansion = expansionNodeId;
            }

            _expansions.TryAdd(
                expansionNodeId,
                new() {
                    NodeId    = expansionNodeId,
                    Name      = expansion.Value.Name.ToString(),
                    SortIndex = (int)expansion.Value.RowId,
                    Regions   = [],
                }
            );

            var expansionNode = _expansions[expansionNodeId];

            expansionNode.Regions.TryAdd(
                regionNodeId,
                new() {
                    NodeId = regionNodeId,
                    Name   = regionName,
                    Maps   = [],
                }
            );

            var regionNode = expansionNode.Regions[regionNodeId];

            regionNode.Maps.TryAdd(
                $"Map_{mapId}",
                new() {
                    NodeId       = aetheryteNodeId,
                    Name         = mapName,
                    Destinations = [],
                }
            );

            var mapNode = regionNode.Maps[$"Map_{mapId}"];

            var partId = GetPartId(GetRegionFromRegionNamePlace(regionNameRowId), territoryRowId);

            TeleportDestination destination = new() {
                NodeId      = aetheryteNodeId,
                Name        = aetheryteName,
                AetheryteId = aetheryte.AetheryteId,
                SubIndex    = aetheryte.SubIndex,
                GilCost     = aetheryte.GilCost,
                SortIndex   = gameData.Value.Order,
                UldPartId   = partId,
                MapId       = (uint)mapId,
                TerritoryId = territory.Value.RowId,
            };

            mapNode.Destinations.TryAdd(aetheryteNodeId, destination);
            _destinations.TryAdd($"{destination.AetheryteId}:{destination.SubIndex}", destination);
        }
    }

    private bool IsAetherytePlayerHousing(IAetheryteEntry entry)
    {
        EstateAetheryteIds ??= DataManager.GetExcelSheet<Aetheryte>()
            .Where(aetheryte => aetheryte.PlaceName.RowId is 1145 or 1160)
            .Select(aetheryte => aetheryte.RowId)
            .ToList();

        return entry.IsSharedHouse
            || entry.IsApartment
            || entry.Plot > 0
            || entry.Ward > 0
            || EstateAetheryteIds.Contains(entry.AetheryteId);
    }

    // gotten from Client::UI::Agent::AgentTeleport_Show -> sub_140C04360 -> sub_140C043D0 -> sub_140C06860
    // sig: E8 ?? ?? ?? ?? 49 8D 4E F8 8B D8
    // was added as a function with the new expansion so possibly unstable
    private int GetPartId(uint region, uint territory)
    {
        return territory switch {
            819          => 8,
            820          => 9,
            958          => 11,
            1186 or 1191 => 14,
            _ => region switch {
                0  => 0,
                1  => 1,
                2  => 2,
                3  => 4,
                6  => 6,
                7  => 7,
                10 => 5,
                12 => 10,
                13 => 12,
                _  => region - 16 > 1 ? 3 : 13
            }
        };
    }

    // gotten from Client::UI::Agent::AgentTeleport_Show -> sub_140C04360 -> sub_140C043D0 -> sub_140C064F0
    // sig: 48 83 EC 28 0F B7 4A 08
    private uint GetRegionFromRegionNamePlace(uint placeNameRegion) =>
        placeNameRegion switch {
            22   => 0,
            23   => 1,
            24   => 2,
            25   => 3,
            497  => 4,
            498  => 5,
            26   => 8,
            2400 => 6,
            2402 => 7,
            2401 => 9,
            2950 => 11,
            3703 => 12,
            3702 => 13,
            3704 => 14,
            3705 => 15,
            4500 => 16,
            4501 => 17,
            4502 => 18,
            _    => 19
        };
}

internal struct TeleportExpansion
{
    public string                             NodeId    { get; set; }
    public string                             Name      { get; set; }
    public int                                SortIndex { get; set; }
    public Dictionary<string, TeleportRegion> Regions   { get; set; }
}

internal struct TeleportRegion
{
    public string NodeId { get; set; }
    public string Name   { get; set; }

    public Dictionary<string, TeleportMap> Maps { get; set; }
}

internal struct TeleportMap
{
    public string NodeId { get; set; }
    public string Name   { get; set; }

    public Dictionary<string, TeleportDestination> Destinations { get; set; }
}

internal struct TeleportDestination
{
    public string NodeId      { get; set; }
    public string Name        { get; set; }
    public uint   AetheryteId { get; set; }
    public byte   SubIndex    { get; set; }
    public uint   GilCost     { get; set; }
    public int    SortIndex   { get; set; }
    public int    UldPartId   { get; set; }
    public uint   MapId       { get; set; }
    public uint   TerritoryId { get; set; }
}
