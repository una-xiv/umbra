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
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;

namespace Umbra.Game;

[Service]
public sealed class ZoneManager(ZoneFactory factory) : IZoneManager
{
    public event Action<Zone>? ZoneChanged;

    private Zone? _zone = null;

    public Zone CurrentZone {
        get {
            if (null == _zone) {
                throw new InvalidOperationException("Zone not initialized");
            }

            return _zone;
        }
    }

    public Zone GetZone(uint zoneId)
    {
        return factory.GetZone(zoneId);
    }

    [OnTick]
    public unsafe void OnTick()
    {
        AgentMap* agentMap = AgentMap.Instance();
        if (agentMap == null || agentMap->CurrentMapId == 0) return;

        if (null == _zone || _zone.Id != agentMap->CurrentMapId) {
            _zone = factory.GetZone(agentMap->CurrentMapId);
            ZoneChanged?.Invoke(_zone);
        }

        _zone?.Update();
    }
}