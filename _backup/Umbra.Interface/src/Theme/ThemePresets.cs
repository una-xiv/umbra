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
using System.Linq;
using Umbra.Common;

namespace Umbra.Interface;

[Service]
public class ThemePresets
{
    public static List<IThemePreset> Presets { get; private set; } = [];

    [OnTick(interval: 1000)]
    internal void OnTick()
    {
        if (Presets.Count == 0) return;

        for (var i = 0; i < Presets.Count; i++) {
            if (Presets.ElementAt(i) is ILiveThemePreset livePreset)
                livePreset.Update();
        }
    }

    [WhenFrameworkCompiling(executionOrder: int.MinValue + 1)]
    internal static void LoadPresets()
    {
        Presets = Framework
            .Assemblies.SelectMany(
                asm => asm
                    .GetTypes()
                    .Where(
                        type => typeof(IThemePreset).IsAssignableFrom(type)
                         && type is { IsInterface: false, IsAbstract: false }
                    )
            )
            .Select(type => Activator.CreateInstance(type) as IThemePreset)
            .Where(i => null != i)
            .ToList()!;
    }
}
