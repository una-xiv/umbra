﻿using System.Collections.Generic;
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

        foreach (ControlNode cn in _nodes.Values) {
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
