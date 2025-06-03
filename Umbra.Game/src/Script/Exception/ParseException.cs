namespace Umbra.Game.Script;

public class ParseException(string message, string snippet, int column) : ScriptException(message)
{
    public override string Message => $"{base.Message} at column {column}: \"{snippet}\".";
}
