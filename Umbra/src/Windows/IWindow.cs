using System;
using System.Numerics;

namespace Umbra.Windows;

public interface IWindow : IDisposable
{
    public event Action? RequestClose;

    public Vector2 Position { get; }
    public Vector2 Size     { get; }

    public bool IsClosed    { get; }
    public bool IsMinimized { get; }
    public bool IsFocused   { get; }
    public bool IsHovered   { get; }

    public void Close();

    public void Render(string instanceId);
}
