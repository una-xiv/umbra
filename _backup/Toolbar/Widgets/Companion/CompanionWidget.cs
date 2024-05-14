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
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;
using Umbra.Common;
using Umbra.Game;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Companion;

[Service]
internal partial class CompanionWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.Companion.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = true;

    private readonly ICompanionManager _companion;
    private readonly Element          _text;
    private readonly Element          _icon;

    public CompanionWidget(ICompanionManager companion)
    {
        _companion = companion;
        _text      = Element.Get("Container.Text");
        _icon      = Element.Get("Container.Icon");

        Element.OnClick       += companion.Summon;
        Element.OnRightClick  += OnRightClick;
        Element.OnMiddleClick += OnMiddleClick;
        Element.OnMouseEnter  += OnMouseEnter;
        Element.OnMouseLeave  += OnMouseLeave;
    }

    public void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        Element.IsVisible = true;
        Element.Tooltip   = I18N.Translate("CompanionWidget.Tooltip");

        if (_companion is { IsActive: true, TimeLeft: > 0 }) {
            string xp = _companion.Level < 20
                ? $" - {(float)_companion.CurrentXp / _companion.RequiredXp * 100:0}% XP"
                : "";

            _text.Get("Name").Text   = $"{_companion.CompanionName}, {_companion.ActiveCommand}";
            _text.Get("Status").Text = $"{TimeLeft} {xp}";
        }

        _text.IsVisible             = _companion.IsActive;
        _icon.Style.Image           = _companion.IconId;
        _icon.Style.ImageRounding   = 8;
        _icon.Style.ImageBrightness = _companion.CanSummon() ? 1.0f : 0.65f;
        _icon.Style.ImageGrayscale  = _companion.CanSummon() ? 0 : 1;
    }

    public void OnUpdate() { }

    private void OnRightClick()
    {
        if (ImGui.GetIO().KeyShift || ImGui.GetIO().KeyCtrl) {
            _companion.Dismiss();
        } else {
            _companion.SwitchBehavior();
        }
    }

    private unsafe void OnMiddleClick()
    {
        // Open Companion Window
        UIModule* um = UIModule.Instance();
        if (um == null) return;

        um->ExecuteMainCommand(42);
    }

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

    private string TimeLeft => TimeSpan.FromSeconds(_companion.TimeLeft).ToString(@"mm\:ss");
}
