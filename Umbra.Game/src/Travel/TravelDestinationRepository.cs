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
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets2;
using Umbra.Common;

namespace Umbra.Game;

[Service]
public class TravelDestinationRepository : ITravelDestinationRepository
{
    public List<TravelDestination> Destinations { get; } = [];

    private readonly IAetheryteList _aetheryteList;
    private readonly IDataManager   _dataManager;
    private readonly IZoneManager   _zoneManager;

    private readonly List<uint> _estateAetherytes;
    private readonly ushort?    _freeAetheryteId;

    public TravelDestinationRepository(
        IAetheryteList aetheryteList,
        IDataManager   dataManager,
        IZoneManager   zoneManager
    )
    {
        _aetheryteList   = aetheryteList;
        _dataManager     = dataManager;
        _zoneManager     = zoneManager;
        _freeAetheryteId = GetFreeAetheryteId();

        _estateAetherytes = _dataManager.GetExcelSheet<Aetheryte>()!
            .Where(aetheryte => aetheryte.PlaceName.Row is 1145 or 1160)
            .Select(aetheryte => aetheryte.RowId)
            .ToList();
    }

    public void Sync()
    {
        if (!_zoneManager.HasCurrentZone) return;

        foreach (var entry in _aetheryteList)
        {
            if (!IsListedAetheryteEntry(entry))
            {
                Destinations.RemoveAll(d => d.Id == entry.AetheryteId && d.SubId == entry.SubIndex);
                continue;
            }

            bool isHousing = entry.IsSharedHouse || entry.IsApartment || _estateAetherytes.Contains(entry.AetheryteId);
            TravelDestination? destination = Destinations.FirstOrDefault(d => d.Id == entry.AetheryteId && d.SubId == entry.SubIndex);

            if (destination == null)
            {
                destination = new(entry, isHousing, _freeAetheryteId == entry.AetheryteId);
                Destinations.Add(destination);
            }
            else
            {
                destination.Update(entry, isHousing, _freeAetheryteId == entry.AetheryteId);
            }
        }
    }

    private bool IsListedAetheryteEntry(IAetheryteEntry entry)
    {
        return entry.IsFavourite
         || entry.IsApartment
         || entry.IsSharedHouse
         || entry.Plot > 0
         || entry.Ward > 0
         || _freeAetheryteId == entry.AetheryteId
         || _estateAetherytes.Contains(entry.AetheryteId);
    }

    private static unsafe ushort? GetFreeAetheryteId()
    {
        ushort id = PlayerState.Instance()->FreeAetheryteId;
        return id > 0 ? id : null;
    }
}
