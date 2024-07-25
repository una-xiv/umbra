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
    /// <remarks>
    /// If the returned value is 0, the inventory type is not available at
    /// the moment. This is especially the case for saddlebags when the player
    /// does not have access to them.
    /// </remarks>
    public uint GetTotalInventorySpace(PlayerInventoryType type);
}
