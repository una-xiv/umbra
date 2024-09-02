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
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using Umbra.Common;

namespace Umbra.Widgets;

[ToolbarWidget("Volume", "Widget.Volume.Name", "Widget.Volume.Description")]
[ToolbarWidgetTags(["volume", "audio", "channels", "sound", "sfx", "bgm"])]
internal sealed partial class VolumeWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : IconToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override VolumeWidgetPopup Popup { get; } = new();

    private IGameConfig _gameConfig = null!;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        _gameConfig = Framework.Service<IGameConfig>();

        SetIcon(FontAwesomeIcon.VolumeUp);

        Node.OnRightClick += _ => ToggleMute();
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        SetIcon(GetVolumeIcon("SoundMaster", "IsSndMaster"));

        Popup.ShowOptions = GetConfigValue<bool>("ShowOptions");
        Popup.ShowBgm     = GetConfigValue<bool>("ShowBgm");
        Popup.ShowSfx     = GetConfigValue<bool>("ShowSfx");
        Popup.ShowVoc     = GetConfigValue<bool>("ShowVoc");
        Popup.ShowAmb     = GetConfigValue<bool>("ShowAmb");
        Popup.ShowSys     = GetConfigValue<bool>("ShowSys");
        Popup.ShowPerf    = GetConfigValue<bool>("ShowPerf");
        Popup.ValueStep   = GetConfigValue<int>("ValueStep");
        Popup.UpIcon      = GetConfigValue<FontAwesomeIcon>("UpIcon");
        Popup.DownIcon    = GetConfigValue<FontAwesomeIcon>("DownIcon");
        Popup.OffIcon     = GetConfigValue<FontAwesomeIcon>("OffIcon");
        Popup.MuteIcon    = GetConfigValue<FontAwesomeIcon>("MuteIcon");

        base.OnUpdate();
    }

    private void ToggleMute()
    {
        _gameConfig.System.Set("IsSndMaster", !_gameConfig.System.GetBool("IsSndMaster"));
    }

    private FontAwesomeIcon GetVolumeIcon(string volumeConfigName, string muteConfigName)
    {
        if (_gameConfig.System.GetBool(muteConfigName)) {
            return GetConfigValue<FontAwesomeIcon>("MuteIcon");
        }

        return _gameConfig.System.GetUInt(volumeConfigName) switch {
            0    => GetConfigValue<FontAwesomeIcon>("OffIcon"),
            < 50 => GetConfigValue<FontAwesomeIcon>("DownIcon"),
            _    => GetConfigValue<FontAwesomeIcon>("UpIcon")
        };
    }
}
