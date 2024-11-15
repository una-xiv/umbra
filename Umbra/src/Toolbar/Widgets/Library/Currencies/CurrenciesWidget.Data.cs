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
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

internal partial class CurrenciesWidget
{
    private static readonly Dictionary<CurrencyType, Currency> Currencies = [];
    private static readonly Dictionary<uint, int>              GcSealsCap = [];

    private static IDataManager DataManager { get; set; } = null!;
    private static IPlayer      Player      { get; set; } = null!;

    private Dictionary<uint, Currency> CustomCurrencies { get; } = [];
    private string                     _lastParsedCustomCurrencyIds = "";

    /// <summary>
    /// Precaches the data for the widget. This can be done statically since
    /// the data never changes during runtime.
    /// </summary>
    private static void Precache()
    {
        DataManager = Framework.Service<IDataManager>();
        Player      = Framework.Service<IPlayer>();

        foreach (var row in DataManager.GetExcelSheet<GrandCompanyRank>()) {
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
        RegisterCurrency(CurrencyType.OrangeCrafterScrips,  4); // Orange Crafter Scrips
        RegisterCurrency(CurrencyType.OrangeGathererScrips, 4); // Orange Gatherer Scrips
        RegisterCurrency(CurrencyType.SkyBuildersScrips,    4); // Sky-builders Scrips
        RegisterCurrency(CurrencyType.BiColorGemstones,     5); // Bicolor Gemstones
    }

    private static void RegisterCurrency(CurrencyType type, uint groupId, bool showCap = true)
    {
        uint id = type switch {
            CurrencyType.LimitedTomestone    => GetLimitedTomestoneItem()?.Item.RowId ?? 0,
            CurrencyType.NonLimitedTomestone => GetNonLimitedTomestoneItem()?.Item.RowId ?? 0,
            _                                => (uint)type
        };

        if (id == 0) return;

        Item? item = DataManager.GetExcelSheet<Item>().FindRow(id);

        if (item == null) {
            Logger.Warning($"Attempted to precache an unknown item #{item} in currency widget.");
            return;
        }

        Currencies[type] = new() {
            Type    = type,
            Id      = id,
            Name    = item.Value.Name.ToDalamudString().TextValue,
            GroupId = groupId,
            Icon    = item.Value.Icon,
            Cap     = showCap ? item.Value.StackSize : 0,
        };
    }

    private string GetCustomAmount(uint id, bool showCap)
    {
        if (false == CustomCurrencies.ContainsKey(id)) return "";

        return CustomCurrencies[id].Cap > 1 && showCap
            ? $"{I18N.FormatNumber(Player.GetItemCount(id))} / {I18N.FormatNumber((int)CustomCurrencies[id].Cap)}"
            : $"{I18N.FormatNumber(Player.GetItemCount(id))}";
    }

    private unsafe string GetAmount(CurrencyType type, bool showCap)
    {
        switch (type) {
            case CurrencyType.Gil:
            case CurrencyType.Mgp:
            case CurrencyType.Ventures:
                return $"{I18N.FormatNumber(Player.GetItemCount(Currencies[type].Id))}";
            case CurrencyType.Maelstrom:
            case CurrencyType.TwinAdder:
            case CurrencyType.ImmortalFlames: {
                if (Player.GrandCompanyId == 0) return "";

                string cap = I18N.FormatNumber(GcSealsCap[PlayerState.Instance()->GetGrandCompanyRank()]);
                string amt = I18N.FormatNumber(Player.GetItemCount(Currencies[type].Id));

                return showCap ? $"{amt} / {cap}" : $"{amt}";
            }
            case CurrencyType.LimitedTomestone: {
                string cap = I18N.FormatNumber((int)Currencies[type].Cap);
                string amt = I18N.FormatNumber(Player.GetItemCount(Currencies[type].Id));

                int weeklyLimit = InventoryManager.GetLimitedTomestoneWeeklyLimit();
                int weeklyCount = InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount();

                return showCap ? $"{amt} / {cap} ({weeklyCount} / {weeklyLimit})" : $"{amt}";
            }
            default: {
                uint cap = Currencies[type].Cap;
                int  amt = Player.GetItemCount(Currencies[type].Id);

                return showCap && cap > 0 ? $"{I18N.FormatNumber(amt)} / {I18N.FormatNumber((int)cap)}" : I18N.FormatNumber(amt);
            }
        }
    }

    /// <summary>
    /// Returns the tomestone currency item that has a weekly limit.
    /// </summary>
    private static TomestonesItem? GetLimitedTomestoneItem()
    {
        return DataManager.GetExcelSheet<TomestonesItem>()
            .FirstOrDefault(tomestone => tomestone.Tomestones.RowId is 3);
    }

    /// <summary>
    /// Returns the tomestone currency item that has no weekly limit.
    /// </summary>
    private static TomestonesItem? GetNonLimitedTomestoneItem()
    {
        return DataManager.GetExcelSheet<TomestonesItem>()
            .FirstOrDefault(tomestone => tomestone.Tomestones.RowId is 2);
    }

    private enum CurrencyType
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
        //WhiteCrafterScrips   = 25199, // (Discontinued)
        //WhiteGathererScrips  = 25200, // (Discontinued)
        OrangeCrafterScrips  = 41784,
        OrangeGathererScrips = 41785,
        SkyBuildersScrips    = 28063,
        BiColorGemstones     = 26807,
        Custom               = -3,
    }

