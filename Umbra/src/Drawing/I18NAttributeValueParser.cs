using System.Text.RegularExpressions;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Drawing;

internal partial class I18NAttributeValueParser : IUdtAttributeValueParser
{
    /// <summary>
    /// Replaces occurrences of _L(name) with the translated string.
    /// </summary>
    public string Parse(string value)
    {
        while (I18nRegex().IsMatch(value)) {
            Match  match = I18nRegex().Match(value);
            string name  = match.Groups[1].Value;

            // TODO: Add i18n parameter support.
            
            value = value.Replace(
                match.Value,
                I18N.Translate(name)
            );
        }
        
        return value;
    }
    
    public void Dispose()
    {
    }
    
    [GeneratedRegex(@"_L\(([A-Za-z0-9_.]+)\)")]
    private static partial Regex I18nRegex();
}
