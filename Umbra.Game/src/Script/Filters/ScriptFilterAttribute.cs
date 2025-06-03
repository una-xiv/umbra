using System;

namespace Umbra.Game.Script.Filters;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ScriptFilterAttribute(string name, string description) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
}
