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
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Umbra.Common;

namespace Umbra.Game;

[Service]
public sealed class GearsetRepository : IDisposable
{
    public event Action<Gearset>? OnGearsetCreated;
    public event Action<Gearset>? OnGearsetChanged;
    public event Action<Gearset>? OnGearsetRemoved;

    public Gearset? CurrentGearset { get; private set; }

    private readonly Dictionary<ushort, Gearset> _gearsets      = [];
    private readonly Dictionary<ushort, Gearset> _validGearsets = [];

    public List<Gearset> GetGearsets()
    {
        return new List<Gearset>(_validGearsets.Values);
    }

    public void Dispose()
    {
        _gearsets.Clear();
        _validGearsets.Clear();
        CurrentGearset = null;
    }

    public GearsetRepository(Player player)
    {
        for (ushort i = 0; i < 100; i++) {
            var gearset = new Gearset(i, player);

            _gearsets.Add(i, gearset);

            gearset.OnCreated += () => {
                _validGearsets.Add(gearset.Id, gearset);
                OnGearsetCreated?.Invoke(gearset);
            };

            gearset.OnChanged += () => { OnGearsetChanged?.Invoke(gearset); };

            gearset.OnRemoved += () => {
                _validGearsets.Remove(gearset.Id);
                OnGearsetRemoved?.Invoke(gearset);
            };
        }
    }

    /// <summary>
    /// Equip a gearset by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public unsafe void EquipGearset(ushort id)
    {
        if (!_gearsets.ContainsKey(id)) {
            throw new KeyNotFoundException($"Gearset #{id} does not exist.");
        }

        RaptureGearsetModule* gsm = RaptureGearsetModule.Instance();
        if (gsm == null) return;

        gsm->EquipGearset(id);
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
            .OrderByDescending(g => g.Id)
            .FirstOrDefault()
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
            .OrderBy(g => g.Id)
            .FirstOrDefault()
            ?.Id;
    }

    public unsafe void DuplicateEquippedGearset()
    {
        RaptureGearsetModule* gsm = RaptureGearsetModule.Instance();
        if (gsm == null) return;

        sbyte newId = gsm->CreateGearset();

        if (newId == -1) {
            Logger.Error($"Failed to create gearset.");
        }
    }

    public unsafe void UpdateEquippedGearset()
    {
        if (null == CurrentGearset) return;

        RaptureGearsetModule* gsm = RaptureGearsetModule.Instance();
        if (gsm == null) return;

        gsm->UpdateGearset(CurrentGearset.Id);
    }

    public unsafe void DeleteEquippedGearset()
    {
        if (null == CurrentGearset) return;

        int? newId = FindNextIdInCategory(CurrentGearset)
         ?? FindPrevIdInCategory(CurrentGearset)
         ?? FindFirstValidGearsetIdExceptFor(CurrentGearset)
         ?? null;

        RaptureGearsetModule* gsm = RaptureGearsetModule.Instance();
        if (gsm == null) return;

        gsm->DeleteGearset(CurrentGearset.Id);

        if (null != newId) {
            gsm->EquipGearset((int)newId);
        }
    }

    public void MoveEquippedGearsetUp()
    {
        if (null == CurrentGearset) return;

        int? newId = FindPrevIdInCategory(CurrentGearset);
        if (null != newId) ReassignEquippedGearset(newId.Value);
    }

    public void MoveEquippedGearsetDown()
    {
        if (null == CurrentGearset) return;

        int? newId = FindNextIdInCategory(CurrentGearset);
        if (null != newId) ReassignEquippedGearset(newId.Value);
    }

    [OnTick(interval: 250)]
    public void OnTick()
    {
        foreach (var gearset in _gearsets.Values) {
            gearset.Sync();

            if (gearset.IsCurrent) {
                CurrentGearset = gearset;
            }
        }
    }

    private unsafe void ReassignEquippedGearset(int newId)
    {
        if (null == CurrentGearset) return;

        var oldId = CurrentGearset.Id;

        Logger.Info($"Attempting to reassign gearset #{oldId} to #{newId}...");

        if (newId < 0
         || newId > 99) {
            Logger.Warning($"Cannot reassign gearset #{oldId} to #{newId}. The new ID exceeds the bounds of 0~99.");
            return;
        }

        RaptureGearsetModule* gsm = RaptureGearsetModule.Instance();
        RaptureHotbarModule*  hbm = RaptureHotbarModule.Instance();

        if (null == gsm
         || null == hbm)
            return;

        var assignedId = gsm->ReassignGearsetId(oldId, newId);

        if (assignedId < 0) {
            Logger.Error($"Failed to assign gearset #{oldId} to #{newId}. (Error: {assignedId})");
            return;
        }

        CurrentGearset = _gearsets[(ushort)newId];
        hbm->ReassignGearsetId(oldId, newId);
    }

    private int? FindFirstValidGearsetIdExceptFor(Gearset gearset)
    {
        return _validGearsets
            .Values.Where(gs => gs.IsValid && gs.Id != gearset.Id)
            .Select(gs => gs.Id)
            .FirstOrDefault();
    }
}
