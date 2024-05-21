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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Umbra.Common;

public static partial class ConfigManager
{
    public static Action<string>? CvarChanged;

    /// <summary>
    /// Sets the value of a config variable.
    /// </summary>
    /// <param name="id">The ID of the variable.</param>
    /// <param name="value">The new value.</param>
    /// <param name="persist">Whether to save the changes to disk. (defaults to true)</param>
    public static void Set(string id, object? value, bool persist = true)
    {
        if (!Cvars.TryGetValue(id, out Cvar? cvar)) {
            throw new($"Config variable {id} does not exist.");
        }

        foreach (var prop in cvar.Properties) {
            if (prop.PropertyType.IsEnum) {
                value = Enum.Parse(prop.PropertyType, value!.ToString()!);
            } else if (prop.PropertyType == typeof(int)
                       && value is long longValue) {
                value = (int)longValue;
            } else if (prop.PropertyType == typeof(float)
                       && value is double doubleValue) {
                value = (float)doubleValue;
            } else if (prop.PropertyType == typeof(bool)
                       && value is int intValue) {
                value = intValue != 0;
            } else if (prop.PropertyType == typeof(uint)
                       && value is not uint) {
                value = Convert.ToUInt32(value);
            }

            prop.SetValue(null, value);
        }

        if (cvar.Value == value) return;

        cvar.Value = value;
        CvarChanged?.Invoke(id);

        if (!_isInitialLoad && cvar.RequiresRestart) {
            _debounceTimer?.Dispose();
            Persist();
            Framework.DalamudFramework.Run(
                async () => {
                    await Task.Delay(250);
                    Framework.Restart();
                });
            return;
        }

        if (persist) {
            _debounceTimer?.Dispose();
            _debounceTimer = new (Persist, null, 1000, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Returns a config variable with the given name.
    /// </summary>
    /// <param name="id">The ID of the variable.</param>
    /// <typeparam name="T">The type of the variable.</typeparam>
    /// <exception cref="Exception">If the variable does not exist.</exception>
    public static T? Get<T>(string id)
    {
        if (!Cvars.TryGetValue(id, out Cvar? cvar)) {
            throw new($"Config variable {id} does not exist.");
        }

        return (T?)cvar.Value;
    }
}
