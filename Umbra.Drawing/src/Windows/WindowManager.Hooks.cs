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
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Umbra.Common;

namespace Umbra.Drawing;

public sealed partial class WindowManager
{
    private unsafe delegate IntPtr AtkUnitBaseReceiveGlobalEvent(AtkUnitBase* thisPtr, ushort cmd, uint a3, IntPtr a4, uint* a5);

    private readonly Hook<AtkUnitBaseReceiveGlobalEvent> _atkGlobalEventHook;

    private bool _isHookEnabled;

    private unsafe Hook<AtkUnitBaseReceiveGlobalEvent> CreateAtkUnitBaseReceiveGlobalEventHook(IGameInteropProvider interopProvider)
    {
        var hook = interopProvider.HookFromSignature<AtkUnitBaseReceiveGlobalEvent>(
            "48 89 5C 24 ?? 48 89 7C 24 ?? 55 41 56 41 57 48 8B EC 48 83 EC 50 44 0F B7 F2",
            AtkUnitBaseReceiveGlobalEventDetour
        );

        return hook;
    }

    [OnTick(interval: 100)]
    public void OnUpdateHookStatus()
    {
        if (HonorEscapeKey && !_isHookEnabled)
        {
            _isHookEnabled = true;
            _atkGlobalEventHook.Enable();
        }
        else if (!HonorEscapeKey && _isHookEnabled)
        {
            _isHookEnabled = false;
            _atkGlobalEventHook.Disable();
        }
    }

    /// <summary>
    /// Intercepts the escape key when a managed window is currently focused.
    /// </summary>
    private unsafe IntPtr AtkUnitBaseReceiveGlobalEventDetour(AtkUnitBase* thisPtr, ushort cmd, uint a3, IntPtr a4, uint* arg)
    {
        // 12 = SendKey, 3 = Close.
        return cmd == 12 && *arg == 3 && HasFocusedWindows()
            ? IntPtr.Zero
            : _atkGlobalEventHook.Original(thisPtr, cmd, a3, a4, arg);
    }
}