    private void UpdateCustomIdList()
    {
        if (CustomCurrencies.Count > 0) {
            foreach (uint id in CustomCurrencies.Keys) {
                if (Popup.HasButton($"CustomCurrency_{id}")) {
                    Popup.SetButtonAltLabel($"CustomCurrency_{id}", GetCustomAmount(id, GetConfigValue<bool>("ShowCap")));
                }
            }
        }

        var idList = GetConfigValue<string>("CustomCurrencyIds");
        if (idList == _lastParsedCustomCurrencyIds) return;

        _lastParsedCustomCurrencyIds = idList;

        foreach (uint id in CustomCurrencies.Keys) {
            Popup.RemoveButton($"CustomCurrency_{id}");
        }

        CustomCurrencies.Clear();
        foreach (string id in idList.Split(',')) {
            if (uint.TryParse(id.Trim(), out uint currencyId)) {
                var item = DataManager.GetExcelSheet<Item>().FindRow(currencyId);
                if (item is null || string.IsNullOrEmpty(item.Value.Name.ToDalamudString().TextValue)) continue;

                // Test if the given id is amongst the values of CurrencyType enum.
                if (Enum.IsDefined(typeof(CurrencyType), (int)currencyId)) {
                    Logger.Warning($"Custom currency id {currencyId} is already defined in CurrencyType enum.");
                    continue;
                }


                CustomCurrencies.Add(
                    currencyId,
                    new() {
                        Type    = CurrencyType.Custom,
                        Id      = currencyId,
                        Name    = item.Value.Name.ToDalamudString().TextValue,
                        Icon    = item.Value.Icon,
                        Cap     = item.Value.StackSize,
                        GroupId = 5
                    }
                );
            }
        }

        SyncTrackedCurrencyOptions();
        HydrateCustomCurrencies();
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

    private void SyncTrackedCurrencyOptions()
    {
        Dictionary<string, string> choices = [];

        foreach (var currency in Currencies.Values) {
            choices[currency.Type.ToString()] = currency.Name;
        }

        foreach (uint id in CustomCurrencies.Keys) {
            choices[id.ToString()] = CustomCurrencies[id].Name;
        }

        SelectWidgetConfigVariable selector = (SelectWidgetConfigVariable)GetConfigVariables().Where(c => c.Id == "TrackedCurrency").First();
        selector.Options = choices;
    }
}
