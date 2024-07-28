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
using System.Numerics;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.Text;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.Library;

[Service]
internal class FateMarkerFactory(IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "FateMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.Fate.Name");
    public override string Description { get; } = I18N.Translate("Markers.Fate.Description");

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            ..DefaultFadeConfigVariables,
        ];
    }

    protected override void OnZoneChanged(IZone zone)
    {
        RemoveAllMarkers();
    }

    [OnTick(interval: 1000)]
    private unsafe void OnUpdate()
    {
        var enabled  = GetConfigValue<bool>("Enabled");
        var fadeDist = GetConfigValue<int>("FadeDistance");

        FateManager* fm = FateManager.Instance();

        if (!enabled || !zoneManager.HasCurrentZone || fm is null) {
            RemoveAllMarkers();
            return;
        }

        long now = DateTimeOffset.Now.ToUnixTimeSeconds();

        List<string> activeIds = [];

        foreach (FateContext* fate in fm->Fates.ToList()) {
            if (fate == null) continue;

            long startTime = fate->StartTimeEpoch;
            long endTime   = startTime + fate->Duration;

            if (startTime > 0 && endTime > 0 && (startTime > now || endTime < now)) continue;

            TimeSpan timeLeft = DateTimeOffset.FromUnixTimeSeconds(endTime).Subtract(DateTimeOffset.Now);

            var id       = $"FATE_{fate->FateId}";
            var progress = "";
            var state    = (FateState)fate->State;

            if (fate->Progress > 0) {
                progress = $" - {fate->Progress}%";
            }

            activeIds.Add(id);
            string prefix = fate->IsBonus ? $"{SeIconChar.BoxedStar.ToIconString()} " : "";

            SetMarker(
                new() {
                    Key           = id,
                    IconId        = fate->IconId,
                    MapId         = zoneManager.CurrentZone.Id,
                    Label         = $"{prefix}{MemoryHelper.ReadSeString(&fate->Name)}",
                    SubLabel      = $"{state} - {timeLeft:mm\\:ss}{progress}",
                    Position      = fate->Location + new Vector3(0, 1.8f, 0),
                    FadeDistance  = new(fadeDist, fadeDist + Math.Max(1, GetConfigValue<int>("FadeAttenuation"))),
                    ShowOnCompass = GetConfigValue<bool>("ShowOnCompass"),
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }
}
