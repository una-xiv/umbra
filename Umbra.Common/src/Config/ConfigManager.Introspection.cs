/* Umbra.Common | (c) 2024 by Una       ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Common is free software: you can       \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Collections.Generic;
using System.Linq;

namespace Umbra.Common;

public static partial class ConfigManager
{
    /// <summary>
    /// Returns a <see cref="Cvar"/> instance with the given ID.
    /// </summary>
    public static Cvar? GetCvar(string id)
    {
        return Cvars.GetValueOrDefault(id);
    }

    /// <summary>
    /// Returns a list of category names inferred from all registered config
    /// variables. Only categories with localized names are included in the
    /// result.
    /// </summary>
    public static List<string> GetCategories()
    {
        return Cvars
            .Values
            .Select(cvar => cvar.Category)
            .Where(c => c != null && I18N.Has($"CVAR.Group.{c}"))
            .Distinct()
            .OrderBy(c => c)
            .ToList()!;
    }

    /// <summary>
    /// Returns a list of <see cref="Cvar"/> instances from the given category.
    /// Only variables with localized names are included in the result.
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static List<Cvar> GetVariablesFromCategory(string category)
    {
        return Cvars
            .Values
            .Where(cvar => cvar.Category == category && I18N.Has($"CVAR.{cvar.Id}.Name") && cvar.Category != null)
            .ToList();
    }
}
