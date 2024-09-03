using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets2;
using System.Linq;
using Umbra.Common;

namespace Umbra.Game;

public readonly struct EquipmentSlot(string itemName, uint iconId, byte slotId, byte durability, byte spiritbond)
{
    public readonly bool   IsEmpty     => ItemName == "";
    public readonly string ItemName   = itemName;
    public readonly uint   IconId     = iconId;
    public readonly byte   SlotId     = slotId;
    public readonly byte   Durability = durability;
    public readonly byte   Spiritbond = spiritbond;
}

[Service]
internal unsafe class EquipmentRepository : IEquipmentRepository
{
    private const float DurabilityRatioPercentage = 300f;
    private const float SpiritbondRatioPercentage = 100f;

    /// <summary>
    /// Returns a list of the player's currently equipped gear.
    /// </summary>
    public List<EquipmentSlot> Slots { get; } = [];

    /// <summary>
    /// Returns the percentage of the lowest durability item of the player's
    /// currently equipped gear.
    /// </summary>
    public byte LowestDurability { get; private set; }
    
    /// <summary>
    /// Returns the average percentage of all durability item of the player's
    /// currently equipped gear
    /// </summary>
    public byte AverageDurability { get; private set; }
    
    /// <summary>
    /// Returns the percentage of the highest durability item of the player's
    /// currently equipped gear.
    /// </summary>
    public byte HighestDurability { get; private set; }
    
    /// <summary>
    /// Returns the percentage of the lowest spiritbond item of the player's
    /// currently equipped gear
    /// </summary>
    public byte LowestSpiritbond { get; private set; }
    
    /// <summary>
    /// Returns the average percentage of all spiritbond item of the player's
    /// currently equipped gear
    /// </summary>
    public byte AverageSpiritbond { get; private set; }
    
    /// <summary>
    /// Returns the percentage of the highest spiritbond item of the player's
    /// currently equipped gear.
    /// </summary>
    public byte HighestSpiritbond { get; private set; }

    private readonly IDataManager        _dataManager;
    private readonly InventoryManager*   _inventoryManager;
    private readonly InventoryContainer* _equipmentContainer;

    public EquipmentRepository(IDataManager dataManager)
    {
        _dataManager            = dataManager;
        _inventoryManager       = InventoryManager.Instance();
        _equipmentContainer     = _inventoryManager->GetInventoryContainer(InventoryType.EquippedItems);

        for (var slot = 0; slot < 13; slot++) {
            Slots.Add(new("", 0, 0, 0, 0));
        }

        Update();
    }

    [OnTick(interval: 1000)]
    public void Update()
    {
        ushort lowestDurability  = ushort.MaxValue;
        ushort lowestSpiritbond  = ushort.MaxValue;
        ushort highestDurability = 0;
        ushort highestSpiritbond = 0;
        int totalDurability      = 0;
        int totalSpiritbond      = 0;
        ushort filledSlots       = 0;
        ushort spiritbondFilled  = 0;

        for (var slot = 0; slot < 13; slot++) {
            InventoryItem* equipment = _equipmentContainer->GetInventorySlot(slot);

            if (equipment == null || equipment->ItemId == 0) {
                Slots[slot] = new("", 0, 0, 0, 0);
                continue;
            }

            filledSlots++;

            lowestDurability  = Math.Min(lowestDurability, equipment->Condition);
            highestDurability = Math.Max(highestDurability, equipment->Condition);

            if (equipment->Spiritbond > 0) {
                spiritbondFilled++;
                lowestSpiritbond = Math.Min(lowestSpiritbond, equipment->Spiritbond);
            };
            
            highestSpiritbond = Math.Max(highestSpiritbond, equipment->Spiritbond);
            
            totalDurability += equipment->Condition;
            totalSpiritbond += equipment->Spiritbond;

            var item = _dataManager.GetExcelSheet<Item>()!.GetRow(equipment->ItemId)!;

            Slots[slot] = new(
                item.Name.ToDalamudString().ToString(),
                item.Icon,
                (byte)slot,
                (byte)(equipment->Condition / DurabilityRatioPercentage),
                (byte)(equipment->Spiritbond / SpiritbondRatioPercentage)
            );
        }

        LowestDurability  = (byte)(lowestDurability / DurabilityRatioPercentage);
        HighestDurability = (byte)(highestDurability / DurabilityRatioPercentage);
        
        LowestSpiritbond  = (byte)(lowestSpiritbond / SpiritbondRatioPercentage);
        HighestSpiritbond = (byte)(highestSpiritbond / SpiritbondRatioPercentage);

        if (filledSlots > 0) {
            AverageDurability = (byte)((totalDurability / (float)filledSlots) / DurabilityRatioPercentage);
            AverageSpiritbond = (byte)((totalSpiritbond / (float)spiritbondFilled) / SpiritbondRatioPercentage);
        } else {
            AverageDurability = 0;
            AverageSpiritbond = 0;
        }
    }
}
