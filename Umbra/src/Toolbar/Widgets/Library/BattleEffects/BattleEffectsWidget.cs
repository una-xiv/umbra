using System.Collections.Generic;
using Dalamud.Interface;

namespace Umbra.Widgets;

[ToolbarWidget(
    "BattleEffects", 
    "Widget.BattleEffects.Name", 
    "Widget.BattleEffects.Description", 
    ["config", "settings", "effects", "vfx"]
)]
internal class BattleEffectsWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : StandardToolbarWidget(info, guid, configValues)
{
    public override BattleEffectsPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features
        => StandardWidgetFeatures.Icon | StandardWidgetFeatures.CustomizableIcon;

    protected override string DefaultIconType => IconTypeFontAwesome;

    protected override FontAwesomeIcon DefaultFontAwesomeIcon => FontAwesomeIcon.WandMagicSparkles;
}
