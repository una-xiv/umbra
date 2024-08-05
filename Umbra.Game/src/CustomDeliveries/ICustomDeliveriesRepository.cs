using System.Collections.Generic;

namespace Umbra.Game.CustomDeliveries;

public interface ICustomDeliveriesRepository
{
    /// <summary>
    /// Contains a list of unlocked Custom Deliveries NPCs and the player's
    /// current standing with them.
    /// </summary>
    public Dictionary<int, CustomDeliveryNpc> Npcs { get; }

    /// <summary>
    /// Opens the "Satisfaction Supply" window for the specified NPC.
    /// </summary>
    /// <param name="npcId"></param>
    public void OpenCustomDeliveriesWindow(int npcId);
}
