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

using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class Player : IPlayer
{
    /// <summary>
    /// The player's current position in the world.
    /// </summary>
    public Vector3 Position { get; private set; }

    /// <summary>
    /// The player's current rotation in radians.
    /// </summary>
    public float Rotation { get; private set; }

    /// <summary>
    /// True if the player is currently moving.
    /// </summary>
    public bool IsMoving { get; private set; }

    /// <summary>
    /// True if the player is currently mounted.
    /// </summary>
    public bool IsMounted { get; private set; }

    /// <summary>
    /// True if the player is currently in combat.
    /// </summary>
    public bool IsInCombat { get; private set; }

    /// <summary>
    /// True if the player is currently casting a spell or ability.
    /// </summary>
    public bool IsCasting { get; private set; }

    /// <summary>
    /// True if the player is currently in an occupied state, meaning they
    /// can't perform any regular actions.
    /// </summary>
    public bool IsOccupied { get; private set; }

    /// <summary>
    /// True if the player is currently between areas.
    /// </summary>
    public bool IsBetweenAreas { get; private set; }

    /// <summary>
    /// True if the player is currently in idle cam (afk) mode.
    /// </summary>
    public bool IsInIdleCam { get; private set; }

    /// <summary>
    /// True if the player is currently watching a cutscene.
    /// </summary>
    public bool IsInCutscene { get; private set; }

    /// <summary>
    /// True if the player is currently dead.
    /// </summary>
    public bool IsDead { get; private set; }

    /// <summary>
    /// True if the player is currently jumping.
    /// </summary>
    public bool IsJumping { get; private set; }

    /// <summary>
    /// True if the player is currently diving.
    /// </summary>
    public bool IsDiving { get; private set; }

    /// <summary>
    /// True if the player is currently bound by duty, meaning they can't
    /// teleport or use the Duty Finder.
    /// </summary>
    public bool IsBoundByDuty { get; private set; }

    /// <summary>
    /// True if the player is currently editing the HUD layout.
    /// </summary>
    public bool IsEditingHud { get; private set; }

    /// <summary>
    /// True if the player can, and is allowed to, use the teleport action.
    /// </summary>
    public bool CanUseTeleportAction { get; private set; }

    /// <summary>
    /// The name of the home world server the player originates from.
    /// </summary>
    public string HomeWorldName { get; private set; } = "";

    /// <summary>
    /// The name of the world server the player is currently in.
    /// </summary>
    public string CurrentWorldName { get; private set; } = "";

    /// <summary>
    /// The ID of the grand company the player is a member of.
    /// </summary>
    public byte GrandCompanyId { get; private set; }

    private readonly IClientState      _clientState;
    private readonly ICondition        _condition;
    private readonly JobInfoRepository _jobInfoRepository;

    public Player(IClientState clientState, ICondition condition, JobInfoRepository jobInfoRepository)
    {
        _clientState       = clientState;
        _condition         = condition;
        _jobInfoRepository = jobInfoRepository;

        OnTick();
    }

    [OnTick]
    public unsafe void OnTick()
    {
        if (null == _clientState.LocalPlayer || !_clientState.LocalPlayer.IsValid()) return;

        IsMoving = Vector3.Distance(Position, _clientState.LocalPlayer.Position) > 0.01f;
        Position = _clientState.LocalPlayer.Position;
        Rotation = _clientState.LocalPlayer.Rotation;
        IsDead   = _clientState.LocalPlayer.IsDead;

        IsCasting = _clientState.LocalPlayer.IsCasting
         || _condition[ConditionFlag.Casting]
         || _condition[ConditionFlag.Casting87];

        IsInCombat = _condition[ConditionFlag.InCombat];
        IsMounted  = _condition[ConditionFlag.Mounted] || _condition[ConditionFlag.Mounted2];

        IsOccupied = _condition[ConditionFlag.BetweenAreas]
         || _condition[ConditionFlag.BetweenAreas51]
         || _condition[ConditionFlag.Occupied]
         || _condition[ConditionFlag.Occupied30]
         || _condition[ConditionFlag.Occupied33]
         || _condition[ConditionFlag.Occupied38]
         || _condition[ConditionFlag.Occupied39]
         || _condition[ConditionFlag.OccupiedInCutSceneEvent]
         || _condition[ConditionFlag.OccupiedInEvent]
         || _condition[ConditionFlag.OccupiedInQuestEvent]
         || _condition[ConditionFlag.OccupiedSummoningBell];

        IsJumping = _condition[ConditionFlag.Jumping] || _condition[ConditionFlag.Jumping61];
        IsDiving = _condition[ConditionFlag.Diving];

        IsBoundByDuty = _condition[ConditionFlag.BoundByDuty]
         || _condition[ConditionFlag.BoundByDuty56]
         || _condition[ConditionFlag.BoundByDuty95];

        IsInCutscene = _condition[ConditionFlag.OccupiedInCutSceneEvent]
         || _condition[ConditionFlag.WatchingCutscene]
         || _condition[ConditionFlag.WatchingCutscene78];

        IsBetweenAreas = _condition[ConditionFlag.BetweenAreas] || _condition[ConditionFlag.BetweenAreas51];
        IsInIdleCam    = GameMain.IsInIdleCam();

        CanUseTeleportAction = !IsDead && !IsCasting && !IsInCombat && !IsJumping && !IsOccupied && !IsBoundByDuty;
        HomeWorldName        = _clientState.LocalPlayer.HomeWorld.GameData!.Name.ToString();
        CurrentWorldName     = _clientState.LocalPlayer.CurrentWorld.GameData!.Name.ToString();

        var ps = PlayerState.Instance();
        GrandCompanyId   = ps->GrandCompany;
    }

    /// <summary>
    /// Get the job information by the specified job ID.
    /// </summary>
    public JobInfo GetJobInfo(byte jobId)
    {
        return _jobInfoRepository.GetJobInfo(jobId);
    }

    /// <summary>
    /// Returns true if the player has the specified item in their inventory.
    /// </summary>
    public unsafe bool HasItemInInventory(uint itemId, uint minItemCount = 1)
    {
        InventoryManager* im = InventoryManager.Instance();
        if (im == null) return false;

        return im->GetInventoryItemCount(itemId) >= minItemCount;
    }

    /// <summary>
    /// Get the count of the specified item in the player's inventory.
    /// </summary>
    public unsafe int GetItemCount(uint itemId)
    {
        InventoryManager* im = InventoryManager.Instance();
        return im == null ? 0 : im->GetInventoryItemCount(itemId);
    }

    /// <summary>
    /// Use the specified inventory item by its item ID.
    /// </summary>
    public unsafe void UseInventoryItem(uint itemId)
    {
        if (IsCasting || IsOccupied || IsDead || IsJumping) return;
        if (false == HasItemInInventory(itemId)) return;

        AgentInventoryContext* aic = AgentInventoryContext.Instance();
        if (aic != null) aic->UseItem(itemId);
    }

    [OnTick(interval: 1000)]
    internal unsafe void CheckForHudEditingMode()
    {
        var layout = AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName("HudLayout");
        IsEditingHud = layout != null && layout->IsVisible;
    }
}
