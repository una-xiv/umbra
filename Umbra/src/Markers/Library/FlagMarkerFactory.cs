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
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.Library;

[Service]
internal class FlagMarkerFactory : WorldMarkerFactory
{
    public override string Id          { get; } = "FlagMarker";
    public override string Name        { get; } = I18N.Translate("Markers.Flag.Name");
    public override string Description { get; } = I18N.Translate("Markers.Flag.Description");

    /// <inheritdoc/>
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

    [OnTick(interval: 500)]
    private unsafe void OnUpdate()
    {
        var enabled = GetConfigValue<bool>("Enabled");

        AgentMap* agentMap = AgentMap.Instance();

        if (!enabled || agentMap is null || agentMap->IsFlagMarkerSet == 0) {
            RemoveAllMarkers();
            return;
        }

        var fadeDist = GetConfigValue<int>("FadeDistance");
        var key      = $"FlagMarker_{agentMap->FlagMapMarker.MapId}";

        SetMarker(
            new() {
                Key           = key,
                IconId        = agentMap->FlagMapMarker.MapMarker.IconId,
                MapId         = agentMap->FlagMapMarker.MapId,
                Position      = new(agentMap->FlagMapMarker.XFloat, 0, agentMap->FlagMapMarker.YFloat),
                FadeDistance  = new(fadeDist, fadeDist + Math.Max(1, GetConfigValue<int>("FadeAttenuation"))),
                ShowOnCompass = GetConfigValue<bool>("ShowOnCompass"),
            }
        );

        RemoveMarkersExcept([key]);
    }
}
