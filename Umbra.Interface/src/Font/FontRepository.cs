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

using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.ManagedFontAtlas;
using Umbra.Common;

namespace Umbra.Interface;

public class FontRepository
{
    private static readonly Dictionary<Font, IFontHandle> FontHandles = [];

    /// <summary>
    /// Returns the <see cref="IFontHandle"/> for the given font.
    /// </summary>
    /// <param name="font"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static IFontHandle Get(Font font)
    {
        if (!FontHandles.TryGetValue(font, out IFontHandle? handle))
            throw new KeyNotFoundException($"Font {font} is not loaded.");

        return handle;
    }

    /// <summary>
    /// Pushes the given font to the font stack.
    /// </summary>
    public static void PushFont(Font font)
    {
        Get(font).Push();
    }

    /// <summary>
    /// Pops the given font from the stack.
    /// </summary>
    public static void PopFont(Font font)
    {
        Get(font).Pop();
    }

    [WhenFrameworkAsyncCompiling]
    internal static async Task LoadFonts()
    {
        List<Task<IFontHandle>> tasks = [
            CreateFontFromStyle(Font.AxisExtraSmall, new(GameFontFamily.Axis, 12)),
            CreateFontFromStyle(Font.AxisSmall,      new(GameFontFamilyAndSize.Axis96)),
            CreateFontFromStyle(Font.Axis,           new(GameFontFamilyAndSize.Axis12)),
            CreateFontFromStyle(Font.AxisLarge,      new(GameFontFamilyAndSize.Axis14)),
            CreateFontFromStyle(Font.Jupiter,        new(GameFontFamilyAndSize.Jupiter23)),
            CreateFontFromStyle(Font.MiedingerSmall, new(GameFontFamilyAndSize.MiedingerMid10)),
            CreateFontFromStyle(Font.Miedinger,      new(GameFontFamilyAndSize.MiedingerMid14) { Bold = true }),
            CreateFontFromStyle(Font.MiedingerLarge, new(GameFontFamily.MiedingerMid, 42f)),

            CreateFontFromDalamudAsset(Font.MonospaceSmall,   DalamudAsset.InconsolataRegular,   new() { SizePx = 10 }),
            CreateFontFromDalamudAsset(Font.Monospace,        DalamudAsset.InconsolataRegular,   new() { SizePx = 14 }),
            CreateFontFromDalamudAsset(Font.MonospaceLarge,   DalamudAsset.InconsolataRegular,   new() { SizePx = 18 }),
            CreateFontFromDalamudAsset(Font.FontAwesomeSmall, DalamudAsset.FontAwesomeFreeSolid, new() { SizePx = 10 }),
            CreateFontFromDalamudAsset(Font.FontAwesome,      DalamudAsset.FontAwesomeFreeSolid, new() { SizePx = 14 }),
        ];

        await Task.WhenAll(tasks);

        // Assign Axis14 as default.
        FontHandles[Font.Default] = FontHandles[Font.Axis];
    }

    [WhenFrameworkDisposing]
    internal static void UnloadFonts()
    {
        foreach (var handle in FontHandles.Values) {
            handle.Dispose();
        }
    }

    private static Task<IFontHandle> CreateFontFromStyle(Font font, GameFontStyle style)
    {
        FontHandles[font] = Framework.DalamudPlugin.UiBuilder.FontAtlas.NewGameFontHandle(style);

        return FontHandles[font].WaitAsync();
    }

    private static Task<IFontHandle> CreateFontFromDalamudAsset(Font font, DalamudAsset asset, SafeFontConfig config)
    {
        FontHandles[font] =
            Framework.DalamudPlugin.UiBuilder.FontAtlas.NewDelegateFontHandle(
                e => e.OnPreBuild(tk => tk.AddDalamudAssetFont(asset, config))
            );

        return FontHandles[font].WaitAsync();
    }
}
