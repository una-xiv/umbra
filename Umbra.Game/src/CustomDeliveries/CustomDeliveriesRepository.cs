using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets2;
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
    public void OpenCustomDeliveriesWindow(int npcId)
    {
        AtkUnitBase* b = RaptureAtkUnitManager.Instance()->GetAddonByName("SatisfactionSupply");

        if (b != null && b->IsReady && b->IsVisible) {
            // Close it first before opening the new one, otherwise it will just close
            // the existing window.
            b->Close(true);
        }

        AgentContentsTimer* act = AgentContentsTimer.Instance();
        if (act == null) return;

        var result = stackalloc AtkValue[1];
        var values = stackalloc AtkValue[2];
        values[0].SetInt(18);    // case
        values[1].SetInt(npcId); // npc index

        act->ReceiveEvent(result, values, 2, 0);
    }

    /// <inheritdoc />
    public unsafe void TeleportToNearbyAetheryte(int npcId)
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

        for (var i = 0; i < 10; i++) {
            byte heartCount    = ssm->SatisfactionRanks[i]; // 1 ~ 5 (Hearts in the UI)
            byte usedAllowance = ssm->UsedAllowances[i];

            SatisfactionSupplyNpc? npc = SupplyNpcs.GetValueOrDefault(i + 1);
            if (npc == null) continue;

            if (!QuestManager.IsQuestComplete(npc.RequiredQuest)) continue;

            if (!Npcs.TryGetValue(i + 1, out CustomDeliveryNpc? customNpc)) {
                var iconId = npc.IconId;

                if (npc.IconId == 0) {
                    // Not all NPCs have an icon, so we need to manually set it.
                    iconId = (i + 1) switch {
                        6 => 61666,
                        9 => 61671,
                        _ => 0,
                    };
                }

                customNpc = new() {
                    Id                   = i + 1,
                    Name                 = npc.Name,
                    IconId               = iconId,
                    HeartCount           = heartCount,
                    DeliveriesThisWeek   = usedAllowance,
                    MaxDeliveriesPerWeek = npc.MaxDeliveriesPerWeek
                };

                Npcs.Add(i + 1, customNpc);
            } else {
                customNpc.HeartCount         = heartCount;
                customNpc.DeliveriesThisWeek = usedAllowance;
            }
        }
    }

    private void HydrateStaticData()
    {
        foreach (var s in DataManager.GetExcelSheet<SatisfactionNpc>()!.ToList()) {
            if (string.IsNullOrEmpty(s.Npc.Value?.Singular.ToDalamudString().TextValue)) continue;

            SupplyNpcs.Add(
                (int)s.RowId,
                new(
                    s.QuestRequired.Row,
                    (uint)s.Icon,
                    s.DeliveriesPerWeek,
                    s.Npc.Value?.Singular.ToDalamudString().TextValue ?? ""
                )
            );
        }
    }

    private record SatisfactionSupplyNpc(
        uint   RequiredQuest,
        uint   IconId,
        byte   MaxDeliveriesPerWeek,
        string Name
    );
}
