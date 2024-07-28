using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.RetainerList;

internal partial class RetainerListWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
        new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.RetainerList.Config.Decorate.Name"),
                I18N.Translate("Widget.RetainerList.Config.Decorate.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "DesaturateIcon",
                I18N.Translate("Widget.RetainerList.Config.DesaturateIcon.Name"),
                I18N.Translate("Widget.RetainerList.Config.DesaturateIcon.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "DisplayMode",
                I18N.Translate("Widget.RetainerList.Config.DisplayMode.Name"),
                I18N.Translate("Widget.RetainerList.Config.DisplayMode.Description"),
                "TextOnly",
                new() {
                    { "TextOnly", I18N.Translate("Widget.RetainerList.Config.DisplayMode.Option.TextOnly") },
                    { "IconOnly", I18N.Translate("Widget.RetainerList.Config.DisplayMode.Option.IconOnly") },
                    { "TextAndIcon", I18N.Translate("Widget.RetainerList.Config.DisplayMode.Option.TextAndIcon") }
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "IconLocation",
                I18N.Translate("Widget.RetainerList.Config.IconLocation.Name"),
                I18N.Translate("Widget.RetainerList.Config.IconLocation.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.RetainerList.Config.IconLocation.Option.Left") },
                    { "Right", I18N.Translate("Widget.RetainerList.Config.IconLocation.Option.Right") }
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.RetainerList.Config.TextYOffset.Name"),
                I18N.Translate("Widget.RetainerList.Config.TextYOffset.Description"),
                -1,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
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
