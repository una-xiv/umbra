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
using Umbra.Common;

namespace Umbra.Windows;

[Service]
public class WindowManager
{
    private readonly Dictionary<string, Window> Instances = [];

    public void Add(string id, Window window)
    {
        if (Instances.TryGetValue(id, out Window? wnd)) {
            wnd.Close();
        }

        Instances[id] = window;
    }

    /// <summary>
    /// Returns true if the window with the given ID is open.
    /// </summary>
    public bool IsOpen(string id)
    {
        return Instances.ContainsKey(id);
    }

    public void Close(string id)
    {
        if (!Instances.Remove(id, out var window)) return;

        window.Close();
    }

    [OnDraw]
    private void OnDraw()
    {
        foreach ((string id, Window window) in Instances) {
            window.Render(id);
        }
    }
}
