namespace Umbra.Game;

public class JobInfo(ClassJob cj)
{
    public byte   Id           { get; } = (byte)cj.RowId;
    public string Name         { get; } = Capitalize(cj.Name.ExtractText());
    public string Abbreviation { get; } = cj.Abbreviation.ExtractText().ToUpper();
    public short  Level        { get; set; }
    public byte   XpPercent    { get; set; }
    public bool   IsMaxLevel   { get; set; }

    public JobCategory Category { get; } = cj.ClassJobCategory.RowId switch
    {
        30 when cj.Role == 1 => JobCategory.Tank,
        30 when cj.Role == 2 => JobCategory.MeleeDps,
        30 when cj.Role == 3 => JobCategory.PhysicalRangedDps,
        31 when cj.Role == 3 => JobCategory.MagicalRangedDps,
        31                   => JobCategory.Healer,
        32                   => JobCategory.Gatherer,
        33                   => JobCategory.Crafter,
        _                    => JobCategory.None,
    };

    public Dictionary<JobIconType, uint> Icons { get; } = new()
    {
        { JobIconType.Default, 62000u + cj.RowId },
        { JobIconType.Framed, 62100u + cj.RowId },
        { JobIconType.Gearset, 62800u + cj.RowId },
        { JobIconType.Glowing, GetGlowingIconType(cj.RowId) },
        { JobIconType.Light, CrestIconConvert(cj.RowId,  0) },
        { JobIconType.Dark, CrestIconConvert(cj.RowId,   1) },
        { JobIconType.Gold, CrestIconConvert(cj.RowId,   2) },
        { JobIconType.Orange, CrestIconConvert(cj.RowId, 3) },
        { JobIconType.Red, CrestIconConvert(cj.RowId,    4) },
        { JobIconType.Purple, CrestIconConvert(cj.RowId, 5) },
        { JobIconType.Blue, CrestIconConvert(cj.RowId,   6) },
        { JobIconType.Green, CrestIconConvert(cj.RowId,  7) },
    };

    public string ColorName { get => Category switch
        {
            JobCategory.Tank              => "Role.Tank",
            JobCategory.Healer            => "Role.Healer",
            JobCategory.MeleeDps          => "Role.MeleeDps",
            JobCategory.PhysicalRangedDps => "Role.PhysicalRangedDps",
            JobCategory.MagicalRangedDps  => "Role.MagicalRangedDps",
            JobCategory.Crafter           => "Role.Crafter",
            JobCategory.Gatherer          => "Role.Gatherer",
            _                             => "Role.Unknown"
        };
    }

