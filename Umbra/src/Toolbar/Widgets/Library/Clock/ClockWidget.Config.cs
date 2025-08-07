namespace Umbra.Widgets;

internal partial class ClockWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            
            new SelectWidgetConfigVariable(
                "TimeSource",
                I18N.Translate("Widget.Clock.Config.TimeSource.Name"),
                I18N.Translate("Widget.Clock.Config.TimeSource.Description"),
                "LT",
                new() {
                    ["LT"] = I18N.Translate("Widget.Clock.Config.TimeSource.LT"),
                    ["ST"] = I18N.Translate("Widget.Clock.Config.TimeSource.ST"),
                    ["ET"] = I18N.Translate("Widget.Clock.Config.TimeSource.ET"),
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.TimeSource") },
            new BooleanWidgetConfigVariable(
                "ClickToSwitch",
                I18N.Translate("Widget.Clock.Config.ClickToSwitch.Name"),
                I18N.Translate("Widget.Clock.Config.ClickToSwitch.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.TimeSource") },
            new BooleanWidgetConfigVariable(
                "ShowSeconds",
                I18N.Translate("Widget.Clock.Config.ShowSeconds.Name"),
                I18N.Translate("Widget.Clock.Config.ShowSeconds.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.FormatOptions") },
            new BooleanWidgetConfigVariable(
                "Use24HourFormat",
                I18N.Translate("Widget.Clock.Config.Use24HourFormat.Name"),
                I18N.Translate("Widget.Clock.Config.Use24HourFormat.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.FormatOptions") },
            new StringWidgetConfigVariable(
                "AmLabel",
                I18N.Translate("Widget.Clock.Config.AmLabel.Name"),
                I18N.Translate("Widget.Clock.Config.AmLabel.Description"),
                "am",
                16
            ) { Category = I18N.Translate("Widget.ConfigCategory.FormatOptions") },
            new StringWidgetConfigVariable(
                "PmLabel",
                I18N.Translate("Widget.Clock.Config.PmLabel.Name"),
                I18N.Translate("Widget.Clock.Config.PmLabel.Description"),
                "pm",
                16
            ) { Category = I18N.Translate("Widget.ConfigCategory.FormatOptions") },
            new BooleanWidgetConfigVariable(
                "UseCustomPrefix",
                I18N.Translate("Widget.Clock.Config.UseCustomPrefix.Name"),
                I18N.Translate("Widget.Clock.Config.UseCustomPrefix.Description"),
                false
            ),
            new StringWidgetConfigVariable(
                "PrefixText",
                I18N.Translate("Widget.Clock.Config.PrefixText.Name"),
                I18N.Translate("Widget.Clock.Config.PrefixText.Description"),
                "LT",
                16
            ) {
                DisplayIf = () => GetConfigValue<bool>("UseCustomPrefix"),
            },
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.Clock.Config.TextYOffset.Name"),
                I18N.Translate("Widget.Clock.Config.TextYOffset.Description"),
                0,
                -5,
                5
            ),
            new IntegerWidgetConfigVariable(
                "PrefixYOffset",
                I18N.Translate("Widget.Clock.Config.PrefixYOffset.Name"),
                I18N.Translate("Widget.Clock.Config.PrefixYOffset.Description"),
                1,
                -5,
                5
            ),
            new IntegerWidgetConfigVariable(
                "CustomWidth",
                I18N.Translate("Widget.Clock.Config.CustomWidth.Name"),
                I18N.Translate("Widget.Clock.Config.CustomWidth.Description"),
                0,
                0,
                256
            ),
        ];
    }
}
