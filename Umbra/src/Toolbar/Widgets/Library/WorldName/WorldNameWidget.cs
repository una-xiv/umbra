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
using Dalamud.Game.Text.SeStringHandling;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget("WorldName", "Widget.WorldName.Name", "Widget.WorldName.Description")]
internal partial class WorldNameWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override WidgetPopup? Popup => null;

    private IPlayer?  _player;
    private string?   _currentWorldName;
    private bool?     _hideOnHomeWorld;
    private bool?     _decorate;
    private int?      _textYOffset;
    private bool?     _showIcon;
    private string?   _iconLocation;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        _player = Framework.Service<IPlayer>();
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        var hideOnHomeWorld = GetConfigValue<bool>("HideOnHomeWorld");
        var decorate        = GetConfigValue<bool>("Decorate");
        var textYOffset     = GetConfigValue<int>("TextYOffset");
        var showIcon        = GetConfigValue<bool>("ShowIcon");
        var iconLocation    = GetConfigValue<string>("IconLocation");

        if (hideOnHomeWorld == _hideOnHomeWorld
            && decorate == _decorate
            && textYOffset == _textYOffset
            && showIcon == _showIcon
            && iconLocation == _iconLocation
            && _currentWorldName == _player?.CurrentWorldName)
            return;

        _hideOnHomeWorld  = hideOnHomeWorld;
        _decorate         = decorate;
        _textYOffset      = textYOffset;
        _showIcon         = showIcon;
        _iconLocation     = iconLocation;
        _currentWorldName = _player?.CurrentWorldName;

        Node.Style.IsVisible = !hideOnHomeWorld || _currentWorldName != _player?.HomeWorldName;

        SeStringBuilder str = new SeStringBuilder();

        if (iconLocation == "Left" && showIcon && _currentWorldName != _player?.HomeWorldName) {
            str.AddIcon(BitmapFontIcon.CrossWorld);
        }

        str.AddText(_currentWorldName ?? "Unknown World");

        if (iconLocation == "Right" && showIcon && _currentWorldName != _player?.HomeWorldName) {
            str.AddIcon(BitmapFontIcon.CrossWorld);
        }

        Node.QuerySelector("Label")!.Style.TextOffset = new(0, textYOffset + 1);
        SetLabel(str.Build());
        SetGhost(!decorate);
    }
}
