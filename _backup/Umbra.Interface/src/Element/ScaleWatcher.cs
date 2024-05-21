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

using System.Threading;
using ImGuiNET;
using Umbra.Common;

namespace Umbra.Interface;

[Service]
internal class ScaleWatcher
{
    private float  _currentFontScale = ImGui.GetIO().FontGlobalScale;
    private Timer? _debounceTimer;

    [OnTick(interval: 250)]
    internal void OnTick()
    {
        float scale = ImGui.GetIO().FontGlobalScale;

        if (scale == _currentFontScale) return;
        _currentFontScale = scale;

        // Debounce the plugin restart for a second in case someone is using the
        // slider to adjust the scale in Dalamud settings.
        _debounceTimer?.Dispose();
        _debounceTimer = new(_ => Framework.Restart(), null, 1000, Timeout.Infinite);
    }
}
