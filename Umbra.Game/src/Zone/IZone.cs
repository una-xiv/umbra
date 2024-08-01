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
using System.Numerics;
using Lumina.Excel.GeneratedSheets;

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
