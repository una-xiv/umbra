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
using Umbra.Common;

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow
{
    private static byte[]  LogoTexture => _logoTexture ??= GetEmbeddedTexture("Logo.png");
    private static byte[]? _logoTexture;

    private static byte[] GetEmbeddedTexture(string name)
    {
        foreach (var asm in Framework.Assemblies) {
            using var stream = asm.GetManifestResourceStream(name);
            if (stream == null) continue;

            var imageData = new byte[stream.Length];
            int _         = stream.Read(imageData, 0, imageData.Length);

            return imageData;
        }

        throw new InvalidOperationException($"Failed to load embedded texture '{name}'");
    }
}
