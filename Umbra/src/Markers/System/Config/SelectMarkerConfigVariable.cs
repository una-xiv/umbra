/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Markers;

public class SelectMarkerConfigVariable(
    string                     id,
    string                     name,
    string?                    description,
    string                     defaultValue,
    Dictionary<string, string> options,
    bool                       allowCustom = false
) : MarkerConfigVariable<string>(id, name, description, defaultValue)
{
    public Dictionary<string, string> Options { get; set; } = options;

    /// <inheritdoc/>
    protected override string Sanitize(object? value)
    {
        if (value is not string str) {
            return string.Empty;
        }

        if (!allowCustom && !Options.ContainsKey(str)) {
            Logger.Warning($"Invalid value for {Id} in {Name} ({value}) Available options: {string.Join(", ", Options.Keys)}");
            return DefaultValue;
        }

        return str;
    }
}
