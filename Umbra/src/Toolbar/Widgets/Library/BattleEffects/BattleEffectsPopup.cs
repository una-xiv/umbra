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
using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class BattleEffectsPopup : WidgetPopup
{
    private readonly Dictionary<string, ControlNode> _nodes = [];
    private readonly IGameConfig                     _gameConfig;

    public BattleEffectsPopup()
    {
        _gameConfig = Framework.Service<IGameConfig>();
        List<string> vn = ["Full", "Limited", "None"];
        List<string> bc = ["BahamutSize", "PhoenixSize", "GarudaSize", "TitanSize", "IfritSize"];

        CreateHeader(I18N.Translate("Widget.BattleEffects.EffectIntensity"));
        CreateControl("BattleEffectSelf",       0, 2, vn, ["BattleEffectSelf"],       VfxFormatter);
        CreateControl("BattleEffectParty",      0, 2, vn, ["BattleEffectParty"],      VfxFormatter);
        CreateControl("BattleEffectOther",      0, 2, vn, ["BattleEffectOther"],      VfxFormatter);
        CreateControl("BattleEffectPvPEnemyPc", 0, 2, vn, ["BattleEffectPvPEnemyPc"], VfxFormatter);

        CreateHeader(I18N.Translate("Widget.BattleEffects.SummonerPetSize"));
        CreateControl("SummonerPetSizeAll", 0, 2, ["Small", "Medium", "Large"], bc);
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        foreach ((string key, ControlNode cn) in _nodes) {
            var value = (int)_gameConfig.UiConfig.GetUInt(cn.Id);

            cn.SliderNode.Value    = cn.Formatter?.Invoke(value) ?? value;
            cn.ValueNode.NodeValue = cn.ValueNames[cn.SliderNode.Value];
        }
    }

    private int VfxFormatter(int value)
    {
        return value switch {
            0 => 2,
            1 => 1,
            _ => 0,
        };
    }
}
