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

using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class ZoneFactory(
    IDataManager            dataManager,
    WeatherForecastProvider weatherForecastProvider,
    ZoneMarkerFactory       markerFactory,
    IPlayer                 player
) : IDisposable
{
    private readonly Dictionary<uint, Zone> _zoneCache = [];

    public void Dispose()
    {
        _zoneCache.Clear();
    }

    public Zone GetZone(uint zoneId)
    {
        if (_zoneCache.TryGetValue(zoneId, out var cachedZone)) return cachedZone;

        if (null == dataManager.GetExcelSheet<Map>()!.GetRow(zoneId)) {
            throw new InvalidOperationException($"Zone {zoneId} does not exist");
        }

        var zone = new Zone(dataManager, weatherForecastProvider, markerFactory, player, zoneId);

        _zoneCache[zoneId] = zone;

        return zone;
    }
}
