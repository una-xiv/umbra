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
    private Dictionary<string, string> GuardTexts { get; } = new() {
        { "en", "You sense something" },
        { "de", "Du spürst eine Schatztruhe" },
        { "fr", "Le trésor est" },
        { "ja", "財宝の気配を" }
    };

    private Dictionary<string, string> CarrotTexts { get; } = new() {
        {
            "en",
            @"You sense something\s*(?<dist>far(?:,\s*far)?)?(?:\s*immediately)?\s*to the (?<dir>\w+)\."
        }, {
            "de",
            @"Du spürst eine Schatztruhe\s*(?<dist>(sehr weit|weit|sehr nah))?\s*(?<dir>nördlich|nordöstlich|östlich|südöstlich|südlich|südwestlich|westlich|nordwestlich)\s*von dir!"
        }, {
            "fr",
            @"Le trésor est\s*(?<dist>très loin|assez loin|non loin|tout près)\s*d'ici,\s*(?<dir>.+)!"
        }, {
            "ja",
            @"財宝の気配を、(?<dir>北|北東|東|南東|南|南西|西|北西)方向の\s*(?<dist>(とても遠く|遠く))?から感じているようだ(。|……|！)?"
        }
    };

    private Dictionary<string, List<string>> DistanceTexts { get; } = new() {
        { "en", ["far", "far, far"] },
        { "de", ["weit", "sehr weit"] },
        { "fr", ["assez loin", "très loin"] },
        { "ja", ["遠く", "とても遠く"] }
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
        }, {
            "fr", new() {
                { "au nord", "north" },
                { "au nord-est", "northeast" },
                { "à l'est", "east" },
                { "au sud-est", "southeast" },
                { "au sud", "south" },
                { "au sud-ouest", "southwest" },
                { "à l'ouest", "west" },
                { "au nord-ouest", "northwest" }
            }
        }, {
            "ja", new() {
                { "北", "north" },
                { "北東", "northeast" },
                { "東", "east" },
                { "南東", "southeast" },
                { "南", "south" },
                { "南西", "southwest" },
                { "西", "west" },
                { "北西", "northwest" }
            }
        }
    };

    private void OnChatMessage(
        XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled
    )
    {
        string lang = I18N.GetCurrentLanguage();

        if (false == CarrotTexts.ContainsKey(lang)) {
            return;
        }

        string msg = message.TextValue.Trim();

        if (!msg.StartsWith(GuardTexts[lang], StringComparison.OrdinalIgnoreCase)) {
            return;
        }

        string                     carrotPattern = CarrotTexts[lang];
        List<string>               distTexts     = DistanceTexts[lang];
        Dictionary<string, string> dirTexts      = DirectionTexts[lang];

        var result = Regex.Match(msg, carrotPattern, RegexOptions.IgnoreCase);

        if (!result.Success) {
            return;
        }

        if (dirTexts.ContainsKey(result.Groups["dir"].Value) == false) {
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
