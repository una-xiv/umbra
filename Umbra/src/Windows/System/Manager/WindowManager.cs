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
using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Windows;

[Service]
internal class WindowManager
{
    private readonly Dictionary<string, Window>          _instances = [];
    private readonly Dictionary<string, Action<Window>?> _callbacks = [];

    /// <summary>
    /// Presents a window and invokes the given callback once the window is
    /// closed. Use the callback to retrieve data from the window instance.
    /// </summary>
    /// <param name="id">A unique ID for this instance.</param>
    /// <param name="window">An instance of <see cref="Window"/></param>
    /// <param name="callback">A callback function.</param>
    /// <typeparam name="T"></typeparam>
    public void Present<T>(string id, T window, Action<T>? callback = null) where T : Window
    {
        Framework.DalamudFramework.Run(
            () => {
                if (_instances.TryGetValue(id, out Window? wnd)) {
                    wnd.Close();
                }

                window.RequestClose += () => {
                    _callbacks[id]?.Invoke(window);
                    _instances.Remove(id);
                    _callbacks.Remove(id);
                };

                _instances[id] = window;
                _callbacks[id] = callback is not null ? o => callback((T)o) : null;
            }
        );
    }

    /// <summary>
    /// Returns true if the window with the given ID is open.
    /// </summary>
    public bool IsOpen(string id)
    {
        return _instances.ContainsKey(id);
    }

    public void Close(string id)
    {
        if (!_instances.Remove(id, out var window)) return;
        _callbacks.Remove(id);

        window.Close();
    }

    [OnDraw]
    private void OnDraw()
    {
        lock (_instances) {
            foreach ((string id, Window window) in _instances) {
                window.Render(id);
            }
        }
    }
}
