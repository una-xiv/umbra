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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using Umbra.Common;
using Una.Drawing;

namespace Umbra;

[Service]
internal class UmbraColors
{
    public static event Action? OnColorProfileChanged;

    [ConfigVariable("ColorProfileData")] private static string ColorProfileData { get; set; } = "";
    [ConfigVariable("ColorProfileName")] private static string ColorProfileName { get; set; } = "Default";
    private static Dictionary<string, Dictionary<string, uint>> ColorProfiles { get; set; } = [];
    private static Timer? _debounceTimer;

    [WhenFrameworkCompiling]
    private static void Initialize()
    {
        RegisterDefaultColors();

        if (ColorProfileData != "") RestoreColorProfiles();
        if (ColorProfiles.Count == 0) AddDefaultColorProfile();

        _debounceTimer         =  new(1000);
        _debounceTimer.Enabled =  false;
        _debounceTimer.Elapsed += (_, _) => Save();

        Apply(ColorProfileName);
    }

    [WhenFrameworkDisposing]
    private static void Dispose()
    {
        ColorProfiles.Clear();
        _debounceTimer?.Dispose();
    }

    public static void UpdateCurrentProfile()
    {
        _debounceTimer!.Stop();
        _debounceTimer!.Start();
        _debounceTimer!.Enabled = true;
    }

    /// <summary>
    /// Returns the current profile name.
    /// </summary>
    public static string GetCurrentProfileName()
    {
        return ColorProfileName;
    }

    /// <summary>
    /// Applies a color profile with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool Apply(string name)
    {
        if (!ColorProfiles.TryGetValue(name, out Dictionary<string, uint>? colors)) return false;

        ConfigManager.Set("ColorProfileName", name);
        ColorProfileName = name;

        foreach (string id in Color.GetAssignedNames()) {
            if (colors.TryGetValue(id, out uint value)) {
                Color.AssignByName(id, value);
            }
        }

        OnColorProfileChanged?.Invoke();

        return true;
    }

    /// <summary>
    /// Returns a list of available color profiles.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetColorProfileNames()
    {
        return ColorProfiles.Keys.ToList();
    }

    /// <summary>
    /// Generates a sharable string that represents the current color profile, including its name.
    /// </summary>
    public static string Export(string? name = null)
    {
        name ??= ColorProfileName;

        if (!ColorProfiles.TryGetValue(name, out Dictionary<string, uint>? profile)) {
            return "";
        }

        return Encode(
            JsonConvert.SerializeObject(
                new Dictionary<string, Dictionary<string, uint>> { { name, profile } }
            )
        );
    }

    /// <summary>
    /// Imports a color profile from a sharable string.
    /// </summary>
    /// <param name="data">The data that contains a color profile.</param>
    /// <param name="overwrite">Whether to overwrite a color profile with the same name.</param>
    /// <param name="newName">A new profile name if overwrite is false and one already exists.</param>
    /// <returns></returns>
    public static ImportResult Import(string data, bool overwrite, string? newName)
    {
        try {
            string json          = Decode(data);
            var    colorProfiles = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, uint>>>(json);

            if (null == colorProfiles) return ImportResult.InvalidFormat;

            switch (colorProfiles.Count) {
                case 0:
                    return ImportResult.NoProfileInData;
                case > 1:
                    return ImportResult.InvalidFormat;
            }

            string                   name    = colorProfiles.Keys.First();
            Dictionary<string, uint> profile = colorProfiles[name];

            name = string.IsNullOrEmpty(newName) ? name : newName;

            if (ColorProfiles.ContainsKey(name) && !overwrite) {
                return ImportResult.DuplicateName;
            }

            ColorProfiles[name] = profile;
            PersistColorProfiles();
            Apply(name);

            return ImportResult.Success;
        } catch (Exception e) {
            Logger.Warning($"Failed to import color profile: {e.Message}");
            return ImportResult.InvalidFormat;
        }
    }

