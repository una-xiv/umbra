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

using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class GearsetRepository : IGearsetRepository, IDisposable
{
    public event Action<Gearset>? OnGearsetCreated;
    public event Action<Gearset>? OnGearsetChanged;
    public event Action<Gearset>? OnGearsetRemoved;
    public event Action<Gearset>? OnGearsetEquipped;

    public Gearset? CurrentGearset { get; private set; }

    private readonly Dictionary<ushort, Gearset> _gearsets      = [];
    private readonly Dictionary<ushort, Gearset> _validGearsets = [];

    private readonly Hook<RaptureGearsetModule.Delegates.LinkGlamourPlate> _linkGlamourPlateHook;

    public unsafe GearsetRepository(
        IGameInteropProvider interopProvider,
        IGearsetCategoryRepository categoryRepository,
        IPlayer player
    )
    {
        _linkGlamourPlateHook = interopProvider.HookFromAddress<RaptureGearsetModule.Delegates.LinkGlamourPlate>(
            RaptureGearsetModule.MemberFunctionPointers.LinkGlamourPlate,
            OnLinkGlamourPlateToGearset
        );

        _linkGlamourPlateHook.Enable();

        for (ushort i = 0; i < 100; i++)
        {
            var gearset = new Gearset(i, categoryRepository, player);

            _gearsets.Add(i, gearset);

            gearset.OnCreated += () =>
            {
                _validGearsets.Add(gearset.Id, gearset);
                OnGearsetCreated?.Invoke(gearset);
            };

            gearset.OnChanged += () => { OnGearsetChanged?.Invoke(gearset); };

            gearset.OnRemoved += () =>
            {
                _validGearsets.Remove(gearset.Id);
                OnGearsetRemoved?.Invoke(gearset);
            };
        }

        OnTick();
    }

    private unsafe void OnLinkGlamourPlateToGearset(RaptureGearsetModule* gsm, int gearsetId, byte glamourPlateId)
    {
        _linkGlamourPlateHook!.Original(gsm, gearsetId, glamourPlateId);

        if (CurrentGearset?.Id == gearsetId) {
            // The game does not apply the linked glamour plates until the gearset itself is reequipped.
            // This is silly behavior if the gearset is the currently equipped one. Not sure if this is just
            // an oversight of SE or a bug in the game itself.
            gsm->EquipGearset(gearsetId);
        }
    }

    public List<Gearset> GetGearsets()
    {
        return [.._validGearsets.Values];
    }

    public void Dispose()
    {
        _gearsets.Clear();
        _validGearsets.Clear();
        _linkGlamourPlateHook.Dispose();
        CurrentGearset = null;
    }

    /// <summary>
    /// Equip a gearset by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public unsafe void EquipGearset(ushort id)
    {
        if (!_gearsets.TryGetValue(id, out var gs))
        {
            throw new KeyNotFoundException($"Gearset #{id} does not exist.");
        }

        RaptureGearsetModule* gsm = RaptureGearsetModule.Instance();
        if (gsm == null) return;

        gsm->EquipGearset(id);
        OnGearsetEquipped?.Invoke(gs);
    }

    /// <summary>
    /// Opens the glamour selection window that allows linking
    /// a glamour plate to the given gearset.
    /// </summary>
    /// <param name="gearset"></param>
    public unsafe void OpenGlamourSetLinkWindow(Gearset gearset)
    {
        AgentMiragePrismMiragePlate* amp = AgentMiragePrismMiragePlate.Instance();
        if (amp == null) return;

        if (!GameMain.IsInSanctuary())
        {
            Framework.Service<IToastGui>().ShowError(I18N.Translate("UnableToApplyGlamourPlatesHere"));
            return;
        }

        amp->OpenForGearset(gearset.Id, gearset.GlamourSetLink, (ushort)AgentCharaCard.Instance()->AddonId);
    }

    /// <summary>
    /// Unlinks a linked glamour set from the given gearset.
    /// </summary>
    public unsafe void UnlinkGlamourSet(Gearset gearset)
    {
        RaptureGearsetModule* gsm = RaptureGearsetModule.Instance();
        if (gsm == null) return;

        RaptureGearsetModule.Instance()->LinkGlamourPlate(gearset.Id, 0);
    }

    /// <summary>
    /// Find the previous gearset ID in the same category.
    /// </summary>
    /// <param name="gearset"></param>
    /// <returns></returns>
    public ushort? FindPrevIdInCategory(Gearset gearset)
    {
        return _validGearsets
            .Values
            .Where(g => g.Category == gearset.Category && g.Id < gearset.Id)
            .MaxBy(g => g.Id)
            ?.Id;
    }

    /// <summary>
    /// Find the next gearset ID in the same category.
    /// </summary>
    /// <param name="gearset"></param>
    /// <returns></returns>
    public ushort? FindNextIdInCategory(Gearset gearset)
    {
        return _validGearsets
            .Values
            .Where(g => g.Category == gearset.Category && g.Id > gearset.Id)
            .MinBy(g => g.Id)
            ?.Id;
    }

    public unsafe void DuplicateEquippedGearset()
    {
        RaptureGearsetModule* gsm = RaptureGearsetModule.Instance();
        if (gsm == null) return;

        sbyte newId = gsm->CreateGearset();

        if (newId == -1)
        {
            Logger.Error($"Failed to create gearset.");
        }

        _gearsets[(ushort)newId].Sync();
        EquipGearset((ushort)newId);
    }

    public unsafe void UpdateEquippedGearset()
    {
        if (null == CurrentGearset) return;

        RaptureGearsetModule* gsm = RaptureGearsetModule.Instance();
        if (gsm == null) return;

        gsm->UpdateGearset(CurrentGearset.Id);
        OnGearsetChanged?.Invoke(CurrentGearset);
    }

    public unsafe void DeleteGearset(Gearset gearset)
    {
        // Don't allow removing the current gearset.
        if (gearset.IsCurrent) return;

        var result = stackalloc AtkValue[1];
        var values = stackalloc AtkValue[2];
        values[0].SetInt(2);          // case
        values[1].SetInt(gearset.Id); // gearsetIndex
        AgentGearSet.Instance()->ReceiveEvent(result, values, 2, 0);
    }

    public void MoveGearsetUp(Gearset gearset)
    {
        int? newId = FindPrevIdInCategory(gearset);
        if (null != newId) ReassignGearset(gearset, newId.Value);
    }

    public void MoveGearsetDown(Gearset gearset)
    {
        if (null == CurrentGearset) return;

        int? newId = FindNextIdInCategory(gearset);
        if (null != newId) ReassignGearset(gearset, newId.Value);
    }

    [OnTick(interval: 250)]
    public unsafe void OnTick()
    {
        // Update current first.
        var gsm = RaptureGearsetModule.Instance();
        if (gsm == null) return;
        if (!_gearsets.ContainsKey((ushort)gsm->CurrentGearsetIndex)) return;

        var currentGearset = _gearsets[(ushort)gsm->CurrentGearsetIndex];

        if (currentGearset is { IsCurrent: true, IsValid: true })
        {
            CurrentGearset = currentGearset;
        }

        foreach (var gearset in _gearsets.Values)
        {
            gearset.Sync();

            if (gearset is { IsValid: true, IsCurrent: true })
            {
                CurrentGearset = gearset;
            }
        }
    }

    private unsafe void ReassignGearset(Gearset gearset, int newId)
    {
        ushort oldId = gearset.Id;

        Logger.Debug($"Attempting to reassign gearset #{oldId} to #{newId}...");

        if (newId is < 0 or > 99)
        {
            Logger.Warning($"Cannot reassign gearset #{oldId} to #{newId}. The new ID exceeds the bounds of 0~99.");
            return;
        }

        RaptureGearsetModule* gsm = RaptureGearsetModule.Instance();
        RaptureHotbarModule*  hbm = RaptureHotbarModule.Instance();

        if (null == gsm || null == hbm) return;

        int assignedId = gsm->ReassignGearsetId(oldId, newId);

        if (assignedId < 0)
        {
            Logger.Error($"Failed to assign gearset #{oldId} to #{newId}. (Error: {assignedId})");
            return;
        }

        gearset = _gearsets[(ushort)newId];
        hbm->ReassignGearsetId(oldId, newId);
        OnGearsetChanged?.Invoke(gearset);
    }
}