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
using System.Text.Json;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class JobInfoRepository : IDisposable
{
    private readonly Dictionary<byte, JobInfo> _jobInfos   = [];
    private readonly Dictionary<byte, sbyte>   _expArrayId = [];

    private readonly IDataManager _dataManager;

    public JobInfoRepository(IDataManager dataManager)
    {
        _dataManager = dataManager;

        dataManager.GetExcelSheet<ClassJob>()!
            .ToList()
            .ForEach(
                cj =>
                {
                    _expArrayId[(byte)cj.RowId] = cj.ExpArrayIndex;
                    _jobInfos[(byte)cj.RowId]   = new(cj);
                }
            );
    }

    public void Dispose()
    {
        _jobInfos.Clear();
    }

    public JobInfo GetJobInfo(byte jobId)
    {
        return _jobInfos[jobId]
            ?? throw new KeyNotFoundException($"Job #{jobId} does not exist.");
    }

    [OnTick(interval: 500)]
    public unsafe void OnTick()
    {
        PlayerState* ps = PlayerState.Instance();
        if (ps == null) return;

        foreach (var jobInfo in _jobInfos.Values)
        {
            if (_expArrayId[jobInfo.Id] == -1) continue;

            jobInfo.Level = ps->ClassJobLevels[_expArrayId[jobInfo.Id]];

            // Blue Mage hack.
            if (jobInfo is { Id: 36, Level: 80 })
            {
                jobInfo.XpPercent  = 0;
                jobInfo.IsMaxLevel = true;
                continue;
            }

            var grow = _dataManager.GetExcelSheet<ParamGrow>()!.GetRow((uint)jobInfo.Level);

            // Hardcoded max level.
            if (jobInfo.Level == 100 || grow == null || grow.ExpToNext == 0)
            {
                jobInfo.XpPercent  = 0;
                jobInfo.IsMaxLevel = true;
                continue;
            }

            int currentXp = ps->ClassJobExperience[_expArrayId[jobInfo.Id]];
            jobInfo.XpPercent  = (byte)(currentXp / (float)grow.ExpToNext * 100);
            jobInfo.IsMaxLevel = false;
        }
    }
}