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
using System.Numerics;
using ImGuiNET;
using Umbra.Common;

namespace Umbra.Interface;

public abstract partial class Window : IDisposable
{
    [ConfigVariable(
        "window.enableClipping",
        "Window Settings",
        "Enable window clipping",
        "Allows windows to render behind native game windows. This may have a slight performance cost and may not work properly for all native game windows."
    )]
    private static bool EnableClipping { get; set; } = true;

    public event Action? OnClose;

    protected abstract string Id { get; }

    protected Size   DefaultSize { get; set; } = new(400, 400);
    protected Size   MinSize     { get; set; } = new(400, 300);
    protected Size   MaxSize     { get; set; } = new(800, 600);
    protected string Title       { get; set; } = "Untitled Window";

    /// <summary>
    /// Determines if the window is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    public bool IsFocused { get; private set; }
    public bool IsHovered { get; private set; }

    protected Window()
    {
        BindCloseButtonEvents();
    }

    /// <summary>
    /// Closes this window.
    /// </summary>
    public void Close()
    {
        OnClose?.Invoke();
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
