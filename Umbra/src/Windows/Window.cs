using ImGuiNET;
using System;
using System.Numerics;
using Umbra.Common;
using Una.Drawing;
using Una.Drawing.Clipping;

namespace Umbra.Windows;

public abstract class Window : IWindow
{
    [ConfigVariable("Window.EnableClipping", "General", "WindowSettings")]
    private static bool EnableClipping { get; set; } = true;

    public event Action? RequestClose;

    public Vector2 Position    { get; set; }
    public Vector2 Size        { get; set; }
    public bool    IsClosed    { get; } = false;
    public bool    IsMinimized { get; private set; }
    public bool    IsFocused   { get; private set; }
    public bool    IsHovered   { get; private set; }

    protected Node         RootNode       { get; set; } = null!;
    protected Node         WindowNode     { get; }
    protected UdtDocument? Document       { get; set; }
    private   UdtDocument  WindowDocument { get; }

    private bool    _isDisposed;
    private bool    _isOpened;
    private Vector2 _currentSize;

    protected Window()
    {
        Position = Vector2.Zero;
        Size     = Vector2.Zero;

        WindowDocument = UmbraDrawing.DocumentFrom("umbra.windows.window.xml");
        WindowNode     = WindowDocument.RootNode!;

        WindowNode.QuerySelector(".button.close")!.OnMouseUp    += _ => Dispose();
        WindowNode.QuerySelector(".button.collapse")!.OnMouseUp += _ => ToggleMinimize();
        WindowNode.QuerySelector(".button.expand")!.OnMouseUp   += _ => ToggleMinimize();
    }

    public void Close()
    {
        Dispose();
    }

    public void Render(string instanceId)
    {
        if (_isDisposed) return;
        if (!_isOpened) {
            _isOpened = true;

            try {
                Document = UmbraDrawing.DocumentFrom(UdtResourceName);
                if (null == Document.RootNode) {
                    Dispose();
                    throw new Exception($"{UdtResourceName} does not contain a root node.");
                }

                RootNode = Document.RootNode;

                WindowNode.QuerySelector(".title")!.NodeValue = Title;
                WindowNode.QuerySelector("#Content")?.AppendChild(RootNode);
                WindowNode.ToggleClass("no-collapse", GetWindowFlags().HasFlag(ImGuiWindowFlags.NoCollapse));

                OnOpen();
            } catch (Exception e) {
                Logger.Error(e.Message);
                Logger.Error(e.StackTrace);
                Dispose();
                return;
            }
        }

        try {
            RenderWindow(instanceId);
        } catch (Exception e) {
            Logger.Error(e.Message);
            Logger.Error(e.StackTrace);
            Dispose();
        }
    }

    protected abstract string  UdtResourceName { get; }
    protected abstract string  Title           { get; }
    protected abstract Vector2 MinSize         { get; }
    protected abstract Vector2 MaxSize         { get; }
    protected abstract Vector2 DefaultSize     { get; }

    protected virtual ImGuiWindowFlags GetWindowFlags() => ImGuiWindowFlags.None;

    protected virtual void OnOpen()
    {
    }

    protected virtual void OnClose()
    {
    }

    protected virtual void OnDisposed()
    {
    }

    protected virtual void OnDraw()
    {
    }

    protected virtual void SetWindowSizeConstraints()
    {
        ImGui.SetNextWindowSize(DefaultSize, ImGuiCond.FirstUseEver);

        if (!IsMinimized) {
            ImGui.SetNextWindowSizeConstraints(MinSize * Node.ScaleFactor, MaxSize * Node.ScaleFactor);

            if (_currentSize.Y > 35 * Node.ScaleFactor) {
                ImGui.SetNextWindowSize(_currentSize, ImGuiCond.Always);
            }
        } else {
            ImGui.SetNextWindowSizeConstraints(
                MinSize with { Y = 35 * Node.ScaleFactor },
                MaxSize with { Y = 35 * Node.ScaleFactor }
            );
        }
    }

    protected virtual void ComputeWindowNodeSize()
    {
        Vector2 size = ImGui.GetWindowSize() / Node.ScaleFactor;
        size.X = (float)Math.Floor(size.X);
        size.Y = (float)Math.Floor(size.Y);

        WindowNode.Style.Size = new((int)size.X - 2, (int)size.Y - 2);
    }

    protected void RenderModalBackground()
    {
        Vector2       workSize = ImGui.GetMainViewport().WorkSize;
        Vector2       workPos  = ImGui.GetMainViewport().WorkPos;
        ImDrawListPtr dl       = ImGui.GetWindowDrawList();

        dl.PushClipRectFullScreen();

        const uint colTop     = 0xC0212021;
        const uint colMid     = 0xA0000000;
        float      halfHeight = workSize.Y / 2f;
        float      x1         = workPos.X;
        float      x2         = workSize.X + workPos.X;
        float      y1         = workPos.Y;
        float      y2         = workPos.Y + halfHeight;
        float      y3         = workPos.Y + workSize.Y;

        dl.AddRectFilledMultiColor(new(x1, y1), new(x2, y2), colTop, colTop, colMid, colMid);
        dl.AddRectFilledMultiColor(new(x1, y2), new(x2, y3), colMid, colMid, colTop, colTop);

        dl.PopClipRect();
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        _isOpened   = false;

        RequestClose?.Invoke();

        OnClose();
        OnDisposed();

        RootNode?.Dispose();
        WindowNode.Dispose();
    }

