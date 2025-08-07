namespace Umbra.Windows;

[Service]
public class WindowManager(UmbraDelvClipRects delvClipRects) : IDisposable
{
    public event Action<IWindow>? OnWindowOpened;
    public event Action<IWindow>? OnWindowClosed;

    public static bool HasFocusedWindow { get; private set; }
    
    private readonly Dictionary<string, IWindow>          _instances = [];
    private readonly Dictionary<string, Action<IWindow>?> _callbacks = [];

    public void Dispose()
    {
        foreach (var win in _instances.Values)
            win.Dispose();

        _instances.Clear();

        foreach (var handler in OnWindowOpened?.GetInvocationList() ?? []) OnWindowOpened -= (Action<IWindow>)handler;
        foreach (var handler in OnWindowClosed?.GetInvocationList() ?? []) OnWindowClosed -= (Action<IWindow>)handler;
    }

    /// <summary>
    /// Presents a window and invokes the given callback once the window is
    /// closed. Use the callback to retrieve data from the window instance.
    /// </summary>
    /// <param name="id">A unique ID for this instance.</param>
    /// <param name="window">An instance of <see cref="Window"/></param>
    /// <param name="onClose">A callback function.</param>
    /// <param name="onCreate">A callback function that is invoked when the window is created.</param>
    /// <typeparam name="T"></typeparam>
    public void Present<T>(string id, T window, Action<T>? onClose = null, Action<T>? onCreate = null) where T : IWindow
    {
        Framework.DalamudFramework.Run(
            () => {
                if (_instances.TryGetValue(id, out IWindow? wnd)) {
                    wnd.Close();
                }

                window.RequestClose += () => {
                    delvClipRects.RemoveClipRect($"Umbra.Window.{id}");
                    _callbacks[id]?.Invoke(window);
                    _instances.Remove(id);
                    _callbacks.Remove(id);
                    OnWindowClosed?.Invoke(window);
                };

                _instances[id] = window;
                _callbacks[id] = onClose is not null ? o => onClose((T)o) : null;

                onCreate?.Invoke(window);
                OnWindowOpened?.Invoke(window);
            }
        );
    }

    /// <summary>
    /// Returns true if the window with the given ID is open.
    /// </summary>
    public bool IsOpen(string id)
    {
        return _instances.ContainsKey(id);
    }

    public void Close(string id)
    {
        if (!_instances.Remove(id, out var window)) return;

        delvClipRects.RemoveClipRect($"Umbra.Window.{id}");
        _callbacks.Remove(id);

        window.Close();
    }

    [OnDraw]
    private void OnDraw()
    {
        bool hasFocusedWindow = false;

        lock (_instances) {
            foreach ((string id, IWindow window) in _instances) {
                window.Render(id);
                
                if (window.IsFocused) hasFocusedWindow = true;
                if (!window.IsClosed) delvClipRects.SetClipRect($"Umbra.Window.{id}", window.Position, window.Size);
            }
        }
        
        HasFocusedWindow = hasFocusedWindow;
    }
}
