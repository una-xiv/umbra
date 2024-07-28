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
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace Umbra.Game;

public sealed class Gearset(ushort id, IGearsetCategoryRepository categoryRepository, IPlayer player) : IDisposable
{
    public ushort Id { get; } = id;

    public bool IsValid { get; private set; }

    public string          Name              { get; private set; } = string.Empty;
    public byte            JobId             { get; private set; }
    public short           ItemLevel         { get; private set; }
    public bool            IsCurrent         { get; private set; }
    public GearsetCategory Category          { get; private set; } = GearsetCategory.None;
    public short           JobLevel          { get; private set; }
    public byte            JobXp             { get; private set; }
    public string          JobName           { get; private set; } = string.Empty;
    public bool            IsMaxLevel        { get; private set; }
    public bool            HasMissingItems   { get; private set; }
    public bool            AppearanceDiffers { get; private set; }
    public bool            IsMainHandMissing { get; private set; }
    public byte            GlamourSetLink    { get; private set; }

    public event Action? OnCreated;
    public event Action? OnChanged;
    public event Action? OnRemoved;

    public void Dispose()
    {
        foreach (var handler in OnCreated?.GetInvocationList() ?? []) OnCreated -= (Action)handler;
        foreach (var handler in OnChanged?.GetInvocationList() ?? []) OnChanged -= (Action)handler;
        foreach (var handler in OnRemoved?.GetInvocationList() ?? []) OnRemoved -= (Action)handler;
    }

    /// <summary>
    /// Synchronizes the gearset information from the game client.
    /// </summary>
    public unsafe void Sync()
    {
        RaptureGearsetModule* gsm         = RaptureGearsetModule.Instance();
        PlayerState*          playerState = PlayerState.Instance();

        if (gsm == null || playerState == null)
        {
            if (IsValid) OnRemoved?.Invoke();
            IsValid = false;
            return;
        }

        var gearset = gsm->GetGearset(Id);

        if (gearset == null || !gsm->IsValidGearset(Id))
        {
            if (IsValid) OnRemoved?.Invoke();
            IsValid   = false;
            IsCurrent = false;
            return;
        }

        bool isNew = !IsValid;
        IsValid = true;

        // Intermediate values.
        var    isChanged       = false;
        string name            = gearset->NameString;
        byte   jobId           = gearset->ClassJob;
        short  itemLevel       = gearset->ItemLevel;
        bool   isCurrent       = gsm->CurrentGearsetIndex == Id && gearset->ClassJob > 0;
        byte   jobXp           = player.GetJobInfo(jobId).XpPercent;
        string jobName         = player.GetJobInfo(jobId).Name;
        short  jobLevel        = player.GetJobInfo(jobId).Level;
        bool   isMaxLevel      = player.GetJobInfo(jobId).IsMaxLevel;
        bool   mainHandMissing = gearset->Flags.HasFlag(RaptureGearsetModule.GearsetFlag.MainHandMissing);
        byte   glamourSetLink  = gearset->GlamourSetLink;

        // Check for missing items.
        var hasMissingItems   = false;
        var appearanceDiffers = false;

        foreach (var item in gearset->Items)
        {
            if (item.Flags.HasFlag(RaptureGearsetModule.GearsetItemFlag.ItemMissing))
            {
                hasMissingItems = true;
            }

            if (item.Flags.HasFlag(RaptureGearsetModule.GearsetItemFlag.AppearanceDiffers))
            {
                appearanceDiffers = true;
            }
        }

        // Check for changes.
        if (Name != name)
        {
            Name      = name;
            isChanged = true;
        }

        if (JobId != jobId)
        {
            JobId    = jobId;
            Category = categoryRepository.GetCategoryFromJobId(jobId);
        }

        if (ItemLevel != itemLevel)
        {
            ItemLevel = itemLevel;
            isChanged = true;
        }

        if (IsCurrent != isCurrent)
        {
            IsCurrent = isCurrent;
            isChanged = true;
        }

        if (IsMaxLevel != isMaxLevel)
        {
            IsMaxLevel = isMaxLevel;
            isChanged  = true;
        }

        if (JobXp != jobXp)
        {
            JobXp     = jobXp;
            isChanged = true;
        }

        if (JobName != jobName)
        {
            JobName   = jobName;
            isChanged = true;
        }

        if (JobLevel != jobLevel)
        {
            JobLevel  = jobLevel;
            isChanged = true;
        }

        if (IsMainHandMissing != mainHandMissing)
        {
            IsMainHandMissing = mainHandMissing;
            isChanged         = true;
        }

        if (AppearanceDiffers != appearanceDiffers)
        {
            AppearanceDiffers = appearanceDiffers;
            isChanged         = true;
        }

        if (HasMissingItems != hasMissingItems)
        {
            HasMissingItems = hasMissingItems;
            isChanged       = true;
        }

        if (GlamourSetLink != glamourSetLink)
        {
            GlamourSetLink = glamourSetLink;
            isChanged      = true;
        }

        if (isNew)
            OnCreated?.Invoke();
        else if (isChanged) OnChanged?.Invoke();
    }
}
