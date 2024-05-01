/* Umbra.Drawing | (c) 2024 by Una      ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Drawing is free software: you can       \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Umbra.Common;

[AttributeUsage(AttributeTargets.Property)]
public class ConfigVariableAttribute(
    string    id,
    string?   category        = null,
    string?   subCategory     = null,
    float     min             = float.MinValue,
    float     max             = float.MaxValue,
    float     step            = 0,
    string[]? options         = null,
    bool      requiresRestart = false
) : Attribute
{
    /// <summary>
    /// The ID of the config variable used to store the value in the
    /// configuration file and should solely consist of alphanumeric
    /// characters, periods and underscores.
    /// </summary>
    /// <remarks>
    /// If this config variable should be shown in the settings UI, a
    /// translation of it should be available in the localization files under
    /// the name "CVAR.{Id}.Name" and "CVAR.{Id}.Description".
    /// </remarks>
    public string Id { get; } = id;

    /// <summary>
    /// The UI category of the config variable. This is used for display
    /// purposes in the configuration UI. A value of NULL results in the
    /// variable not being visible in the UI.
    /// </summary>
    /// <remarks>
    /// This name should correspond to a translation key in the localization
    /// files that is formatted as "CVAR.Group.{Category}".
    /// </remarks>
    public string? Category { get; } = category;

    /// <summary>
    /// The UI subcategory of the config variable. This is used for display
    /// purposes in the configuration UI to group variables together. A value
    /// of NULL results the variable to be placed at the top level of the
    /// panel if the category is not NULL.
    /// </summary>
    /// <remarks>
    /// This name should correspond to a translation key in the localization
    /// files that is formatted as "CVAR.SubGroup.{Category}".
    /// </remarks>
    public string? SubCategory { get; } = subCategory;

    /// <summary>
    /// The minimum value of the config variable. This is used for input
    /// validation in the configuration UI.
    /// </summary>
    public float Min { get; } = min;

    /// <summary>
    /// The maximum value of the config variable. This is used for input
    /// validation in the configuration UI.
    /// </summary>
    public float Max { get; } = max;

    /// <summary>
    /// The step value of the config variable. This is used for input
    /// validation in the configuration UI.
    /// </summary>
    public float Step { get; } = step;

    /// <summary>
    /// A list of options for the config variable. This is used for select
    /// input types in the configuration UI.
    /// </summary>
    public List<string> Options = options?.ToList() ?? [];

    /// <summary>
    /// Restarts the plugin when the config variable is changed.
    /// </summary>
    public bool RequiresRestart { get; } = requiresRestart;
}
