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

using Dalamud.Interface;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.MainMenu;

internal sealed partial class VolumeWidget
{
    private readonly DropdownElement _dropdownElement = new(
        id: "VolumePopup",
        flow: Flow.Vertical,
        children: [
            new(
                id: "Channels",
                flow: Flow.Horizontal,
                gap: 8,
                padding: new(0, 8)
            )
        ]
    );

    private void CreateChannelWidget(string id, string tooltip, string volumeConfigName, string muteConfigName)
    {
        Element el = new(
            id: id,
            flow: Flow.Vertical,
            size: new(32, 280),
            children: [
                new(
                    id: "Label",
                    text: id,
                    size: new(32, 20),
                    anchor: Anchor.TopCenter,
                    tooltip: tooltip,
                    style: new() {
                        Font         = Font.AxisExtraSmall,
                        TextAlign    = Anchor.BottomCenter,
                        TextColor    = Theme.Color(ThemeColor.Text),
                        OutlineColor = Theme.Color(ThemeColor.TextOutline),
                        OutlineWidth = 1,
                        TextOffset   = new(0, -1)
                    }
                ),
                new(
                    id: "Value",
                    text: "100%",
                    size: new(32, 10),
                    anchor: Anchor.TopCenter,
                    tooltip: tooltip,
                    style: new() {
                        Font         = Font.AxisExtraSmall,
                        TextAlign    = Anchor.MiddleCenter,
                        TextColor    = Theme.Color(ThemeColor.TextMuted),
                        OutlineColor = Theme.Color(ThemeColor.TextOutline),
                        OutlineWidth = 1,
                        TextOffset   = new(0, -1)
                    }
                ),
                new(
                    id: "Slider",
                    anchor: Anchor.TopCenter,
                    flow: Flow.Horizontal,
                    size: new(32, 220),
                    children: [
                        new VerticalSliderElement(id, new(16, 220), Anchor.TopCenter)
                            { Padding = new(top: 8, bottom: 4, left: 4) },
                    ]
                ),
                new(
                    id: "MuteButton",
                    anchor: Anchor.TopCenter,
                    size: new(24, 24),
                    text: FontAwesomeIcon.VolumeMute.ToIconString(),
                    style: new() {
                        Font                  = Font.FontAwesome,
                        TextAlign             = Anchor.MiddleCenter,
                        TextOffset            = new(0, -1),
                        TextColor             = Theme.Color(ThemeColor.Text),
                        BackgroundColor       = Theme.Color(ThemeColor.BackgroundDark),
                        BackgroundBorderColor = Theme.Color(ThemeColor.Border),
                        BackgroundRounding    = 4,
                        BackgroundBorderWidth = 1
                    }
                )
            ]
        );

        VerticalSliderElement slider     = el.Get("Slider").Get<VerticalSliderElement>();
        Element               muteButton = el.Get("MuteButton");
        Element               valueLabel = el.Get("Value");

        muteButton.OnMouseEnter += () => {
            muteButton.Style.BackgroundColor       = Theme.Color(ThemeColor.BackgroundLight);
            muteButton.Style.BackgroundBorderColor = Theme.Color(ThemeColor.BorderLight);
        };

        muteButton.OnMouseLeave += () => {
            muteButton.Style.BackgroundColor       = Theme.Color(ThemeColor.BackgroundDark);
            muteButton.Style.BackgroundBorderColor = Theme.Color(ThemeColor.Border);
        };

        muteButton.OnClick += () => {
            _gameConfig.System.Set(muteConfigName, !_gameConfig.System.GetBool(muteConfigName));
        };

        slider.OnValueChanged      += (value) => { _gameConfig.System.Set(volumeConfigName, (uint)value); };
        slider.OnBeforeCompute     += () => { slider.Value    = (int)_gameConfig.System.GetUInt(volumeConfigName); };
        valueLabel.OnBeforeCompute += () => { valueLabel.Text = $"{_gameConfig.System.GetUInt(volumeConfigName)}%"; };
        muteButton.OnBeforeCompute += () => muteButton.Text = GetVolumeIcon(volumeConfigName, muteConfigName);

        _dropdownElement.Get("Channels").AddChild(el);
    }

    private string GetVolumeIcon(string volumeConfigName, string muteConfigName)
    {
        if (_gameConfig.System.GetBool(muteConfigName)) {
            return FontAwesomeIcon.VolumeMute.ToIconString();
        }

        return _gameConfig.System.GetUInt(volumeConfigName) switch {
            0    => FontAwesomeIcon.VolumeOff.ToIconString(),
            < 50 => FontAwesomeIcon.VolumeDown.ToIconString(),
            _    => FontAwesomeIcon.VolumeUp.ToIconString()
        };
    }
}
