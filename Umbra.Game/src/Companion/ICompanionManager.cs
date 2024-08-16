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


namespace Umbra.Game;

public interface ICompanionManager
{
    /// <summary>
    /// The name of the chocobo companion.
    /// </summary>
    public string CompanionName { get; }

    /// <summary>
    /// True if the player has Gysahl Greens in their inventory.
    /// </summary>
    public bool HasGysahlGreens { get; }

    /// <summary>
    /// True if the player has the given food type in their inventory.
    /// </summary>
    public bool HasCompanionFood(CompanionFood foodType);

    /// <summary>
    /// Feed the companion the given food type if it is in the player's
    /// inventory.
    /// </summary>
    public void UseCompanionFood(CompanionFood foodType);

    /// <summary>
    /// The amount of time left before the companion is automatically dismissed.
    /// </summary>
    public float TimeLeft { get; }

    /// <summary>
    /// Returns the time left as a formatted string.
    /// </summary>
    public string TimeLeftString { get; }

    /// <summary>
    /// The current level of the companion (max 20).
    /// </summary>
    public ushort Level { get; }

    /// <summary>
    /// The amount of XP the companion has for its current level.
    /// </summary>
    public uint CurrentXp { get; }

    /// <summary>
    /// How much XP is required to level up the companion.
    /// </summary>
    public uint RequiredXp { get; }

    /// <summary>
    /// The icon ID of the companion based on its current behavior and whether
    /// it is active or not.
    /// </summary>
    public uint IconId { get; }

    /// <summary>
    /// The name of the currently active command.
    /// </summary>
    public byte ActiveCommand { get; }

    /// <summary>
    /// Whether the companion is currently active and physically present in the world.
    /// </summary>
    public bool IsActive { get; }

    /// <summary>
    /// Returns true if the player can, and is allowed, to summon the companion.
    /// </summary>
    /// <returns></returns>
    public bool CanSummon();

    /// <summary>
    /// Summon the companion. This requires the player to have Gysahl Greens in their inventory and be allowed to
    /// execute actions at the time of invocation.
    /// </summary>
    public void Summon();

    /// <summary>
    /// Opens the companion window.
    /// </summary>
    public void OpenWindow();

    /// <summary>
    /// Returns the name of a stance with the given ID.
    /// </summary>
    public string GetStanceName(uint id);

    /// <summary>
    /// Returns the icon ID of a stance with the given ID.
    /// </summary>
    public uint GetStanceIcon(uint id);

    /// <summary>
    /// Dismiss the companion.
    /// </summary>
    public void Dismiss();

    /// <summary>
    /// Switch the behavior (active command) of the companion based on its current behavior and unlocked skills.
    /// </summary>
    public void SetStance(uint actionId);

    /// <summary>
    /// Returns true if the player can switch the behavior of the companion to the given stance.
    /// </summary>
    public bool CanSetStance(uint actionId);
}
