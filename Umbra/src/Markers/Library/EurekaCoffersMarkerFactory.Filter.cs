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
using System.Text.RegularExpressions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Umbra.Common;

namespace Umbra.Markers.Library;

internal sealed partial class EurekaCoffersMarkerFactory
{
    private Dictionary<string, string> CarrotTexts { get; } = new() {
        {
            "en",
            @"You sense something\s*(?<dist>far(?:,\s*far)?)?(?:\s*immediately)?\s*to the (?<dir>\w+)\."
        }, {
            "de",
            @"Du spürst eine Schatztruhe\s*(?<dist>(sehr weit|weit|sehr nah))?\s*(?<dir>nördlich|nordöstlich|östlich|südöstlich|südlich|südwestlich|westlich|nordwestlich)\s*von dir!"
        },
    };

    private Dictionary<string, List<string>> DistanceTexts { get; } = new() {
        { "en", ["far", "far, far"] },
        { "de", ["weit", "sehr weit"] }
    };

    private Dictionary<string, Dictionary<string, string>> DirectionTexts { get; } = new() {
        {
            "en", new() {
                { "north", "north" },
                { "northeast", "northeast" },
                { "east", "east" },
                { "southeast", "southeast" },
                { "south", "south" },
                { "southwest", "southwest" },
                { "west", "west" },
                { "northwest", "northwest" }
            }
        }, {
            "de", new() {
                { "nördlich", "north" },
                { "nordöstlich", "northeast" },
                { "östlich", "east" },
                { "südöstlich", "southeast" },
                { "südlich", "south" },
                { "südwestlich", "southwest" },
                { "westlich", "west" },
                { "nordwestlich", "northwest" }
            }
        }
    };

    private void OnChatMessage(XivChatType type, uint senderId, SeString sender, SeString message)
    {
        string lang = I18N.GetCurrentLanguage();

        if (false == CarrotTexts.ContainsKey(lang)) {
            return;
        }

        string                     msg           = message.TextValue.Trim();
        string                     carrotPattern = CarrotTexts[lang];
        List<string>               distTexts     = DistanceTexts[lang];
        Dictionary<string, string> dirTexts      = DirectionTexts[lang];

        if (false == msg.StartsWith("You sense something ")) {
            return;
        }

        var result = Regex.Match(msg, carrotPattern, RegexOptions.IgnoreCase);

        if (!result.Success || dirTexts.ContainsKey(result.Groups["dir"].Value) == false) {
            return;
        }

        string direction = dirTexts[result.Groups["dir"].Value];
        string distanceT = result.Groups["dist"].Value;

        int minDistance;
        int maxDistance;

        if (distanceT == distTexts[0]) {
            minDistance = 100;
            maxDistance = 200;
        } else if (distanceT == distTexts[1]) {
            minDistance = 200;
            maxDistance = int.MaxValue;
        } else {
            minDistance = 0;
            maxDistance = 100;
        }

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

        AddMapMarkers();
    }
}
