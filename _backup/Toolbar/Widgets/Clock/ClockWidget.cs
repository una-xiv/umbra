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
using System.Collections.Generic;
using Dalamud.Game.Text;
using Umbra.Common;
using Umbra.Drawing;

namespace Umbra;

[Service]
internal sealed partial class ClockWidget : IToolbarWidget
{
    [ConfigVariable(
        "Toolbar.Widget.Clock.Enabled",
        "Toolbar Widgets",
        "Show clock",
        "Display a clock widget that shows Eorzea and local time. Local and server time can be toggled when clicked."
    )]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.Height")] private static int Height { get; set; } = 32;

    private readonly Element _et;
    private readonly Element _lt;
    private readonly Element _st;

    public ClockWidget()
    {
        _et = BuildClockElement("ET", SeIconChar.EorzeaTimeEn);
        _lt = BuildClockElement("LT", SeIconChar.LocalTimeEn);
        _st = BuildClockElement("ST", SeIconChar.ServerTimeEn);

        Element.AddChild(_st).AddChild(_lt).AddChild(_et);
        _st.IsVisible = false;

        _lt.OnClick += () => {
            _lt.IsVisible = false;
            _st.IsVisible = true;
        };

        _st.OnClick += () => {
            _st.IsVisible = false;
            _lt.IsVisible = true;
        };
    }

    [OnDraw]
    public void OnDraw()
    {
        Element.IsVisible = Enabled;
        if (!Enabled) return;

        // ------------ Update time displays ------------- //
        UpdateLocalTime();
        UpdateServerTime();
        UpdateEorzeaTime();

        // --- Resize elements based on toolbar height --- //
        Element.Size      = new(0, Height);
        Element.Gap       = Height >= 40 ? 1 : 6;
        Element.Direction = Height >= 40 ? Direction.Vertical : Direction.Horizontal;
        Element.Anchor    = Height >= 40 ? Anchor.Top | Anchor.Right : Anchor.Middle | Anchor.Right;
        Element.Padding   = Height >= 40 ? new(top: 2) : new(0);

        new List<Element>([_et, _lt, _st]).ForEach(
            element => {
                element.Size = new(0, Height >= 40 ? (Height / 2) - 2 : Height - 6);
                element.Anchor = Height >= 40 ? Anchor.Top | Anchor.Right : Anchor.Middle | Anchor.Right;
                element.Get("Icon").GetNode<TextNode>().Margin = new(left: element.Size.Height <= 21 ? 3 : 6);
            }
        );
    }

    private void UpdateServerTime()
    {
        if (!_st.IsVisible) return;

        var serverTime = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.GetServerTime();
        var hours      = serverTime / 3600 % 24;
        var minutes    = serverTime / 60   % 60;

        _st.Get("Text").GetNode<TextNode>().Text = $"{hours:D2}:{minutes:D2}";
    }

    private void UpdateLocalTime()
    {
        _lt.Get("Text").GetNode<TextNode>().Text = DateTime.Now.ToString("HH:mm");
    }

    private unsafe void UpdateEorzeaTime()
    {
        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();

        if (fw == null) {
            _et.IsVisible = false;
            return;
        }

        long eorzeaTime = fw->ClientTime.EorzeaTime;
        long hours      = eorzeaTime / 3600 % 24;
        long minutes    = eorzeaTime / 60   % 60;

        _et.IsVisible                            = true;
        _et.Get("Text").GetNode<TextNode>().Text = $"{hours:D2}:{minutes:D2}";
    }
}
