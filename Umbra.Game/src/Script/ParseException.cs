using System;

namespace Umbra.Game.Script;

public class ParseException(string message, string snippet, int column) : Exception
{
    public string Snippet { get; } = snippet;
    public int    Column  { get; } = column;

    public override string Message => $"{message} at column {Column} in snippet: \"{Snippet}\".";
}
