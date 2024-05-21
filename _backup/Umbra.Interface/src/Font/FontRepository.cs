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

    private static IFontAtlas FontAtlas { get; set; } = null!;

    [WhenFrameworkAsyncCompiling]
    internal static async Task LoadFonts()
    {
        FontAtlas = Framework.DalamudPlugin.UiBuilder.CreateFontAtlas(
            FontAtlasAutoRebuildMode.Async,
            true,
            "UmbraFonts"
        );

        List<Task<IFontHandle>> tasks = [
            CreateGameFont(Font.AxisExtraSmall, new(GameFontFamily.Axis, 12.79f * Element.ScaleFactor)),
            CreateGameFont(Font.AxisSmall, new(GameFontFamily.Axis, 14.66f * Element.ScaleFactor)),
            CreateGameFont(Font.Axis, new(GameFontFamily.Axis, 16 * Element.ScaleFactor)),
            CreateGameFont(Font.AxisLarge, new(GameFontFamily.Axis, 18.66f * Element.ScaleFactor)),
            CreateGameFont(Font.AxisExtraLarge, new(GameFontFamily.Axis, 24 * Element.ScaleFactor) { Bold = true }),
            CreateGameFont(Font.Miedinger, new(GameFontFamily.MiedingerMid, 18 * Element.ScaleFactor) { Bold = true }),
            CreateGameFont(Font.MiedingerLarge, new(GameFontFamily.MiedingerMid, 42 * Element.ScaleFactor)),

            CreateFontFromDalamudAsset(
                Font.MonospaceSmall,
                DalamudAsset.InconsolataRegular,
                new() { SizePx = 12.79f * Element.ScaleFactor }
            ),

            CreateFontFromDalamudAsset(
                Font.Monospace,
                DalamudAsset.InconsolataRegular,
                new() { SizePx = 14.66f * Element.ScaleFactor }
            ),

            CreateFontFromDalamudAsset(
                Font.FontAwesomeSmall,
                DalamudAsset.FontAwesomeFreeSolid,
                new() { SizePx = 10 * Element.ScaleFactor }
            ),
            CreateFontFromDalamudAsset(
                Font.FontAwesome,
                DalamudAsset.FontAwesomeFreeSolid,
                new() { SizePx = 14 * Element.ScaleFactor }
            ),
        ];

        await FontAtlas.BuildFontsAsync();
        await Task.WhenAll(tasks);

        FontHandles[Font.Default] = FontHandles[Font.Axis];
    }

    [WhenFrameworkDisposing(executionOrder: int.MaxValue)]
    internal static void DisposeFonts()
    {
        foreach (Font font in FontHandles.Keys) {
            if (font == Font.Default) continue;

            FontHandles[font].Dispose();
        }

        FontHandles.Clear();
        FontAtlas.Dispose();
    }

    private static Task<IFontHandle> CreateFontFromDalamudAsset(Font font, DalamudAsset asset, SafeFontConfig config)
    {
        FontHandles[font] = FontAtlas.NewDelegateFontHandle(
            e => e.OnPreBuild(
                tk => {
                    tk.Font = tk.AddDalamudAssetFont(asset, config);
                    tk.SetFontScaleMode(tk.Font, FontScaleMode.UndoGlobalScale);
                }
            )
        );

        return FontHandles[font].WaitAsync();
    }

    private static Task<IFontHandle> CreateGameFont(Font font, GameFontStyle style)
    {
        FontHandles[font] = FontAtlas.NewDelegateFontHandle(
            buildStep => buildStep.OnPreBuild(
                tk => {
                    tk.Font = tk.AddGameGlyphs(style, null, default);
                    tk.SetFontScaleMode(tk.Font, FontScaleMode.UndoGlobalScale);
                }
            )
        );

        return FontHandles[font].WaitAsync();
    }
}