    private static string Capitalize(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Split the input string into words
        var words = input.Split(' ');

        // Capitalize the first letter of each word
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }
        }

        // Join the words back into a single string
        return string.Join(" ", words);
    }

    public uint GetIcon(JobIconType type) => Icons[type];

    public uint GetIcon(string type)
    {
        return type switch
        {
            "Default" => GetIcon(JobIconType.Default),
            "Framed"  => GetIcon(JobIconType.Framed),
            "Gearset" => GetIcon(JobIconType.Gearset),
            "Glowing" => GetIcon(JobIconType.Glowing),
            "Light"   => GetIcon(JobIconType.Light),
            "Dark"    => GetIcon(JobIconType.Dark),
            "Gold"    => GetIcon(JobIconType.Gold),
            "Orange"  => GetIcon(JobIconType.Orange),
            "Red"     => GetIcon(JobIconType.Red),
            "Purple"  => GetIcon(JobIconType.Purple),
            "Blue"    => GetIcon(JobIconType.Blue),
            "Green"   => GetIcon(JobIconType.Green),
            _         => GetIcon(JobIconType.Default)
        };
    }

    public int GetUldIcon(JobIconType _)
    {
        return GetPixelatedIcon(cj.RowId);
    }

    private static uint CrestIconConvert(uint jobId, ushort offset)
    {
        uint o = offset * 500u;

        return jobId switch
        {
            1  => o + 91022u, // Gladiator
            2  => o + 91023u, // Pugilist
            3  => o + 91024u, // Marauder
            4  => o + 91025u, // Lancer
            5  => o + 91026u, // Archer
            6  => o + 91028u, // Conjurer
            7  => o + 91029u, // Thaumaturge
            8  => o + 91031u, // Carpenter
            9  => o + 91032u, // Blacksmith
            10 => o + 91033u, // Armorer
            11 => o + 91034u, // Goldsmith
            12 => o + 91035u, // Leatherworker
            13 => o + 91036u, // Weaver
            14 => o + 91037u, // Alchemist
            15 => o + 91038u, // Culinarian
            16 => o + 91039u, // Miner
            17 => o + 91040u, // Botanist
            18 => o + 91041u, // Fisher
            19 => o + 91079u, // Paladin
            20 => o + 91080u, // Monk
            21 => o + 91081u, // Warrior
            22 => o + 91082u, // Dragoon
            23 => o + 91083u, // Bard
            24 => o + 91084u, // White Mage
            25 => o + 91085u, // Black Mage
            26 => o + 91030u, // Arcanist
            27 => o + 91086u, // Summoner
            28 => o + 91087u, // Scholar
            29 => o + 91121u, // Rogue
            30 => o + 91122u, // Ninja
            31 => o + 91125u, // Machinist
            32 => o + 91123u, // Dark Knight
            33 => o + 91124u, // Astrologian
            34 => o + 91127u, // Samurai
            35 => o + 91128u, // Red Mage
            36 => o + 91129u, // Blue Mage
            37 => o + 91130u, // Gunbreaker
            38 => o + 91131u, // Dancer
            39 => o + 91132u, // Reaper
            40 => o + 91133u, // Sage
            41 => o + 91185u, // Viper
            42 => o + 91186u, // Pictomancer
            _  => o + 91169u  // Unknown
        };
    }

    private static uint GetGlowingIconType(uint jobId)
    {
        return jobId switch
        {
            1  => 62301u, // Gladiator
            2  => 62302u, // Pugilist
            3  => 62303u, // Marauder
            4  => 62304u, // Lancer
            5  => 62305u, // Archer
            6  => 62306u, // Conjurer
            7  => 62307u, // Thaumaturge
            8  => 62502u, // Carpenter
            9  => 62503u, // Blacksmith
            10 => 62504u, // Armorer
            11 => 62505u, // Goldsmith
            12 => 62506u, // Leatherworker
            13 => 62507u, // Weaver
            14 => 62508u, // Alchemist
            15 => 62509u, // Culinarian
            16 => 62510u, // Miner
            17 => 62511u, // Botanist
            18 => 62512u, // Fisher
            19 => 62401u, // Paladin
            20 => 62402u, // Monk
            21 => 62403u, // Warrior
            22 => 62404u, // Dragoon
            23 => 62405u, // Bard
            24 => 62406u, // White Mage
            25 => 62407u, // Black Mage
            26 => 62308u, // Arcanist
            27 => 62408u, // Summoner
            28 => 62409u, // Scholar
            29 => 62309u, // Rogue
            30 => 62410u, // Ninja
            31 => 62411u, // Machinist
            32 => 62412u, // Dark Knight
            33 => 62413u, // Astrologian
            34 => 62414u, // Samurai
            35 => 62415u, // Red Mage
            36 => 62416u, // Blue Mage
            37 => 62417u, // Gunbreaker
            38 => 62418u, // Dancer
            39 => 62419u, // Reaper
            40 => 62420u, // Sage
            41 => 62421u, // Viper
            42 => 62422u, // Pictomancer
            _  => 62301u  // Unknown
        };
    }

    public static int GetPixelatedIcon(uint jobId)
    {
        return jobId switch
        {
            1  => 0, // Gladiator
            2  => 1, // Pugilist
            3  => 2, // Marauder
            4  => 3, // Lancer
            5  => 4, // Archer
            6  => 6, // Conjurer
            7  => 7, // Thaumaturge
            8  => 0, // Carpenter
            9  => 1, // Blacksmith
            10 => 2, // Armorer
            11 => 3, // Goldsmith
            12 => 4, // Leatherworker
            13 => 5, // Weaver
            14 => 6, // Alchemist
            15 => 7, // Culinarian
            16 => 8, // Miner
            17 => 9, // Botanist
            18 => 10, // Fisher
            19 => 0, // Paladin
            20 => 1, // Monk
            21 => 2, // Warrior
            22 => 3, // Dragoon
            23 => 5, // Bard
            24 => 6, // White Mage
            25 => 7, // Black Mage
            26 => 8, // Arcanist
            27 => 9, // Summoner
            28 => 10, // Scholar
            29 => 11, // Rogue
            30 => 11, // Ninja
            31 => 12, // Machinist
            32 => 13, // Dark Knight
            33 => 14, // Astrologian
            34 => 16, // Samurai
            35 => 17, // Red Mage
            36 => 18, // Blue Mage
            37 => 19, // Gunbreaker
            38 => 20, // Dancer
            39 => 21, // Reaper
            40 => 22, // Sage
            41 => 23, // Viper
            42 => 24, // Pictomancer
            _  => 0  // Unknown
        };
    }
}
