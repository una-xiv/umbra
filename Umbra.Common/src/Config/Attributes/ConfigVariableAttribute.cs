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

namespace Umbra.Common;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ConfigVariableAttribute(string id, string? category = null, string? name = null, string? description = null, float min = float.MinValue, float max = float.MaxValue, float step = 0) : Attribute
{
    /// <summary>
    /// The ID of the config variable.
    /// </summary>
    /// <remarks>
    /// This represents the key used to store the value in the configuration
    /// file and should solely consist of alphanumeric characters, periods
    /// and underscores.
    /// </remarks>
    public string Id { get; } = id;

    /// <summary>
    /// The UI category of the config variable. This is used for display
    /// purposes in the configuration UI. A value of NULL results in the
    /// variable not being visible in the UI.
    /// </summary>
    public string? Category { get; set; } = category;
    
    /// <summary>
    /// The human-readable name of the config variable. This is used for
    /// display purposes in the configuration UI. A NULL value results in
    /// the variable not being visible in the UI.
    /// </summary>
    public string? Name { get; set; } = name;

    /// <summary>
    /// The description of the config variable. This is used for display
    /// purposes in the configuration UI.
    /// </summary>
    public string? Description { get; set; } = description;

    /// <summary>
    /// The minimum value of the config variable. This is used for input
    /// validation in the configuration UI.
    /// </summary>
    public float Min { get; set; } = min;

    /// <summary>
    /// The maximum value of the config variable. This is used for input
    /// validation in the configuration UI.
    /// </summary>
    public float Max { get; set; } = max;

    /// <summary>
    /// The step value of the config variable. This is used for input
    /// validation in the configuration UI.
    /// </summary>
    public float Step { get; set; } = step;
}
