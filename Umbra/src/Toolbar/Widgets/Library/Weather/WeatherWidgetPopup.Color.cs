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
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Data.Files;
using Umbra.Common;

namespace Umbra.Widgets;

internal partial class WeatherWidgetPopup
{
    private static readonly Dictionary<uint, uint> Cache = [];

    private static uint GetDominantColor(uint iconId)
    {
        if (Cache.TryGetValue(iconId, out uint color)) return color;

        var texFile = GetIconTexFile(iconId);
        if (null == texFile) return 0;

        byte[] data = texFile.GetRgbaImageData();

        var r = 0;
        var b = 0;
        var g = 0;
        var t = 0;

        for (var i = 0; i < data.Length; i += 4) {
            byte red   = texFile.ImageData[i];
            byte green = texFile.ImageData[i + 1];
            byte blue  = texFile.ImageData[i + 2];
            byte alpha = texFile.ImageData[i + 3];

            if (alpha < 50) continue;

            r += red;
            g += green;
            b += blue;
            t++;
        }

        color         = t > 0 ? (uint)((r / t) << 16 | (g / t) << 8 | b / t) : 0u;
        Cache[iconId] = color;

        return color;
    }

    private static TexFile? GetIconTexFile(uint iconId)
    {
        var substitutionProvider = Framework.Service<ITextureSubstitutionProvider>();
        var textureProvider      = Framework.Service<ITextureProvider>();
        var dataManager          = Framework.Service<IDataManager>();

        string? originalPath = textureProvider.GetIconPath(iconId);
        if (originalPath is null) return null;

        string path = substitutionProvider.GetSubstitutedPath(originalPath);

        return Path.IsPathRooted(path)
            ? dataManager.GameData.GetFileFromDisk<TexFile>(path)
            : dataManager.GetFile<TexFile>(path);
    }
}
