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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Game.Inventory;
using Umbra.Game.Societies;

namespace Umbra.Game;

[Service]
internal sealed class Player : IPlayer
{
    /// <summary>
    /// The name of the player.
    /// </summary>
    public string Name => _clientState.LocalPlayer?.Name.TextValue ?? "Unnamed Player";

    /// <summary>
    /// The current online status ID.
    /// </summary>
    public uint OnlineStatusId { get; private set; }

    /// <summary>
    /// The current job ID.
    /// </summary>
    public byte JobId { get; private set; }

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
    /// True if the player is currently a member of a party.
    /// </summary>
    public bool IsInParty { get; private set; }

    /// <summary>
    /// True if the player is currently engaged in a PvP duty.
    /// </summary>
    public bool IsInPvP { get; private set; }

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
    /// True if the player is currently bound by duty in an instance.
    /// </summary>
    public bool IsBoundByInstancedDuty { get; private set; }

    /// <summary>
    /// True if the player is currently occupied in a quest event.
    /// </summary>
    public bool IsInQuestEvent { get; private set; }

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

    /// <summary>
    /// True if the player is a mentor.
    /// </summary>
    public bool IsMentor { get; private set; }

    /// <summary>
    /// True if the player is a trade mentor.
    /// </summary>
    public bool IsTradeMentor { get; private set; }

    /// <summary>
    /// True if the player is a battle mentor.
    /// </summary>
    public bool IsBattleMentor { get; private set; }

    /// <summary>
    /// Whether the player is currently in a sanctuary.
    /// </summary>
    public bool IsInSanctuary { get; private set; }

    /// <summary>
    /// True if the player has their weapon drawn.
    /// </summary>
    public bool IsWeaponDrawn { get; private set; }

    /// <summary>
    /// The player's current job level.
    /// </summary>
    public short Level { get; private set; }

    /// <summary>
    /// The player's current synced level. This value is 0 if the player is
    /// not synced to a lower level.
    /// </summary>
    public short SyncedLevel { get; private set; }

    /// <summary>
    /// True if the player has reached the maximum level on their current job.
    /// </summary>
    public bool IsMaxLevel { get; private set; }

    /// <summary>
    /// The player's current experience points.
    /// </summary>
    public uint CurrentExperience { get; private set; }

    /// <summary>
    /// The total amount of rested experience points the player has.
    /// </summary>
    public uint RestedExperience { get; private set; }

    /// <summary>
    /// The total amount of experience points required for the current level.
    /// </summary>
    public uint TotalRequiredExperience { get; private set; }

    /// <summary>
    /// Represents a list of societies (tribes) the player can be allied with
    /// that contains tribe and reputation information.
    /// </summary>
    public IEnumerable<Society> Societies => _societiesRepository.Societies.Values.ToList();

    public IEquipmentRepository Equipment { get; }

    public IPlayerInventory Inventory { get; }

    private readonly IClientState        _clientState;
    private readonly ICondition          _condition;
    private readonly IDataManager        _dataManager;
    private readonly IPartyList          _partyList;
    private readonly JobInfoRepository   _jobInfoRepository;
    private readonly SocietiesRepository _societiesRepository;
    private readonly IGameGui            _gameGui;

    private readonly uint[] _acceptedOnlineStatusIds = [47, 32, 31, 27, 28, 29, 30, 12, 17, 21, 22, 23];

    public Player(
        IClientState         clientState,
        ICondition           condition,
        IDataManager         dataManager,
        IEquipmentRepository equipmentRepository,
        IGameGui             gameGui,
        IPartyList           partyList,
        JobInfoRepository    jobInfoRepository,
        SocietiesRepository  societiesRepository,
        IPlayerInventory     playerInventory
    )
    {
        _clientState         = clientState;
        _condition           = condition;
        _dataManager         = dataManager;
        _gameGui             = gameGui;
        _partyList           = partyList;
        _jobInfoRepository   = jobInfoRepository;
        _societiesRepository = societiesRepository;

        Equipment = equipmentRepository;
        Inventory = playerInventory;

        OnTick();
    }

