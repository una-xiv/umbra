/* Umbra.Interface | (c) 2024 by Una    ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Interface is free software: you can    \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Interface is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Umbra.Common;

namespace Umbra.Interface;

public static class Theme
{
    [ConfigVariable("Theme")] private static string ThemeColors { get; set; } = "{}";

    private static readonly Dictionary<ThemeColor, uint> Colors = new() {
        { ThemeColor.Background, 0xFF212021 },
        { ThemeColor.BackgroundDark, 0xFF1A1A1A },
        { ThemeColor.BackgroundLight, 0xFF2F2F2F },
        { ThemeColor.BackgroundActive, 0xFF3F3F3F },
        { ThemeColor.Border, 0xFF3F3F3F },
        { ThemeColor.BorderDark, 0xFF101010 },
        { ThemeColor.BorderLight, 0xFF707070 },
        { ThemeColor.Text, 0xFFC0C0C0 },
        { ThemeColor.TextLight, 0xFFFFFFFF },
        { ThemeColor.TextMuted, 0xFFA0A0A0 },
        { ThemeColor.TextOutline, 0x80000000 },
        { ThemeColor.TextOutlineLight, 0xC0000000 },
        { ThemeColor.HighlightBackground, 0x305FCFFF },
        { ThemeColor.HighlightForeground, 0xFFFFFFFF },
        { ThemeColor.HighlightOutline, 0x80000000 },
        { ThemeColor.Accent, 0xFF5FCFFF },
    };

    public static string[] ColorNames => Enum.GetNames<ThemeColor>();

    /// <summary>
    /// Creates a new color object with the specified name.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Color(ThemeColor color)
    {
        return new(Enum.GetName(color), Colors[color]);
    }

    /// <summary>
    /// Sets the color of a theme element.
    /// </summary>
    public static void SetColor(ThemeColor color, uint value, bool applyImmediately = false)
    {
        Colors[color] = value;

        SaveToConfig();

        if (applyImmediately) UpdateColorScheme();
    }

    /// <summary>
    /// Returns the color of a theme element.
    /// </summary>
    public static uint GetColor(ThemeColor color)
    {
        return Colors[color];
    }

    public static void ApplyFromPreset(IThemePreset preset)
    {
        SetColor(ThemeColor.Background,          preset.Background);
        SetColor(ThemeColor.BackgroundDark,      preset.BackgroundDark);
        SetColor(ThemeColor.BackgroundLight,     preset.BackgroundLight);
        SetColor(ThemeColor.BackgroundActive,    preset.BackgroundActive);
        SetColor(ThemeColor.Border,              preset.Border);
        SetColor(ThemeColor.BorderDark,          preset.BorderDark);
        SetColor(ThemeColor.BorderLight,         preset.BorderLight);
        SetColor(ThemeColor.Text,                preset.Text);
        SetColor(ThemeColor.TextLight,           preset.TextLight);
        SetColor(ThemeColor.TextMuted,           preset.TextMuted);
        SetColor(ThemeColor.TextOutline,         preset.TextOutline);
        SetColor(ThemeColor.TextOutlineLight,    preset.TextOutlineLight);
        SetColor(ThemeColor.HighlightBackground, preset.HighlightBackground);
        SetColor(ThemeColor.HighlightForeground, preset.HighlightForeground);
        SetColor(ThemeColor.Accent,              preset.Accent);

        UpdateColorScheme();
        SaveToConfig();
    }

    /// <summary>
    /// Updates the color scheme of drawn elements.
    /// </summary>
    public static void UpdateColorScheme()
    {
        foreach ((ThemeColor color, uint value) in Colors) {
            Element.ThemeColors[Enum.GetName(color)!] = value;
        }
    }

    /// <summary>
    /// Imports a theme from the clipboard.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static void ImportFromClipboard()
    {
        try {
            string data = ImGui.GetClipboardText();

            var colors =
                JsonSerializer.Deserialize<Dictionary<ThemeColor, uint>>(
                    Encoding.UTF8.GetString(Convert.FromBase64String(data))
                );

            if (colors == null) {
                throw new();
            }

            foreach ((ThemeColor key, uint color) in colors) {
                SetColor(key, color);
            }

            UpdateColorScheme();
        } catch {
            Framework
                .Service<INotificationManager>()
                .AddNotification(
                    new() {
                        Content = "Failed to import theme from clipboard.",
                        Type    = NotificationType.Error
                    }
                );
        }
    }

    /// <summary>
    /// Exports the current theme to the clipboard.
    /// </summary>
    public static void ExportToClipboard()
    {
        string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Colors)));
        ImGui.SetClipboardText(data);

        Framework
            .Service<INotificationManager>()
            .AddNotification(
                new() {
                    Content = "Umbra theme copied to clipboard.",
                    Type    = NotificationType.Success
                }
            );
    }

    private static void SaveToConfig()
    {
        ConfigManager.Set("Theme", JsonSerializer.Serialize(Colors));
    }

    private static void LoadFromConfig()
    {
        if (ThemeColors == "{}") {
            ApplyFromPreset(new Presets.DarkPreset());
            return;
        }

        try {
            var colors = JsonSerializer.Deserialize<Dictionary<ThemeColor, uint>>(ThemeColors) ?? Colors;

            foreach ((ThemeColor key, uint color) in colors) {
                SetColor(key, color);
            }

            UpdateColorScheme();
        } catch {
            Logger.Warning("Failed to load theme colors from user profile. Reverting to default.");
            ApplyFromPreset(new Presets.DarkPreset());
        }
    }

    [WhenFrameworkCompiling(executionOrder: int.MinValue + 1)]
    internal static void LoadPresets()
    {
        LoadFromConfig();
    }
}
