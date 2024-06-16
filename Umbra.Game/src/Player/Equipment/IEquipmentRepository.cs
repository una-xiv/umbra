using System.Collections.Generic;

namespace Umbra.Game;

public interface IEquipmentRepository
{
    /// <summary>
    /// Returns a list of the player's currently equipped gear.
    /// </summary>
    public List<EquipmentSlot> Slots { get; }

    /// <summary>
    /// Returns the percentage of the lowest durability item of the player's
    /// currently equipped gear.
    /// </summary>
    public byte LowestDurability { get; }

    /// <summary>
    /// Returns the percentage of the highest spiritbond item of the player's
    /// currently equipped gear.
    /// </summary>
    public byte HighestSpiritbond { get; }

}
