using Dalamud.Interface;
using Dalamud.Plugin.Services;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Widgets;

namespace Umbra.Widget.Library.FpsConfig;

[ToolbarWidget(
    "FpsConfig",
    "Widget.FpsConfig.Name",
    "Widget.FpsConfig.Description",
    ["fps", "performance", "config"]
)]
public class FpsConfigWidget(WidgetInfo info, string? guid = null, Dictionary<string, object>? configValues = null) : StandardToolbarWidget(info, guid, configValues)
{
    public override MenuPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features { get; } =
        StandardWidgetFeatures.Icon | StandardWidgetFeatures.CustomizableIcon;

    protected override FontAwesomeIcon DefaultFontAwesomeIcon { get; } = FontAwesomeIcon.GaugeSimpleHigh;
    protected override string          DefaultIconType        { get; } = "FA";

    private readonly MenuPopup.Button _btnFpsNone  = new MenuPopup.Button(I18N.Translate("Widget.FpsConfig.Button.FpsNone")) { OnClick  = () => SetFps(0), ClosePopupOnClick = false };
    private readonly MenuPopup.Button _btnFpsAuto  = new MenuPopup.Button(I18N.Translate("Widget.FpsConfig.Button.FpsAuto")) { OnClick  = () => SetFps(1), ClosePopupOnClick = false };
    private readonly MenuPopup.Button _btnFps60    = new MenuPopup.Button(I18N.Translate("Widget.FpsConfig.Button.Fps60")) { OnClick    = () => SetFps(2), ClosePopupOnClick = false };
    private readonly MenuPopup.Button _btnFps30    = new MenuPopup.Button(I18N.Translate("Widget.FpsConfig.Button.Fps30")) { OnClick    = () => SetFps(3), ClosePopupOnClick = false };
    private readonly MenuPopup.Button _btnLimitBg  = new MenuPopup.Button(I18N.Translate("Widget.FpsConfig.Button.LimitBg")) { OnClick  = ToggleLimitBg, ClosePopupOnClick   = false };
    private readonly MenuPopup.Button _btnLimitAfk = new MenuPopup.Button(I18N.Translate("Widget.FpsConfig.Button.LimitAfk")) { OnClick = ToggleLimitAfk, ClosePopupOnClick  = false };

    protected override void OnLoad()
    {
        var group = new MenuPopup.Group(I18N.Translate("Widget.FpsConfig.FpsGroup"));
        
        group.Add(_btnFpsNone);
        group.Add(_btnFpsAuto);
        group.Add(_btnFps60);
        group.Add(_btnFps30);
        
        Popup.Add(group);
        Popup.Add(new MenuPopup.Separator());
        Popup.Add(_btnLimitBg);
        Popup.Add(_btnLimitAfk);
    }

    protected override void OnUnload()
    {
    }

    protected override void OnDraw()
    {
        var gameConfig = Framework.Service<IGameConfig>().System;
        var fpsValue   = gameConfig.GetUInt("Fps");
        var limitBg    = gameConfig.GetUInt("FPSInActive") == 1;
        var limitAfk   = gameConfig.GetUInt("FPSDownAFK") == 1;

        _btnFpsNone.Icon  = fpsValue == 0 ? FontAwesomeIcon.Check : null;
        _btnFpsAuto.Icon  = fpsValue == 1 ? FontAwesomeIcon.Check : null;
        _btnFps60.Icon    = fpsValue == 2 ? FontAwesomeIcon.Check : null;
        _btnFps30.Icon    = fpsValue == 3 ? FontAwesomeIcon.Check : null;
        _btnLimitBg.Icon  = limitBg ? FontAwesomeIcon.Check : null;
        _btnLimitAfk.Icon = limitAfk ? FontAwesomeIcon.Check : null;
    }

    private static void SetFps(uint value)
    {
        Framework.Service<IGameConfig>().System.Set("Fps", value);
    }

    private static void ToggleLimitBg()
    {
        bool value = Framework.Service<IGameConfig>().System.GetUInt("FPSInActive") == 1;
        Framework.Service<IGameConfig>().System.Set("FPSInActive", value ? 0u : 1u);
    }

    private static void ToggleLimitAfk()
    {
        bool value = Framework.Service<IGameConfig>().System.GetUInt("FPSDownAFK") == 1;
        Framework.Service<IGameConfig>().System.Set("FPSDownAFK", value ? 0u : 1u);
    }
}
