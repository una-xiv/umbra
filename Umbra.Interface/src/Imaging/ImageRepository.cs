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
using System.IO;
using Dalamud.Interface.Internal;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Lumina.Data.Files;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Umbra.Common;

namespace Umbra.Interface;

public class ImageRepository
{
    [PluginService] private ITextureProvider TextureProvider { get; set; } = null!;
    [PluginService] private IDataManager DataManager { get; set; } = null!;

    private static ImageRepository Instance { get; set; } = null!;

    [WhenFrameworkCompiling]
    internal static void Initialize()
    {
        Framework.DalamudPlugin.Inject(Instance = new());
    }

    /// <summary>
    /// Loads a local texture from disk relative to the plugin assembly location.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IDalamudTextureWrap GetLocalTexture(string path)
    {
        if (Instance.TextureProvider == null)
            throw new InvalidOperationException("AssetManager has not been initialized.");

        var pathInfo = new FileInfo(Path.Combine(Path.GetDirectoryName(Framework.DalamudPlugin.AssemblyLocation.FullName)!, path));
        var texture  = Instance.TextureProvider.GetTextureFromFile(pathInfo);

        return texture
            ?? throw new InvalidOperationException($"Failed to load texture \"{path}\".");
    }

    /// <summary>
    /// Loads a local image from disk relative to the plugin assembly location.
    /// </summary>
    public static Image<Rgba32> GetLocalImage(string path)
    {
        var pathInfo = new FileInfo(Path.Combine(Path.GetDirectoryName(Framework.DalamudPlugin.AssemblyLocation.FullName)!, path));

        return Image.Load<Rgba32>(pathInfo.FullName);
    }

    /// <summary>
    /// Loads an icon by its ID.
    /// </summary>
    /// <param name="iconId">The icon ID.</param>
    /// <returns>The icon texture.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IDalamudTextureWrap GetIcon(uint iconId)
    {
        if (Instance.TextureProvider == null)
            throw new InvalidOperationException("AssetManager has not been initialized.");

        return Instance.TextureProvider.GetIcon(iconId)
            ?? throw new InvalidOperationException($"Failed to load icon #{iconId}.");
    }

    /// <summary>
    /// Returns a <see cref="TexFile"/> instance for the icon by its ID.
    /// </summary>
    /// <param name="iconId">The icon ID.</param>
    /// <returns>The icon file.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static TexFile GetIconFile(uint iconId)
    {
        if (Instance.DataManager == null || Instance.TextureProvider == null)
            throw new InvalidOperationException("AssetManager has not been initialized.");

        string iconPath = Instance.TextureProvider.GetIconPath(iconId)
            ?? throw new InvalidOperationException($"Failed to get icon path for #{iconId}.");

        return Instance.DataManager.GetFile<TexFile>(iconPath)
            ?? throw new InvalidOperationException($"Failed to load icon file for #{iconId}.");
    }

    /// <summary>
    /// Loads an image from raw pixel data.
    /// </summary>
    /// <param name="data">The pixel data.</param>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="channels">The amount of color channels in the image data.</param>
    /// <returns>The image texture.</returns>
    public static IDalamudTextureWrap LoadImageRaw(byte[] data, int width, int height, int channels)
    {
        return Framework.DalamudPlugin.UiBuilder.LoadImageRaw(data, width, height, channels);
    }
}
