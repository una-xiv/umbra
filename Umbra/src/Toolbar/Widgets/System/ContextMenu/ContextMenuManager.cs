using ImGuiNET;
using System;
using System.Numerics;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Widgets;

[Service]
internal sealed class ContextMenuManager(UmbraDelvClipRects clipRects) : IDisposable
{
    private ContextMenu? _contextMenu;
    private Action?      _closeCallback;

    public void Dispose()
    {
        if (null != _contextMenu) {
            _contextMenu.OnEntryInvoked -= OnEntryInvoked;
            _contextMenu.Dispose();
        }

        foreach (var i in _closeCallback?.GetInvocationList() ?? []) {
            _closeCallback -= (Action)i;
        }

        _closeCallback = null;
        _contextMenu   = null;
    }

    /// <summary>
    /// Presents the given context menu.
    /// </summary>
    public void Present(ContextMenu menu, Action? closeCallback = null)
    {
        if (null != _contextMenu) {
            _contextMenu.OnEntryInvoked -= OnEntryInvoked;
            _contextMenu.Dispose();
            _closeCallback?.Invoke();
        }

        menu.OnEntryInvoked += OnEntryInvoked;

        _contextMenu   = menu;
        _closeCallback = closeCallback;
        _isOpen        = false;
    }

    /// <summary>
    /// Draws the currently active context menu.
    /// </summary>
    /// <remarks>
    /// This method is invoked from the WidgetPopup.Render method to ensure the
    /// spawned context menu is a child of the widget popup itself. This ensures
    /// that the widget popup does not close itself due to focus loss to another
    /// ImGui window.
    /// </remarks>
    public void Draw()
    {
        if (null == _contextMenu) {
            clipRects.RemoveClipRect("Umbra.ContextMenu");
            return;
        }

        Rect boundingBox = _contextMenu.Node.Bounds.MarginRect;
        ImGui.SetNextWindowSize(new(boundingBox.Width + 32, boundingBox.Height + 32));

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,     Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding,   0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
        ImGui.PushStyleColor(ImGuiCol.Border,  0);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, 0);

        if (ImGui.BeginPopup(_contextMenu.Id, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoSavedSettings)) {
            Vector2 cursorPos = ImGui.GetCursorScreenPos();
            Point position    = new((int)cursorPos.X + 16, (int)cursorPos.Y + 16);

            _contextMenu.Node.Render(ImGui.GetWindowDrawList(), position);

            clipRects.SetClipRect("Umbra.ContextMenu", new(position.X, position.Y), new(boundingBox.Width, boundingBox.Height));

            ImGui.EndPopup();
        } else if (_isOpen) {
            _isOpen      = false;

            ImGui.PopStyleColor(2);
            ImGui.PopStyleVar(5);

            clipRects.RemoveClipRect("Umbra.ContextMenu");
            _closeCallback?.Invoke();
            _contextMenu = null;
            return;
        }

        if (!_isOpen) {
            _contextMenu.Node.Reflow();
            ImGui.OpenPopup(_contextMenu.Id);
            _isOpen = true;
        }

        ImGui.PopStyleColor(2);
        ImGui.PopStyleVar(5);
    }

    private bool _isOpen;

    private void OnEntryInvoked(ContextMenuEntry _)
    {
        if (null == _contextMenu) return;

        _contextMenu.OnEntryInvoked -= OnEntryInvoked;

        _closeCallback?.Invoke();
        _closeCallback = null;
        _contextMenu   = null;
    }
}