    /// <summary>
    /// Saves the current color profile. Writes the current assigned colors to a
    /// profile with the given name, or the current profile if the name
    /// is omitted.
    /// </summary>
    public static void Save(string? name = null)
    {
        name ??= ColorProfileName;

        Dictionary<string, uint> colors = [];

        foreach (string id in Color.GetAssignedNames()) {
            colors.Add(id, Color.GetNamedColor(id));
        }

        ColorProfiles[name] = colors;
        PersistColorProfiles();

        if (ColorProfileName != name) Apply(name);

        _debounceTimer.Enabled = false;
        _debounceTimer.Stop();
    }

    /// <summary>
    /// Deletes the profile with the given name.
    /// </summary>
    /// <param name="name"></param>
    public static void Delete(string name)
    {
        if (name == "Default" || !ColorProfiles.ContainsKey(name)) return;

        ColorProfiles.Remove(name);
        PersistColorProfiles();

        if (ColorProfileName == name) Apply("Default");
    }

    private static void AddDefaultColorProfile()
    {
        Dictionary<string, uint> colors = [];

        foreach (string id in Color.GetAssignedNames()) {
            colors.Add(id, Color.GetNamedColor(id));
        }

        ColorProfiles.TryAdd("Default", colors);
        ConfigManager.Set("ColorProfileName", "Default");
    }

    /// <summary>
    /// Restores the color profiles from the user configuration.
    /// </summary>
    private static void RestoreColorProfiles()
    {
        if (ColorProfileData == "") return;

        try {
            string json = Decode(ColorProfileData);
            var    data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, uint>>>(json);

            if (null == data) return;

            ColorProfiles = data;
        } catch (Exception e) {
            Logger.Error($"Failed to restore color profiles: {e.Message}");
        }
    }

    /// <summary>
    /// Writes the current color profiles to the user configuration.
    /// </summary>
    private static void PersistColorProfiles()
    {
        string json = JsonConvert.SerializeObject(ColorProfiles);
        ConfigManager.Set("ColorProfileData", Encode(json));
    }

    private static string Encode(string text)
    {
        byte[]    bytes  = Encoding.UTF8.GetBytes(text);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(output, CompressionLevel.SmallestSize)) {
            deflateStream.Write(bytes, 0, bytes.Length);
        }

        bytes = output.ToArray();

