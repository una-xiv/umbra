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

using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
using Umbra.Windows;

namespace Umbra;

[Service]
internal sealed class UmbraSound : IDisposable
{
    [ConfigVariable("Sound.Enabled", "General", "Sound")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Sound.OpenPopupId", "General", "Sound", 0, 100)]
    private static int OpenPopupId { get; set; } = 14;

    [ConfigVariable("Sound.ClosePopupId", "General", "Sound", 0, 100)]
    private static int ClosePopupId { get; set; } = 15;

    [ConfigVariable("Sound.OpenWindowId", "General", "Sound", 0, 100)]
    private static int OpenWindowId { get; set; } = 23;

    [ConfigVariable("Sound.CloseWindowId", "General", "Sound", 0, 100)]
    private static int CloseWindowId { get; set; } = 24;

    private WidgetManager WidgetManager { get; }
    private WindowManager WindowManager { get; }

    public UmbraSound(WidgetManager widgetManager, WindowManager windowManager)
    {
        WidgetManager = widgetManager;
        WindowManager = windowManager;

        widgetManager.OnPopupOpened += OnPopupOpened;
        widgetManager.OnPopupClosed += OnPopupClosed;
        windowManager.OnWindowOpened += OnWindowOpened;
        windowManager.OnWindowClosed += OnWindowClosed;
    }

    public void Dispose()
    {
        WidgetManager.OnPopupOpened -= OnPopupOpened;
        WidgetManager.OnPopupClosed -= OnPopupClosed;
    }

    private static void OnPopupOpened(WidgetPopup _)
    {
        PlaySound((uint)OpenPopupId);
    }

    private static void OnPopupClosed(WidgetPopup _)
    {
        PlaySound((uint)ClosePopupId);
    }

    private static void OnWindowOpened(Window _)
    {
        PlaySound((uint)OpenWindowId);
    }

    private static void OnWindowClosed(Window _)
    {
        PlaySound((uint)CloseWindowId);
    }

    private static void PlaySound(uint id)
    {
        if (!Enabled) return;

        UIModule.PlaySound(id);
    }
}
