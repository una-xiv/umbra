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

using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace Umbra.Game;

public class JobInfo(ClassJob cj)
{
    public byte   Id           { get; } = (byte)cj.RowId;
    public string Name         { get; } = Capitalize(cj.Name.ToString());
    public string Abbreviation { get; } = cj.Abbreviation.ToString().ToUpper();
    public short  Level        { get; set; }
    public byte   XpPercent    { get; set; }
    public bool   IsMaxLevel   { get; set; }

    public JobCategory Category { get; } = cj.ClassJobCategory.Row switch
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
}