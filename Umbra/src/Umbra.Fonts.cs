﻿/* Umbra | (c) 2024 by Una              ____ ___        ___.
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
using System.IO;
using System.Linq;
using SkiaSharp;
using Umbra.Common;
using Una.Drawing;

namespace Umbra;

[Service]
internal sealed class UmbraFonts
{
    [ConfigVariable("Font.Default.Name")]
    private static string DefaultFont { get; set; } = "Dalamud Default";

    [ConfigVariable("Font.Default.Size")]
    private static float DefaultFontOffset { get; set; } = 0;

    [ConfigVariable("Font.Monospace.Name")]
    private static string MonospaceFont { get; set; } = "Dalamud Monospace";

    [ConfigVariable("Font.Monospace.Size")]
    private static float MonospaceFontOffset { get; set; } = 0;

    [ConfigVariable("Font.Emphasis.Name")]
    private static string EmphasisFont { get; set; } = "Dalamud Default";

    [ConfigVariable("Font.Emphasis.Size")]
    private static float EmphasisFontOffset { get; set; } = 0;

    private Dictionary<string, FontConfig> FontFamilies { get; } = [];

    public UmbraFonts()
    {
        FontFamilies.Add("Dalamud Default", new(true, GetDalamudFontAsset("NotoSansKR-Regular.otf")));
        FontFamilies.Add("Dalamud Monospace", new(true, GetDalamudFontAsset("Inconsolata-Regular.ttf")));
        FontFamilies.Add("Dalamud Icons", new(true, GetDalamudFontAsset("FontAwesomeFreeSolid.otf")));

        foreach (string name in FontRegistry.GetFontFamilies()) {
            FontFamilies.Add(name, new(false, name));
        }

        ApplyFont(0, DefaultFont, DefaultFontOffset);
        ApplyFont(1, MonospaceFont, MonospaceFontOffset);
        ApplyFont(3, EmphasisFont, EmphasisFontOffset);

        ConfigManager.CvarChanged += OnCvarChanged;
    }

    public List<string> GetFontFamilies()
    {
        return FontFamilies.Keys.ToList();
    }

    public void SetDefaultFont(string name, float sizeOffset)
    {
        if (!FontFamilies.TryGetValue(name, out var font)) return;

        ConfigManager.Set("Font.Default.Name", name);
        ConfigManager.Set("Font.Default.Size", sizeOffset);
    }

    public void SetMonospaceFont(string name, float sizeOffset)
    {
        if (!FontFamilies.TryGetValue(name, out var font)) return;

        ConfigManager.Set("Font.Monospace.Name", name);
        ConfigManager.Set("Font.Monospace.Size", sizeOffset);
    }

    public void SetEmphasisFont(string name, float sizeOffset)
    {
        if (!FontFamilies.TryGetValue(name, out var font)) return;

        ConfigManager.Set("Font.Emphasis.Name", name);
        ConfigManager.Set("Font.Emphasis.Size", sizeOffset);
    }

    private readonly struct FontConfig(bool isFile, string name)
    {
        public bool   IsFile { get; } = isFile;
        public string Name   { get; } = name;
    }

    private static string GetDalamudFontAsset(string name)
    {
        return Path.Combine(
            Framework.DalamudPlugin.DalamudAssetDirectory.FullName,
            "UIRes",
            name
        );
    }

    private void OnCvarChanged(string name)
    {
        switch (name) {
            case "Font.Default.Name" or "Font.Default.Size":
                ApplyFont(0, DefaultFont, DefaultFontOffset);
                break;
            case "Font.Monospace.Name" or "Font.Monospace.Size":
                ApplyFont(1, MonospaceFont, MonospaceFontOffset);
                break;
            case "Font.Emphasis.Name" or "Font.Emphasis.Size":
                ApplyFont(3, EmphasisFont, EmphasisFontOffset);
                break;
        }
    }

    private void ApplyFont(uint fontId, string name, float sizeOffset)
    {
        if (!FontFamilies.TryGetValue(name, out var font)) return;

        if (font.IsFile) {
            FontRegistry.SetNativeFontFamily(fontId, new FileInfo(font.Name), sizeOffset);
        } else {
            FontRegistry.SetNativeFontFamily(fontId, font.Name, fontId == 3 ? SKFontStyleWeight.ExtraBold : SKFontStyleWeight.Medium, sizeOffset);
        }
    }
}
