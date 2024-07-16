/* Umbra.Common | (c) 2024 by Una       ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Common is free software: you can        \/     \/             \/
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
using System.Diagnostics;
using System.IO;
using Lumina.Data;
using Newtonsoft.Json;

namespace Umbra.Common;

public static class I18N
{
    [ConfigVariable("General.LanguageOverride", "General", options: ["None", "en", "de", "fr", "ja", "zh"], requiresRestart: true)]
    internal static string LanguageOverride { get; set; } = "None";

    private static readonly Dictionary<string, Dictionary<string, string>> Translations = [];

    /// <summary>
    /// Returns a translated string in the currently configured language.
    /// </summary>
    /// <param name="key">The key of the translation.</param>
    /// <param name="args">Values of placeholders to replace in the translated text.</param>
    /// <returns>The translated text.</returns>
    public static string Translate(string key, params object?[] args)
    {
        return !Dict.TryGetValue(key, out string? str)
            ? $"Missing translation: {key}"
            : string.Format(str, args);
    }

    /// <summary>
    /// Returns true if a translation for the given key exists.
    /// </summary>
    /// <param name="key">The translation key to look for.</param>
    public static bool Has(string key)
    {
        return Dict.ContainsKey(key);
    }

    public static string GetCurrentLanguage()
    {
        if (LanguageOverride != "None" && Translations.ContainsKey(LanguageOverride))
            return LanguageOverride;

        return Translations.ContainsKey(Framework.DalamudPlugin.UiLanguage)
            ? Framework.DalamudPlugin.UiLanguage
            : "en";
    }

    private static Dictionary<string, string> Dict {
        get {
            string lang = string.IsNullOrEmpty(LanguageOverride) || LanguageOverride == "None"
                ? Framework.DalamudPlugin.UiLanguage
                : LanguageOverride;

            return Translations.TryGetValue(lang, out Dictionary<string, string>? translation)
                ? translation
                : Translations["en"];
        }
    }

    [WhenFrameworkCompiling]
    internal static void LoadTranslations()
    {
        FileInfo[] files =
            new DirectoryInfo(Path.Combine(Framework.DalamudPlugin.AssemblyLocation.DirectoryName!, "i18n")).GetFiles(
                "*.json"
            );

        foreach (FileInfo file in files) {
            string lang = file.Name.Replace(".json", "");
            string json = File.ReadAllText(file.FullName);

            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
             ?? throw new($"Failed to parse translation file {file.FullName}");

            Translations[lang] = [];

            Logger.Debug($"Loading translation '{lang}' with {dict.Count} entries.");

            foreach ((string key, string value) in dict) {
                Translations[lang][key] = value;
            }
        }

        ValidateTranslationKeys();
    }

    [WhenFrameworkDisposing]
    internal static void Dispose()
    {
        // Updated translation files are loaded simply by logging out and back in.
        Translations.Clear();
    }

    // [Conditional("DEBUG")]
    private static void ValidateTranslationKeys()
    {
        var en = Translations["en"];

        foreach (string key in en.Keys) {
            foreach (string lang in Translations.Keys) {
                if (lang == "en") continue;

                if (!Translations[lang].ContainsKey(key)) {
                    Logger.Warning($"Language \"{lang}\" is missing translation \"{key}\".");
                }
            }
        }

        // Find obsolete translations from other languages than english.
        foreach (string lang in Translations.Keys) {
            if (lang == "en") continue;

            foreach (string key in Translations[lang].Keys) {
                if (!en.ContainsKey(key)) {
                    Logger.Warning($"Language \"{lang}\" has obsolete translation \"{key}\".");
                }
            }
        }
    }
}