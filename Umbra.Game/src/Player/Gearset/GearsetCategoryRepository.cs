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

using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class GearsetCategoryRepository : IGearsetCategoryRepository
{
    private readonly Dictionary<byte, GearsetCategory> _gearsetCategories = [];

    public GearsetCategoryRepository(IDataManager dataManager)
    {
        dataManager.GetExcelSheet<ClassJob>()!
            .ToList()
            .ForEach(
                classJob => {
                    _gearsetCategories[(byte)classJob.RowId] = classJob.ClassJobCategory.Row switch {
                        30 when classJob.Role == 1 => GearsetCategory.Tank,
                        30 when classJob.Role == 2 => GearsetCategory.Melee,
                        30 when classJob.Role == 3 => GearsetCategory.Ranged,
                        31 when classJob.Role == 3 => GearsetCategory.Caster,
                        31                         => GearsetCategory.Healer,
                        32                         => GearsetCategory.Gatherer,
                        33                         => GearsetCategory.Crafter,
                        _                          => GearsetCategory.None,
                    };
                }
            );
    }

    public GearsetCategory GetCategoryFromJobId(byte jobId)
    {
        return _gearsetCategories.TryGetValue(jobId, out GearsetCategory category)
            ? category
            : GearsetCategory.None;
    }

    public uint GetCategoryColor(GearsetCategory category)
    {
        return category switch {
            GearsetCategory.Tank     => 0xFFA54A3B,
            GearsetCategory.Healer   => 0xFF12613B,
            GearsetCategory.Melee    => 0xFF2E3069,
            GearsetCategory.Ranged   => 0xFF2E3069, // 0xFF2C89A6,
            GearsetCategory.Caster   => 0xFF2E3069, // 0xFFA72A5A,
            GearsetCategory.Crafter  => 0xFFA72A5A,
            GearsetCategory.Gatherer => 0xFF2C89A6,
            _                        => 0xFFFFFF
        };
    }
}