namespace Umbra.Game.Script;

public class ScriptException(string message) : Exception
{
    public override string Message { get; } = message;
}
