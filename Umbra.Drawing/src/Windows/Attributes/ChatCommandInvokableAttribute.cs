using System;

namespace Umbra.Drawing;

/// <summary>
/// Allows spawning the annotated window class using a chat command.
/// </summary>
/// <param name="command"></param>
/// <param name="description"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class ChatCommandInvokableAttribute(string command, string? description = "") : Attribute
{
    /// <summary>
    /// The chat command that invokes this window.
    /// </summary>
    public string Command { get; } = command;

    /// <summary>
    /// The description of the command.
    /// </summary>
    public string? Description { get; } = description;
}
