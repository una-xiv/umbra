using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets;

internal partial class DtrBarWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "HideNative",
                I18N.Translate("Widget.DtrBar.Config.HideNative.Name"),
                I18N.Translate("Widget.DtrBar.Config.HideNative.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "PlainText",
                I18N.Translate("Widget.DtrBar.Config.PlainText.Name"),
                I18N.Translate("Widget.DtrBar.Config.PlainText.Description"),
                false
            ),
            new SelectWidgetConfigVariable(
                "DecorateMode",
                I18N.Translate("Widget.DtrBar.Config.DecorateMode.Name"),
                I18N.Translate("Widget.DtrBar.Config.DecorateMode.Description"),
                "Always",
                new() {
                    { "Always", I18N.Translate("Widget.DtrBar.Config.DecorateMode.Option.Always") },
                    { "Never", I18N.Translate("Widget.DtrBar.Config.DecorateMode.Option.Never") },
                    { "Auto", I18N.Translate("Widget.DtrBar.Config.DecorateMode.Option.Auto") },
                }
            ),
            new IntegerWidgetConfigVariable(
                "ItemSpacing",
                I18N.Translate("Widget.DtrBar.Config.ItemSpacing.Name"),
                I18N.Translate("Widget.DtrBar.Config.ItemSpacing.Description"),
                6,
                0,
                64
            ),
            new IntegerWidgetConfigVariable(
                "MaxTextWidth",
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.MaxTextWidth.Name"),
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.MaxTextWidth.Description"),
                0,
                0,
                1000
            ),
            new IntegerWidgetConfigVariable(
                "TextSize",
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextSize.Name"),
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextSize.Description"),
                13,
                8,
                24
            ),
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.DtrBar.Config.TextYOffset.Name"),
                I18N.Translate("Widget.DtrBar.Config.TextYOffset.Description"),
                0,
                -5,
                5
            ),
        ];
    }
}
