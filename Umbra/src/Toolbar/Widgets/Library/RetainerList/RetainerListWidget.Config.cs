using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.RetainerList;

internal partial class RetainerListWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
            new SelectWidgetConfigVariable(
                "IconType",
                I18N.Translate("Widget.RetainerList.Config.IconType.Name"),
                I18N.Translate("Widget.RetainerList.Config.IconType.Description"),
                "Default",
                new() {
                    { "Default", I18N.Translate("Widget.RetainerList.Config.IconType.Option.Default") },
                    { "Framed", I18N.Translate("Widget.RetainerList.Config.IconType.Option.Framed") },
                    { "Gearset", I18N.Translate("Widget.RetainerList.Config.IconType.Option.Gearset") },
                    { "Glowing", I18N.Translate("Widget.RetainerList.Config.IconType.Option.Glowing") },
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        ];
    }
}
