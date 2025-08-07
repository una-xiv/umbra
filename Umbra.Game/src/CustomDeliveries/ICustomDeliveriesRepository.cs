﻿namespace Umbra.Game.CustomDeliveries;

public interface ICustomDeliveriesRepository
{
    /// <summary>
    /// Contains a list of unlocked Custom Deliveries NPCs and the player's
    /// current standing with them.
    /// </summary>
    public Dictionary<int, CustomDeliveryNpc> Npcs { get; }

    /// <summary>
    /// Returns the amount of deliveries the player can still make this week.
    /// </summary>
    public int DeliveriesRemainingThisWeek { get; }

    /// <summary>
    /// Opens the "Satisfaction Supply" window for the specified NPC or the
    /// list window if no NPC is specified.
    /// </summary>
    /// <param name="npcId">The ID of the supply npc</param>
    public void OpenCustomDeliveriesWindow(int? npcId);

    /// <summary>
    /// Teleports the player to a nearby aetheryte for the specified NPC.
    /// Does nothing if the player is not allowed to teleport in its current conditions.
    /// </summary>
    /// <param name="npcId">The ID of the supply npc</param>
    public void TeleportToNearbyAetheryte(int npcId);
}
