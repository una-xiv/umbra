using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.Compass;

internal sealed partial class CompassWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.Decorate.Name"),
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.Decorate.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "UseCameraAngle",
                I18N.Translate("Widget.Compass.Config.UseCameraAngle.Name"),
                I18N.Translate("Widget.Compass.Config.UseCameraAngle.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "Width",
                I18N.Translate("Widget.Compass.Config.Width.Name"),
                I18N.Translate("Widget.Compass.Config.Width.Description"),
                250,
                64,
                1024
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new FloatWidgetConfigVariable(
                "FishEyePower",
                I18N.Translate("Widget.Compass.Config.FishEyePower.Name"),
                I18N.Translate("Widget.Compass.Config.FishEyePower.Description"),
                0.75f,
                0.5f,
                1.8f
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "AngleRangeDegrees",
                I18N.Translate("Widget.Compass.Config.AngleRangeDegrees.Name"),
                I18N.Translate("Widget.Compass.Config.AngleRangeDegrees.Description"),
                180,
                45,
                360
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new ColorWidgetConfigVariable(
                "CenterLineColor",
                I18N.Translate("Widget.Compass.Config.CenterLineColor.Name"),
                I18N.Translate("Widget.Compass.Config.CenterLineColor.Description"),
                0xFF0000FF
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new ColorWidgetConfigVariable(
                "LineColor",
                I18N.Translate("Widget.Compass.Config.LineColor.Name"),
                I18N.Translate("Widget.Compass.Config.LineColor.Description"),
                0xFFACFFAA
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new ColorWidgetConfigVariable(
                "TextColor",
                I18N.Translate("Widget.Compass.Config.TextColor.Name"),
                I18N.Translate("Widget.Compass.Config.TextColor.Description"),
                0xFFFFFFFF
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        ];
    }
}
