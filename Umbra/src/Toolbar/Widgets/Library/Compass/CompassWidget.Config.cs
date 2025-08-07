namespace Umbra.Widgets;

internal sealed partial class CompassWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            
            new BooleanWidgetConfigVariable(
                "UseCameraAngle",
                I18N.Translate("Widget.Compass.Config.UseCameraAngle.Name"),
                I18N.Translate("Widget.Compass.Config.UseCameraAngle.Description"),
                true
            ),
            new IntegerWidgetConfigVariable(
                "Width",
                I18N.Translate("Widget.Compass.Config.Width.Name"),
                I18N.Translate("Widget.Compass.Config.Width.Description"),
                250,
                64,
                1024
            ),
            new FloatWidgetConfigVariable(
                "FishEyePower",
                I18N.Translate("Widget.Compass.Config.FishEyePower.Name"),
                I18N.Translate("Widget.Compass.Config.FishEyePower.Description"),
                0.75f,
                0.5f,
                1.8f
            ),
            new IntegerWidgetConfigVariable(
                "AngleRangeDegrees",
                I18N.Translate("Widget.Compass.Config.AngleRangeDegrees.Name"),
                I18N.Translate("Widget.Compass.Config.AngleRangeDegrees.Description"),
                180,
                45,
                360
            ),
            new ColorWidgetConfigVariable(
                "CenterLineColor",
                I18N.Translate("Widget.Compass.Config.CenterLineColor.Name"),
                I18N.Translate("Widget.Compass.Config.CenterLineColor.Description"),
                0xFF0000FF
            ),
            new ColorWidgetConfigVariable(
                "LineColor",
                I18N.Translate("Widget.Compass.Config.LineColor.Name"),
                I18N.Translate("Widget.Compass.Config.LineColor.Description"),
                0xFFACFFAA
            ),
            new ColorWidgetConfigVariable(
                "TextColor",
                I18N.Translate("Widget.Compass.Config.TextColor.Name"),
                I18N.Translate("Widget.Compass.Config.TextColor.Description"),
                0xFFFFFFFF
            ),
        ];
    }
}
