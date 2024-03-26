/* Umbra.Drawing | (c) 2024 by Una      ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Drawing is free software: you can       \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;

namespace Umbra.Drawing;

public abstract partial class Window
{
    public event Action? OnClose;

    /// <summary>
    /// True if the window is currently focused.
    /// </summary>
    public bool IsFocused { get; private set; }

    /// <summary>
    /// True if the window is currently hovered.
    /// </summary>
    public bool IsHovered { get; private set; }

    /// <summary>
    /// The ID of the window. This should be unique across all windows.
    /// </summary>
    protected abstract string Id { get; }

    /// <summary>
    /// The name of the window to display in the title bar.
    /// </summary>
    protected string Title { get; set; } = "Unnamed Window";

    /// <summary>
    /// The minimum content size of the window in pixels.
    /// </summary>
    protected Size MinSize { get; set; } = new(400, 300);

    /// <summary>
    /// The maximum content size of the window in pixels.
    /// </summary>
    protected Size MaxSize { get; set; } = new(500, 400);

    public Window()
    {
        _titlebar.Get("Buttons.Close").OnClick += Close;
    }

    /// <summary>
    /// Closes the window.
    /// </summary>
    /// <remarks>
    /// If the window implements <see cref="IDisposable"/>, its Dispose method
    /// will be invoked by the window manager before the window is removed.
    /// </remarks>
    public void Close()
    {
        OnClose?.Invoke();
    }

    /// <summary>
    /// Adds the given element to the window content.
    /// </summary>
    public void AddElement(Element element)
    {
        _contents.AddChild(element);
    }

    /// <summary>
    /// Removes the given element from the window content.
    /// </summary>
    public void RemoveElement(Element element)
    {
        _contents.RemoveChild(element);
    }

    protected abstract void OnBeforeDraw();
    protected abstract void OnAfterDraw();
}
