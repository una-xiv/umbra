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
using Dalamud.Game.Text;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Clock2;

[Service]
internal partial class StackedClockWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.StackedClock.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = true;

    private bool _isShowingServerTime;
    private bool _isFirstFrame = true;

    public StackedClockWidget()
    {
        Element.Get("LT").OnClick += () => _isShowingServerTime = true;
        Element.Get("ST").OnClick += () => _isShowingServerTime = false;

        Element.OnBeforeCompute += () => {
            var isModified = false;

            if (_isFirstFrame || Element.Flow == Flow.Horizontal) {
                isModified   = true;
                Element.Flow = Flow.Vertical;
                Element.Gap  = 2;
            }

            if (isModified || _isFirstFrame) {
                _isFirstFrame = false;

                switch (Framework.DalamudPlugin.UiLanguage) {
                    case "de":
                        Element.Get("ET.Container.Prefix").Text = SeIconChar.EorzeaTimeDe.ToIconString();
                        Element.Get("LT.Container.Prefix").Text = SeIconChar.LocalTimeDe.ToIconString();
                        Element.Get("ST.Container.Prefix").Text = SeIconChar.ServerTimeDe.ToIconString();
                        break;
                    case "fr":
                        Element.Get("ET.Container.Prefix").Text = SeIconChar.EorzeaTimeFr.ToIconString();
                        Element.Get("LT.Container.Prefix").Text = SeIconChar.LocalTimeFr.ToIconString();
                        Element.Get("ST.Container.Prefix").Text = SeIconChar.ServerTimeFr.ToIconString();
                        break;
                    case "jp":
                        Element.Get("ET.Container.Prefix").Text = SeIconChar.EorzeaTimeJa.ToIconString();
                        Element.Get("LT.Container.Prefix").Text = SeIconChar.LocalTimeJa.ToIconString();
                        Element.Get("ST.Container.Prefix").Text = SeIconChar.ServerTimeJa.ToIconString();
                        break;
                    default:
                        Element.Get("ET.Container.Prefix").Text = SeIconChar.EorzeaTimeEn.ToIconString();
                        Element.Get("LT.Container.Prefix").Text = SeIconChar.LocalTimeEn.ToIconString();
                        Element.Get("ST.Container.Prefix").Text = SeIconChar.ServerTimeEn.ToIconString();
                        break;
                }

                Element.Get("LT.Container.Prefix").Style.TextOffset = new(0, -2);
                Element.Get("LT.Container.Time").Style.TextOffset   = new(0, -2);
                Element.Get("ST.Container.Prefix").Style.TextOffset = new(0, -2);
                Element.Get("ST.Container.Time").Style.TextOffset   = new(0, -2);

                foreach (var clock in Element.Children) {
                    clock.Get<BackgroundElement>().IsVisible = false;
                    clock.Get<BorderElement>().IsVisible     = false;
                    clock.Size                               = new(0, 0);
                    clock.Get("Container").Padding           = new(0, 0);
                    clock.Get("Container.Prefix").Style.Font = Font.AxisSmall;
                    clock.Get("Container.Time").Style.Font   = Font.AxisSmall;
                    clock.Get("Container.Prefix").Invalidate();
                    clock.Get("Container.Time").Invalidate();
                }
            }
        };
    }

    public void OnUpdate()
    {
        if (!Enabled) return;

        UpdateEorzeaTime();
        UpdateLocalTime();
        UpdateServerTime();
    }

    public void OnDraw()
    {
        Element.IsVisible = Enabled;
        if (!Enabled) return;

        Element.Get("LT").IsVisible = !_isShowingServerTime;
        Element.Get("ST").IsVisible = _isShowingServerTime;
    }

    private void UpdateServerTime()
    {
        if (!_isShowingServerTime) return;

        long serverTime = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.GetServerTime();
        long hours      = serverTime / 3600 % 24;
        long minutes    = serverTime / 60   % 60;

        Element.Get("ST.Container.Time").Text = $"{hours:D2}:{minutes:D2}";
    }

    private void UpdateLocalTime()
    {
        Element.Get("LT.Container.Time").Text = DateTime.Now.ToString("HH:mm");
    }

    private unsafe void UpdateEorzeaTime()
    {
        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();

        if (fw == null) {
            return;
        }

        long eorzeaTime = fw->ClientTime.EorzeaTime;
        long hours      = eorzeaTime / 3600 % 24;
        long minutes    = eorzeaTime / 60   % 60;

        Element.Get("ET.Container.Time").Text = $"{hours:D2}:{minutes:D2}";
    }
}
