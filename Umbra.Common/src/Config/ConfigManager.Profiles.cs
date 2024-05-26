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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;

namespace Umbra.Common;

public static partial class ConfigManager
{
    public const string DefaultProfileName = "Default";

    public static Action<string>? CurrentProfileChanged;

    private static string                    ActiveProfileName   { get; set; } = DefaultProfileName;
    private static Dictionary<ulong, string> ProfileAssociations { get; set; } = [];
    private static List<string>              ProfileNames        { get; set; } = [];

    public static List<string> GetProfileNames()
    {
        if (ProfileNames.Count == 0) {
            // There is always one.
            ProfileNames = ConfigDirectory
                .GetFiles("*.profile.json")
                .Select(file => file.Name[..^13])
                .ToList();
        }

        return ProfileNames.ToList(); // Deref.
    }

    /// <summary>
    /// Returns the name of the current profile.
    /// </summary>
    /// <returns></returns>
    public static string GetActiveProfileName()
    {
        return ActiveProfileName;
    }

    /// <summary>
    /// Exports the current profile to a base64-encoded string.
    /// </summary>
    /// <returns></returns>
    public static string ExportProfile()
    {
        var data = new Dictionary<string, object?>();

        foreach (var cvar in Cvars.Values) {
            data[cvar.Id] = cvar.Value;
        }

        string json  = JsonConvert.SerializeObject(data);
        byte[] bytes = Deflate(System.Text.Encoding.UTF8.GetBytes(json));

        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Imports a base64-encoded profile string.
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="data"></param>
    public static void ImportProfile(string profileName, string data)
    {
        if (profileName != ActiveProfileName) {
            SetProfile(profileName);
        }

        byte[] bytes    = Inflate(Convert.FromBase64String(data));
        string json     = System.Text.Encoding.UTF8.GetString(bytes);
        var    dataDict = JsonConvert.DeserializeObject<Dictionary<string, object?>>(json);

        if (null == dataDict) {
            Logger.Error("Failed to import profile.");
            return;
        }

        // Prevent restarting...
        _isInitialLoad = true;

        foreach ((string id, object? value) in dataDict) {
            if (Cvars.ContainsKey(id)) {
                Set(id, value);
                Logger.Debug($"Imported '{id}' with value: {value}");
            }
        }

        _isInitialLoad = false;
    }

    /// <summary>
    /// Copies the data from the given profile to the current one.
    /// </summary>
    /// <param name="profileName"></param>
    public static void CopyFromProfile(string profileName)
    {
        if (ActiveProfileName == profileName) return;

        if (!FileExists($"{profileName}.profile.json")) {
            Logger.Error($"Profile {profileName} does not exist.");
            return;
        }

        string sourcePath = Path.Combine(ConfigDirectory.FullName, $"{profileName}.profile.json");
        string destPath   = Path.Combine(ConfigDirectory.FullName, $"{ActiveProfileName}.profile.json");

        File.Copy(sourcePath, destPath, true);
        ProfileNames.Clear();

        LoadWithoutRestart();
    }

    /// <summary>
    /// <para>
    /// Deletes the current profile.
    /// </para>
    /// <para>
    /// Reverts back to the default profile after deletion.
    /// </para>
    /// </summary>
    /// <param name="profileName"></param>
    public static void DeleteProfile(string profileName)
    {
        if (!FileExists($"{profileName}.profile.json")) {
            Logger.Error($"Profile {profileName} does not exist.");
            return;
        }

        if (profileName == DefaultProfileName) {
            Logger.Error("Cannot delete the default profile.");
            return;
        }

        File.Delete(Path.Combine(ConfigDirectory.FullName, $"{profileName}.profile.json"));
        ProfileNames.Clear();

        if (profileName == ActiveProfileName) {
            SetProfile(DefaultProfileName);
        }
    }

    /// <summary>
    /// <para>
    /// Sets the current profile to the given profile name.
    /// </para>
    /// <para>
    /// Copies the current profile to the new profile if it does not already exist.
    /// </para>
    /// </summary>
    /// <param name="profileName"></param>
    public static void SetProfile(string profileName)
    {
        if (ActiveProfileName == profileName) return;

        string lastProfile = ActiveProfileName;
        ActiveProfileName = profileName;

        ProfileAssociations[Framework.LocalCharacterId] = profileName;
        WriteFile("profiles.json", ProfileAssociations);

        if (!FileExists($"{profileName}.profile.json")) {
            CopyFromProfile(lastProfile);
        }

        LoadWithoutRestart();
        CurrentProfileChanged?.Invoke(profileName);
    }

    /// <summary>
    /// Loads the "profiles.json" file from the config directory which contains
    /// profile names associated with player characters. A new profile-db file
    /// is created if it does not already exist.
    /// </summary>
    private static void RestoreProfileAssociations()
    {
        if (!Framework.DalamudPlugin.ConfigDirectory.Exists) {
            Framework.DalamudPlugin.ConfigDirectory.Create();
        }

        if (!FileExists("profiles.json")) {
            ProfileAssociations.Add(Framework.LocalCharacterId, "Default");
            ActiveProfileName = ProfileAssociations[Framework.LocalCharacterId];
            WriteFile("profiles.json", ProfileAssociations);
            return;
        }

        ProfileAssociations = ReadFile<Dictionary<ulong, string>>("profiles.json") ?? new();

        if (ProfileAssociations.TryAdd(Framework.LocalCharacterId, "Default")) {
            WriteFile("profiles.json", ProfileAssociations);
        }

        ActiveProfileName = ProfileAssociations[Framework.LocalCharacterId];
        CurrentProfileChanged?.Invoke(ActiveProfileName);
    }

    /// <summary>
    /// Returns the file name of the currently active profile.
    /// </summary>
    /// <returns></returns>
    private static string GetCurrentConfigFileName()
    {
        return $"{ActiveProfileName}.profile.json";
    }

    /// <summary>
    /// Deflates a byte array using zlib compression.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] Deflate(byte[] data)
    {
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(output, CompressionLevel.SmallestSize)) {
            deflateStream.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    /// <summary>
    /// Inflates a byte array using zlib compression.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] Inflate(byte[] data)
    {
        using var input  = new MemoryStream(data);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress)) {
            deflateStream.CopyTo(output);
        }

        return output.ToArray();
    }
}
