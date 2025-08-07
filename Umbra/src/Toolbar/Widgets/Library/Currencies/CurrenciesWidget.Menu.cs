using System.Text;

namespace Umbra.Widgets;

internal sealed partial class CurrenciesWidget
{
    private Dictionary<Group, MenuPopup.Group> Groups { get; } = new() {
        { Group.None, new MenuPopup.Group("") },
        { Group.TheHunt, new MenuPopup.Group(I18N.Translate("Widget.Currencies.Group.TheHunt")) },
        { Group.Tomestones, new MenuPopup.Group(I18N.Translate("Widget.Currencies.Group.Tomestones")) },
        { Group.PvP, new MenuPopup.Group(I18N.Translate("Widget.Currencies.Group.PvP")) },
        { Group.CraftingAndGathering, new MenuPopup.Group(I18N.Translate("Widget.Currencies.Group.CraftingGathering")) },
        { Group.Miscellaneous, new MenuPopup.Group(I18N.Translate("Widget.Currencies.Group.Miscellaneous")) },
    };

    private Dictionary<uint, MenuPopup.Button> CurrencyButtons { get; } = [];

    private void InitializeMenu()
    {
        foreach (var group in Groups.Values) Popup.Add(group);
        foreach (var currency in DefaultCurrencies.Values) AddOrUpdateButtonForCurrency(currency);
    }

    private enum Group
    {
        None,
        TheHunt,
        Tomestones,
        PvP,
        CraftingAndGathering,
        Miscellaneous,
    }

    private void OnCurrencyButtonClicked(Currency currency)
    {
        SetConfigValue("TrackedCurrency", GetTrackedCurrencyId() == currency.Id ? "" : currency.Id.ToString());
    }

    private void AddOrUpdateButtonForCurrency(Currency currency)
    {
        if (!Groups.TryGetValue(currency.Group, out var group)) return;

        if (!CurrencyButtons.TryGetValue(currency.Id, out var button)) {
            button = new MenuPopup.Button(currency.Name) {
                AltText = GetCountText(currency, GetConfigValue<bool>("ShowCap")),
                Icon    = currency.IconId,
                OnClick = () => OnCurrencyButtonClicked(currency),
            };

            group.Add(button);
            CurrencyButtons.Add(currency.Id, button);
        }

        string cvarName  = $"EnabledCurrency_{currency.Id}";
        bool   isEnabled = !HasConfigVariable(cvarName) || (HasConfigVariable(cvarName) && GetConfigValue<bool>(cvarName));

        button.Selected = GetTrackedCurrencyId() == currency.Id;
        button.AltText  = GetCountText(currency, GetConfigValue<bool>("ShowCap"));
        button.IsVisible = isEnabled && currency.Id switch {
            ItemIdMaelstrom      => Player.GrandCompanyId == 1,
            ItemIdTwinAdder      => Player.GrandCompanyId == 2,
            ItemIdImmortalFlames => Player.GrandCompanyId == 3,
            _                    => true
        };

        if (!GetConfigValue<bool>("UseThresholdColors") || currency.Capacity == 0) {
            button.AltTextColor = null;
            return;
        }

        int  thresholdPercentage = GetConfigValue<int>("ThresholdPercentage");
        uint warningColor        = GetConfigValue<uint>("ThresholdColor");
        uint capColor            = GetConfigValue<uint>("ThresholdCapColor");

        if (currency.Count == currency.Capacity) {
            button.AltTextColor = new(capColor);
        } else if (thresholdPercentage > 0) {
            button.AltTextColor = currency.Count >= currency.Capacity * thresholdPercentage / 100 ? new(warningColor) : null;
        } else {
            button.AltTextColor = null;
        }
    }

    private string GetCountText(Currency currency, bool showCap)
    {
        StringBuilder sb = new();

        sb.Append(I18N.FormatNumber(currency.Count));

        if (showCap && currency.Capacity > 0) {
            sb.Append(" / ");
            sb.Append(I18N.FormatNumber(currency.Capacity));

            if (currency.WeeklyCapacity > 0) {
                sb.Append(" (");
                sb.Append(I18N.FormatNumber(currency.WeeklyCount));
                sb.Append(" / ");
                sb.Append(I18N.FormatNumber(currency.WeeklyCapacity));
                sb.Append(')');
            }
        }

        return sb.ToString();
    }
}
