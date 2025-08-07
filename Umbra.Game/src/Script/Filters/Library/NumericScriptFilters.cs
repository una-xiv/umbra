using System.Globalization;

namespace Umbra.Game.Script.Filters.Library;

[Service]
internal class NumericScriptFilters
{
    [ScriptFilter("format", "Formats a number to show decimal places in accordance with Umbra's decimal configuration.")]
    public string Format(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || !float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var number)) {
            return "0";
        }

        return I18N.FormatNumber(number);
    }
}
