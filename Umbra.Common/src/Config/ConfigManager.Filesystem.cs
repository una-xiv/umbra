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

using Newtonsoft.Json;

namespace Umbra.Common;

public static partial class ConfigManager
{
    private static DirectoryInfo ConfigDirectory => new(Framework.DalamudPlugin.ConfigDirectory.FullName);

    /// <summary>
    /// Returns true if a file with the given name exists in the plugin's
    /// config directory.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    private static bool FileExists(string fileName)
    {
        return new FileInfo(Path.Combine(ConfigDirectory.FullName, fileName)).Exists;
    }

    /// <summary>
    /// Reads a JSON file from the config directory.
    /// </summary>
    /// <param name="fileName">The name of the JSON file.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static T? ReadFile<T>(string fileName)
    {
        FileInfo file = new(Path.Combine(ConfigDirectory.FullName, fileName));

        return file.Exists
            ? JsonConvert.DeserializeObject<T>(File.ReadAllText(file.FullName))
            : default;
    }

    /// <summary>
    /// Writes a JSON file to the config directory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private static void WriteFile<T>(string fileName, T obj)
    {
        FileInfo file = new(Path.Combine(ConfigDirectory.FullName, fileName));
        File.WriteAllText(file.FullName, JsonConvert.SerializeObject(obj, Formatting.Indented));
    }
}
