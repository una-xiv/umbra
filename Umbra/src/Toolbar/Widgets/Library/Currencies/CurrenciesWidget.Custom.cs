namespace Umbra.Widgets;

internal sealed partial class CurrenciesWidget
{
    private readonly Dictionary<uint, Currency> _customCurrencies = [];

    private string _lastSeenCustomCurrencyIds = string.Empty;

    private void UpdateCustomCurrencyIds()
    {
        string customCurrencyIds = GetConfigValue<string>("CustomCurrencyIds");

        if (customCurrencyIds == _lastSeenCustomCurrencyIds) return;
        _lastSeenCustomCurrencyIds = customCurrencyIds;

        foreach (var currency in _customCurrencies.Values) {
            var group  = Groups[currency.Group];
            var button = CurrencyButtons[currency.Id];
            group.Remove(button, true);
        }

        _customCurrencies.Clear();

        foreach (var idString in customCurrencyIds.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
            if (!uint.TryParse(idString, out uint id)) continue;
            if (DefaultCurrencies.ContainsKey(id)) continue;

            Currency? currency = CreateCurrencyFromItemId(id);
            if (currency == null) continue;

            currency.Group = Group.Miscellaneous;
            
            _customCurrencies.Add(id, currency);
            AddOrUpdateButtonForCurrency(currency);
        }
    }
}
