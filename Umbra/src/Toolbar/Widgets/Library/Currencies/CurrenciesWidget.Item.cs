using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets;

internal sealed partial class CurrenciesWidget
{
    private                 Dictionary<uint, Currency> DefaultCurrencies { get; } = [];
    private static readonly Dictionary<uint, int>      GcSealsCap = [];

    private const uint ItemIdGil                  = 1;
    private const uint ItemIdMgp                  = 29;
    private const uint ItemIdVentures             = 21072;
    private const uint ItemIdMaelstrom            = 20;
    private const uint ItemIdTwinAdder            = 21;
    private const uint ItemIdImmortalFlames       = 22;
    private const uint ItemIdAlliedSeals          = 27;
    private const uint ItemIdCenturioSeals        = 10307;
    private const uint ItemIdSackOfNuts           = 26533;
    private const uint ItemIdPoetics              = 28;
    private const uint ItemIdWolfMarks            = 25;
    private const uint ItemIdTrophyCrystals       = 36656;
    private const uint ItemIdPurpleCrafterScrips  = 33913;
    private const uint ItemIdPurpleGathererScrips = 33914;
    private const uint ItemIdOrangeCrafterScrips  = 41784;
    private const uint ItemIdOrangeGathererScrips = 41785;
    private const uint ItemIdSkyBuildersScrips    = 28063;
    private const uint ItemIdBiColorGemstones     = 26807;

    private void InitializeDefaultItems()
    {
        foreach (var row in DataManager.GetExcelSheet<GrandCompanyRank>()) {
            GcSealsCap[row.RowId] = (int)row.MaxSeals;
        }
        
        DefaultCurrencies.Clear();
        DefaultCurrencies.Add(ItemIdGil, CreateCurrencyFromItemId(ItemIdGil)!);
        DefaultCurrencies.Add(ItemIdMgp, CreateCurrencyFromItemId(ItemIdMgp)!);
        DefaultCurrencies.Add(ItemIdVentures, CreateCurrencyFromItemId(ItemIdVentures)!);
        DefaultCurrencies.Add(ItemIdMaelstrom, CreateCurrencyFromItemId(ItemIdMaelstrom)!);
        DefaultCurrencies.Add(ItemIdTwinAdder, CreateCurrencyFromItemId(ItemIdTwinAdder)!);
        DefaultCurrencies.Add(ItemIdImmortalFlames, CreateCurrencyFromItemId(ItemIdImmortalFlames)!);
        DefaultCurrencies.Add(ItemIdAlliedSeals, CreateCurrencyFromItemId(ItemIdAlliedSeals, Group.TheHunt)!);
        DefaultCurrencies.Add(ItemIdCenturioSeals, CreateCurrencyFromItemId(ItemIdCenturioSeals, Group.TheHunt)!);
        DefaultCurrencies.Add(ItemIdSackOfNuts, CreateCurrencyFromItemId(ItemIdSackOfNuts, Group.TheHunt)!);

        var limitedTomestone    = GetLimitedTomestoneItem();
        var nonLimitedTomestone = GetNonLimitedTomestoneItem();
        if (limitedTomestone != null) DefaultCurrencies.Add(limitedTomestone.Value.Item.RowId, CreateCurrencyFromTomestone(limitedTomestone.Value, Group.Tomestones)!);
        if (nonLimitedTomestone != null) DefaultCurrencies.Add(nonLimitedTomestone.Value.Item.RowId, CreateCurrencyFromTomestone(nonLimitedTomestone.Value, Group.Tomestones)!);

        DefaultCurrencies.Add(ItemIdPoetics, CreateCurrencyFromItemId(ItemIdPoetics, Group.Tomestones)!);
        DefaultCurrencies.Add(ItemIdWolfMarks, CreateCurrencyFromItemId(ItemIdWolfMarks, Group.PvP)!);
        DefaultCurrencies.Add(ItemIdTrophyCrystals, CreateCurrencyFromItemId(ItemIdTrophyCrystals, Group.PvP)!);
        DefaultCurrencies.Add(ItemIdPurpleCrafterScrips, CreateCurrencyFromItemId(ItemIdPurpleCrafterScrips, Group.CraftingAndGathering)!);
        DefaultCurrencies.Add(ItemIdPurpleGathererScrips, CreateCurrencyFromItemId(ItemIdPurpleGathererScrips, Group.CraftingAndGathering)!);
        DefaultCurrencies.Add(ItemIdOrangeCrafterScrips, CreateCurrencyFromItemId(ItemIdOrangeCrafterScrips, Group.CraftingAndGathering)!);
        DefaultCurrencies.Add(ItemIdOrangeGathererScrips, CreateCurrencyFromItemId(ItemIdOrangeGathererScrips, Group.CraftingAndGathering)!);
        DefaultCurrencies.Add(ItemIdSkyBuildersScrips, CreateCurrencyFromItemId(ItemIdSkyBuildersScrips, Group.CraftingAndGathering)!);
        DefaultCurrencies.Add(ItemIdBiColorGemstones, CreateCurrencyFromItemId(ItemIdBiColorGemstones, Group.Miscellaneous)!);
    }

