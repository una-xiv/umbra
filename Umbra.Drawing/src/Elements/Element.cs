/* Umbra.Drawing | (c) 2024 by Una      ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Drawing is free software: you can       \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Umbra.Common;

namespace Umbra.Drawing;

public class Element
{
    /// <summary>
    /// The ID of the element that can be used to retrieve this instance from its ancestors.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The index of the element. Elements are sorted by this value when rendered.
    /// </summary>
    public int SortIndex { get; set; }

    /// <summary>
    /// The gap between children of this element.
    /// </summary>
    public int Gap { get; set; }

    /// <summary>
    /// True if the element is visible and should be rendered.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// True if the element is disabled and cannot be interacted with.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// The opacity of the element. A value of 0 is fully transparent and a value of 1 is fully opaque.
    /// </summary>
    public float Opacity { get; set; }

    /// <summary>
    /// True if the element should expand its width or height to fit its parent element based on the parent flow direction.
    /// </summary>
    public bool Fit { get; set; }

    /// <summary>
    /// The tooltip of the element. Displayed when the mouse hovers over the element.
    /// </summary>
    /// <remarks>
    /// This only works if the element is interactive. Elements become interactive when they have at least one mouse event attached.
    /// </remarks>
    public string? Tooltip { get; set; }

    /// <summary>
    /// The size of the element. A value of 0 on either axis will cause the element to expand to fit its children.
    /// </summary>
    public Size Size { get; set; }

    /// <summary>
    /// The padding of the element. Padding is the space between the content and the border of the element.
    /// </summary>
    public Spacing Padding { get; set; }

    /// <summary>
    /// The margin of the element. Margin is the space between the border of the element and its parent.
    /// </summary>
    public Spacing Margin { get; set; }

    /// <summary>
    /// The direction of the element. Determines how children are positioned.
    /// </summary>
    public Direction Direction { get; set; }

    /// <summary>
    /// The anchor of the element. Determines how the element is positioned relative to its parent.
    /// </summary>
    public Anchor Anchor { get; set; }

    /// <summary>
    /// The parent element. Null if this element is the root element.
    /// </summary>
    public Element? Parent { get; set; }

    /// <summary>
    /// The position of the element. This value should only be set if the element is the root element.
    /// For all other elements, the position is computed based on the parent element. Setting this value
    /// on a non-root element will cause the element to be positioned relative to the screen instead.
    /// For positioning relative to the parent element, use the <see cref="Margin"/> and <see cref="Anchor"/>
    /// properties instead.
    /// </summary>
    public Vector2? Position { get; set; }

    /// <summary>
    /// A list of child elements that are directly attached to this element.
    /// </summary>
    public List<Element> Children { get; private set; } = [];

    /// <summary>
    /// A list of nodes that are attached to this element that are responsible for rendering content
    /// to the screen.
    /// </summary>
    public List<INode> Nodes { get; private set; } = [];

    /// <summary>
    /// The computed content box of the element. This is the area that the content of the element
    /// is rendered to.
    /// </summary>
    public Rect ContentBox { get; private set; }

    /// <summary>
    /// True if the mouse cursor is currently hovering the element.
    /// </summary>
    /// <remarks>
    /// This value is only set if the element is interactive. Elements become interactive when they
    /// have at least one mouse event attached.
    /// </remarks>
    public bool IsMouseOver;

    /// <summary>
    /// The computed bounding box of the element. This is the same as the content box, but includes
    /// the margins of the element as well.
    /// </summary>
    public Rect BoundingBox { get; private set; }

    public event Action<Element>? OnChildAdded;
    public event Action<Element>? OnChildRemoved;
    public event Action<Element>? OnBeforeRender;
    public event Action<Element>? OnAfterRender;
    public event Action?          OnBeforeCompute;
    public event Action?          OnMouseEnter;
    public event Action?          OnMouseLeave;
    public event Action?          OnClick;
    public event Action?          OnRightClick;
    public event Action?          OnMiddleClick;
    public event Action?          OnMouseDown;
    public event Action?          OnMouseUp;

    private          double                               _mouseOverTimeStart;
    private readonly Dictionary<ImGuiMouseButton, bool>   _isMouseDown      = [];
    private readonly Dictionary<ImGuiMouseButton, double> _mouseDownTime    = [];
    private readonly Dictionary<string, Element>          _childLookupCache = [];

    public Element(
        string         id         = "",
        int            sortIndex  = 0,
        int            gap        = 0,
        bool           isVisible  = true,
        bool           isDisabled = false,
        float          opacity    = 1f,
        bool           fit        = false,
        string?        tooltip    = null,
        Size           size       = new(),
        Spacing        padding    = new(),
        Spacing        margin     = new(),
        Direction      direction  = Direction.Vertical,
        Anchor         anchor     = Anchor.Top | Anchor.Left,
        Vector2?       position   = null,
        List<Element>? children   = null,
        List<INode>?   nodes      = null
    )
    {
        Id         = id;
        SortIndex  = sortIndex;
        Gap        = gap;
        IsVisible  = isVisible;
        IsDisabled = isDisabled;
        Opacity    = opacity;
        Fit        = fit;
        Tooltip    = tooltip;
        Size       = size;
        Padding    = padding;
        Margin     = margin;
        Direction  = direction;
        Anchor     = anchor;
        Position   = position;
        Children   = children ?? Children;
        Nodes      = nodes    ?? Nodes;

        if (Id.Contains('.')) throw new ArgumentException("Element IDs cannot contain periods.");

        Nodes.ForEach(
            node => {
                if (node.Id?.Contains('.') ?? false) throw new ArgumentException("Node IDs cannot contain periods.");
            }
        );

        // Assign parent nodes to children.
        foreach (var child in Children) {
            child.Parent = this;
        }
    }

    /// <summary>
    /// The fully qualified name of the element that is based on the ID of this element and the IDs of its ancestors.
    /// </summary>
    private string Fqn => Parent?.Fqn + "." + Id;

    /// <summary>
    /// Adds a child element to this element.
    /// </summary>
    /// <param name="child">The child element to add.</param>
    /// <returns>The current element.</returns>
    public Element AddChild(Element child)
    {
        if (child.Id.Contains('.')) throw new ArgumentException("Element IDs cannot contain periods.");

        if (child.Parent == this
         && Children.Contains(child))
            return this;

        child.Parent?.RemoveChild(child);
        child.Parent = this;

        if (!Children.Contains(child)) {
            Children.Add(child);
            OnChildAdded?.Invoke(child);

            child.OnChildRemoved += c => {
                _childLookupCache.Remove(c.Fqn);

                foreach (var key in _childLookupCache.Keys.ToArray()) {
                    if (key.StartsWith(c.Fqn + '.')) _childLookupCache.Remove(key);
                }

                // Emit recurisvely.
                OnChildRemoved?.Invoke(c);
            };
        }

        return this;
    }

    /// <summary>
    /// Removes a child element from this element.
    /// </summary>
    /// <param name="child">The child element to remove.</param>
    /// <returns>The current element.</returns>
    public Element RemoveChild(Element child)
    {
        if (!Children.Contains(child)) {
            throw new Exception($"Element ({Id}) does not contain the specified child ({child.Id}).");
        }

        _childLookupCache.Remove(child.Fqn);

        foreach (var key in _childLookupCache.Keys.ToArray()) {
            if (key.StartsWith(child.Fqn + '.')) _childLookupCache.Remove(key);
        }

        OnChildRemoved?.Invoke(child);

        Children.Remove(child);
        child.Parent = null;

        return this;
    }

    /// <summary>
    /// Adds a node to this element. Nodes are objects that are responsible for rendering content to the screen.
    /// </summary>
    /// <param name="node">An instance of the node to attach to this element that should be rendered.</param>
    /// <returns>The current element.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Element AddNode(INode node)
    {
        if (node.Id?.Contains('.') ?? false) throw new ArgumentException("Node IDs cannot contain periods.");

        Nodes.Add(node);

        return this;
    }

    /// <summary>
    /// Removes a node from this element.
    /// </summary>
    /// <param name="node">The node to remove from this element.</param>
    /// <returns>The current element.</returns>
    public Element RemoveNode(INode node)
    {
        if (!Nodes.Contains(node)) return this;

        Nodes.Remove(node);

        return this;
    }

    /// <summary>
    /// Retrieves an element by its fully qualified name.
    /// </summary>
    /// <param name="id">The fully qualified name of the element to retrieve.</param>
    /// <returns>The element with the specified fully qualified name.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Element Get(string id)
    {
        if (_childLookupCache.TryGetValue(id, out Element? cachedElement)) return cachedElement;

        return _childLookupCache[id] = _get(id.Split('.'), []);
    }

    /// <summary>
    /// Determines whether an element with the specified fully qualified name exists in this element.
    /// </summary>
    /// <param name="id">The fully qualified name of the element to test for.</param>
    /// <returns></returns>
    public bool Has(string id)
    {
        try {
            Get(id);
            return true;
        } catch (ArgumentException) {
            return false;
        }
    }

    /// <summary>
    /// Retrieves a node by its fully qualified name and type.
    /// </summary>
    /// <param name="id">The fully qualified name of the node to retrieve.</param>
    /// <typeparam name="T">The type of the node to retrieve.</typeparam>
    /// <returns>The node with the specified fully qualified name and type.</returns>
    public T GetNode<T>(string id) where T : INode
    {
        foreach (var node in Nodes) {
            if (node.Id == id
             && node is T t)
                return t;
        }

        throw new ArgumentException($"Node with ID '{id}' not found in {Id}.");
    }

    /// <summary>
    /// Retrieves the first node that matches the given type.
    /// </summary>
    /// <typeparam name="T">The type of the node to retrieve.</typeparam>
    /// <returns>The first node that matches the given type.</returns>
    public T GetNode<T>() where T : INode
    {
        foreach (var node in Nodes) {
            if (node is T t) return t;
        }

        throw new ArgumentException($"Node of type '{typeof(T).Name}' not found in {Id}.");
    }

    /// <summary>
    /// Renders the element and its children to the screen at the given position.
    /// </summary>
    public void Render(ImDrawListPtr drawList, Vector2 position)
    {
        if (IsWindowDrawList(drawList)) {
            position += ImGui.GetWindowPos();
        }

        ComputeLayout(position);
        Render(drawList);
    }

    /// <summary>
    /// Renders the element and its children to the screen.
    /// </summary>
    /// <param name="drawList">The draw list to render the element to.</param>
    public void Render(ImDrawListPtr drawList)
    {
        bool isWindowDrawList = IsWindowDrawList(drawList);

        if (!IsDisabled && IsInteractive) {
            if (Id == "") {
                Logger.Warning($"Element is interactive but has no ID. This may cause unexpected behavior.");
            }

            if (!isWindowDrawList) {
                ImGui.SetNextWindowPos(ContentBox.Min, ImGuiCond.Always);
                ImGui.SetNextWindowSize(ContentBox.Size.XY, ImGuiCond.Always);

                ImGui.Begin(
                    Fqn,
                    ImGuiWindowFlags.NoTitleBar
                  | ImGuiWindowFlags.NoResize
                  | ImGuiWindowFlags.NoMove
                  | ImGuiWindowFlags.NoScrollbar
                  | ImGuiWindowFlags.NoScrollWithMouse
                  | ImGuiWindowFlags.NoBringToFrontOnFocus
                  | ImGuiWindowFlags.NoNavFocus
                  | ImGuiWindowFlags.NoNav
                  | ImGuiWindowFlags.NoSavedSettings
                  | ImGuiWindowFlags.NoBackground
                  | ImGuiWindowFlags.NoDecoration
                );
            }

            ImGui.SetCursorScreenPos(ContentBox.Min);
            ImGui.InvisibleButton($"##{Fqn}", ContentBox.Size.XY);

            if (ImGui.IsItemHovered()) {
                if (_mouseOverTimeStart == 0) _mouseOverTimeStart = ImGui.GetTime();

                if (Tooltip                               != null
                 && Tooltip.Length                        > 0
                 && ImGui.GetTime() - _mouseOverTimeStart > 0.5) {
                    ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,  new Vector2(6, 6));
                    ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 6);
                    ImGui.PushStyleColor(ImGuiCol.PopupBg, 0xFF303030);
                    ImGui.BeginTooltip();
                    ImGui.SetCursorPos(new(6, 4));
                    FontRepository.PushFont(Font.AxisSmall);
                    ImGui.TextUnformatted(Tooltip);
                    FontRepository.PopFont(Font.AxisSmall);
                    ImGui.EndTooltip();
                    ImGui.PopStyleColor();
                    ImGui.PopStyleVar(2);
                }

                if (!IsMouseOver) OnMouseEnter?.Invoke();
                IsMouseOver = true;

                ImGuiMouseButton? button = null;

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    button = ImGuiMouseButton.Left;
                else if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                    button                                                     = ImGuiMouseButton.Right;
                else if (ImGui.IsMouseClicked(ImGuiMouseButton.Middle)) button = ImGuiMouseButton.Middle;

                if (button != null) {
                    _isMouseDown[(ImGuiMouseButton)button]   = true;
                    _mouseDownTime[(ImGuiMouseButton)button] = ImGui.GetTime();
                    OnMouseDown?.Invoke();
                } else {
                    foreach (var mouseButton in _isMouseDown.Keys.ToArray()) {
                        if (ImGui.IsMouseReleased(mouseButton)) {
                            OnMouseUp?.Invoke();

                            if (ImGui.GetTime() - _mouseDownTime[mouseButton] < 0.5) {
                                switch (mouseButton) {
                                    case ImGuiMouseButton.Left:
                                        OnClick?.Invoke();
                                        break;
                                    case ImGuiMouseButton.Right:
                                        OnRightClick?.Invoke();
                                        break;
                                    case ImGuiMouseButton.Middle:
                                        OnMiddleClick?.Invoke();
                                        break;
                                }
                            }

                            _isMouseDown[mouseButton]   = false;
                            _mouseDownTime[mouseButton] = 0;
                        }
                    }
                }
            } else {
                if (IsMouseOver) OnMouseLeave?.Invoke();

                IsMouseOver         = false;
                _mouseOverTimeStart = 0;

                foreach (var mouseButton in _isMouseDown.Keys.ToArray()) {
                    if (_isMouseDown[mouseButton]) {
                        _isMouseDown[mouseButton]   = false;
                        _mouseDownTime[mouseButton] = 0;
                        OnMouseUp?.Invoke();
                    }
                }
            }

            if (!isWindowDrawList) {
                ImGui.End();
            }
        }

        OnBeforeRender?.Invoke(this);

        float opacity = GetComputedOpacity();

        var rect = new Rect(
            ContentBox.Min.X  + Padding.Left,
            ContentBox.Min.Y  + Padding.Top,
            ContentBox.Width  - Padding.Left - Padding.Right,
            ContentBox.Height - Padding.Top  - Padding.Bottom
        );

        if (Parent == null) {
            if (Anchor.HasFlag(Anchor.Bottom)) {
                rect.Y -= BoundingBox.Height;
            }

            if (Anchor.HasFlag(Anchor.Right)) {
                rect.X -= BoundingBox.Width;
            }

            if (Anchor.HasFlag(Anchor.Center)) {
                rect.X -= BoundingBox.Width / 2;
            }
        }

        foreach (var node in Nodes) {
            if (!node.IsVisible) continue;
            node.Render(drawList, rect, opacity);
        }

        foreach (var child in Children) {
            if (!child.IsVisible) continue;

            child.Render(drawList);
        }

        OnAfterRender?.Invoke(this);
    }

    /// <summary>
    /// Computes the layout of the element and its children.
    /// </summary>
    /// <param name="position">The position to compute the layout at. Uses to the parent element content position if omitted.</param>
    public void ComputeLayout(Vector2? position = null)
    {
        OnBeforeCompute?.Invoke();

        if (position != null) Position = position;

        if (Parent is null
         && Position is null) {
            Position = new(0, 0);
        }

        SortChildren();

        Size  computedSize = GetComputedSize();
        float x            = Position?.X ?? Parent?.Position?.X ?? 0;
        float y            = Position?.Y ?? Parent?.Position?.Y ?? 0;

        ContentBox = new(x, y, computedSize.Width, computedSize.Height);

        BoundingBox = new(
            x                   - Margin.Left,
            y                   - Margin.Top,
            computedSize.Width  + Margin.Left + Margin.Right,
            computedSize.Height + Margin.Top  + Margin.Bottom
        );

        if (Parent is null) {
            if (Anchor.HasFlag(Anchor.Bottom)) {
                y -= computedSize.Height;
            }

            if (Anchor.HasFlag(Anchor.Right)) {
                x -= computedSize.Width;
            }

            if (Anchor.HasFlag(Anchor.Center)) {
                x -= computedSize.Width / 2;
            }

            if (Anchor.HasFlag(Anchor.Middle)) {
                y -= computedSize.Height / 2;
            }
        }

        var leftChildren   = Children.Where(child => child.IsVisible && child.Anchor.HasFlag(Anchor.Left)).ToArray();
        var rightChildren  = Children.Where(child => child.IsVisible && child.Anchor.HasFlag(Anchor.Right)).ToArray();
        var topChildren    = Children.Where(child => child.IsVisible && child.Anchor.HasFlag(Anchor.Top)).ToArray();
        var bottomChildren = Children.Where(child => child.IsVisible && child.Anchor.HasFlag(Anchor.Bottom)).ToArray();
        var centerChildren = Children.Where(child => child.IsVisible && child.Anchor.HasFlag(Anchor.Center)).ToArray();
        var middleChildren = Children.Where(child => child.IsVisible && child.Anchor.HasFlag(Anchor.Middle)).ToArray();

        // Position left-aligned children.
        float currentX = x + Padding.Left;
        float currentY = y + Padding.Top;

        foreach (var child in leftChildren) {
            var childSize = child.GetComputedSize();
            child.Position = new(currentX + child.Margin.Left, currentY + child.Margin.Top);

            if (Direction == Direction.Horizontal)
                currentX += childSize.Width + child.Margin.Right + child.Margin.Left + Gap;
        }

        // Position right-aligned children.
        currentX = x + computedSize.Width - Padding.Right;
        currentY = y                      + Padding.Top;

        foreach (var child in rightChildren) {
            var childSize = child.GetComputedSize();
            child.Position = new(currentX - childSize.Width - child.Margin.Right, currentY + child.Margin.Top);

            if (Direction == Direction.Horizontal)
                currentX -= childSize.Width + child.Margin.Right + child.Margin.Left + Gap;
        }

        // Position center-aligned children.
        currentX = x + computedSize.Width / 2;
        currentY = y + Padding.Top;

        currentX -= centerChildren.Sum(child => child.GetComputedSize().Width + child.Margin.Right + child.Margin.Left)
          / 2;

        foreach (var child in centerChildren) {
            var childSize = child.GetComputedSize();
            child.Position = new(currentX + child.Margin.Left, currentY + child.Margin.Top);

            if (Direction == Direction.Horizontal)
                currentX += childSize.Width + child.Margin.Right + child.Margin.Left + Gap;
        }

        // Position top-aligned children.
        currentX = x + Padding.Left;
        currentY = y + Padding.Top;

        foreach (var child in topChildren) {
            var childSize = child.GetComputedSize();
            child.Position = new(child.Position?.X ?? (currentX + child.Margin.Left), currentY + child.Margin.Top);

            if (Direction == Direction.Vertical)
                currentY += childSize.Height + child.Margin.Bottom + child.Margin.Top + Gap;
        }

        // Position bottom-aligned children.
        currentX = x                       + Padding.Left;
        currentY = y + computedSize.Height - Padding.Bottom;

        foreach (var child in bottomChildren) {
            var childSize = child.GetComputedSize();

            child.Position = new(
                child.Position?.X ?? (currentX + child.Margin.Left),
                currentY - childSize.Height - child.Margin.Bottom
            );

            if (Direction == Direction.Vertical)
                currentY -= childSize.Height + child.Margin.Bottom + child.Margin.Top + Gap;
        }

        // Position middle-aligned children.
        currentX = x + Padding.Left;
        currentY = y + computedSize.Height / 2;

        foreach (var child in middleChildren) {
            var childSize = child.GetComputedSize();

            child.Position = new(
                child.Position?.X ?? (currentX + child.Margin.Left),
                currentY - (childSize.Height / 2)
            );

            if (Direction == Direction.Vertical)
                currentY += childSize.Height + child.Margin.Bottom + child.Margin.Top + Gap;
        }

        foreach (var child in Children) {
            if (!child.IsVisible) continue;

            child.ComputeLayout();
        }

        // Find children that should fit the width or height of this element.
        foreach (var child in Children) {
            if (!child.Fit
             || !child.IsVisible)
                continue;

            if (Direction == Direction.Horizontal) {
                // Expand the childs height through its padding to fit inside our container.
                if (child.BoundingBox.Height < computedSize.Height) {
                    child.Size = new(
                        width: child.BoundingBox.Width - child.Padding.Left - child.Padding.Right,
                        height: computedSize.Height
                      - Padding.Top
                      - Padding.Bottom
                      - child.Padding.Top
                      - child.Padding.Bottom
                    );
                }
            } else {
                // Expand the childs width through its padding to fit inside our container.
                if (child.BoundingBox.Width < computedSize.Width) {
                    child.Size = new(
                        width: computedSize.Width
                      - Padding.Left
                      - Padding.Right
                      - child.Padding.Left
                      - child.Padding.Right,
                        height: child.BoundingBox.Height - child.Padding.Top - child.Padding.Bottom
                    );
                }
            }

            child.ComputeLayout();
        }
    }

    // <summary>
    // Returns the computed size of the element.
    // </summary>
    // <returns>The computed size of the element.</returns>
    public Size GetComputedSize()
    {
        var width  = Size.Width;
        var height = Size.Height;

        if (width == 0) {
            width = GetComputedTotalWidth();
        } else {
            width += Padding.Left + Padding.Right;
        }

        if (height == 0) {
            height = GetComputedTotalHeight();
        } else {
            height += Padding.Top + Padding.Bottom;
        }

        return new(width, height);
    }

    /// <summary>
    /// Returns the computed total width of the element based on its children and nodes.
    /// </summary>
    /// <returns>The computed total width of the element.</returns>
    public float GetComputedTotalWidth()
    {
        SortChildren();

        if (Fit && Parent?.Direction == Direction.Vertical) Size = new(0, Size.Height);

        if (Size.Width > 0) {
            return Size.Width + Padding.Left + Padding.Right;
        }

        float max = 0;

        foreach (var content in Nodes) {
            max = Math.Max(max, content.GetComputedSize()?.Width ?? 0);
        }

        foreach (var child in Children) {
            if (!child.IsVisible) continue;

            var width = child.GetComputedTotalWidth() + child.Margin.Left + child.Margin.Right;

            if (Direction == Direction.Horizontal) {
                max += width;
            } else if (width > max) {
                max = width;
            }
        }

        return max
          + Padding.Left
          + Padding.Right
          + (Direction == Direction.Horizontal ? (Children.Count(child => child.IsVisible) - 1) * Gap : 0);
    }

    /// <summary>
    /// Returns the computed total height of the element based on its children and nodes.
    /// </summary>
    /// <returns>The computed total height of the element.</returns>
    public float GetComputedTotalHeight()
    {
        SortChildren();

        if (Fit && Parent?.Direction == Direction.Horizontal) Size = new(Size.Width, 0);

        if (Size.Height > 0) {
            return Size.Height + Padding.Top + Padding.Bottom;
        }

        float max = 0;

        foreach (var content in Nodes) {
            max = Math.Max(max, content.GetComputedSize()?.Height ?? 0);
        }

        foreach (var child in Children) {
            if (!child.IsVisible) continue;

            var height = child.GetComputedTotalHeight() + child.Margin.Top + child.Margin.Bottom;

            if (Direction == Direction.Vertical) {
                max += height;
            } else if (height > max) {
                max = height;
            }
        }

        return max
          + Padding.Top
          + Padding.Bottom
          + (Direction == Direction.Vertical ? (Children.Count(child => child.IsVisible) - 1) * Gap : 0);
    }

    /// <summary>
    /// True if the element has any interactive events.
    /// </summary>
    public bool IsInteractive =>
        OnMouseEnter != null || OnMouseLeave != null || OnClick != null || OnMouseDown != null || OnMouseUp != null;

    private Element _get(string[] idChain, List<string> anchestors)
    {
        string? id = idChain.FirstOrDefault();
        idChain = idChain.Skip(1).ToArray();

        if (id == null) throw new ArgumentException("ID cannot be empty.");

        if (id             == Id
         && idChain.Length == 0)
            return this;

        if (id == Id) {
            anchestors.Add(id);
            return _get(idChain, anchestors);
        }

        var child = Children.Find(child => child.Id == id)
         ?? (
                anchestors.Count > 0
                    ? throw new ArgumentException(
                        $"Element with ID '{id}' not found in '{string.Join(".", anchestors)}'."
                    )
                    : throw new ArgumentException($"Element with ID '{id}' not found.")
            );

        if (idChain.Length == 0) return child;

        anchestors.Add(id);

        return child._get(idChain, anchestors);
    }

    private static unsafe bool IsWindowDrawList(ImDrawListPtr drawList)
    {
        return drawList.NativePtr != ImGui.GetBackgroundDrawList().NativePtr
         && drawList.NativePtr    != ImGui.GetForegroundDrawList().NativePtr;
    }

    private float GetComputedOpacity()
    {
        return Opacity * (Parent?.GetComputedOpacity() ?? 1f);
    }

    private void SortChildren()
    {
        // If all sortIndices of children are 0, abort.
        if (Children.All(child => child.SortIndex == 0)) return;

        Children.Sort((a, b) => a.SortIndex.CompareTo(b.SortIndex));
    }
}
