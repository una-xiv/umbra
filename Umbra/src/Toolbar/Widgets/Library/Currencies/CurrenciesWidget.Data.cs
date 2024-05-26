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
using System.Globalization;
using System.Linq;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

public partial class CurrenciesWidget
{
    private static readonly Dictionary<CurrencyType, Currency> Currencies = [];
    private static readonly Dictionary<uint, int>              GcSealsCap = [];

    private static IDataManager DataManager { get; set; } = null!;
    private static IPlayer      Player      { get; set; } = null!;

    /// <summary>
    /// Precaches the data for the widget. This can be done statically since
    /// the data never changes during runtime.
    /// </summary>
    private static unsafe void Precache()
    {
        DataManager = Framework.Service<IDataManager>();
        Player      = Framework.Service<IPlayer>();

        foreach (var row in DataManager.GetExcelSheet<GrandCompanyRank>()!) {
            GcSealsCap[row.RowId] = (int)row.MaxSeals;
        }

        RegisterCurrency(CurrencyType.Gil,                  0); // Gil
        RegisterCurrency(CurrencyType.Mgp,                  0); // MGP
        RegisterCurrency(CurrencyType.Ventures,             0); // Venture
        RegisterCurrency(CurrencyType.Maelstrom,            0); // Maelstrom GC
        RegisterCurrency(CurrencyType.TwinAdder,            0); // Twin Adder GC
        RegisterCurrency(CurrencyType.ImmortalFlames,       0); // Immortal Flames GC
        RegisterCurrency(CurrencyType.AlliedSeals,          1); // Allied Seals
        RegisterCurrency(CurrencyType.CenturioSeals,        1); // Centurio Seals
        RegisterCurrency(CurrencyType.SackOfNuts,           1); // Sack of Nuts
        RegisterCurrency(CurrencyType.LimitedTomestone,     2);
        RegisterCurrency(CurrencyType.NonLimitedTomestone,  2);
        RegisterCurrency(CurrencyType.Poetics,              2); // Poetics
        RegisterCurrency(CurrencyType.WolfMarks,            3); // Wolf Marks
        RegisterCurrency(CurrencyType.TrophyCrystals,       3); // Trophy Crystals
        RegisterCurrency(CurrencyType.PurpleCrafterScrips,  4); // Purple Crafter Scrips
        RegisterCurrency(CurrencyType.PurpleGathererScrips, 4); // Purple Gatherer Scrips
        RegisterCurrency(CurrencyType.WhiteCrafterScrips,   4); // White Crafter Scrips
        RegisterCurrency(CurrencyType.WhiteGathererScrips,  4); // White Gatherer Scrips
        RegisterCurrency(CurrencyType.SkyBuildersScrips,    4); // Sky-builders Scrips
        RegisterCurrency(CurrencyType.BiColorGemstones,     5); // Bicolor Gemstones
    }

    private static void RegisterCurrency(CurrencyType type, uint groupId, bool showCap = true)
    {
        uint id = type switch {
            CurrencyType.LimitedTomestone    => GetLimitedTomestoneItem().Item.Row,
            CurrencyType.NonLimitedTomestone => GetNonLimitedTomestoneItem().Item.Row,
            _                                => (uint)type
        };

        Item? item = DataManager.GetExcelSheet<Item>()!.GetRow(id);

        if (item == null) {
            Logger.Warning($"Attempted to precache an unknown item #{item} in currency widget.");
            return;
        }

        Currencies[type] = new() {
            Type    = type,
            Id      = id,
            Name    = item.Name.ToDalamudString().TextValue,
            GroupId = groupId,
            Icon    = item.Icon,
            Cap     = showCap ? item.StackSize : 0,
        };
    }

    private static unsafe string GetAmount(CurrencyType type)
    {
        switch (type) {
            case CurrencyType.Gil:
            case CurrencyType.Mgp:
            case CurrencyType.Ventures:
                return $"{FormatNumber(Player.GetItemCount(Currencies[type].Id), '.')}";
            case CurrencyType.Maelstrom:
            case CurrencyType.TwinAdder:
            case CurrencyType.ImmortalFlames: {
                if (Player.GrandCompanyId == 0) return "";

                string cap = FormatNumber(GcSealsCap[PlayerState.Instance()->GetGrandCompanyRank()], '.');
                string amt = FormatNumber(Player.GetItemCount(Currencies[type].Id),                  '.');

                return $"{amt} / {cap}";
            }
            case CurrencyType.LimitedTomestone: {
                string cap = FormatNumber((int)Currencies[type].Cap,                '.');
                string amt = FormatNumber(Player.GetItemCount(Currencies[type].Id), '.');

                int weeklyLimit = InventoryManager.GetLimitedTomestoneWeeklyLimit();
                int weeklyCount = InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount();

                return $"{amt} / {cap} ({weeklyCount} / {weeklyLimit})";
            }
            default: {
                uint cap = Currencies[type].Cap;
                int  amt = Player.GetItemCount(Currencies[type].Id);
                return cap > 0 ? $"{FormatNumber(amt, ',')} / {FormatNumber((int)cap, '.')}" : FormatNumber(amt, '.');
            }
        }
    }

    /// <summary>
    /// Returns the tomestone currency item that has a weekly limit.
    /// </summary>
    private static TomestonesItem GetLimitedTomestoneItem()
    {
        return DataManager.GetExcelSheet<TomestonesItem>()!
            .First(tomestone => tomestone.Tomestones.Row is 3);
    }

    /// <summary>
    /// Returns the tomestone currency item that has no weekly limit.
    /// </summary>
    private static TomestonesItem GetNonLimitedTomestoneItem()
    {
        return DataManager.GetExcelSheet<TomestonesItem>()!
            .First(tomestone => tomestone.Tomestones.Row is 2);
    }

    private enum CurrencyType : int
    {
        Gil                  = 1,
        Mgp                  = 29,
        Ventures             = 21072,
        Maelstrom            = 20,
        TwinAdder            = 21,
        ImmortalFlames       = 22,
        AlliedSeals          = 27,
        CenturioSeals        = 10307,
        SackOfNuts           = 26533,
        LimitedTomestone     = -1,
        NonLimitedTomestone  = -2,
        Poetics              = 28,
        WolfMarks            = 25,
        TrophyCrystals       = 36656,
        PurpleCrafterScrips  = 33913,
        PurpleGathererScrips = 33914,
        WhiteCrafterScrips   = 25199,
        WhiteGathererScrips  = 25200,
        SkyBuildersScrips    = 28063,
        BiColorGemstones     = 26807
    }

    public static string FormatNumber(int value, char separator)
    {
        // Create a custom NumberFormatInfo instance
        NumberFormatInfo nfi = new() {
            NumberGroupSeparator = separator.ToString(),
            NumberGroupSizes     = [3]
        };

        return value.ToString("N0", nfi);
    }

    private record Currency
    {
        public CurrencyType Type    { get; init; }
        public uint         Id      { get; init; }
        public string       Name    { get; init; } = string.Empty;
        public uint         GroupId { get; init; }
        public uint         Icon    { get; init; }
        public uint         Cap     { get; init; }
    }
}
