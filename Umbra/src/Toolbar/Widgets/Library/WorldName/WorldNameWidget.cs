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

    private IPlayer _player = Framework.Service<IPlayer>();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        _player = Framework.Service<IPlayer>();
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        var  hideOnHomeWorld = GetConfigValue<bool>("HideOnHomeWorld");
        var  decorate        = GetConfigValue<bool>("Decorate");
        var  textYOffset     = GetConfigValue<int>("TextYOffset");
        var  iconLocation    = GetConfigValue<string>("IconLocation");
        bool showIcon        = GetConfigValue<string>("DisplayMode") != "TextOnly";
        bool isVisible       = !hideOnHomeWorld || _player.CurrentWorldName != _player.HomeWorldName;

        Node.Style.IsVisible = isVisible;

        if (isVisible) {
            SeStringBuilder str = new SeStringBuilder();

            if (iconLocation == "Left" && showIcon && _player.CurrentWorldName != _player.HomeWorldName) {
                str.AddIcon(BitmapFontIcon.CrossWorld);
            }

            str.AddText(_player.CurrentWorldName);

            if (iconLocation == "Right" && showIcon && _player.CurrentWorldName != _player.HomeWorldName) {
                str.AddIcon(BitmapFontIcon.CrossWorld);
            }

            SetLabel(str.Build());
            SetGhost(!decorate);
        }

        base.OnUpdate();
    }
}
