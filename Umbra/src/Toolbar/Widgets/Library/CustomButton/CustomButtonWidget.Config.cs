using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets;

internal partial class CustomButtonWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            new BooleanWidgetConfigVariable(
                "HideLabel",
                I18N.Translate("Widget.CustomButton.Config.HideLabel.Name"),
                I18N.Translate("Widget.CustomButton.Config.HideLabel.Description"),
                false
            ),
            new StringWidgetConfigVariable(
                "Label",
                I18N.Translate("Widget.CustomButton.Config.Label.Name"),
                I18N.Translate("Widget.CustomButton.Config.Label.Description"),
                "My button",
                1024
            ),
            new StringWidgetConfigVariable(
                "Tooltip",
                I18N.Translate("Widget.CustomButton.Config.Tooltip.Name"),
                I18N.Translate("Widget.CustomButton.Config.Tooltip.Description"),
                ""
            ),
            new SelectWidgetConfigVariable(
                "Mode",
                I18N.Translate("Widget.CustomButton.Config.Mode.Name"),
                I18N.Translate("Widget.CustomButton.Config.Mode.Description"),
                "Command",
                new() {
                    { "Command", I18N.Translate("Widget.CustomButton.Config.Mode.Option.Command") },
                    { "URL", I18N.Translate("Widget.CustomButton.Config.Mode.Option.URL") }
                }
            ),
            new StringWidgetConfigVariable(
                "Command",
                I18N.Translate("Widget.CustomButton.Config.Command.Name"),
                I18N.Translate("Widget.CustomButton.Config.Command.Description"),
                "/echo Hello, world!"
            ),
            new SelectWidgetConfigVariable(
                "AltMode",
                I18N.Translate("Widget.CustomButton.Config.AltMode.Name"),
                I18N.Translate("Widget.CustomButton.Config.AltMode.Description"),
                "None",
                new() {
                    { "None", I18N.Translate("Widget.CustomButton.Config.Mode.Option.None") },
                    { "Command", I18N.Translate("Widget.CustomButton.Config.Mode.Option.Command") },
                    { "URL", I18N.Translate("Widget.CustomButton.Config.Mode.Option.URL") }
                }
            ),
            new StringWidgetConfigVariable(
                "AltCommand",
                I18N.Translate("Widget.CustomButton.Config.AltCommand.Name"),
                I18N.Translate("Widget.CustomButton.Config.AltCommand.Description"),
                ""
            ) { DisplayIf = () => GetConfigValue<string>("AltMode") != "None" },
        ];
    }
}
