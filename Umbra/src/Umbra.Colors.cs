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
    [ConfigVariable("ColorProfileName")] private static string ColorProfileName { get; set; } = "Umbra (built-in)";
    private static Dictionary<string, Dictionary<string, uint>> ColorProfiles { get; set; } = [];
    private static Timer? _debounceTimer;

    private static Dictionary<string, string> BuiltInProfiles { get; } = new() {
        { "Umbra (built-in)", "jZXLbtswEEX/xevCIIfkkPQuD6AN0KAFaqDdyrbqCFVFQ5HSAkH+vbRk8yFSajZe+B7M485w9Lq6L38Wfd2tNq+r71VzMH/Wt8X+17E1fXNYbTgowpBozT6k8ufq+NSNjFKSMekZ0x7KdpBAg6QSnbSturrcFW2cRWqwP6hSLIjECVEMEuRjWxyqsunoQFELEi1mKRjTaUJQp+m25d+hI02VFAR4FvjSd3XVlKsNA6AAgsu0qLvaPJe3fdeZZrG/gHtHqwH9ybxcYMoEMIZL8I/FnkLQhdVco4SMkSHtnLDNKWo9VcTx3/atqeuzZa0ddXaVPPLU/95l1yVGfNOIlArFZ7ibfVe9DGUpiWiLC6y8ThisIDiJBLfRafdWfey70m4raBQcYBrUeWELk2gnTnik31fPxa4eAqDmSlEeJL/Z7+1u3pnajO0xIJIJ69RDc+q79fSxCOsR82pmcUYlXWf//7Ub+wwI04JEYnbFp7X4abjBhgV5WQihlMYwQWbPkuyO8bs1LcGbyiQSOEeScRUBce6DIcQ25MeS1BKFcY5sjTnv3PqhKYaN84XRAWTaHkaplkAYQUBBhPBgFCk8aykwuWhJqst2ACpNNEWBQZDsnT4cy3Tl/P2ayN6Zc5lEcT0+jAkW3CubjAnqmYUiJiOcRXx0accoKHd67qI7IRq+ewYBkD2HTr08ofjKOHXmMzHoX82pP/3H4gmznv3OLdLJ9y6g540f9Mey6WftiwhvhJaokas8Fg0yuOApOe+7w5LVAmqfkFKcinxMNw/7EjgTXCBZBK9h30f73hQRjCBD9fb2Dw==" },
        { "Metal (built-in)", "jZXBbtswDIbfJeeikChKFHNrO2ArsGIDVmC7ponRGvPswLW7AUXffbSbSHIie0PRHvp/ISnyJ/O6uiu6TbVav66+l/Wu+X15vdn+fGybvt6t1ggsP0QaLs7lz+XjUzcyhjQB+Mg07a5oB8k7i2gVBum+7KriYdOeZDHOKufcORYjsSayWp8hH9vNrizqTi8GOlIwUtoRObRn1H3xZ3yRV2yN8S4LfOm7qqyL1doqY7RD4jPspmqei+u+65p6tdbIzJJviTq+83/YT83LiAJqqxHMYvofi89JwUNYoQGlg2AX6dCEoZUEOtLftm1TVUOvWpnx+1C0Z29dBnnqfz285xSbaZpDYm3OaW09znBX2658KUaQnEOg6Ns4Wg/KJvUOQrAysWdG5SfqXd8VYlNg62QXFE4/Gzth2aO3PFE/lM+bh2r8uPNsLYCiAFxtt2LJm6Zq3h+nwKEs08Xqtt733ekmohH/Wx/UsBqsvSJGPCrhqaGp8f+Ht4io5Z3yNxUzQz2tJEyCkZW2CqblJINSkgApDR9EkuRKeZXJfTT3bAGxoQjgDFrNJzVEwiqy4gEDaaLJRLxnKYcyhURMyrxvmsFjl7f1ZnRYLEcPYZgVw1jHPAh5cBIJRRILoHY5APJASHXwwwDwcBs4CRK9guxInDx4cPdYTE0mx9N5lGOhMnLSeCVXB0C7HJYa5CRT9pYnUjra9CAkSPQXSTfl/AU92W9Pls1ESGfOCgZdTYBoTZb9Np4narI0JA9T0+Bha8BqZ/3hII/y12bf79MOayRDzsAccZl+myXD/gcNC/T86Ef9rqj72DtpgPzmidgFJ9tpiPLYZIrJuT4n560SsKyvpInem3zMMAy1qB+iLUPJCXh7+ws=" },
        { "Clear Blue (built-in)", "hZXbbtswDIbfJddFQZESJfauB2ArsGIDVmC7TRMvNebZhWt3A4q++2g3s6z4MCBX4SeK/PlTft3cZD+2bdFsLl433/JyX/0+v9rufh7qqi33mwsCNk7Ygz+bhj/lh0c9iCxEDlhCZKp6n9WbC4tBvDPEOITu86bIHrb1+BYDDjmEKfP/NB/q7T7PysYoBeQQIIBbpLCn0HixwU6o++xP010mJngHOA98bpsiLzOVBtEgOuunRV0X1XN21TZNVS43N4Jinwa8W833sXo5KmK0WyJeg7+vdjMGh7RiddoobpUeNNAagjirZwb+666uiqITq9YJq0yBWYC0rxnksf310N8KCE5EFpBYnSFkZ2GBu9w1+Us2Cw6zVYcwG5MEjkae616jd22TqUmxWwP9iU+iUQvjPKMXsEn8Jn/ePhR9AlbXBWNHVV3udurK66qo3mdKCJ4cnW1uy6e2STbRggmAHrqJH6Mz2/EemRo5/v+vGxYBQSPJoVlzn9YSLeiDRbKUFhTDzrkQhMcXzPhscvvA+GCk2+dpCVFUvR6AAlhJqxgRXR8qTirD/FgmtSRpBkXuq6rz3Pltue0dFwszPcjBkzG8BmIPShBryUcwyTR+qqYApi/e5KqjO0gNq4WDjJPMvqv7Q3ZqOWJwzCQz4URACl7IzWHRDYG8Nmwjs1LEyQj1BifGqdtOkGgnNMGE983q43EFpHtVMQkkmYNnnURIgNnncIgeV6hLzt6zTc+OHkdGUGuhH+Jfqqf2aSwxInkwnjwuMefpF2707Vql8fR7OKKXhe/jd1nZzn0Kp8QghCZiGQuRYFFuJbvH14WFhMu6D9ictdiQQbOQc5hHoBC4f05WsGNSXWFLzjpep2NnDoiMKgBvb38B" },
        { "YoRHa Dark (built-in)", "jZXbattAEIbfxdclzGFnD77LAdpAQwsNpLeOLRIRVQqKlBZC3r1r2dldWSu1t/4/zc7hn/Hb6mrTPtFq/ba6K+td8/vsYrN9emibvt6t1oosAqFy/Gkqfy0fHruB8QQCJEzT7op2kFhIGVRBui27qrjftKevWKMtyBT7d6TP7WZXFnWH/0XR4TmnmLWZULfFn6Eih4rYx8kC3/quKutitWa21jmjmSbcZdW8FBd91zX1ao0gpK1dgmKdCEYW431pXvckkdFOO8dmCf45VAPEYmUZPIb1tHLakJNFOvQAAvZj2zZVte9R62eb9U5EHvtf94d5OT96gBkkJuVDGTRqhjvfduVrkQXDSIG1N6qMhGBhx6RRNI/Um74rDvb0jmJOx5zagNAXyhbsSL4qXzb31f57/y6jZhEdgPPt1nvxsqmadmy36/q572ZX8KhmduKgfNRqjShEY9PfM8VEMdQiPlkDDmmaSphFXNY0nygr612MJo0f5whGCzBnHv/wNSowSGxgmkLSUyJBv3nuJItIWMcKmADSp5IAosUQg1KZVCLmv75tmr3Pzq7rzeCymI6/OPu9ZhBleQmkATRDyjqCo0jpGZ0Cx7MFPojVmaeOliDvRTKO9kchBIl2CZO5K3cPRbdw6k/k2BAS7YwSpTGDRQ8k/wgHZiGJk8GlNYyQGJ0NETgX9KnvEyGZuVJGOW+uMRDjOl8dah6pcW/8Cvu0xt+GxUHnb4hxVkHQvzfP/fOkxccJ5pmz8Z/ZaRfnaFqIPd/4Qb8p6n62fSNivhEjLGm3QfF3VazKk8lRsGQFMgFz1tIaLSDnY8Z5AGj/x+w0LIIfR4dRSMSCXaRjbZqNsCKw7+9/AQ==" },
        { "YoRHa Light (built-in)", "jZXJbtswEIbfxefCIGe4DH3LAjQBGrRADbRX2VIdoYpoKFJaIMi7l/IikhIlFL7p/zyc5R/yfXVf/Mq6ql1t3lc/yjq3f9a32f73obFdna82AgwDlCT1p6n8pTw8t2eGgCQjz9gmL5peIk5aEZODtC3bqthlTXzKPOYjgWCMECbI5ybLy6Ju+WKgKwUnCrlBZabUtvibrigEvnZtVdbFasPBcBJSJZK6q+xrcdu1ra0dxyQooiXI18mZlovxHuzbtbcoAVEtwT+TM0yBQ1gjjNKQaE5IDz1gA/Z939iq6nvUuNkmz/XIc/eyS84rRnytbmLCEMxwN/u2fCvSBriMNCkMFtbEjbMXi9Snri3y5OmRDYATZ0ihW5x8X75mu6r/PygjiLgQPvrNfu+8eGcrO6rusT527WgFibT7GRrUwSuGCXTuk1dlWqv/7ovhKIwROhS9pZXWGricZhLYLg5+TsfLo2z78N5bTCvJEBNnXxjggmkOqNk0Bd/ScEPDLDxBBgVDYCw8Kj2TSSoec+rW2t5m68c6O5nMp8NPYZQxYPrlngfhDLr7C1F7MIoUVjQFIF6nyVEXR4CzImgD0SnJuzg/FG3qpkeBCdk3BJ1XFQMyOoEFYyYjEZlnFpIYDS6sIUKCu0ADMGMGPbXigxBEFm5WqIXWEeDjuqhui2M1WBtGSvJYja/B0+dv9tgdx531r0mKWUdP2H/TML5jA3q+3yf9qai72a5FRFA/uEeTTBoLV5PcDnNQIk2GJknU2WOLl04q5twYRvol2jIULP/Hxz8=" },
    };

    [WhenFrameworkCompiling]
    private static void Initialize()
    {
        RegisterDefaultColors();

        if (ColorProfileData != "") RestoreColorProfiles();
        AddBuiltInColorProfiles();

        if (!ColorProfiles.ContainsKey(ColorProfileName)) {
            Apply("Umbra (built-in)");
        } else {
            Apply(ColorProfileName);
        }

        _debounceTimer         =  new(1000);
        _debounceTimer.Enabled =  false;
        _debounceTimer.Elapsed += (_, _) => Save();

        ConfigManager.CurrentProfileChanged += OnConfigProfileChanged;
    }

    [WhenFrameworkDisposing]
    private static void Dispose()
    {
        ColorProfiles.Clear();
        _debounceTimer?.Dispose();

        ConfigManager.CurrentProfileChanged -= OnConfigProfileChanged;
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
    /// Returns true if the current profile is a built-in profile.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool IsBuiltInProfile(string name)
    {
        return BuiltInProfiles.ContainsKey(name);
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
    /// <param name="apply">Whether to apply the imported theme immediately.</param>
    /// <returns></returns>
    public static ImportResult Import(string data, bool overwrite, string? newName, bool apply = true)
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

            if (ColorProfiles.ContainsKey(name) && (!overwrite || IsBuiltInProfile(name))) {
                return ImportResult.DuplicateName;
            }

            ColorProfiles[name] = profile;
            PersistColorProfiles();

            if (apply) Apply(name);

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

        if (_debounceTimer is not null) {
            _debounceTimer.Enabled = false;
            _debounceTimer.Stop();
        }
    }

    /// <summary>
    /// Deletes the profile with the given name.
    /// </summary>
    /// <param name="name"></param>
    public static void Delete(string name)
    {
        if (IsBuiltInProfile(name) || !ColorProfiles.ContainsKey(name)) return;

        ColorProfiles.Remove(name);
        PersistColorProfiles();

        if (ColorProfileName == name) Apply("Umbra (built-in)");
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

    public static void RegisterDefaultColors()
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
        Color.AssignByName("Widget.TextOutline",                  0x80000000);
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
        Color.AssignByName("Role.Tank",                           0xAFA54A3B);
        Color.AssignByName("Role.Healer",                         0xAF12613B);
        Color.AssignByName("Role.MeleeDps",                       0xAF2E3069);
        Color.AssignByName("Role.PhysicalRangedDps",              0xAF2E3069);
        Color.AssignByName("Role.MagicalRangedDps",               0xAF2E3069);
        Color.AssignByName("Role.Crafter",                        0xAFA72A5A);
        Color.AssignByName("Role.Gatherer",                       0xAF2C89A6);
    }

    private static void OnConfigProfileChanged(string _)
    {
        Apply(ColorProfileName);
    }

    private static void AddBuiltInColorProfiles()
    {
        foreach ((string name, string data) in BuiltInProfiles) {
            Import(data, true, name, false);
        }
    }
}
