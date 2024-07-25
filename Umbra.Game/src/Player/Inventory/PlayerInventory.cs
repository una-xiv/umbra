using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Game.Inventory;

[Service]
internal class PlayerInventory : IPlayerInventory
{
    public uint GetOccupiedInventorySpace(PlayerInventoryType type)
    {
        return GetInventoryTypes(type)
            .Aggregate<InventoryType, uint>(0, (current, iType) => current + GetUsedSizeOf(iType));
    }

    public uint GetTotalInventorySpace(PlayerInventoryType type)
    {
        return GetInventoryTypes(type)
            .Aggregate<InventoryType, uint>(0, (current, iType) => current + GetTotalSizeOf(iType));
    }

    private static unsafe uint GetUsedSizeOf(InventoryType type)
    {
        try {
            InventoryManager*   im  = InventoryManager.Instance();
            InventoryContainer* inv = im->GetInventoryContainer(type);

            if (inv->Loaded == 0 || inv->Size == 0) return 0;

            uint usedSpace = 0;

            for (var i = 0; i <= inv->Size; i++) {
                var slot = inv->GetInventorySlot(i);
                if (slot == null) continue;
                if (slot->ItemId > 0) usedSpace++;
            }

            return usedSpace;
        } catch {
            // Absolute failsafe in case the inventory type is not available.
            // We are safeguarding for this before calling this method, but
            // better safe than sorry.
            return 0;
        }
    }

    private static unsafe uint GetTotalSizeOf(InventoryType type)
    {
        try {
            InventoryManager*   im  = InventoryManager.Instance();
            InventoryContainer* inv = im->GetInventoryContainer(type);

            return inv->Loaded == 0 ? 0 : inv->Size;
        } catch {
            // Absolute failsafe in case the inventory type is not available.
            // We are safeguarding for this before calling this method, but
            // better safe than sorry.
            return 0;
        }
    }

    private unsafe IEnumerable<InventoryType> GetInventoryTypes(PlayerInventoryType type)
    {
        if (type is PlayerInventoryType.Saddlebag or PlayerInventoryType.SaddlebagPremium) {
            if (!CanUseSaddlebags()) return [];

            return PlayerState.Instance()->HasPremiumSaddlebag
                ? [
                    InventoryType.SaddleBag1, InventoryType.SaddleBag2,
                    InventoryType.PremiumSaddleBag1, InventoryType.PremiumSaddleBag2
                ]
                : [InventoryType.SaddleBag1, InventoryType.SaddleBag2];
        }

        return [
            InventoryType.Inventory1,
            InventoryType.Inventory2,
            InventoryType.Inventory3,
            InventoryType.Inventory4
        ];
    }

    private unsafe bool CanUseSaddlebags()
    {
        UIModule* uiModule = UIModule.Instance();
        if (uiModule == null) return false;

        AgentHUD* agentHud = AgentHUD.Instance();
        if (agentHud == null) return false;

        if (!uiModule->IsMainCommandUnlocked(77) || !agentHud->IsMainCommandEnabled(77)) {
            return false;
        }

        // TODO: There may be a need to test for territories too, since simply
        //       checking for the BoundByDuty flag causes false positives when
        //       in leve quests.

        return true;
    }
}
