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

using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;

namespace Umbra.Game.Societies;

[Service]
internal sealed class SocietiesRepository(IDataManager dataManager) : IDisposable
{
    public Dictionary<uint, Society> Societies { get; } = [];

    [OnTick(interval: 2000)]
    private unsafe void OnTick()
    {
        QuestManager* qm = QuestManager.Instance();
        if (null == qm) return;

        lock (Societies) {
            for (var i = 1; i < qm->BeastReputation.Length + 1; i++) {
                (BeastTribe tribe, BeastReputationRank rank, ushort currentRep, ushort requiredRep) =
                    GetTribe((byte)(i));

                Societies[tribe.RowId] = new() {
                    Name           = tribe.Name.ToString(),
                    Rank           = rank.AlliedNames.ToString(),
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

    public unsafe (BeastTribe tribe, BeastReputationRank rank, ushort currentRep, ushort neededRep) GetTribe(byte index)
    {
        QuestManager*       qm    = QuestManager.Instance();
        BeastReputationWork tribe = qm->BeastReputation[index - 1];

        var                 rank       = (byte)(tribe.Rank & 0x7F);
        ushort              currentRep = tribe.Value;
        var                 tribeRow   = dataManager.GetExcelSheet<BeastTribe>()!.GetRow(index)!;
        BeastReputationRank rankRow    = dataManager.GetExcelSheet<BeastReputationRank>()!.GetRow(rank)!;

        if (rank >= tribeRow.MaxRank) {
            return (tribeRow, rankRow, currentRep, 0);
        }

        return (tribeRow, rankRow, currentRep, rankRow!.RequiredReputation);
    }
}
