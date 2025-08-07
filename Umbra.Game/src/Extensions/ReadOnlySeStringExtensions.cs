using Lumina.Text.ReadOnly;

namespace Umbra.Game;

public static class ReadOnlySeStringExtensions
{
    public static string ToNameString(this ReadOnlySeString seString)
    {
        var str = seString.ExtractText();
        if (string.IsNullOrWhiteSpace(str)) return string.Empty;
        return str[0].ToString().ToUpper() + str[1..];
    }
}
