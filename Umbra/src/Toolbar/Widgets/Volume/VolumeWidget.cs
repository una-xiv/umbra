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
using FFXIVClientStructs.FFXIV.Client.Sound;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.MainMenu;

[Service]
internal sealed partial class VolumeWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.Volume.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = true;

    private readonly IGameConfig                             _gameConfig;
    private readonly VerticalSliderElement                   _masterSlider;
    private readonly Dictionary<byte, VerticalSliderElement> _channelSliders = [];

    private readonly Dictionary<byte, SoundManager.SoundChannel> _channelMap = new() {
        { 0, SoundManager.SoundChannel.Bgm1 },
        { 1, SoundManager.SoundChannel.Se1 },
        { 2, SoundManager.SoundChannel.Voice },
        { 3, SoundManager.SoundChannel.Env1 },
        { 4, SoundManager.SoundChannel.System },
        { 5, SoundManager.SoundChannel.Perform }
    };

    private bool _isMuted;

    public VolumeWidget(ToolbarPopupContext popupContext, IGameConfig gameConfig)
    {
        _gameConfig = gameConfig;
        popupContext.RegisterDropdownActivator(Element, _dropdownElement);

        _masterSlider = _dropdownElement.Get("Channels.Master.Slider").Get<VerticalSliderElement>();
        _channelSliders.Add(0, _dropdownElement.Get("Channels.BGM.Slider").Get<VerticalSliderElement>());
        _channelSliders.Add(1, _dropdownElement.Get("Channels.SFX.Slider").Get<VerticalSliderElement>());
        _channelSliders.Add(2, _dropdownElement.Get("Channels.VOC.Slider").Get<VerticalSliderElement>());
        _channelSliders.Add(3, _dropdownElement.Get("Channels.AMB.Slider").Get<VerticalSliderElement>());
        _channelSliders.Add(4, _dropdownElement.Get("Channels.SYS.Slider").Get<VerticalSliderElement>());
        _channelSliders.Add(5, _dropdownElement.Get("Channels.PERF.Slider").Get<VerticalSliderElement>());

        unsafe {
            var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
            if (fw == null) return;

            _masterSlider.Value          =  (int)(fw->SoundManager->MasterVolume * 100);
            _masterSlider.OnValueChanged += value => {
                fw->EnvironmentManager->SetMasterVolume(value, true);

                if (_isMuted) {
                    _gameConfig.System.Set("IsSndMaster", false);
                }
            };
        }

        for (byte i = 0; i < 6; i++) {
            byte channel = i;
            _channelSliders[channel].OnValueChanged += v => OnChannelVolumeChanged(channel, v);
        }

        Element.OnMouseEnter += OnMouseEnter;
        Element.OnMouseLeave += OnMouseLeave;
        Element.OnRightClick += OnRightClick;
    }

    public unsafe void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        Element.IsVisible = true;

        _isMuted = _gameConfig.System.GetBool("IsSndMaster");

        Element.Get("Icon").Text = GetMasterVolumeIcon();

        if (!_dropdownElement.IsVisible) return;

        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
        if (fw == null) return;

        _masterSlider.Value = (int)(fw->SoundManager->MasterVolume * 100);
        _masterSlider.Parent!.Parent!.Get("Value").Text = $"{_masterSlider.Value}%";

        for (byte i = 0; i < 6; i++) {
            _channelSliders[i].Value = (int)(fw->SoundManager->GetEffectiveVolume(_channelMap[i]) * 100);
            _channelSliders[i].Parent!.Parent!.Get("Value").Text = $"{_channelSliders[i].Value}%";
        }
    }

    public void OnUpdate() { }

    private string GetMasterVolumeIcon()
    {
        if (_isMuted) {
            return FontAwesomeIcon.VolumeMute.ToIconString();
        }

        return _masterSlider.Value switch {
            0    => FontAwesomeIcon.VolumeOff.ToIconString(),
            < 50 => FontAwesomeIcon.VolumeDown.ToIconString(),
            _    => FontAwesomeIcon.VolumeUp.ToIconString()
        };
    }

    private void OnMouseEnter()
    {
        Element.Get<BorderElement>().Color     = 0xFF6A6A6A;
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.Background);
    }

    private void OnMouseLeave()
    {
        Element.Get<BorderElement>().Color     = Theme.Color(ThemeColor.Border);
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.BackgroundDark);
    }

    private void OnRightClick()
    {
        _gameConfig.System.Set("IsSndMaster", !_isMuted);
    }

    private unsafe void OnChannelVolumeChanged(byte channel, int value)
    {
        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
        if (fw == null) return;

        if (_isMuted) {
            _gameConfig.System.Set("IsSndMaster", false);
        }

        fw->EnvironmentManager->SetVolume(channel, value, true);
    }
}
