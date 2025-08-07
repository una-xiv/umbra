using System.Text;

namespace Umbra.Game.Script;

internal record ScriptContext
{
    /// <summary>
    /// A collection of dependencies that this script context has.
    /// </summary>
    public HashSet<string> Dependencies { get; } = [];
    
    /// <summary>
    /// A list of referenced functions in this script.
    /// </summary>
    public HashSet<string> Functions { get; } = [];

    public List<Node> Nodes { get; } = new();
    
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine("ScriptContext:");
        sb.AppendLine($"\tDependencies: {string.Join(", ", Dependencies)}");
        sb.AppendLine($"\tFunctions: {string.Join(", ", Functions)}");
        sb.AppendLine($"\tNodes:");
        
        foreach (var node in Nodes)
        {
            sb.AppendLine($"{node.ToString(1)}");
        }

        return sb.ToString();
    }
}
