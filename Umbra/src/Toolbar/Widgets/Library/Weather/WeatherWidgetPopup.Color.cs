using System.IO;
using Dalamud.Utility;
using Lumina.Data.Files;

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

        string originalPath = textureProvider.GetIconPath(iconId);
        string path         = substitutionProvider.GetSubstitutedPath(originalPath);

        return Path.IsPathRooted(path)
            ? dataManager.GameData.GetFileFromDisk<TexFile>(path)
            : dataManager.GetFile<TexFile>(path);
    }
}
