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

using System.Collections.Generic;
using Umbra.Markers;
using Dalamud.Plugin.Services;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using Dalamud.Game.ClientState.Objects.SubKinds;
using System.Numerics;
using Dalamud.Game.ClientState.Objects;

namespace Umbra.Common;

[Service]
internal sealed class UnsavouryPlayerMarkerFactory(IObjectTable objectTable, ITargetManager targetManager)
    : IWorldMarkerFactory
{
    [ConfigVariable(
        "Markers.UnsavouryPlayers.Enabled",
        "Enabled Markers",
        "Show players in places where they shouldn't be.",
        "Shows targetable world markers of players that are underground or under the terrain. This can be useful for detecting players that are using cheats to hide from view or to avoid being targeted by other players."
    )]
    private static bool Enabled { get; set; } = false;

    private readonly List<DetectedPlayer> _detectedPlayers = [];

    public List<WorldMarker> GetMarkers()
    {
        if (false == Enabled) return [];

        lock (_detectedPlayers) {
            return _detectedPlayers
                .Select(
                    (pc) => new WorldMarker {
                        IconId          = 60074,
                        Position        = pc.Position,
                        Label           = pc.Name.ToString(),
                        SubLabel        = null,
                        MinFadeDistance = 2,
                        MaxFadeDistance = 3,
                        ShowDirection   = true,
                        OnClick         = () => targetManager.Target = objectTable.FirstOrDefault(p => p.ObjectId == pc.Id)
                    }
                )
                .ToList();
        }
    }

    [OnTick(interval: 23)]
    public void OnTick()
    {
        if (false == Enabled) return;

        lock (_detectedPlayers) {
            _detectedPlayers.Clear();

            objectTable
                .ToList()
                .ForEach(
                    (obj) => {
                        if (!obj.IsValid()
                         || obj.ObjectKind != ObjectKind.Player)
                            return;

                        if (obj is not PlayerCharacter pc) return;

                        var pos = new Vector3(pc.Position.X, pc.Position.Y + 1.2f, pc.Position.Z);

                        if (false == BGCollisionModule.Raycast(pos, new(0, -1, 0), out _)) {
                            // Player is underground or under the terrain.
                            _detectedPlayers.Add(new(pc));
                        }
                    }
                );
        }
    }

    private readonly struct DetectedPlayer(PlayerCharacter pc)
    {
        public uint    Id       { get; } = pc.ObjectId;
        public string  Name     { get; } = pc.Name.ToString();
        public Vector3 Position { get; } = pc.Position;
    }
}
