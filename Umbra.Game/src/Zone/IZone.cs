using System.Collections.Generic;
using System.Numerics;
using Lumina.Excel.Sheets;

namespace Umbra.Game;

public interface IZone
{
    public uint                  Id                  { get; }
    public TerritoryType         Type                { get; }
    public uint                  TerritoryId         { get; }
    public string                Name                { get; }
    public string                SubName             { get; }
    public string                RegionName          { get; }
    public Vector2               Offset              { get; }
    public ushort                SizeFactor          { get; }
    public Map                   MapSheet            { get; }
    public List<ZoneMarker>      StaticMarkers       { get; }
    public List<ZoneMarker>      DynamicMarkers      { get; }
    public List<WeatherForecast> WeatherForecast     { get; }
    public WeatherForecast?      CurrentWeather      { get; }
    public string                CurrentDistrictName { get; }
    public bool                  IsSanctuary         { get; }
    public uint                  InstanceId          { get; }
    public Vector2               PlayerCoordinates   { get; }
}
