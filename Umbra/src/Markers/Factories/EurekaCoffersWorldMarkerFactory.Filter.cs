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
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace Umbra.Markers;

internal sealed partial class EurekaCoffersWorldMarkerFactory
{
    private void OnChatMessage(XivChatType type, uint senderId, SeString sender, SeString message)
    {
        string msg = message.TextValue.Trim();

        if (false == msg.StartsWith("You sense something ")) {
            return;
        }

        var pattern = @"You sense something\s*(?<dist>far(?:,\s*far)?)?(?:\s*immediately)?\s*to the (?<dir>\w+)\.";
        var    result  = Regex.Match(msg, pattern, RegexOptions.IgnoreCase);

        if (!result.Success) {
            return;
        }

        string direction   = result.Groups["dir"].Value;
        string distanceTxt = result.Groups["dist"].Value;

        int minDistance = distanceTxt switch {
            "far"      => 100,
            "far, far" => 200,
            _          => 0
        };

        int maxDistance = distanceTxt switch {
            "far"      => 200,
            "far, far" => int.MaxValue,
            _          => 100
        };

        var playerPos       = _player.Position;
        var cofferPositions = CofferPositions[_zoneManager.CurrentZone.TerritoryId];

        var coffers = cofferPositions
            .Where(
                c => {
                    float distance = Vector3.Distance(playerPos, c);
                    return distance >= minDistance && distance <= maxDistance;
                }
            )
            .OrderBy(c => Vector3.Distance(playerPos, c));

        // Filter out coffers that are not in the given direction.
        if (direction.Equals("south", StringComparison.OrdinalIgnoreCase)) {
            _detectedCofferPositions = coffers
                .Where(c => c.Z > playerPos.Z && Math.Abs(c.X - playerPos.X) <= Math.Abs(c.Z - playerPos.Z))
                .ToList();
        } else if (direction.Equals("north", StringComparison.OrdinalIgnoreCase)) {
            _detectedCofferPositions = coffers
                .Where(c => c.Z < playerPos.Z && Math.Abs(c.X - playerPos.X) <= Math.Abs(c.Z - playerPos.Z))
                .ToList();
        } else if (direction.Equals("east", StringComparison.OrdinalIgnoreCase)) {
            _detectedCofferPositions = coffers
                .Where(c => c.X > playerPos.X && Math.Abs(c.X - playerPos.X) >= Math.Abs(c.Z - playerPos.Z))
                .ToList();
        } else if (direction.Equals("west", StringComparison.OrdinalIgnoreCase)) {
            _detectedCofferPositions = coffers
                .Where(c => c.X < playerPos.X && Math.Abs(c.X - playerPos.X) >= Math.Abs(c.Z - playerPos.Z))
                .ToList();
        } else if (direction.Equals("southeast", StringComparison.OrdinalIgnoreCase)) {
            _detectedCofferPositions = coffers.Where(c => c.Z >= playerPos.Z && c.X >= playerPos.X).ToList();
        } else if (direction.Equals("southwest", StringComparison.OrdinalIgnoreCase)) {
            _detectedCofferPositions = coffers.Where(c => c.Z >= playerPos.Z && c.X <= playerPos.X).ToList();
        } else if (direction.Equals("northeast", StringComparison.OrdinalIgnoreCase)) {
            _detectedCofferPositions = coffers.Where(c => c.Z <= playerPos.Z && c.X >= playerPos.X).ToList();
        } else if (direction.Equals("northwest", StringComparison.OrdinalIgnoreCase)) {
            _detectedCofferPositions = coffers.Where(c => c.Z <= playerPos.Z && c.X <= playerPos.X).ToList();
        }
    }
}
