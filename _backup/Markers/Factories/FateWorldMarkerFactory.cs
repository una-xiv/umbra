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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers;

[Service]
internal sealed class FateWorldMarkerFactory(IZoneManager zoneManager) : IWorldMarkerFactory
{
    [ConfigVariable("Markers.Fate.Enabled", "EnabledMarkers")]
    private static bool Enabled { get; set; } = false;

    private readonly List<WorldMarker> _worldMarkers = [];

    public List<WorldMarker> GetMarkers()
    {
        if (false == Enabled) return [];

        lock (_worldMarkers) {
            return zoneManager.HasCurrentZone
                ? _worldMarkers.ToList()
                : [];
        }
    }

    [OnTick(interval: 1000)]
    public unsafe void OnTick()
    {
        FateManager* fm = FateManager.Instance();

        lock (_worldMarkers) {
            _worldMarkers.Clear();

            long now = DateTimeOffset.Now.ToUnixTimeSeconds();

            foreach (FateContext* fate in fm->Fates.ToList()) {
                if (fate == null) continue;

                long startTime = fate->StartTimeEpoch;
                long endTime   = startTime + fate->Duration;

                if (startTime > 0 && endTime > 0 && (startTime > now || endTime < now)) continue;

                TimeSpan timeLeft = DateTimeOffset.FromUnixTimeSeconds(endTime).Subtract(DateTimeOffset.Now);
                var      progress = "";

                if (fate->Progress > 0) {
                    progress = $" ({fate->Progress}%)";
                }

                var state = (FateState)fate->State;

                _worldMarkers.Add(
                    new() {
                        IconId          = fate->MapIconId,
                        Position        = fate->Location + new Vector3(0, 1.8f, 0),
                        Label           = MemoryHelper.ReadSeString(&fate->Name).ToString(),
                        SubLabel        = $"{state} - {timeLeft:mm\\:ss}{progress}",
                        MinFadeDistance = 30,
                        MaxFadeDistance = 60
                    }
                );
            }
        }
    }
}