    [OnTick]
    public unsafe void OnTick()
    {
        if (null == _clientState.LocalPlayer || !_clientState.LocalPlayer.IsValid()) return;

        OnlineStatusId = _clientState.LocalPlayer.OnlineStatus.Id;
        IsMoving       = Vector3.Distance(Position, _clientState.LocalPlayer.Position) > 0.01f;
        Position       = _clientState.LocalPlayer.Position;
        Rotation       = _clientState.LocalPlayer.Rotation;
        IsDead         = _clientState.LocalPlayer.IsDead;
        IsInPvP        = _clientState.IsPvPExcludingDen;
        IsInParty      = _partyList.Length > 0;
        IsInSanctuary  = GameMain.IsInSanctuary();
        JobId          = (byte)_clientState.LocalPlayer.ClassJob.Id;

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
        IsDiving  = _condition[ConditionFlag.Diving];

        IsWeaponDrawn = _clientState.LocalPlayer.StatusFlags.HasFlag(StatusFlags.WeaponOut);

        IsBoundByDuty = _condition[ConditionFlag.BoundByDuty]
            || _condition[ConditionFlag.BoundByDuty56]
            || _condition[ConditionFlag.BoundByDuty95];

        IsBoundByInstancedDuty = _condition[ConditionFlag.BoundByDuty56];

        IsInCutscene = _condition[ConditionFlag.OccupiedInCutSceneEvent]
            || _condition[ConditionFlag.Occupied38] // Pseudo Cutscene during combat
            || _condition[ConditionFlag.WatchingCutscene]
            || _condition[ConditionFlag.WatchingCutscene78];

        IsBetweenAreas = _condition[ConditionFlag.BetweenAreas] || _condition[ConditionFlag.BetweenAreas51];
        IsInIdleCam    = GameMain.IsInIdleCam();

        IsInQuestEvent = _condition[ConditionFlag.OccupiedInQuestEvent]
            && _condition[ConditionFlag.OccupiedInCutSceneEvent];

        // Unknown57 is the transient state the player is in after casting and before being actually mounted.
        CanUseTeleportAction = ActionManager.Instance()->GetActionStatus(ActionType.Action, 5) == 0;
        HomeWorldName        = _clientState.LocalPlayer.HomeWorld.GameData!.Name.ToString();
        CurrentWorldName     = _clientState.LocalPlayer.CurrentWorld.GameData!.Name.ToString();

        var ps = PlayerState.Instance();

        GrandCompanyId = ps->GrandCompany;
        IsBattleMentor = ps->IsBattleMentor() && ps->MentorVersion == 3;
        IsTradeMentor  = ps->IsTradeMentor() && ps->MentorVersion == 3;
        IsMentor       = IsBattleMentor && IsTradeMentor;

        // Experience and level information.
        Level       = GetJobInfo(JobId).Level;
        SyncedLevel = ps->SyncedLevel;

        AgentHUD* hud = AgentHUD.Instance();

        if (hud == null) {
            IsMaxLevel              = true;
            CurrentExperience       = 0;
            RestedExperience        = 0;
            TotalRequiredExperience = 0;
            return;
        }

        IsMaxLevel              = hud->ExpIsMaxLevel;
        CurrentExperience       = hud->ExpCurrentExperience;
        RestedExperience        = hud->ExpRestedExperience;
        TotalRequiredExperience = hud->ExpNeededExperience;
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
    public unsafe bool HasItemInInventory(
        uint      itemId,
        uint      minItemCount = 1,
        ItemUsage itemUsage    = ItemUsage.HqBeforeNq
    )
    {
        InventoryManager* im = InventoryManager.Instance();
        if (im == null) return false;

        var nqCount = im->GetInventoryItemCount(itemId);
        var hqCount = im->GetInventoryItemCount(itemId, true);

        return itemUsage switch {
            ItemUsage.HqOnly     => hqCount >= minItemCount,
            ItemUsage.NqOnly     => nqCount >= minItemCount,
            ItemUsage.HqBeforeNq => hqCount >= minItemCount || nqCount >= minItemCount,
            ItemUsage.NqBeforeHq => nqCount >= minItemCount || hqCount >= minItemCount,
            _                    => false
        };
    }

    /// <summary>
    /// Finds an item or event item by its item ID.
    /// </summary>
    public ResolvedItem? FindResolvedItem(uint itemId)
    {
        Item? item = _dataManager.GetExcelSheet<Item>()!.GetRow(itemId);
        if (item != null) return new(item.RowId, item.Name.ToString(), item.Icon);

        EventItem? eventItem = _dataManager.GetExcelSheet<EventItem>()!.GetRow(itemId);
        if (eventItem != null) return new(eventItem.RowId, eventItem.Name.ToString(), eventItem.Icon);

        return null;
    }

    /// <summary>
    /// Get the count of the specified item in the player's inventory.
    /// </summary>
    public unsafe int GetItemCount(uint itemId, ItemUsage itemUsage = ItemUsage.HqBeforeNq)
    {
        InventoryManager* im = InventoryManager.Instance();
        if (im == null) return 0;

        return itemUsage switch {
            ItemUsage.HqOnly     => im->GetInventoryItemCount(itemId, true),
            ItemUsage.NqOnly     => im->GetInventoryItemCount(itemId),
            ItemUsage.HqBeforeNq => im->GetInventoryItemCount(itemId, true) + im->GetInventoryItemCount(itemId),
            ItemUsage.NqBeforeHq => im->GetInventoryItemCount(itemId) + im->GetInventoryItemCount(itemId, true),
            _                    => 0
        };
    }

    /// <summary>
    /// Use the specified inventory item by its item ID.
    /// </summary>
    public unsafe void UseInventoryItem(uint itemId, ItemUsage usage = ItemUsage.HqBeforeNq)
    {
        if (IsCasting || IsOccupied || IsDead || IsJumping || IsBetweenAreas || IsInCutscene) return;

        InventoryManager* im = InventoryManager.Instance();
        if (im == null) return;

        AgentInventoryContext* aic = AgentInventoryContext.Instance();
        if (aic == null) return;

        switch (usage) {
            case ItemUsage.HqOnly:
                if (im->GetInventoryItemCount(itemId, true) > 0) aic->UseItem(itemId + 1000000);
                return;
            case ItemUsage.NqOnly:
                if (im->GetInventoryItemCount(itemId) > 0) aic->UseItem(itemId);
                return;
            case ItemUsage.HqBeforeNq:
                if (im->GetInventoryItemCount(itemId, true) > 0) {
                    aic->UseItem(itemId + 1000000);
                    return;
                }

                if (im->GetInventoryItemCount(itemId) > 0) aic->UseItem(itemId);
                return;
            case ItemUsage.NqBeforeHq:
                if (im->GetInventoryItemCount(itemId) > 0) {
                    aic->UseItem(itemId);
                    return;
                }

                if (im->GetInventoryItemCount(itemId, true) > 0) aic->UseItem(itemId + 1000000);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(usage), usage, null);
        }
    }

