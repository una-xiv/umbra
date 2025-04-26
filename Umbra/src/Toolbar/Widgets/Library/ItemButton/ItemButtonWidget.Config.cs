using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets;

internal partial class ItemButtonWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
                
            new IntegerWidgetConfigVariable(
                "ItemId",
                I18N.Translate("Widget.ItemButton.Config.ItemId.Name"),
                I18N.Translate("Widget.ItemButton.Config.ItemId.Description"),
                0,
                0
            ),
            new SelectWidgetConfigVariable(
                "ItemUsage",
                I18N.Translate("Widget.ItemButton.Config.ItemUsage.Name"),
                I18N.Translate("Widget.ItemButton.Config.ItemUsage.Description"),
                "HqBeforeNq",
                new() {
                    { "HqBeforeNq", I18N.Translate("Widget.ItemButton.Config.ItemUsage.Option.HqBeforeNq") },
                    { "NqBeforeHq", I18N.Translate("Widget.ItemButton.Config.ItemUsage.Option.NqBeforeHq") },
                    { "HqOnly", I18N.Translate("Widget.ItemButton.Config.ItemUsage.Option.HqOnly") },
                    { "NqOnly", I18N.Translate("Widget.ItemButton.Config.ItemUsage.Option.NqOnly") }
                }
            ),
            new BooleanWidgetConfigVariable(
                "HideIfNotOwned",
                I18N.Translate("Widget.ItemButton.Config.HideIfNotOwned.Name"),
                I18N.Translate("Widget.ItemButton.Config.HideIfNotOwned.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowLabel",
                I18N.Translate("Widget.ItemButton.Config.ShowLabel.Name"),
                I18N.Translate("Widget.ItemButton.Config.ShowLabel.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowCount",
                I18N.Translate("Widget.ItemButton.Config.ShowCount.Name"),
                I18N.Translate("Widget.ItemButton.Config.ShowCount.Description"),
                true
            ),
        ];
    }
}
