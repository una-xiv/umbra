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
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    private IDataManager   DataManager   { get; } = Framework.Service<IDataManager>();
    private IAetheryteList AetheryteList { get; } = Framework.Service<IAetheryteList>();

    private readonly Dictionary<string, TeleportExpansion> _expansions = [];

    private List<uint>? EstateAetheryteIds { get; set; }

    /// <summary>
    /// Hydrates the aetheryte points into the expansion/region/destination structure that can be used to build
    /// the graphical nodes in the popup menu.
    /// </summary>
    private void HydrateAetherytePoints()
    {
        foreach (var aetheryte in AetheryteList) {
            // Don't index housing aetherytes...
            if (IsAetherytePlayerHousing(aetheryte)) continue;

            var gameData = aetheryte.AetheryteData.GameData;
            if (gameData == null) continue;

            var territory = gameData.Territory.Value;
            if (territory == null) continue;

            var expansion = territory.ExVersion.Value;
            if (expansion == null) continue;

            var regionName    = territory.Map.Value?.PlaceNameRegion.Value?.Name.ToString();
            var mapName       = territory.Map.Value?.PlaceName.Value?.Name.ToString();
            var aetheryteName = gameData.PlaceName.Value?.Name.ToString();

            if (regionName == null || mapName == null || aetheryteName == null) continue;

            var expansionNodeId = $"Ex_{expansion.RowId}";
            var regionNodeId    = $"Ex_{expansion.RowId}_{territory.Map.Value!.PlaceNameRegion.Row}";
            var aetheryteNodeId = $"Ex_{expansion.RowId}_{territory.RowId}_{gameData.RowId}";

            _expansions.TryAdd(
                expansionNodeId,
                new() {
                    NodeId    = expansionNodeId,
                    Name      = expansion.Name.ToString(),
                    SortIndex = (int)expansion.RowId,
                    Regions   = [],
                }
            );

            var expansionNode = _expansions[expansionNodeId];

            expansionNode.Regions.TryAdd(
                regionNodeId,
                new() {
                    NodeId       = regionNodeId,
                    Name         = regionName,
                    Destinations = [],
                }
            );

            var regionNode = expansionNode.Regions[regionNodeId];

            regionNode.Destinations.TryAdd(
                aetheryteNodeId,
                new() {
                    NodeId      = aetheryteNodeId,
                    Name        = aetheryteName,
                    AetheryteId = aetheryte.AetheryteId,
                    SubIndex    = aetheryte.SubIndex,
                    GilCost     = aetheryte.GilCost,
                }
            );
        }
    }

    private bool IsAetherytePlayerHousing(IAetheryteEntry entry)
    {
        EstateAetheryteIds ??= DataManager.GetExcelSheet<Aetheryte>()!
            .Where(aetheryte => aetheryte.PlaceName.Row is 1145 or 1160)
            .Select(aetheryte => aetheryte.RowId)
            .ToList();

        return entry.IsSharedHouse
            || entry.IsApartment
            || entry.Plot > 0
            || entry.Ward > 0
            || EstateAetheryteIds.Contains(entry.AetheryteId);
    }
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

    public Dictionary<string, TeleportDestination> Destinations { get; set; }
}

internal struct TeleportDestination
{
    public string NodeId      { get; set; }
    public string Name        { get; set; }
    public uint   AetheryteId { get; set; }
    public byte   SubIndex    { get; set; }
    public uint   GilCost     { get; set; }
}
