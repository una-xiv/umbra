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
using Una.Drawing;

namespace Umbra.Windows;

public abstract partial class Window
{
    internal event Action? RequestClose;

    protected abstract Node Node { get; }

    /// <summary>
    /// Invoked when the window is opened, before drawing the first frame.
    /// </summary>
    protected abstract void OnOpen();

    /// <summary>
    /// Invoked on every frame for as long as the window is open.
    /// </summary>
    protected abstract void OnUpdate(int instanceId);

    /// <summary>
    /// Invoked when the window is closed.
    /// </summary>
    protected abstract void OnClose();

    /// <summary>
    /// Closes this window.
    /// </summary>
    public void Close()
    {
        _isOpened = false;
        OnClose();
        RequestClose?.Invoke();
    }

    private bool _isOpened = false;

    public void Render(string id)
    {
        if (!_isOpened) {
            _isOpened = true;

            if (_windowNode.ChildNodes.Count == 0) {
                _windowNode.AppendChild(Node);
            }

            OnOpen();
        }

        RenderWindow(id);
    }
}
