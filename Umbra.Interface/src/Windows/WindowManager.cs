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
using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using Umbra.Common;

namespace Umbra.Interface;

[Service]
public sealed partial class WindowManager : IDisposable
{
    [ConfigVariable("WindowManager.PlaySoundEffects", "WindowSettings")]
    private static bool PlaySoundEffects { get; set; } = true;

    [ConfigVariable("WindowManager.HonorEscapeKey", "WindowSettings")]
    private static bool HonorEscapeKey { get; set; } = true;

    private const uint SfxOpen  = 23u;
    private const uint SfxClose = 24u;

    private readonly Dictionary<Type, Window> _windows = [];
    private readonly ClipRectSolver           _clipRectSolver;
    private readonly IKeyState                _keyState;

    private bool _isDisposing;
    private bool _wasEscPressed;

    public WindowManager(
        ClipRectSolver       clipRectSolver,
        IGameInteropProvider interopProvider,
        IKeyState            keyState
    )
    {
        _atkGlobalEventHook = CreateAtkUnitBaseReceiveGlobalEventHook(interopProvider);
        _clipRectSolver     = clipRectSolver;
        _keyState           = keyState;
    }

    /// <summary>
    /// Creates a window of the given type.
    /// </summary>
    public T CreateWindow<T>() where T : Window, new()
    {
        if (_windows.ContainsKey(typeof(T))) {
            return (T)_windows[typeof(T)];
        }

        var window = Framework.InstantiateWithDependencies<T>();
        _windows[typeof(T)] = window;

        window.OnClose += DisposeWindow<T>;

        if (PlaySoundEffects) UIModule.PlaySound(SfxOpen);

        return window;
    }

    /// <summary>
    /// Dispose a window of the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void DisposeWindow<T>() where T : Window
    {
        if (_windows.ContainsKey(typeof(T))) {
            _windows[typeof(T)].Dispose();
            _windows.Remove(typeof(T));
            if (PlaySoundEffects) UIModule.PlaySound(SfxClose);
        }
    }

    /// <summary>
    /// Returns true if any window is currently focused.
    /// </summary>
    /// <returns></returns>
    public bool HasFocusedWindows()
    {
        return _windows.Values.Any(window => window.IsFocused);
    }

    public Window? GetFocusedWindow()
    {
        return _windows.Values.FirstOrDefault(window => window.IsFocused);
    }

    public void Dispose()
    {
        _isDisposing = true;

        foreach (var window in _windows.Values) {
            window.Dispose();
        }

        _windows.Clear();
        _atkGlobalEventHook.Dispose();
    }

    [OnDraw]
    internal void OnDraw()
    {
        if (_isDisposing) return;

        if (_keyState[VirtualKey.ESCAPE] && HasFocusedWindows()) {
            _wasEscPressed = true;
        } else if (_wasEscPressed && HasFocusedWindows()) {
            GetFocusedWindow()?.Close();
            _wasEscPressed = false;
        }

        foreach (var window in _windows.Values) {
            window.Render(_clipRectSolver);
        }
    }
}