    protected static Node CreateCollapsiblePanel(Node targetNode, string label, bool expanded = true)
    {
        Node panelNode = UmbraDrawing.DocumentFrom("umbra.components.collapsible_group.xml").RootNode!;
        Node headNode  = panelNode.QuerySelector(".header")!;
        Node nameNode  = panelNode.QuerySelector(".header > .text")!;
        Node bodyNode  = panelNode.QuerySelector(".body")!;

        nameNode.NodeValue = label;
        targetNode.AppendChild(panelNode);

        headNode.OnClick += _ => {
            panelNode.ToggleClass("collapsed");
            panelNode.ToggleClass("expanded");
        };

        panelNode.ToggleClass("collapsed", !expanded);
        panelNode.ToggleClass("expanded", expanded);
        
        return bodyNode;
    }

    private void RenderWindow(string instanceId)
    {
        SetWindowSizeConstraints();

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, WindowNode.ComputedStyle.BorderRadius);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);

        ImGui.PushStyleColor(ImGuiCol.WindowBg, 0x00000000);
        ImGui.PushStyleColor(ImGuiCol.ChildBg, 0x00000000);

        ImGui.Begin($"{instanceId}",
            DefaultWindowFlags | (IsMinimized ? ImGuiWindowFlags.NoSavedSettings : ImGuiWindowFlags.None) | GetWindowFlags()
        );

        Size      = ImGui.GetWindowSize();
        Position  = ImGui.GetWindowPos();
        IsFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows);
        IsHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows);

        if (!IsMinimized) {
            _currentSize = ImGui.GetWindowSize();
        }

        if (GetWindowFlags().HasFlag(ImGuiWindowFlags.Modal)) {
            RenderModalBackground();
            ImGui.SetWindowFocus(instanceId);
        }

        ComputeWindowNodeSize();

        WindowNode.Style.ShadowSize = IsFocused ? new(64) : new(0);

        try {
            if (!EnableClipping || IsFocused) {
                RenderWindowInstance(instanceId);
            } else {
                RenderWindowClipped(instanceId);
            }
        } catch (Exception e) {
            Logger.Error($"An exception occurred while rendering window \"{GetType().Name}\": {e.Message}");
            if (e.InnerException != null) Logger.Error($" -> {e.InnerException.Message}");
            Logger.Info(e.InnerException?.StackTrace ?? e.StackTrace ?? " - No stack trace available -");
            Dispose();
        } finally {
            ImGui.End();
            ImGui.PopStyleColor(2);
            ImGui.PopStyleVar(6);
        }
    }

    private void RenderWindowClipped(string instanceId)
    {
        var ownClipRect = new ClipRect(ImGui.GetWindowPos(), ImGui.GetWindowPos() + ImGui.GetWindowSize());
        var result      = ClipRectSolver.Solve(ownClipRect);

        // If there are no solved rects, meaning nothing is intersecting with our window.
        if (result.SolvedRects.Count == 0) {
            RenderWindowInstance(instanceId);
            return;
        }

        // A game window is fully overlapping our window, so we don't want to
        // process any inputs.
        if (result.IsOverlapped) {
            ImGui.SetNextFrameWantCaptureKeyboard(false);
            ImGui.SetNextFrameWantCaptureMouse(false);
            return;
        }

        // Disable mouse input when it's hovering native rects.
        if (!WindowManager.HasFocusedWindow) {
            foreach (var rect in result.NativeRects) {
                if (rect.Contains(ImGui.GetMousePos())) {
                    ImGui.SetNextFrameWantCaptureKeyboard(false);
                    ImGui.SetNextFrameWantCaptureMouse(false);
                }
            }
        }

        for (var i = 0; i < result.SolvedRects.Count; i++) {
            var rect = result.SolvedRects[i];
            ImGui.PushClipRect(rect.Min, rect.Max, false);
            RenderWindowInstance(instanceId, i);
            ImGui.PopClipRect();
        }
    }

    private void RenderWindowInstance(string id, int instanceId = 0)
    {
        var drawList = ImGui.GetWindowDrawList();

        ImGui.SetCursorPos(new(0, 0));
        ImGui.BeginChild($"Window_{id}##{instanceId}", ImGui.GetWindowSize(), false);

        if (!IsMinimized) OnDraw();

        WindowNode.Render(drawList, new(2, 2));
        ImGui.EndChild();
    }

    private void ToggleMinimize()
    {
        IsMinimized = !IsMinimized;
        WindowNode.ToggleClass("collapsed", IsMinimized);

        if (IsMinimized) {
            ImGui.SetWindowSize(_currentSize with { Y = 32 * Node.ScaleFactor }, ImGuiCond.FirstUseEver);
        } else {
            ImGui.SetWindowSize(_currentSize, ImGuiCond.Once);
        }
    }

    private static ImGuiWindowFlags DefaultWindowFlags =>
        ImGuiWindowFlags.NoTitleBar
        | ImGuiWindowFlags.NoCollapse
        | ImGuiWindowFlags.NoDocking
        | ImGuiWindowFlags.NoScrollbar
        | ImGuiWindowFlags.NoScrollWithMouse
        | ImGuiWindowFlags.NoBackground;
}
