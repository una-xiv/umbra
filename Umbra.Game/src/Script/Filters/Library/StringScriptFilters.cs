using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Game.Script.Filters.Library;

[Service]
internal class StringScriptFilters
{
    [ScriptFilter("initials", "Returns the initials of a string.")]
    public string Initials(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var words = input.Split(' ');

        List<string> initials = [];

        initials.AddRange(
            from word in words
            where !string.IsNullOrEmpty(word)
            select word[0].ToString() + '.'
        );

        return string.Join(" ", initials);
    }
}
