﻿/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
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

using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;

namespace Umbra.Game.Societies;

[Service]
internal sealed class SocietiesRepository : ISocietiesRepository, IDisposable
{
    public Dictionary<uint, Society> Societies       { get; } = [];
    public uint                      WeeklyAllowance { get; private set; }

    internal static Dictionary<uint, uint> SocietyToAetheryteId { get; } = new() {
        { 1, 19 },   // Amalj'aa - Little Ala Mhigo
        { 2, 4 },    // Sylphs - The Hawthorne Hut
        { 3, 16 },   // Kobolds - Camp Overlook
        { 4, 14 },   // Sahagin - Aleport
        { 5, 7 },    // Ixal - Fallgourd Float
        { 6, 73 },   // Vanu Vanu - Ok'Zundu
        { 7, 76 },   // Vath - Tailfeather
        { 8, 79 },   // Moogles - Zenith
        { 9, 105 },  // Kojin - Tamamizu
        { 10, 99 },  // Ananta - The Peering Stones
        { 11, 128 }, // Namazu - Dhoro Iloh
        { 12, 144 }, // Pixies - Lydha Lran
        { 13, 143 }, // Qitari - Fanow
        { 14, 136 }, // Dwarves - The Ostall Imperative
        { 15, 169 }, // Arkasodara - Yedlihmad
        { 16, 181 }, // Omicrons - Base Omicron
        { 17, 175 }, // Loporrits - Bestways Burrow
    };

    private IDataManager DataManager { get; }

    public SocietiesRepository(IDataManager dataManager)
    {
        DataManager = dataManager;

        OnTick();
    }

    [OnTick(interval: 2000)]
    private unsafe void OnTick()
    {
        QuestManager* qm = QuestManager.Instance();
        if (null == qm) return;

        WeeklyAllowance = qm->GetBeastTribeAllowance();

        lock (Societies) {
            for (var i = 1; i < qm->BeastReputation.Length + 1; i++) {
                (BeastTribe tribe, BeastReputationRank rankRow, ushort currentRep, ushort requiredRep, byte rank,
                    byte maxRank, string rankName) =
                    GetTribe((byte)(i));

                string name = tribe.Name.ToString();

                Societies[tribe.RowId] = new() {
                    Id             = tribe.RowId,
                    Name           = name[0].ToString().ToUpper() + name[1..],
                    Rank           = rank,
                    MaxRank        = maxRank,
                    RankName       = rankName,
                    RankColor      = rankRow.Color.Row,
                    ExpansionId    = tribe.Expansion.Row,
                    ExpansionName  = tribe.Expansion.Value!.Name.ToString(),
                    IconId         = tribe.Icon,
                    CurrencyItemId = tribe.CurrencyItem.Row,
                    CurrentRep     = currentRep,
                    RequiredRep    = requiredRep,
                };
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose() { }

    /// <inheritdoc/>
    public unsafe void TeleportToAetheryte(uint societyId)
    {
        if (!Framework.Service<IPlayer>().CanUseTeleportAction
            || !SocietyToAetheryteId.TryGetValue(societyId, out uint aetheryteId)
           ) {
            return;
        }

        Telepo.Instance()->Teleport(aetheryteId, 0);
    }

    private unsafe (BeastTribe tribe, BeastReputationRank rankRow, ushort currentRep, ushort neededRep, byte rank,
        byte maxRank, string rankName)
        GetTribe(byte index)
    {
        QuestManager*       qm    = QuestManager.Instance();
        BeastReputationWork tribe = qm->BeastReputation[index - 1];

        var                 rank       = (byte)(tribe.Rank & 0x7F);
        ushort              currentRep = tribe.Value;
        var                 tribeRow   = DataManager.GetExcelSheet<BeastTribe>()!.GetRow(index)!;
        BeastReputationRank rankRow    = DataManager.GetExcelSheet<BeastReputationRank>()!.GetRow(rank)!;
        byte                maxRank    = tribeRow.MaxRank;
        string              rankName   = rankRow.AlliedNames.ToString();
        ushort              neededRep  = rankRow.RequiredReputation;

        if (tribeRow.Expansion.Row != 0
            && tribeRow.Unknown7 != 0
            && QuestManager.IsQuestComplete(tribeRow.Unknown7))
        {
            rank++;
            rankName  = rankRow.Name.ToString();
            neededRep = 0;
        }
        else if (tribeRow.Expansion.Row == 0) {
            rankName = rankRow.Name.ToString();
        }

        if (rank > maxRank)
            maxRank = rank;

        return (tribeRow, rankRow, currentRep, neededRep, rank, maxRank, rankName);
    }
}
