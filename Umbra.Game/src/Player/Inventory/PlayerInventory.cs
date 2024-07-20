using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Game.Inventory;

[Service]
internal class PlayerInventory : IPlayerInventory
{
    public uint GetOccupiedInventorySpace(PlayerInventoryType type)
    {
        return GetInventoryTypes(type).Aggregate<InventoryType, uint>(0, (current, iType) => current + GetUsedSizeOf(iType));
    }

    public uint GetTotalInventorySpace(PlayerInventoryType type)
    {
        return GetInventoryTypes(type).Aggregate<InventoryType, uint>(0, (current, iType) => current + GetTotalSizeOf(iType));
    }

    private static unsafe uint GetUsedSizeOf(InventoryType type)
    {
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
    }

    private static unsafe uint GetTotalSizeOf(InventoryType type)
    {
        InventoryManager*   im  = InventoryManager.Instance();
        InventoryContainer* inv = im->GetInventoryContainer(type);

        return inv->Loaded == 0 ? 0 : inv->Size;
    }

    private static IEnumerable<InventoryType> GetInventoryTypes(PlayerInventoryType type)
    {
        return type switch
        {
            PlayerInventoryType.Inventory =>
            [
                InventoryType.Inventory1,
                InventoryType.Inventory2,
                InventoryType.Inventory3,
                InventoryType.Inventory4
            ],
            PlayerInventoryType.Saddlebag =>
            [
                InventoryType.SaddleBag1,
                InventoryType.SaddleBag2,
            ],
            PlayerInventoryType.SaddlebagPremium =>
            [
                InventoryType.SaddleBag1,
                InventoryType.SaddleBag2,
                InventoryType.PremiumSaddleBag1,
                InventoryType.PremiumSaddleBag2
            ],
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}