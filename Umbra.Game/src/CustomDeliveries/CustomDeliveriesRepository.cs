using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
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

    private IDataManager DataManager { get; }

    /// <inheritdoc />
    public Dictionary<int, CustomDeliveryNpc> Npcs { get; } = [];

    public CustomDeliveriesRepository(IDataManager dataManager)
    {
        DataManager = dataManager;

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
        AgentContentsTimer* act = AgentContentsTimer.Instance();
        if (act == null) return;

        var result = stackalloc AtkValue[1];
        var values = stackalloc AtkValue[2];
        values[0].SetInt(18);    // case
        values[1].SetInt(npcId); // npc index

        act->ReceiveEvent(result, values, 2, 0);
    }

    [OnTick(interval: 2500)]
    private void OnTick()
    {
        SatisfactionSupplyManager* ssm = SatisfactionSupplyManager.Instance();
        if (ssm == null) return;

        for (var i = 0; i < 10; i++) {
            byte   heartCount    = ssm->SatisfactionRanks[i]; // 1 ~ 5 (Hearts in the UI)
            byte   usedAllowance = ssm->UsedAllowances[i];

            SatisfactionSupplyNpc? npc = SupplyNpcs.GetValueOrDefault(i + 1);
            if (npc == null) continue;

            if (!QuestManager.IsQuestComplete(npc.RequiredQuest)) continue;

            if (!Npcs.TryGetValue(i + 1, out CustomDeliveryNpc? customNpc)) {
                customNpc = new() {
                    Id                   = i + 1,
                    Name                 = npc.Name,
                    IconId               = npc.IconId,
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
