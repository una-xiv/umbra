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

using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.MainMenu;

[Service]
internal sealed partial class VolumeWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.Volume.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = true;

    private readonly IGameConfig _gameConfig;

    public VolumeWidget(ToolbarPopupContext popupContext, IGameConfig gameConfig)
    {
        _gameConfig = gameConfig;
        popupContext.RegisterDropdownActivator(Element, _dropdownElement);

        CreateChannelWidget("Master", "Master volume",         "SoundMaster",  "IsSndMaster");
        CreateChannelWidget("BGM",    "Background music",      "SoundBgm",     "IsSndBgm");
        CreateChannelWidget("SFX",    "Sound effects",         "SoundSe",      "IsSndSe");
        CreateChannelWidget("VOC",    "Voice",                 "SoundVoice",   "IsSndVoice");
        CreateChannelWidget("AMB",    "Ambient sound effects", "SoundEnv",     "IsSndEnv");
        CreateChannelWidget("SYS",    "System sounds",         "SoundSystem",  "IsSndSystem");
        CreateChannelWidget("PERF",   "Performance music",     "SoundPerform", "IsSndPerform");

        Element.OnMouseEnter += OnMouseEnter;
        Element.OnMouseLeave += OnMouseLeave;
        Element.OnRightClick += OnRightClick;
    }

    public void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        Element.IsVisible        = true;
        Element.Get("Icon").Text = GetVolumeIcon("SoundMaster", "IsSndMaster");
    }

    public void OnUpdate() { }

    private void OnMouseEnter()
    {
        Element.Get<BorderElement>().Color     = Theme.Color(ThemeColor.BorderLight);
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.Background);
    }

    private void OnMouseLeave()
    {
        Element.Get<BorderElement>().Color     = Theme.Color(ThemeColor.Border);
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.BackgroundDark);
    }

    private void OnRightClick()
    {
        _gameConfig.System.Set("IsSndMaster", !_gameConfig.System.GetBool("IsSndMaster"));
    }
}
