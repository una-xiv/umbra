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

using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;

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
        { 18, 238 }, // Pelupelu - Dock Poga
        { 19, 206 }, // Mamool Ja - Mamook
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

                Societies[tribe.RowId] = new() {
                    Id             = tribe.RowId,
                    Name           = tribe.Name.ToNameString(),
                    Rank           = rank,
                    MaxRank        = maxRank,
                    RankName       = rankName,
                    RankColor      = rankRow.Color.RowId,
                    ExpansionId    = tribe.Expansion.RowId,
                    ExpansionName  = tribe.Expansion.Value.Name.ExtractText(),
                    IconId         = tribe.Icon,
                    CurrencyItemId = tribe.CurrencyItem.RowId,
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

        byte                rank       = PlayerState.Instance()->GetBeastTribeRank(index);
        ushort              currentRep = tribe.Value;
        var                 tribeRow   = DataManager.GetExcelSheet<BeastTribe>().GetRow(index);
        BeastReputationRank rankRow    = DataManager.GetExcelSheet<BeastReputationRank>().GetRow(rank);
        byte                maxRank    = tribeRow.MaxRank;
        string              rankName   = rankRow.AlliedNames.ExtractText();
        ushort              neededRep  = rankRow.RequiredReputation;

        if (tribeRow.Expansion.RowId != 0
            && tribeRow.Unknown1 != 0
            && QuestManager.IsQuestComplete(tribeRow.Unknown1)) {
            rank++;
            rankName  = rankRow.Name.ExtractText();
            neededRep = 0;
        } else if (tribeRow.Expansion.RowId == 0) {
            rankName = rankRow.Name.ExtractText();
        }

        if (rank > maxRank) maxRank = rank;

        return (tribeRow, rankRow, currentRep, neededRep, rank, maxRank, rankName);
    }
}
