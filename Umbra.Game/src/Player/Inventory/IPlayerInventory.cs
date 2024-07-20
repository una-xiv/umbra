namespace Umbra.Game.Inventory;

public interface IPlayerInventory
{
    /// <summary>
    /// Returns the amount of occupied inventory space for the given inventory type.
    /// </summary>
    public uint GetOccupiedInventorySpace(PlayerInventoryType type);

    /// <summary>
    /// Returns the total amount of inventory space for the given inventory type.
    /// </summary>
    public uint GetTotalInventorySpace(PlayerInventoryType type);
}