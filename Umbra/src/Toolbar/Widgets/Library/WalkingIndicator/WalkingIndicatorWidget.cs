using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets;

[ToolbarWidget(
    "WalkingIndicator",
    "Widget.WalkingIndicator.Name",
    "Widget.WalkingIndicator.Description",
    ["walking", "running", "movement", "speed", "indicator"]
)]
internal class WalkingIndicatorWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.Icon;

    protected override unsafe void OnLoad()
    {
        SetFontAwesomeIcon(FontAwesomeIcon.Running);

        Node.OnMouseUp += _ => {
            Control* ctrl = Control.Instance();
            if (ctrl == null) return;

            ctrl->IsWalking = !ctrl->IsWalking;
        };
    }

    protected override unsafe void OnDraw()
    {
        Control* ctrl = Control.Instance();
        SetFontAwesomeIcon(ctrl->IsWalking
            ? GetConfigValue<FontAwesomeIcon>("WalkingIcon")
            : GetConfigValue<FontAwesomeIcon>("RunningIcon")
        );

        IsVisible = !GetConfigValue<bool>("OnlyShowWhenWalking") || ctrl->IsWalking;
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),

            new BooleanWidgetConfigVariable(
                "OnlyShowWhenWalking",
                I18N.Translate("Widget.WalkingIndicator.Config.OnlyShowWhenWalking.Name"),
                I18N.Translate("Widget.WalkingIndicator.Config.OnlyShowWhenWalking.Description"),
                true
            ),
            new FaIconWidgetConfigVariable(
                "WalkingIcon",
                I18N.Translate("Widget.WalkingIndicator.Config.WalkingIcon.Name"),
                I18N.Translate("Widget.WalkingIndicator.Config.WalkingIcon.Description"),
                FontAwesomeIcon.Walking
            ) { Category = I18N.Translate("Widgets.Standard.Config.Category.Icon") },
            new FaIconWidgetConfigVariable(
                "RunningIcon",
                I18N.Translate("Widget.WalkingIndicator.Config.RunningIcon.Name"),
                I18N.Translate("Widget.WalkingIndicator.Config.RunningIcon.Description"),
                FontAwesomeIcon.Running
            ) { Category = I18N.Translate("Widgets.Standard.Config.Category.Icon") },
        ];
    }
}
