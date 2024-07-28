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
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class ZoneManager(ZoneFactory factory, IClientState clientState) : IZoneManager, IDisposable
{
    public event Action<IZone>? ZoneChanged;

    private Zone? _zone = null;

    public bool HasCurrentZone => _zone != null;

    public IZone CurrentZone {
        get {
            if (null == _zone) {
                throw new InvalidOperationException("Zone not initialized");
            }

            return _zone;
        }
    }

    public void Dispose()
    {
        foreach (var handler in ZoneChanged?.GetInvocationList() ?? []) ZoneChanged -= (Action<IZone>)handler;

        _zone = null;
    }

    public IZone GetZone(uint zoneId)
    {
        return factory.GetZone(zoneId);
    }

    [OnTick]
    internal void OnTick()
    {
        if (clientState.MapId == 0) {
            return;
        }

        // Don't do anything if the local player isn't available.
        if (clientState.LocalPlayer == null) return;

        if (null == _zone || _zone.Id != clientState.MapId) {
            _zone = factory.GetZone(clientState.MapId);
            ZoneChanged?.Invoke(_zone);
        }

        _zone?.Update();
    }
}
