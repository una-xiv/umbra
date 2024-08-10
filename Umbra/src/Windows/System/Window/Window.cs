/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Numerics;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows;

internal abstract partial class Window : IDisposable
{
    internal event Action? RequestClose;

    internal Vector2 Position { get; set; }
    internal Vector2 Size     { get; set; }

    protected abstract Node Node { get; }

    /// <summary>
    /// Returns the title of this window.
    /// </summary>
    protected abstract string Title { get; }

    /// <summary>
    /// Invoked when the window is opened, before drawing the first frame.
    /// </summary>
    protected abstract void OnOpen();

    /// <summary>
    /// Invoked on every frame for as long as the window is open.
    /// </summary>
    protected abstract void OnUpdate(int instanceId);

    /// <summary>
    /// Invoked when the window is closed.
    /// </summary>
    protected abstract void OnClose();

    protected virtual void OnDisposed() { }

    public void Dispose()
    {
        OnDisposed();
        _windowNode.Dispose();
        Node.Dispose();
    }

    /// <summary>
    /// Closes this window.
    /// </summary>
    public void Close()
    {
        _isOpened = false;
        OnClose();
        RequestClose?.Invoke();
    }

    private bool _isOpened = false;

    public bool IsClosed => !_isOpened;

    public void Render(string id)
    {
        if (!_isOpened) {
            _isOpened = true;

            if (ContentNode.ChildNodes.Count == 0) {
                ContentNode.AppendChild(Node);
                MinimizeButtonNode.OnMouseUp                        += ToggleMinimize;
                _windowNode.QuerySelector("CloseButton")!.OnMouseUp += _ => Close();
            }

            OnOpen();
        }

        TitlebarNode.QuerySelector("TitleText")!.NodeValue = Title;

        MinimizeButtonNode.NodeValue = IsMinimized
            ? FontAwesomeIcon.ChevronDown.ToIconString()
            : FontAwesomeIcon.WindowMinimize.ToIconString();

        RenderWindow(id);
    }

    private Node TitlebarNode       => _windowNode.QuerySelector(".window--titlebar")!;
    private Node TitlebarTextNode   => _windowNode.QuerySelector(".window--titlebar-text")!;
    private Node ContentNode        => _windowNode.QuerySelector(".window--content")!;
    private Node MinimizeButtonNode => _windowNode.QuerySelector("#MinimizeButton")!;

    private Vector2 CurrentWindowSize { get; set; }
    private Vector2 StoredWindowSize  { get; set; }

    private void ToggleMinimize(Node _)
    {
        IsMinimized = !IsMinimized;

        if (IsMinimized) {
            StoredWindowSize            = CurrentWindowSize;
            ContentNode.Style.IsVisible = false;
            ImGui.SetWindowSize(CurrentWindowSize with { Y = 35 * Node.ScaleFactor }, ImGuiCond.FirstUseEver);
        } else {
            ContentNode.Style.IsVisible = true;
            ImGui.SetWindowSize(CurrentWindowSize, ImGuiCond.Once);
        }
    }
}
