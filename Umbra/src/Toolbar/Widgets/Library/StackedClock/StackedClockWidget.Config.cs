using System.Collections.Generic;
using Umbra.Common;
using NotImplementedException = System.NotImplementedException;

namespace Umbra.Widgets.Library.StackedClock;

internal partial class StackedClockWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new SelectWidgetConfigVariable(
                "TimeSourceTop",
                I18N.Translate("Widget.StackedClock.Config.TimeSourceTop.Name"),
                I18N.Translate("Widget.StackedClock.Config.TimeSourceTop.Description"),
                "LT",
                new() {
                    ["LT"] = I18N.Translate("Widget.StackedClock.Config.TimeSource.Option.LT"),
                    ["ST"] = I18N.Translate("Widget.StackedClock.Config.TimeSource.Option.ST"),
                    ["ET"] = I18N.Translate("Widget.StackedClock.Config.TimeSource.Option.ET"),
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.TimeSource") },
            new SelectWidgetConfigVariable(
                "TimeSourceBottom",
                I18N.Translate("Widget.StackedClock.Config.TimeSourceBottom.Name"),
                I18N.Translate("Widget.StackedClock.Config.TimeSourceBottom.Description"),
                "ET",
                new() {
                    ["LT"]   = I18N.Translate("Widget.StackedClock.Config.TimeSource.Option.LT"),
                    ["ST"]   = I18N.Translate("Widget.StackedClock.Config.TimeSource.Option.ST"),
                    ["ET"]   = I18N.Translate("Widget.StackedClock.Config.TimeSource.Option.ET"),
                    ["None"] = I18N.Translate("Widget.StackedClock.Config.TimeSource.Option.None"),
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.TimeSource") },
            new StringWidgetConfigVariable(
                "TimeFormatTop",
                I18N.Translate("Widget.StackedClock.Config.TimeFormatTop.Name"),
                I18N.Translate("Widget.StackedClock.Config.TimeFormatTop.Description"),
                "HH:mm"
            ) { Category = I18N.Translate("Widget.ConfigCategory.TimeSource") },
            new StringWidgetConfigVariable(
                "TimeFormatBottom",
                I18N.Translate("Widget.StackedClock.Config.TimeFormatBottom.Name"),
                I18N.Translate("Widget.StackedClock.Config.TimeFormatBottom.Description"),
                "HH:mm"
            ) { Category = I18N.Translate("Widget.ConfigCategory.TimeSource") },
            new StringWidgetConfigVariable(
                "TimeFormatPopup",
                I18N.Translate("Widget.StackedClock.Config.TimeFormatPopup.Name"),
                I18N.Translate("Widget.StackedClock.Config.TimeFormatPopup.Description"),
                "HH:mm:ss"
            ) { Category = I18N.Translate("Widget.ConfigCategory.TimeSource") },
            new SelectWidgetConfigVariable(
                "PrefixPosition",
                I18N.Translate("Widget.StackedClock.Config.PrefixPosition.Name"),
                I18N.Translate("Widget.StackedClock.Config.PrefixPosition.Description"),
                "Left",
                new() {
                    ["Left"]  = I18N.Translate("Widget.StackedClock.Config.PrefixPosition.Option.Left"),
                    ["Right"] = I18N.Translate("Widget.StackedClock.Config.PrefixPosition.Option.Right"),
                    ["None"] = I18N.Translate("Widget.StackedClock.Config.PrefixPosition.Option.None"),
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.TimeSource") },
            new BooleanWidgetConfigVariable(
                "EnablePopup",
                I18N.Translate("Widget.StackedClock.Config.EnablePopup.Name"),
                I18N.Translate("Widget.StackedClock.Config.EnablePopup.Description"),
                true
            ),
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
            ..TwoLabelTextOffsetVariables,
        ];
    }
}
