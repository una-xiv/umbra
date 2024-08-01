/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Game is free software: you can          \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Game is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Sheet = Lumina.Excel.GeneratedSheets;

namespace Umbra.Game;

internal sealed class Zone : IZone
{
    public uint                  Id                  { get; private set; }
    public TerritoryType         Type                { get; private set; }
    public uint                  TerritoryId         { get; private set; }
    public string                Name                { get; private set; }
    public string                SubName             { get; private set; }
    public string                RegionName          { get; private set; }
    public Vector2               Offset              { get; private set; }
    public ushort                SizeFactor          { get; private set; }
    public Sheet.Map             MapSheet            { get; private set; }
    public List<ZoneMarker>      StaticMarkers       { get; private set; }
    public List<ZoneMarker>      DynamicMarkers      { get; private set; }
    public List<WeatherForecast> WeatherForecast     { get; private set; }
    public WeatherForecast?      CurrentWeather      { get; private set; }
    public bool                  IsSanctuary         { get; private set; }
    public string                CurrentDistrictName { get; private set; }
    public uint                  InstanceId          { get; private set; }
    public Vector2               PlayerCoordinates   { get; private set; }

    private readonly IDataManager            _dataManager;
    private readonly ZoneMarkerFactory       _markerFactory;
    private readonly WeatherForecastProvider _forecastProvider;
    private readonly IPlayer                 _player;

    public Zone(
        IDataManager            dataManager,
        WeatherForecastProvider forecastProvider,
        ZoneMarkerFactory       markerFactory,
        IPlayer                 player,
        uint                    zoneId
    )
    {
        _dataManager      = dataManager;
        _markerFactory    = markerFactory;
        _forecastProvider = forecastProvider;
        _player           = player;

        Id                  = zoneId;
        MapSheet            = dataManager.GetExcelSheet<Sheet.Map>()!.GetRow(zoneId)!;
        Type                = (TerritoryType)MapSheet.TerritoryType.Value!.TerritoryIntendedUse;
        TerritoryId         = MapSheet.TerritoryType.Row;
        Name                = MapSheet.PlaceName.Value!.Name.ToString();
        SubName             = MapSheet.PlaceNameSub.Value!.Name.ToString();
        RegionName          = MapSheet.PlaceNameRegion.Value!.Name.ToString();
        Offset              = new(MapSheet.OffsetX, MapSheet.OffsetY);
        SizeFactor          = MapSheet.SizeFactor;
        IsSanctuary         = false;
        CurrentDistrictName = "-";

        StaticMarkers = dataManager.GetExcelSheet<Sheet.MapMarker>()!
            .Where(m => m.RowId == MapSheet.MapMarkerRange && m.X > 0 && m.Y > 0)
            .Select(m => markerFactory.FromMapMarkerSheet(MapSheet, m))
            .ToList();

        DynamicMarkers  = [];
        WeatherForecast = [];

        Update();
    }

    internal unsafe void Update()
    {
        AgentMap* agentMap = AgentMap.Instance();
        if (agentMap == null || agentMap->CurrentMapId == 0) return;

        PlayerCoordinates = MapUtil.WorldToMap(new(_player.Position.X, _player.Position.Z), MapSheet);

        if (agentMap->CurrentMapId != Id) {
            DynamicMarkers.Clear();
            return;
        }

        TerritoryInfo* territoryInfo = TerritoryInfo.Instance();
        if (territoryInfo == null) return;

        IsSanctuary = territoryInfo->InSanctuary;
        InstanceId  = UIState.Instance()->PublicInstance.InstanceId;

        HousingManager* housingManager = HousingManager.Instance();

        if (housingManager == null || housingManager->CurrentTerritory == null) {
            CurrentDistrictName = _dataManager.GetExcelSheet<Sheet.PlaceName>()!
                    .GetRow(territoryInfo->AreaPlaceNameId)
                    ?.Name.ToString()
                ?? "???";
        } else {
            CurrentDistrictName = GetHousingDistrictName();
        }

        if (string.IsNullOrEmpty(CurrentDistrictName)) {
            CurrentDistrictName = " ";
        }

        Map* map = Map.Instance();
        if (map == null) return;

        lock (DynamicMarkers) {
            DynamicMarkers.Clear();

            foreach (var marker in agentMap->MiniMapGatheringMarkers) {
                if (0 == marker.MapMarker.IconId) continue;
                var m = _markerFactory.FromMinimapGatheringMarker(MapSheet, marker);
                DynamicMarkers.Add(m);
            }

            DynamicMarkers.AddRange(
                map->ActiveLevequestMarkers
                    .ToList()
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->CustomTalkMarkers
                    .ToList()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->GemstoneTraderMarkers
                    .ToList()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->GuildLeveAssignmentMarkers
                    .ToList()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->HousingMarkers
                    .ToArray()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->LevequestMarkers
                    .ToArray()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->QuestMarkers
                    .ToArray()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->TripleTriadMarkers
                    .ToList()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->UnacceptedQuestMarkers
                    .ToList()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );
        }

        lock (WeatherForecast) {
            WeatherForecast = _forecastProvider.GetWeatherForecast((ushort)TerritoryId);

            if (WeatherForecast.Count > 0) {
                var    time       = WeatherForecast[0].Time;
                string timeString = I18N.Translate("WeatherForecast.Always");

                if (WeatherForecast.Count > 1) {
                    time       = WeatherForecast[1].Time;
                    timeString = WeatherForecast[1].TimeString;
                }

                CurrentWeather = new(time, timeString, WeatherForecast[0].Name, WeatherForecast[0].IconId);
            }
        }
    }

    private unsafe string GetHousingDistrictName()
    {
        var housingManager = HousingManager.Instance();
        if (housingManager == null) return string.Empty;

        int ward = housingManager->GetCurrentWard() + 1;
        if (ward == 0) return string.Empty;

        List<string> result = [];

        sbyte plot     = housingManager->GetCurrentPlot();
        short room     = housingManager->GetCurrentRoom();
        byte  division = housingManager->GetCurrentDivision();

        result.Add($"{I18N.Translate("Housing.Ward")} {ward}");
        if (division == 2 || plot is >= 30 or -127) result.Add(I18N.Translate("Housing.Subdivision"));

        switch (plot) {
            case < -1:
                result.Add(
                    $"{I18N.Translate("Housing.Apartment")} {(room == 0 ? I18N.Translate("Housing.Lobby") : $"{I18N.Translate("Housing.Room")} {room}")}"
                );

                break;

            case > -1:
                result.Add($"{I18N.Translate("Housing.Plot")} {plot + 1}");
                if (room > 0) result.Add($"{I18N.Translate("Housing.Room")} {room}");

                break;
        }

        return string.Join(" ", result);
    }
}