    private record Currency
    {
        public uint   Id             { get; set; }
        public string Name           { get; set; } = string.Empty;
        public uint   IconId         { get; set; }
        public int    Count          { get; set; }
        public uint   Capacity       { get; set; }
        public int    WeeklyCapacity { get; set; }
        public int    WeeklyCount    { get; set; }
        public bool   IsTracked      { get; set; }
        public Group  Group          { get; set; }
    }

    private unsafe void UpdateCurrency(Currency currency)
    {
        var item = DataManager.GetExcelSheet<Item>().FindRow(currency.Id);
        if (item == null) return;

        currency.Count = Player.GetItemCount(currency.Id);

        if (IsLimitedTomestone(currency)) {
            currency.WeeklyCapacity = InventoryManager.GetLimitedTomestoneWeeklyLimit();
            currency.WeeklyCount    = InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount();
        }
    }

    private unsafe Currency? CreateCurrencyFromItemId(uint itemId, Group group = Group.None)
    {
        var item = DataManager.GetExcelSheet<Item>().FindRow(itemId);
        if (item == null) return null;

        uint stackSize = itemId is ItemIdMaelstrom or ItemIdTwinAdder or ItemIdImmortalFlames
            ? ((Player.GrandCompanyId == 0) ? 0 : (uint)GcSealsCap[PlayerState.Instance()->GetGrandCompanyRank()])
            : (itemId > 1 ? item.Value.StackSize : 0);
        
        if (itemId is ItemIdMaelstrom or ItemIdTwinAdder or ItemIdImmortalFlames) {
            stackSize = (uint)GcSealsCap[PlayerState.Instance()->GetGrandCompanyRank()];
        }

        Currency currency = new() {
            Id             = item.Value.RowId,
            Name           = item.Value.Name.ToString(),
            IconId         = item.Value.Icon,
            Count          = Player.GetItemCount(item.Value.RowId),
            Capacity       = stackSize,
            WeeklyCapacity = 0,
            WeeklyCount    = 0,
            IsTracked      = false,
            Group          = group
        };

        return currency;
    }

    private unsafe Currency? CreateCurrencyFromTomestone(TomestonesItem tomestonesItem, Group group = Group.None)
    {
        var currency = CreateCurrencyFromItemId(tomestonesItem.Item.RowId, group);
        if (currency == null) return null;

        if (IsLimitedTomestone(currency)) {
            currency.WeeklyCapacity = InventoryManager.GetLimitedTomestoneWeeklyLimit();
            currency.WeeklyCount    = InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount();
        }

        return currency;
    }

    /// <summary>
    /// Returns the tomestone currency item that has a weekly limit.
    /// </summary>
    private TomestonesItem? GetLimitedTomestoneItem()
    {
        return DataManager.GetExcelSheet<TomestonesItem>()
                          .FirstOrDefault(tomestone => tomestone.Tomestones.RowId is 3);
    }

    /// <summary>
    /// Returns the tomestone currency item that has no weekly limit.
    /// </summary>
    private TomestonesItem? GetNonLimitedTomestoneItem()
    {
        return DataManager.GetExcelSheet<TomestonesItem>()
                          .FirstOrDefault(tomestone => tomestone.Tomestones.RowId is 2);
    }

    private bool IsLimitedTomestone(Currency currency)
    {
        return currency.Id == GetLimitedTomestoneItem()?.Item.RowId;
    }
}
