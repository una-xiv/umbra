using System.Collections.Generic;
using System.Drawing;
using Umbra.Common;
using Color = Una.Drawing.Color;

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

            new BooleanWidgetConfigVariable(
                "UseThresholdColors",
                I18N.Translate("Widget.Currencies.Config.UseThresholdColors.Name"),
                I18N.Translate("Widget.Currencies.Config.UseThresholdColors.Description"),
                false
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group    = I18N.Translate("Widget.Currencies.Config.ThresholdColors")
            },

            new IntegerWidgetConfigVariable(
                "ThresholdPercentage",
                I18N.Translate("Widget.Currencies.Config.ThresholdPercentage.Name"),
                I18N.Translate("Widget.Currencies.Config.ThresholdPercentage.Description"),
                75,
                0,
                100
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group    = I18N.Translate("Widget.Currencies.Config.ThresholdColors"),
                DisplayIf = () => GetConfigValue<bool>("UseThresholdColors")
            },

            new ColorWidgetConfigVariable(
                "ThresholdColor",
                I18N.Translate("Widget.Currencies.Config.ThresholdColor.Name"),
                I18N.Translate("Widget.Currencies.Config.ThresholdColor.Description"),
                0xFF44FFFF
            ) {
                Category  = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group     = I18N.Translate("Widget.Currencies.Config.ThresholdColors"),
                DisplayIf = () => GetConfigValue<bool>("UseThresholdColors")
            },

            new ColorWidgetConfigVariable(
                "ThresholdCapColor",
                I18N.Translate("Widget.Currencies.Config.ThresholdCapColor.Name"),
                I18N.Translate("Widget.Currencies.Config.ThresholdCapColor.Description"),
                0xFF4444FF
            ) {
                Category  = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group     = I18N.Translate("Widget.Currencies.Config.ThresholdColors"),
                DisplayIf = () => GetConfigValue<bool>("UseThresholdColors")
            },

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
