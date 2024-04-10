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

    public string                CurrentDistrictName => _closestAreaMarker?.Name ?? "";


    private readonly IPlayer                 _player;
    private readonly IDataManager            _dataManager;
    private readonly ZoneMarkerFactory       _markerFactory;
    private readonly WeatherForecastProvider _forecastProvider;

    private ZoneMarker? _closestAreaMarker;

    public Zone(
        IDataManager            dataManager,
        WeatherForecastProvider forecastProvider,
        ZoneMarkerFactory       markerFactory,
        IPlayer                 player,
        uint                    zoneId
    )
    {
        _player           = player;
        _dataManager      = dataManager;
        _markerFactory    = markerFactory;
        _forecastProvider = forecastProvider;

        Id          = zoneId;
        MapSheet    = dataManager.GetExcelSheet<Sheet.Map>()!.GetRow(zoneId)!;
        Type        = (TerritoryType)MapSheet.TerritoryType.Value!.TerritoryIntendedUse;
        TerritoryId = MapSheet.TerritoryType.Row;
        Name        = MapSheet.PlaceName.Value!.Name.ToString();
        SubName     = MapSheet.PlaceNameSub.Value!.Name.ToString();
        RegionName  = MapSheet.PlaceNameRegion.Value!.Name.ToString();
        Offset      = new(MapSheet.OffsetX, MapSheet.OffsetY);
        SizeFactor  = MapSheet.SizeFactor;

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

        if (agentMap->CurrentMapId != Id) {
            DynamicMarkers.Clear();
            _closestAreaMarker = null;
            return;
        }

        Map* map = Map.Instance();
        if (map == null) return;

        lock (DynamicMarkers) {
            DynamicMarkers.Clear();

            DynamicMarkers.AddRange(
                map->ActiveLevequest
                    .ToList()
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->CustomTalk
                    .ToList()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->GemstoneTraders
                    .ToList()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->GuildLeveAssignments
                    .ToList()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->HousingDataSpan
                    .ToArray()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->LevequestDataSpan
                    .ToArray()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->QuestDataSpan
                    .ToArray()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->TripleTriad
                    .ToList()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );

            DynamicMarkers.AddRange(
                map->UnacceptedQuests
                    .ToList()
                    .SelectMany(i => i.MarkerData.ToList())
                    .Where(m => m.MapId == Id)
                    .Select(m => _markerFactory.FromMapMarkerData(MapSheet, m))
                    .ToList()
            );
        }

        var playerPos = _player.Position.ToVector2();

        ZoneMarker? marker = StaticMarkers
            .Concat(DynamicMarkers)
            .Where(
                m => m.Name != ""
                 && m.Type is ZoneMarkerType.Pin
                        or ZoneMarkerType.Settlement
                        or ZoneMarkerType.Area
                        or ZoneMarkerType.Aetheryte
                        or ZoneMarkerType.Aethernet
            )
            .OrderBy(m => Vector2.Distance(playerPos, m.WorldPosition.ToVector2()))
            .FirstOrDefault();

        if (_closestAreaMarker?.Name != marker?.Name) {
            _closestAreaMarker = marker;
        }

        lock (WeatherForecast) {
            WeatherForecast.Clear();

            var weatherRate =
                _dataManager.GetExcelSheet<Sheet.WeatherRate>()!.GetRow(MapSheet.TerritoryType.Value!.WeatherRate);

            if (weatherRate != null) {
                WeatherForecast.AddRange(_forecastProvider.GetForecast(weatherRate, 6));

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
    }
}
