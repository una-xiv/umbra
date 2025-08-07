namespace Umbra.Game.Script.Filters.Library;

[Service]
internal class StringScriptFilters
{
    [ScriptFilter("trim", "Trims whitespace from the start and end of the text.")]
    public string Trim(string input)
    {
        return input.Trim();
    }
    
    [ScriptFilter("lower", "Converts text to lowercase.")]
    public string ToLower(string input)
    {
        return input.ToLowerInvariant();
    }

    [ScriptFilter("upper", "Converts text to uppercase.")]
    public string ToUpper(string input)
    {
        return input.ToUpperInvariant();
    }
    
    [ScriptFilter("first", "Extracts the first word from the text.")]
    public string First(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var words = input.Split(' ');
        return words.Length > 0 ? words[0] : string.Empty;
    }

    [ScriptFilter("last", "Extracts the last word from the text.")]
    public string Last(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        
        var words = input.Split(' ');
        return words.Length > 0 ? words[^1] : string.Empty;
    }
    
    [ScriptFilter("initials", "Returns the initials of the text. For example, \"Player Name\" becomes \"P. N.\"")]
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
