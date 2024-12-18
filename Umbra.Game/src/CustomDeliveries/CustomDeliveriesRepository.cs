using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Game.CustomDeliveries;

[Service]
internal sealed unsafe class CustomDeliveriesRepository : ICustomDeliveriesRepository, IDisposable
{
    private static Dictionary<int, SatisfactionSupplyNpc> SupplyNpcs { get; } = [];

    private static Dictionary<int, uint> SupplyNpcToAetheryteId { get; } = new() {
        { 1, 75 },   // Zhloe Aliapoh -- Idyllshire
        { 2, 104 },  // M'naago -- Rhalgr's Reach
        { 3, 105 },  // Kurenai -- Tamamizu
        { 4, 75 },   // Adkiragh -- Idyllshire
        { 5, 134 },  // Kai-Shirr -- Eulmore
        { 6, 70 },   // Ehll Tou -- Foundation
        { 7, 70 },   // Charlemend -- Foundation
        { 8, 182 },  // Ameliance -- Old Sharlayan
        { 9, 144 },  // Anden -- Lydha Lran
        { 10, 167 }, // Margrat -- Sharlyan Hamlet
        { 11, 208 }, // Nitowikwe -- Sheshenewezi Springs
    };

    private IDataManager DataManager { get; }
    private IPlayer      Player      { get; }

    /// <inheritdoc />
    public Dictionary<int, CustomDeliveryNpc> Npcs { get; } = [];

    /// <inheritdoc />
    public int DeliveriesRemainingThisWeek { get; private set; }

    public CustomDeliveriesRepository(IDataManager dataManager, IPlayer player)
    {
        DataManager = dataManager;
        Player      = player;

        HydrateStaticData();
        OnTick();
    }

    public void Dispose()
    {
        SupplyNpcs.Clear();
        Npcs.Clear();
    }

    /// <inheritdoc />
    public void OpenCustomDeliveriesWindow(int? npcId)
    {
        AtkUnitBase* b = RaptureAtkUnitManager.Instance()->GetAddonByName("SatisfactionSupply");

        if (b != null && b->IsReady && b->IsVisible) {
            // Close it first before opening the new one, otherwise it will just close
            // the existing window.
            b->Close(true);
        }

        AgentContentsTimer* act = AgentContentsTimer.Instance();
        if (act == null) return;

        // 12/12 opens the Client List window. 18/npcId opens the NPC window.
        var result = stackalloc AtkValue[1];
        var values = stackalloc AtkValue[2];
        values[0].SetInt(npcId == null ? 12 : 18); // case
        values[1].SetInt(npcId ?? 12); // npc index

        act->ReceiveEvent(result, values, 2, 0);
    }

    /// <inheritdoc />
    public void TeleportToNearbyAetheryte(int npcId)
    {
        if (!Player.CanUseTeleportAction) return;
        if (!SupplyNpcToAetheryteId.TryGetValue(npcId, out uint aetheryteId)) return;

        Telepo.Instance()->Teleport(aetheryteId, 0);
    }

    [OnTick(interval: 2500)]
    private void OnTick()
    {
        SatisfactionSupplyManager* ssm = SatisfactionSupplyManager.Instance();
        if (ssm == null) return;

        DeliveriesRemainingThisWeek = ssm->GetRemainingAllowances();

        var npcNum = ssm->SatisfactionRanks.Length;
        var adjustedTimestamp = ssm->TimeAdjustmentForBonusGuarantee + FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.GetServerTime();
        var satisfactionBonusGuaranteeRowId = (uint)((adjustedTimestamp - 1657008000) / 604800 % npcNum);
        var satisfactionBonusGuarantee = DataManager.GetExcelSheet<SatisfactionBonusGuarantee>().GetRow(satisfactionBonusGuaranteeRowId);

        for (var i = 0; i < npcNum; i++) {
            byte heartCount    = ssm->SatisfactionRanks[i]; // 1 ~ 5 (Hearts in the UI)
            byte usedAllowance = ssm->UsedAllowances[i];
            int? bonusType     = null;

            if (heartCount == 5)
                for (var j = 0; j < 2; j++) {
                    if (satisfactionBonusGuarantee.BonusDoH[j] == i + 1)
                        bonusType = 5;
                    if (satisfactionBonusGuarantee.BonusDoL[j] == i + 1)
                        bonusType = 6;
                    if (satisfactionBonusGuarantee.BonusFisher[j] == i + 1)
                        bonusType = 7;
                }

            SatisfactionSupplyNpc? npc = SupplyNpcs.GetValueOrDefault(i + 1);
            if (npc == null) continue;

            if (!QuestManager.IsQuestComplete(npc.RequiredQuest)) continue;

            if (!Npcs.TryGetValue(i + 1, out CustomDeliveryNpc? customNpc)) {
                var iconId = npc.IconIds[heartCount];

                customNpc = new() {
                    Id                   = i + 1,
                    Name                 = npc.Name,
                    IconId               = iconId,
                    HeartCount           = heartCount,
                    DeliveriesThisWeek   = usedAllowance,
                    MaxDeliveriesPerWeek = npc.MaxDeliveriesPerWeek,
                    BonusType            = bonusType,
                };

                Npcs.Add(i + 1, customNpc);
            } else {
                customNpc.HeartCount         = heartCount;
                customNpc.DeliveriesThisWeek = usedAllowance;
                customNpc.BonusType          = bonusType;
            }
        }
    }

    private void HydrateStaticData()
    {
        foreach (var s in DataManager.GetExcelSheet<SatisfactionNpc>().ToList()) {
            if (null == s.Npc.ValueNullable) continue;

            SupplyNpcs.Add(
                (int)s.RowId,
                new(
                    s.QuestRequired.RowId,
                    s.RankParams.Select(t => (uint)t.ImageId).ToArray(),
                    s.DeliveriesPerWeek,
                    s.Npc.Value.Singular.ExtractText()
                )
            );
        }
    }

    private record SatisfactionSupplyNpc(
        uint   RequiredQuest,
        uint[]   IconIds,
        byte   MaxDeliveriesPerWeek,
        string Name
    );
}
