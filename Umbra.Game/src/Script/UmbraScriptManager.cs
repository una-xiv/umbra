namespace Umbra.Game.Script;

[Service]
public class UmbraScriptManager
{
    private Dictionary<string, CachedScript> ScriptCache { get; } = [];
}

internal class CachedScript(UmbraScript script)
{
    public UmbraScript Script { get; } = script;
    
    
}
