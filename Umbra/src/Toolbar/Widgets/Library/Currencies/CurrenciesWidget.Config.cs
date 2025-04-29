using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets;

internal sealed partial class CurrenciesWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        InitializeDefaultItems();
        Dictionary<string, string>        trackedSelectOptions = new() { { "", "None" } };
        List<BooleanWidgetConfigVariable> visibilityOptions    = [];

        foreach (Currency currency in DefaultCurrencies.Values) {
            trackedSelectOptions.Add(currency.Id.ToString(), currency.Name);
            visibilityOptions.Add(new BooleanWidgetConfigVariable(
                $"EnabledCurrency_{currency.Id}",
                 currency.Name,
                null,
                true
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group    = "Visible Currencies"
            });
        }

        return [
            ..base.GetConfigVariables(),
            new BooleanWidgetConfigVariable(
                "EnableMouseInteraction",
                I18N.Translate("Widget.Currencies.Config.EnableMouseInteraction.Name"),
                I18N.Translate("Widget.Currencies.Config.EnableMouseInteraction.Description"),
                true
            ),
            new StringWidgetConfigVariable(
                "CustomCurrencyIds",
                I18N.Translate("Widget.Currencies.Config.CustomCurrencyIds.Name"),
                I18N.Translate("Widget.Currencies.Config.CustomCurrencyIds.Description"),
                ""
            ),
            new SelectWidgetConfigVariable(
                "TrackedCurrency",
                I18N.Translate("Widget.Currencies.Config.TrackedCurrency.Name"),
                I18N.Translate("Widget.Currencies.Config.TrackedCurrency.Description"),
                "",
                trackedSelectOptions,
                true
            ),
            new StringWidgetConfigVariable(
                "CustomLabel",
                I18N.Translate("Widget.Currencies.Config.CustomWidgetLabel.Name"),
                I18N.Translate("Widget.Currencies.Config.CustomWidgetLabel.Description"),
                "",
                100
            ),
            new BooleanWidgetConfigVariable(
                "ShowName",
                I18N.Translate("Widget.Currencies.Config.ShowName.Name"),
                I18N.Translate("Widget.Currencies.Config.ShowName.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowCapOnWidget",
                I18N.Translate("Widget.Currencies.Config.ShowCapOnWidget.Name"),
                I18N.Translate("Widget.Currencies.Config.ShowCapOnWidget.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowCap",
                I18N.Translate("Widget.Currencies.Config.ShowCap.Name"),
                I18N.Translate("Widget.Currencies.Config.ShowCap.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            ..visibilityOptions,
        ];
    }

    private uint GetTrackedCurrencyId()
    {
        string value = GetConfigValue<string>("TrackedCurrency");
        
        if (string.IsNullOrEmpty(value)) {
            return 0;
        }
        
        return uint.TryParse(value, out var id) ? id : 0;
    }

    private Currency? GetTrackedCurrency()
    {
        uint id = GetTrackedCurrencyId();
        if (id == 0) return null;
        
        return DefaultCurrencies.GetValueOrDefault(id) ?? _customCurrencies.GetValueOrDefault(id);
    }
}
