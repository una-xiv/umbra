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

using Dalamud.Utility;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

[ToolbarWidget("OnlineStatus", "Widget.OnlineStatus.Name", "Widget.OnlineStatus.Description")]
internal partial class OnlineStatusWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    private IPlayer _player = null!;
    private uint?   _statusId;
    private string? _displayMode;
    private uint?   _iconId;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Node.OnRightClick += _ => OpenSearchInfoWindow();
        Node.Tooltip      =  I18N.Translate("Widget.OnlineStatus.Tooltip");

        Popup.UseGrayscaleIcons = false;

        _player = Framework.Service<IPlayer>();
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        var displayMode = GetConfigValue<string>("DisplayMode");

        if (_statusId != _player.OnlineStatusId || _displayMode != displayMode) {
            _statusId    = _player.OnlineStatusId;
            _displayMode = displayMode;
            UpdateStatus();
        }

        SetPopupStatusOption(47, true);                                // Online
        SetPopupStatusOption(17, true);                                // Away from Keyboard
        SetPopupStatusOption(12, true);                                // Busy
        SetPopupStatusOption(22, true);                                // Role-Playing
        SetPopupStatusOption(21, _player.IsGeneralActionUnlocked(13)); // Looking to Meld Materia
        SetPopupStatusOption(23, !_player.IsInParty);                  // Looking for Party
        SetPopupStatusOption(27, _player.IsMentor);                    // Mentor
        SetPopupStatusOption(28, _player.IsBattleMentor);              // PvE Battle Mentor
        SetPopupStatusOption(30, _player.IsBattleMentor);              // PvP Battle Mentor
        SetPopupStatusOption(29, _player.IsTradeMentor);               // Trade Mentor

        SetIcon(_iconId);
        base.OnUpdate();
    }

    private void UpdateStatus()
    {
        uint statusId = _player.OnlineStatusId == 0 ? 47 : _player.OnlineStatusId;
        var  status   = GetStatusById(statusId);

        SetLabel(status.Name.ToDalamudString().TextValue);

        _iconId = status.Icon;

        foreach (Node node in Node.QuerySelectorAll(".icon")) {
            node.Style.Margin = new();
        }
    }
}
