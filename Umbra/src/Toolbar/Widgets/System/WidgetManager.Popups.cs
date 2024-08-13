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
using Umbra.Common;

namespace Umbra.Widgets.System;

internal partial class WidgetManager
{
    internal event Action<WidgetPopup>? OnPopupOpened;
    internal event Action<WidgetPopup>? OnPopupClosed;

    private WidgetPopup?   _currentPopup;
    private ToolbarWidget? _currentActivator;

    /// <summary>
    /// True if a popup is currently open.
    /// </summary>
    public bool HasOpenPopup => _currentPopup != null;

    /// <summary>
    /// Opens the given popup attached to the specified activator.
    /// </summary>
    private void OpenPopup(ToolbarWidget activator, WidgetPopup popup)
    {
        if (_currentActivator == activator && _currentPopup == popup) {
            return;
        }

        ClosePopup();

        popup.Reset();

        _currentActivator = activator;
        _currentPopup     = popup;

        _currentPopup.OnPopupOpen += InvokeOnPopupOpened;

        Toolbar.AllowAutoHide = false;
    }

    /// <summary>
    /// Opens the popup attached to the specified activator, but only if a
    /// popup is currently open.
    /// </summary>
    private void OpenPopupIfAnyIsOpen(ToolbarWidget activator, WidgetPopup popup)
    {
        if (_currentActivator is null && _currentPopup is null) return;
        OpenPopup(activator, popup);
    }

    private void ClosePopup()
    {
        if (_currentPopup != null) {
            ClipRects.RemoveClipRect($"Umbra.Popup.{_currentActivator?.Id}");
            _currentPopup.OnPopupOpen -= InvokeOnPopupOpened;
            OnPopupClosed?.Invoke(_currentPopup);
        }

        _currentPopup?.Reset();

        _currentActivator = null;
        _currentPopup     = null;

        Toolbar.AllowAutoHide = true;
    }

    [OnDraw(executionOrder: 0)]
    private void OnUpdatePopups()
    {
        if (_currentPopup is null || _currentActivator is null) return;

        if (!_currentPopup.Render(_currentActivator)) {
            ClosePopup();
            return;
        }

        ClipRects.SetClipRect($"Umbra.Popup.{_currentActivator.Id}", _currentPopup.Position, _currentPopup.Size);
    }

    private void InvokeOnPopupOpened()
    {
        if (null != _currentPopup) OnPopupOpened?.Invoke(_currentPopup);
    }
}