    /// <summary>
    /// Sets the player's online status to the specified status ID.
    /// </summary>
    /// <param name="statusId">A RowId from the <see cref="OnlineStatus"/> excel sheet.</param>
    public unsafe void SetOnlineStatus(uint statusId)
    {
        if (_condition[ConditionFlag.BoundByDuty56]) return;
        if (!_acceptedOnlineStatusIds.Contains(statusId)) return;

        // Mentor status checks.
        switch (statusId) {
            case 27 when !IsMentor:
            case 28 when !IsBattleMentor:
            case 30 when !IsBattleMentor:
            case 29 when !IsTradeMentor:
                return;
        }

        InfoProxyDetail* ic = InfoProxyDetail.Instance();
        if (null == ic) return;

        if (statusId == 23 && ic->UpdateData.LookingForPartyClassJobIdMask == 0) {
            // Assign a job mask if the player is looking for a party but has no job mask.
            ic->SetUpdateClassJobId(JobId);
            ic->SetUpdateLookingForPartyClassJobIdMask((ulong)1 << (JobId - 1));
        }

        ic->SendOnlineStatusUpdate(statusId);
    }

    public unsafe bool IsGeneralActionUnlocked(uint actionId)
    {
        try {
            GeneralAction? action = Framework.Service<IDataManager>().GetExcelSheet<GeneralAction>()!.GetRow(actionId);

            return action != null
                && (action.UnlockLink == 0 || UIState.Instance()->IsUnlockLinkUnlocked(action.UnlockLink));
        } catch {
            // Fall-through.
        }

        return false;
    }

    public unsafe void UseGeneralAction(uint actionId)
    {
        if (!IsGeneralActionUnlocked(actionId)) return;

        try {
            ActionManager.Instance()->UseAction(ActionType.GeneralAction, actionId);
        } catch {
            // Fall-through.
        }
    }

    public unsafe string GetActionCooldownString(ActionType type, uint actionId)
    {
        try {
            ActionManager* am = ActionManager.Instance();
            if (am == null) return "";

            if (am->IsActionOffCooldown(type, actionId)) return "";
            if (!am->IsRecastTimerActive(type, actionId)) return "";

            float recastTime  = am->GetRecastTime(type, actionId);
            float elapsedTime = am->GetRecastTimeElapsed(type, actionId);

            if (recastTime <= 0 || elapsedTime <= 0) return "";

            TimeSpan ts = TimeSpan.FromSeconds(recastTime - elapsedTime);

            return $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        } catch {
            // Fall-through.
            return "";
        }
    }

    [OnTick(interval: 1000)]
    internal unsafe void CheckForHudEditingMode()
    {
        var layout = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonByName("HudLayout");
        IsEditingHud = layout != null && layout->IsVisible;
    }
}

public enum ItemUsage
{
    HqBeforeNq,
    NqBeforeHq,
    NqOnly,
    HqOnly
}

public readonly struct ResolvedItem(uint id, string name, uint iconId)
{
    public uint   Id     { get; } = id;
    public string Name   { get; } = name;
    public uint   IconId { get; } = iconId;
}
