using System.Numerics;
using System.Text.RegularExpressions;

namespace Umbra.Game;

public static class ChatLocationFilter
{
    /// <summary>
    /// Filters the given list of positions based on the provided chat message.
    /// This is typically used for Eureka Bunny Coffers and Occult Pots, where
    /// the chat message received from using the carrot or flask indicates in
    /// which direction the target is located. These messages look like "You
    /// sense something far to the north".
    /// </summary>
    /// <param name="message">The message to parse.</param>
    /// <param name="playerPosition">The position of the player character.</param>
    /// <param name="positions">The full list of possible locations.</param>
    /// <returns>The filtered list of possible locations.</returns>
    public static List<Vector3>? FilterPositions(string message, Vector3 playerPosition, List<Vector3> positions)
    {
        string lang = I18N.GetCurrentLanguage();

        if (false == SenseTexts.ContainsKey(lang)) {
            return null;
        }

        string msg = message.Trim();

        if (!msg.StartsWith(GuardTexts[lang], StringComparison.OrdinalIgnoreCase)) {
            return null;
        }

        string                     carrotPattern = SenseTexts[lang];
        List<string>               distTexts     = DistanceTexts[lang];
        Dictionary<string, string> dirTexts      = DirectionTexts[lang];

        var result = Regex.Match(msg, carrotPattern, RegexOptions.IgnoreCase);

        if (!result.Success || !dirTexts.TryGetValue(result.Groups["dir"].Value, out var direction)) {
            return null;
        }

        string distText = result.Groups["dist"].Value;

        int minDistance;
        int maxDistance;

        if (distText == distTexts[0]) {
            minDistance = 100;
            maxDistance = 200;
        } else if (distText == distTexts[1]) {
            minDistance = 200;
            maxDistance = int.MaxValue;
        } else {
            minDistance = 0;
            maxDistance = 100;
        }

        var playerPos = playerPosition;

        var coffers = positions
                     .Where(c => {
                              float distance = Vector3.Distance(playerPos, c);
                              return distance >= minDistance && distance <= maxDistance;
                          }
                      )
                     .OrderBy(c => Vector3.Distance(playerPos, c));

        List<Vector3> filteredPositions = [];
        
        // Filter out coffers that are not in the given direction.
        if (direction.Equals("south", StringComparison.OrdinalIgnoreCase)) {
            filteredPositions = coffers
                               .Where(c => c.Z > playerPos.Z && Math.Abs(c.X - playerPos.X) <= Math.Abs(c.Z - playerPos.Z))
                               .ToList();
        } else if (direction.Equals("north", StringComparison.OrdinalIgnoreCase)) {
            filteredPositions = coffers
                               .Where(c => c.Z < playerPos.Z && Math.Abs(c.X - playerPos.X) <= Math.Abs(c.Z - playerPos.Z))
                               .ToList();
        } else if (direction.Equals("east", StringComparison.OrdinalIgnoreCase)) {
            filteredPositions = coffers
                               .Where(c => c.X > playerPos.X && Math.Abs(c.X - playerPos.X) >= Math.Abs(c.Z - playerPos.Z))
                               .ToList();
        } else if (direction.Equals("west", StringComparison.OrdinalIgnoreCase)) {
            filteredPositions = coffers
                               .Where(c => c.X < playerPos.X && Math.Abs(c.X - playerPos.X) >= Math.Abs(c.Z - playerPos.Z))
                               .ToList();
        } else if (direction.Equals("southeast", StringComparison.OrdinalIgnoreCase)) {
            filteredPositions = coffers.Where(c => c.Z >= playerPos.Z && c.X >= playerPos.X).ToList();
        } else if (direction.Equals("southwest", StringComparison.OrdinalIgnoreCase)) {
            filteredPositions = coffers.Where(c => c.Z >= playerPos.Z && c.X <= playerPos.X).ToList();
        } else if (direction.Equals("northeast", StringComparison.OrdinalIgnoreCase)) {
            filteredPositions = coffers.Where(c => c.Z <= playerPos.Z && c.X >= playerPos.X).ToList();
        } else if (direction.Equals("northwest", StringComparison.OrdinalIgnoreCase)) {
            filteredPositions = coffers.Where(c => c.Z <= playerPos.Z && c.X <= playerPos.X).ToList();
        }

        return filteredPositions;
    }

    private static Dictionary<string, string> GuardTexts { get; } = new() {
        { "en", "You sense something" },
        { "de", "Du spürst eine Schatztruhe" },
        { "fr", "Le trésor est" },
        { "ja", "財宝の気配を" }
    };

    private static Dictionary<string, string> SenseTexts { get; } = new() {
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

    private static Dictionary<string, List<string>> DistanceTexts { get; } = new() {
        { "en", ["far", "far, far"] },
        { "de", ["weit", "sehr weit"] },
        { "fr", ["assez loin", "très loin"] },
        { "ja", ["遠く", "とても遠く"] }
    };

    private static Dictionary<string, Dictionary<string, string>> DirectionTexts { get; } = new() {
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
}
