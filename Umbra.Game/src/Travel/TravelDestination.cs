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

using System.Collections.Generic;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace Umbra.Game;

public class TravelDestination
{
    private const uint IconAetheryte = 60453u;
    private const uint IconApartment = 60789u;
    private const uint IconEstate    = 60751u;
    private const uint IconFcHouse   = 60753u;
    private const uint IconOther     = 60442u;

    public uint   Id             { get; private set; }
    public uint   SubId          { get; private set; }
    public uint   GilCost        { get; private set; }
    public string Name           { get; private set; } = string.Empty;
    public bool   IsHousing      { get; private set; }
    public byte   Plot           { get; private set; }
    public byte   Ward           { get; private set; }
    public bool   IsApartment    { get; private set; }
    public bool   IsSharedHouse  { get; private set; }
    public bool   IsFcHouse      { get; private set; }
    public bool   IsPrivateHouse { get; private set; }
    public uint?  IconId         { get; private set; }

    private static readonly Dictionary<uint, string> InterfaceTexts = [];
    private static readonly Dictionary<uint, string> TerritoryNames = [];

    internal TravelDestination(IAetheryteEntry entry, bool isEstate, bool isFree)
    {
        Update(entry, isEstate, isFree);
    }

    /// <summary>
    /// Updates this travel destination with the data from the given aetheryte
    /// entry.
    /// </summary>
    internal void Update(IAetheryteEntry entry, bool isHousing, bool isFree)
    {
        Id             = entry.AetheryteId;
        SubId          = entry.SubIndex;
        GilCost        = isFree ? 0 : entry.GilCost;
        Ward           = entry.Ward;
        Plot           = entry.Plot;
        IsHousing      = isHousing;
        IsApartment    = IsHousing && entry.IsApartment;
        IsSharedHouse  = IsHousing && entry.IsSharedHouse;
        IsFcHouse      = IsHousing && entry.AetheryteId is 56 or 57 or 58 or 96 or 164;
        IsPrivateHouse = IsHousing && !IsApartment && !IsSharedHouse && !IsFcHouse && entry.SubIndex > 0;
        Name           = GetDestinationName(entry);
        IconId         = GetIconId();
    }

    private string GetDestinationName(IAetheryteEntry entry)
    {
        if (!IsHousing) {
            return entry.AetheryteData.GameData!.PlaceName.Value!.Name.ToString();
        }

        // Apartment.
        if (IsApartment) {
            return $"{GetTerritoryName(entry.TerritoryId)} - {GetUiText(8518)}";
        }

        // Shared house.
        if (IsSharedHouse || entry is { Ward: > 0, Plot: > 0 }) {
            return
                $"{GetTerritoryName(entry.TerritoryId)} - {GetUiText(6351)} {entry.Ward}, {GetUiText(14312)} {entry.Plot}";
        }

        // Private house.
        if (IsPrivateHouse) {
            return $"{GetTerritoryName(entry.TerritoryId)} - {GetUiText(6538)}";
        }

        // Free company.
        if (IsFcHouse) {
            return $"{GetTerritoryName(entry.TerritoryId)} - {GetUiText(6875)}";
        }

        // Unknown (most likely also private estates)
        return $"{GetTerritoryName(entry.TerritoryId)} - {entry.AetheryteData.GameData!.PlaceName.Value!.Name}";
    }

    private uint? GetIconId()
    {
        if (IsApartment) return IconApartment;
        if (IsFcHouse) return IconFcHouse;
        if (IsHousing) return IconEstate;

        return (Id > 0 && SubId == 0) ? IconAetheryte : IconOther;
    }

    private static unsafe string GetUiText(uint id)
    {
        if (InterfaceTexts.TryGetValue(id, out string? cachedText)) {
            return cachedText;
        }

        RaptureTextModule* tm = UIModule.Instance()->GetRaptureTextModule();

        byte* sp = tm->GetAddonText(id);

        if (null == sp) {
            return "???";
        }

        return InterfaceTexts[id] = MemoryHelper.ReadSeStringNullTerminated(new(sp)).ToString();
    }

    private static string GetTerritoryName(uint territoryId)
    {
        if (TerritoryNames.TryGetValue(territoryId, out string? cachedName)) {
            return cachedName;
        }

        var territory = Common.Framework
            .Service<IDataManager>()
            .GetExcelSheet<Lumina.Excel.GeneratedSheets.TerritoryType>()!
            .GetRow(territoryId);

        if (null == territory) {
            return TerritoryNames[territoryId] = "???";
        }

        return TerritoryNames[territoryId] = territory.PlaceName.Value?.Name.ToString() ?? "???";
    }

    public override string ToString()
    {
        return $"TravelDestination<#{Id}>(\"{Name}\")";
    }

    public string GetCacheKey()
    {
        return $"{Id}-{SubId}-{Ward}-{Plot}-{IsApartment}-{IsSharedHouse}-{IsFcHouse}-{IsPrivateHouse}";
    }
}