        return Convert.ToBase64String(bytes);
    }

    private static string Decode(string text)
    {
        byte[]    bytes  = Convert.FromBase64String(text);
        using var input  = new MemoryStream(bytes);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress)) {
            deflateStream.CopyTo(output);
        }

        return Encoding.UTF8.GetString(output.ToArray());
    }

    public enum ImportResult
    {
        InvalidFormat,
        NoProfileInData,
        DuplicateName,
        Success
    }

    private static void RegisterDefaultColors()
    {
        Color.AssignByName("Window.Background",                   0xFF212021);
        Color.AssignByName("Window.BackgroundLight",              0xFF292829);
        Color.AssignByName("Window.Border",                       0xFF484848);
        Color.AssignByName("Window.TitlebarBackground",           0xFF101010);
        Color.AssignByName("Window.TitlebarBorder",               0xFF404040);
        Color.AssignByName("Window.TitlebarGradient1",            0xFF2F2E2F);
        Color.AssignByName("Window.TitlebarGradient2",            0xFF1A1A1A);
        Color.AssignByName("Window.TitlebarText",                 0xFFD0D0D0);
        Color.AssignByName("Window.TitlebarTextOutline",          0xC0000000);
        Color.AssignByName("Window.TitlebarCloseButton",          0xFF101010);
        Color.AssignByName("Window.TitlebarCloseButtonBorder",    0xFF404040);
        Color.AssignByName("Window.TitlebarCloseButtonHover",     0xFF304090);
        Color.AssignByName("Window.TitlebarCloseButtonX",         0xFFD0D0D0);
        Color.AssignByName("Window.TitlebarCloseButtonXHover",    0xFFFFFFFF);
        Color.AssignByName("Window.TitlebarCloseButtonXOutline",  0xFF000000);
        Color.AssignByName("Window.ScrollbarTrack",               0xFF212021);
        Color.AssignByName("Window.ScrollbarThumb",               0xFF484848);
        Color.AssignByName("Window.ScrollbarThumbHover",          0xFF808080);
        Color.AssignByName("Window.ScrollbarThumbActive",         0xFF909090);
        Color.AssignByName("Window.Text",                         0xFFD0D0D0);
        Color.AssignByName("Window.TextLight",                    0xFFFFFFFF);
        Color.AssignByName("Window.TextMuted",                    0xB0C0C0C0);
        Color.AssignByName("Window.TextOutline",                  0xC0000000);
        Color.AssignByName("Window.TextDisabled",                 0xA0A0A0A0);
        Color.AssignByName("Window.AccentColor",                  0xFF4c8eb9);
        Color.AssignByName("Input.Background",                    0xFF151515);
        Color.AssignByName("Input.Border",                        0xFF404040);
        Color.AssignByName("Input.Text",                          0xFFD0D0D0);
        Color.AssignByName("Input.TextMuted",                     0xA0D0D0D0);
        Color.AssignByName("Input.TextOutline",                   0xC0000000);
        Color.AssignByName("Input.BackgroundHover",               0xFF212021);
        Color.AssignByName("Input.BorderHover",                   0xFF707070);
        Color.AssignByName("Input.TextHover",                     0xFFFFFFFF);
        Color.AssignByName("Input.TextOutlineHover",              0xFF000000);
        Color.AssignByName("Input.BackgroundDisabled",            0xE0212021);
        Color.AssignByName("Input.BorderDisabled",                0xC0404040);
        Color.AssignByName("Input.TextDisabled",                  0xA0A0A0A0);
        Color.AssignByName("Input.TextOutlineDisabled",           0xC0000000);
        Color.AssignByName("Toolbar.InactiveBackground1",         0xC02A2A2A);
        Color.AssignByName("Toolbar.InactiveBackground2",         0xC01F1F1F);
        Color.AssignByName("Toolbar.Background1",                 0xFF2F2E2F);
        Color.AssignByName("Toolbar.Background2",                 0xFF1A1A1A);
        Color.AssignByName("Toolbar.InactiveBorder",              0xA0484848);
        Color.AssignByName("Toolbar.Border",                      0xFF484848);
        Color.AssignByName("Widget.Background",                   0xFF101010);
        Color.AssignByName("Widget.BackgroundDisabled",           0xFF2C2C2C);
        Color.AssignByName("Widget.BackgroundHover",              0xFF2F2F2F);
        Color.AssignByName("Widget.Border",                       0xFF484848);
        Color.AssignByName("Widget.BorderDisabled",               0xFF484848);
        Color.AssignByName("Widget.BorderHover",                  0xFF8A8A8A);
        Color.AssignByName("Widget.Text",                         0xFFD0D0D0);
        Color.AssignByName("Widget.TextDisabled",                 0xA0D0D0D0);
        Color.AssignByName("Widget.TextHover",                    0xFFFFFFFF);
        Color.AssignByName("Widget.TextMuted",                    0xFF909090);
        Color.AssignByName("Widget.TextOutline",                  0xC0000000);
        Color.AssignByName("Widget.PopupBackground",              0xFF101010);
        Color.AssignByName("Widget.PopupBackground.Gradient1",    0xFF2F2E2F);
        Color.AssignByName("Widget.PopupBackground.Gradient2",    0xFF1A1A1A);
        Color.AssignByName("Widget.PopupBorder",                  0xFF484848);
        Color.AssignByName("Widget.PopupMenuText",                0xFFD0D0D0);
        Color.AssignByName("Widget.PopupMenuTextMuted",           0xFFB0B0B0);
        Color.AssignByName("Widget.PopupMenuTextDisabled",        0xFF808080);
        Color.AssignByName("Widget.PopupMenuTextHover",           0xFFFFFFFF);
        Color.AssignByName("Widget.PopupMenuBackgroundHover",     0x802F5FFF);
        Color.AssignByName("Widget.PopupMenuTextOutline",         0xA0000000);
        Color.AssignByName("Widget.PopupMenuTextOutlineHover",    0xA0000000);
        Color.AssignByName("Widget.PopupMenuTextOutlineDisabled", 0x30000000);
    }
}
