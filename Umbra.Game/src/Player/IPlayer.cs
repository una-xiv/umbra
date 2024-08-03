/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Game is free software: you can          \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Game is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using System.Numerics;
using Umbra.Game.Inventory;
using Umbra.Game.Societies;

namespace Umbra.Game;

public interface IPlayer
{
    /// <summary>
    /// The name of the player.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The current online status ID.
    /// </summary>
    public uint OnlineStatusId { get; }

    /// <summary>
    /// The current job ID.
    /// </summary>
    public byte JobId { get; }

    /// <summary>
    /// The player's current position in the world.
    /// </summary>
    public Vector3 Position { get; }

    /// <summary>
    /// The player's current rotation in radians.
    /// </summary>
    public float Rotation { get; }

    /// <summary>
    /// True if the player is currently moving.
    /// </summary>
    public bool IsMoving { get; }

    /// <summary>
    /// True if the player is currently mounted.
    /// </summary>
    public bool IsMounted { get; }

    /// <summary>
    /// True if the player is currently in combat.
    /// </summary>
    public bool IsInCombat { get; }

    /// <summary>
    /// True if the player is currently engaged in a PvP duty.
    /// </summary>
    public bool IsInPvP { get; }

    /// <summary>
    /// True if the player is currently casting a spell or ability.
    /// </summary>
    public bool IsCasting { get; }

    /// <summary>
    /// True if the player is currently in an occupied state, meaning they
    /// can't perform any regular actions.
    /// </summary>
    public bool IsOccupied { get; }

    /// <summary>
    /// True if the player is currently between areas.
    /// </summary>
    public bool IsBetweenAreas { get; }

    /// <summary>
    /// True if the player is currently in idle cam (afk) mode.
    /// </summary>
    public bool IsInIdleCam { get; }

    /// <summary>
    /// True if the player is currently watching a cutscene.
    /// </summary>
    public bool IsInCutscene { get; }

    /// <summary>
    /// True if the player is currently dead.
    /// </summary>
    public bool IsDead { get; }

    /// <summary>
    /// True if the player is currently jumping.
    /// </summary>
    public bool IsJumping { get; }

    /// <summary>
    /// True if the player is currently diving.
    /// </summary>
    public bool IsDiving { get; }

    /// <summary>
    /// True if the player is currently bound by duty, meaning they can't
    /// teleport or use the Duty Finder.
    /// </summary>
    public bool IsBoundByDuty { get; }

    /// <summary>
    /// True if the player is currently bound by duty in an instance.
    /// </summary>
    public bool IsBoundByInstancedDuty { get; }

    /// <summary>
    /// True if the player is currently occupied in a quest event.
    /// </summary>
    public bool IsInQuestEvent { get; }

    /// <summary>
    /// True if the player is currently editing the HUD layout.
    /// </summary>
    public bool IsEditingHud { get; }

    /// <summary>
    /// True if the player can, and is allowed to, use the teleport action.
    /// </summary>
    public bool CanUseTeleportAction { get; }

    /// <summary>
    /// The name of the home world server the player originates from.
    /// </summary>
    public string HomeWorldName { get; }

    /// <summary>
    /// The name of the world server the player is currently in.
    /// </summary>
    public string CurrentWorldName { get; }

    public byte GrandCompanyId { get; }

    /// <summary>
    /// True if the player is a mentor.
    /// </summary>
    public bool IsMentor { get; }

    /// <summary>
    /// True if the player is a trade mentor.
    /// </summary>
    public bool IsTradeMentor { get; }

    /// <summary>
    /// True if the player is a battle mentor.
    /// </summary>
    public bool IsBattleMentor { get; }

    /// <summary>
    /// Sets the player's online status to the specified status ID.
    /// </summary>
    /// <param name="statusId">
    /// A RowId from the <see cref="Lumina.Excel.GeneratedSheets.OnlineStatus"/> excel sheet.
    /// </param>
    public void SetOnlineStatus(uint statusId);

    /// <summary>
    /// Returns a service instance that allows retrieving information about the
    /// player's currently equipped gear.
    /// </summary>
    public IEquipmentRepository Equipment { get; }

    /// <summary>
    /// Represents a list of societies (tribes) the player can be allied with
    /// that contains tribe and reputation information.
    /// </summary>
    public IEnumerable<Society> Societies { get; }

    /// <summary>
    /// Returns a service instance that allows retrieving information about the
    /// player's different inventory containers.
    /// </summary>
    public IPlayerInventory Inventory { get; }

    /// <summary>
    /// Get the job information by the specified job ID.
    /// </summary>
    public JobInfo GetJobInfo(byte jobId);

    /// <summary>
    /// Returns true if the player has the specified item in their inventory.
    /// </summary>
    public bool HasItemInInventory(uint itemId, uint minItemCount = 1, ItemUsage itemUsage = ItemUsage.HqBeforeNq);

    /// <summary>
    /// Finds an item or event item by its item ID.
    /// </summary>
    public ResolvedItem? FindResolvedItem(uint itemId);

    /// <summary>
    /// Get the count of the specified item in the player's inventory.
    /// </summary>
    public int GetItemCount(uint itemId, ItemUsage itemUsage = ItemUsage.HqBeforeNq);

    /// <summary>
    /// Use the specified inventory item by its item ID.
    /// </summary>
    public void UseInventoryItem(uint itemId, ItemUsage usage = ItemUsage.HqBeforeNq);

    /// <summary>
    /// Returns true if the specified general action is unlocked.
    /// </summary>
    public bool IsGeneralActionUnlocked(uint actionId);

    /// <summary>
    /// Uses the specified general action by its ID.
    /// </summary>
    public void UseGeneralAction(uint actionId);

    /// <summary>
    /// Returns the cooldown time of the specified action in the format "hh:mm:ss".
    /// </summary>
    public string GetActionCooldownString(ActionType actionType, uint actionId);
}
