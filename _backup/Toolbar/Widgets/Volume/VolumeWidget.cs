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

using FFXIVClientStructs.FFXIV.Client.Sound;
using Umbra.Common;
using Umbra.Drawing;

namespace Umbra;

[Service]
internal sealed partial class VolumeWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Height")] private static int Height { get; set; } = 32;

    public VolumeWidget(ToolbarDropdownContext ctx)
    {
        ctx.RegisterDropdownActivator(Element, _dropdownElement);

        _dropdownElement.Content.Get("Master.Slider").GetNode<SliderNode>().OnValueChanged += OnMasterVolumeChanged;

        _dropdownElement.Content.Get("Music.Slider").GetNode<SliderNode>().OnValueChanged +=
            v => OnChannelVolumeChanged(0, v);

        _dropdownElement.Content.Get("Effects.Slider").GetNode<SliderNode>().OnValueChanged +=
            v => OnChannelVolumeChanged(1, v); // ok.

        _dropdownElement.Content.Get("System.Slider").GetNode<SliderNode>().OnValueChanged +=
            v => OnChannelVolumeChanged(4, v); // ok

        _dropdownElement.Content.Get("Voice.Slider").GetNode<SliderNode>().OnValueChanged +=
            v => OnChannelVolumeChanged(2, v); // ok

        _dropdownElement.Content.Get("Ambient.Slider").GetNode<SliderNode>().OnValueChanged +=
            v => OnChannelVolumeChanged(3, v); // ok

        _dropdownElement.Content.Get("Performance.Slider").GetNode<SliderNode>().OnValueChanged +=
            v => OnChannelVolumeChanged(5, v);
    }

    [OnDraw]
    public void OnDraw()
    {
        Element.IsVisible = true;
    }

    [OnTick(interval: 100)]
    public unsafe void OnTick()
    {
        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
        if (fw == null) return;

        var dc = _dropdownElement.Content;

        dc.Get("Master.Slider").GetNode<SliderNode>().Value = fw->SoundManager->MasterVolume;
        dc.Get("Master.Value").GetNode<TextNode>().Text     = $"{fw->SoundManager->MasterVolume * 100:0}%";

        dc.Get("Music.Slider").GetNode<SliderNode>().Value =
            fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.Bgm1);

        dc.Get("Music.Value").GetNode<TextNode>().Text =
            $"{fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.Bgm1) * 100:0}%";

        dc.Get("Effects.Slider").GetNode<SliderNode>().Value =
            fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.Se1);

        dc.Get("Effects.Value").GetNode<TextNode>().Text =
            $"{fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.Se1) * 100:0}%";

        dc.Get("System.Slider").GetNode<SliderNode>().Value =
            fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.System);

        dc.Get("System.Value").GetNode<TextNode>().Text =
            $"{fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.System) * 100:0}%";

        dc.Get("Voice.Slider").GetNode<SliderNode>().Value =
            fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.Voice);

        dc.Get("Voice.Value").GetNode<TextNode>().Text =
            $"{fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.Voice) * 100:0}%";

        dc.Get("Ambient.Slider").GetNode<SliderNode>().Value =
            fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.Env1);

        dc.Get("Ambient.Value").GetNode<TextNode>().Text =
            $"{fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.Env1) * 100:0}%";

        dc.Get("Performance.Slider").GetNode<SliderNode>().Value =
            fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.Perform);

        dc.Get("Performance.Value").GetNode<TextNode>().Text =
            $"{fw->SoundManager->GetEffectiveVolume(SoundManager.SoundChannel.Perform) * 100:0}%";
    }

    private unsafe void OnMasterVolumeChanged(float value)
    {
        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
        if (fw == null) return;

        fw->EnvironmentManager->SetMasterVolume((int)(value * 100), true);
    }

    private unsafe void OnChannelVolumeChanged(uint channel, float value)
    {
        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
        if (fw == null) return;

        fw->EnvironmentManager->SetVolume(channel, (int)(value * 100), true);
    }
}
