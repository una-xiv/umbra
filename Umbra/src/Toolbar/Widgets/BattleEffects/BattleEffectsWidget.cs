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
internal sealed partial class BattleEffectsWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.BattleEffects.Enabled", "Toolbar Widgets")]
    private static bool Enabled { get; set; } = true;

    private readonly IGameConfig _gameConfig;

    private readonly Element _self;
    private readonly Element _party;
    private readonly Element _others;
    private readonly Element _pvp;

    public BattleEffectsWidget(ToolbarPopupContext popupContext, IGameConfig gameConfig)
    {
        _gameConfig = gameConfig;
        popupContext.RegisterDropdownActivator(Element, _dropdownElement);

        _self   = _dropdownElement.Get("Content.Self.Buttons");
        _party  = _dropdownElement.Get("Content.Party.Buttons");
        _others = _dropdownElement.Get("Content.Others.Buttons");
        _pvp    = _dropdownElement.Get("Content.PvP.Buttons");

        BindButtonGroup(_self,   "BattleEffectSelf");
        BindButtonGroup(_party,  "BattleEffectParty");
        BindButtonGroup(_others, "BattleEffectOther");
        BindButtonGroup(_pvp,    "BattleEffectPvPEnemyPc");

        Element.OnMouseEnter += OnMouseEnter;
        Element.OnMouseLeave += OnMouseLeave;
    }

    public unsafe void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        Element.IsVisible = true;

        if (!_dropdownElement.IsVisible) return;

        UpdateButtonGroup(_self,   "BattleEffectSelf");
        UpdateButtonGroup(_party,  "BattleEffectParty");
        UpdateButtonGroup(_others, "BattleEffectOther");
        UpdateButtonGroup(_pvp,    "BattleEffectPvPEnemyPc");
    }

    public void OnUpdate() { }

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

    private void BindButtonGroup(Element buttonGroup, string configId)
    {
        buttonGroup.Get("None").OnClick    += () => _gameConfig.UiConfig.Set(configId, 2);
        buttonGroup.Get("Limited").OnClick += () => _gameConfig.UiConfig.Set(configId, 1);
        buttonGroup.Get("Full").OnClick    += () => _gameConfig.UiConfig.Set(configId, 0);
    }

    private void UpdateButtonGroup(Element buttonGroup, string configId)
    {
        uint value = _gameConfig.UiConfig.GetUInt(configId);

        buttonGroup.Get<ButtonElement>("None").Style.Opacity    = value != 2 ? 0.66f : 1.0f;
        buttonGroup.Get<ButtonElement>("Limited").Style.Opacity = value != 1 ? 0.66f : 1.0f;
        buttonGroup.Get<ButtonElement>("Full").Style.Opacity    = value != 0 ? 0.66f : 1.0f;

        buttonGroup.Get<ButtonElement>("None").IsGhost    = value != 2;
        buttonGroup.Get<ButtonElement>("Limited").IsGhost = value != 1;
        buttonGroup.Get<ButtonElement>("Full").IsGhost    = value != 0;
    }
}
