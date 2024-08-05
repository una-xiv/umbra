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
using System.IO;
using System.Linq;
using SkiaSharp;
using Umbra.Common;
using Una.Drawing;

namespace Umbra;

enum FontId : uint
{
    Default = 0,
    Monospace = 1,
    FontAwesome = 2,
    Emphasis = 3,
    WorldMarkers = 4,
}

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

    [ConfigVariable("Font.WorldMarkers.Name")]
    private static string WorldMarkersFont { get; set; } = "Dalamud Default";

    [ConfigVariable("Font.WorldMarkers.Size")]
    private static float WorldMarkersFontOffset { get; set; } = 0;

    private Dictionary<string, FontConfig> FontFamilies { get; } = [];

    public UmbraFonts()
    {
        FontFamilies.Add("Dalamud Default", new(true, GetDalamudFontAsset("NotoSansKR-Regular.otf")));
        FontFamilies.Add("Dalamud Monospace", new(true, GetDalamudFontAsset("Inconsolata-Regular.ttf")));
        FontFamilies.Add("Dalamud Icons", new(true, GetDalamudFontAsset("FontAwesomeFreeSolid.otf")));

        foreach (string name in FontRegistry.GetFontFamilies()) {
            FontFamilies.Add(name, new(false, name));
        }

        ApplyFont((uint)FontId.Default,      DefaultFont,      DefaultFontOffset);
        ApplyFont((uint)FontId.Monospace,    MonospaceFont,    MonospaceFontOffset);
        ApplyFont((uint)FontId.Emphasis,     EmphasisFont,     EmphasisFontOffset);
        ApplyFont((uint)FontId.WorldMarkers, WorldMarkersFont, WorldMarkersFontOffset);

        ConfigManager.CvarChanged += OnCvarChanged;
    }

    public List<string> GetFontFamilies()
    {
        return FontFamilies.Keys.ToList();
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
                ApplyFont((uint)FontId.Default, DefaultFont, DefaultFontOffset);
                break;
            case "Font.Monospace.Name" or "Font.Monospace.Size":
                ApplyFont((uint)FontId.Monospace, MonospaceFont, MonospaceFontOffset);
                break;
            case "Font.Emphasis.Name" or "Font.Emphasis.Size":
                ApplyFont((uint)FontId.Emphasis, EmphasisFont, EmphasisFontOffset);
                break;
            case "Font.WorldMarkers.Name" or "Font.WorldMarkers.Size":
                ApplyFont((uint)FontId.WorldMarkers, WorldMarkersFont, WorldMarkersFontOffset);
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
