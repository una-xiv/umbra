using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Umbra.Game.Inventory;

[Service]
internal class PlayerInventory : IPlayerInventory
{
    public int GetOccupiedInventorySpace(PlayerInventoryType type)
    {
        return GetInventoryTypes(type)
            .Aggregate(0, (current, iType) => current + GetUsedSizeOf(iType));
    }

    public int GetTotalInventorySpace(PlayerInventoryType type)
    {
        return GetInventoryTypes(type)
            .Aggregate(0, (current, iType) => current + GetTotalSizeOf(iType));
    }

    private static unsafe int GetUsedSizeOf(InventoryType type)
    {
        try {
            InventoryManager*   im  = InventoryManager.Instance();
            InventoryContainer* inv = im->GetInventoryContainer(type);

            if (!inv->IsLoaded || inv->Size == 0) return 0;

            int usedSpace = 0;

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

    private static unsafe int GetTotalSizeOf(InventoryType type)
    {
        try {
            InventoryManager*   im  = InventoryManager.Instance();
            InventoryContainer* inv = im->GetInventoryContainer(type);

            return !inv->IsLoaded ? 0 : inv->Size;
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
