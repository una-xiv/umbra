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
using Umbra.Common;
using Umbra.Drawing;
using Umbra.Game;

namespace Umbra;

[Service]
internal sealed partial class CompanionWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Height")] private static int Height { get; set; } = 32;

    [ConfigVariable(
        "Toolbar.Widget.Companion.Enabled",
        "Toolbar Widgets",
        "Show companion",
        "Shows a chocobo companion widget that allows you to summon and change the behavior of your companion."
    )]
    private static bool Enabled { get; set; } = true;

    private readonly CompanionManager _manager;

    private bool _canSummonCompanion;
    private bool _isCompanionSummoned;

    public CompanionWidget(CompanionManager manager)
    {
        _manager = manager;

        Element.OnClick      += _manager.Summon;
        Element.OnRightClick += CompanionManager.SwitchBehavior;
        Element.OnMouseEnter += () => Element.Get("Info.Name").GetNode<TextNode>().Color = 0xFFFFFFFF;
        Element.OnMouseLeave += () => Element.Get("Info.Name").GetNode<TextNode>().Color = 0xFFC0C0C0;
        Element.Tooltip      =  "Left click to feed, right click to switch behavior.";
    }

    [OnDraw]
    public void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        Element.IsVisible   = true;
        _canSummonCompanion  = _manager.CanSummonCompanion();
        _isCompanionSummoned = _manager.TimeLeft > 0;

        // Update widget size.
        Element.IsDisabled                                = !_canSummonCompanion;
        Element.Size                                      = new(0, Height - 6);
        Element.Get("Info").IsVisible                     = _isCompanionSummoned;
        Element.Get("Icon").Size                          = new(Height - 6, Height - 6);
        Element.Get("Icon").GetNode<IconNode>().Grayscale = _canSummonCompanion ? 0 : 1;
        Element.Get("Icon").GetNode<IconNode>().Opacity   = _canSummonCompanion ? 1 : 0.66f;

        Element.Get("Info.Name").Size                      = new Size(0, Height >= 31 ? Height / 2 - 3 : Height - 9);
        Element.Get("Info.Name").GetNode<TextNode>().Align = Height >= 31 ? Align.BottomRight : Align.MiddleRight;
        Element.Get("Info.Name").GetNode<TextNode>().Font  = Height >= 40 ? Font.Axis : Font.AxisSmall;

        Element.Get("Info.Sub").IsVisible                = Height >= 31;
        Element.Get("Info.Sub").Size                     = new Size(0, Height / 2 - 3);
        Element.Get("Info.Sub").GetNode<TextNode>().Font = Height >= 40 ? Font.AxisSmall : Font.AxisExtraSmall;

        // Update widget content.
        string infoText;
        string nameText;

        if (_manager.Level < 20) {
            nameText = TimeLeft;
            infoText = $"{(float)_manager.CurrentXp / _manager.RequiredXp * 100:0}% XP";
        } else {
            nameText =  _manager.CompanionName;
            infoText = TimeLeft;
        }

        Element.Get("Icon").GetNode<IconNode>().IconId    = _manager.IconId;
        Element.Get("Info.Name").GetNode<TextNode>().Text = nameText;
        Element.Get("Info.Sub").GetNode<TextNode>().Text  = infoText;

        Element.Tooltip = $"{_manager.ActiveCommand} - Left click to feed, right click to switch behavior.";
    }

    private string TimeLeft => TimeSpan.FromSeconds(_manager.TimeLeft).ToString(@"mm\:ss");
}
