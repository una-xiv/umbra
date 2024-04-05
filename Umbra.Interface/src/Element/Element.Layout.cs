/* Umbra.Interface | (c) 2024 by Una    ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Interface is free software: you can    \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Interface is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;

namespace Umbra.Interface;

public partial class Element
{
    /// <summary>
    /// True if this element or any of its children has modified properties
    /// that affect the overall layout of this tree.
    /// </summary>
    public bool IsDirty = true;

    /// <summary>
    /// Defines in which direction elements should flow within this element.
    /// <list type="bullet">
    ///   <item><description><see cref="Flow.Horizontal"/>: Elements will flow from left to right.</description></item>
    ///   <item><description><see cref="Flow.Vertical"/>: Elements will flow from top to bottom.</description></item>
    ///   <item><description><see cref="Flow.None"/>: Elements are stacked on top of each other.</description></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// Children with different anchors are not affected by each other. For
    /// example, if a child has an anchor of <see cref="Anchor.TopLeft"/> and
    /// another child has an anchor of <see cref="Anchor.BottomRight"/>, they
    /// will not affect each other's position and may end up overlapping.
    /// </remarks>
    public Flow Flow {
        get => _flow;
        set {
            if (_flow == value) return;

            _flow   = value;
            IsDirty = true;
        }
    }

    /// <summary>
    /// Defines the anchor point of this element relative to its parent.
    /// </summary>
    public Anchor Anchor {
        get => _anchor;
        set {
            if (_anchor == value) return;

            _anchor = value;
            IsDirty = true;
        }
    }

    /// <summary>
    /// Stretch this element to fill the available space in the current flow
    /// direction. This will look at the available space of the immediate
    /// parent and stretch the element to fill the remaining space. The
    /// parent must have a fixed size for this to work. There can only be one
    /// stretched element in a single flow.
    /// </summary>
    /// <remarks>
    /// A "flow" is the combination of the direction and anchor of the element.
    /// </remarks>
    public bool Stretch {
        get => _stretch;
        set {
            if (_stretch == value) return;

            _stretch     = value;
            IsDirty      = true;
            ComputedSize = Size.Auto;
        }
    }

    /// <summary>
    /// Fits the element to match the max size of its children within the flow.
    /// If the flow direction is horizontal, this option will stretch the
    /// element vertically and vice-versa.
    /// </summary>
    /// <remarks>
    /// A "flow" is the combination of the direction and anchor of the element.
    /// </remarks>
    public bool Fit {
        get => _fit;
        set {
            if (_fit == value) return;

            _fit         = value;
            IsDirty      = true;
            ComputedSize = Size.Auto;
        }
    }

    /// <summary>
    /// Defines the space between elements in the flow direction.
    /// </summary>
    public int Gap {
        get => _gap;
        set {
            if (_gap == value) return;

            _gap         = value;
            IsDirty      = true;
            ComputedSize = Size.Auto;
        }
    }

    /// <summary>
    /// The absolute position of this element.
    /// </summary>
    public Vector2 Position { get; private set; }

    /// <summary>
    /// Represents the computed size of this element.
    /// </summary>
    /// <remarks>
    /// If both sides of the <see cref="Size"/> are explicitly set, this will be equal to <see cref="Size"/> plus
    /// additional <see cref="Margin"/>. If any side is set to <see cref="Size.Auto"/>, the computed size will be
    /// calculated based on the size of the children of this element.
    /// </remarks>
    public Size ComputedSize { get; protected set; }

    /// <summary>
    /// The computed bounding box of the entire element, excluding padding.
    /// </summary>
    public Rect BoundingBox { get; protected set; }

    /// <summary>
    /// The computed bounding box of the element, including padding.
    /// </summary>
    public Rect ContentBox { get; protected set; }

    public event Action? OnBeforeCompute;

    /// <summary>
    /// Computes the layout and position of this element and its children.
    /// </summary>
    /// <param name="rootPosition"></param>
    public void ComputeLayout(Vector2 rootPosition)
    {
        DoBeforeCompute();

        var pos = CalculatePosition(rootPosition);
        if (!ShouldRevalidate() && Position == pos) return;

        Position = pos;

        CalculateAnchoredChildPositions(Anchor.TopLeft);
        CalculateAnchoredChildPositions(Anchor.TopCenter);
        CalculateAnchoredChildPositions(Anchor.TopRight);
        CalculateAnchoredChildPositions(Anchor.MiddleLeft);
        CalculateAnchoredChildPositions(Anchor.MiddleCenter);
        CalculateAnchoredChildPositions(Anchor.MiddleRight);
        CalculateAnchoredChildPositions(Anchor.BottomLeft);
        CalculateAnchoredChildPositions(Anchor.BottomCenter);
        CalculateAnchoredChildPositions(Anchor.BottomRight);

        ComputeBoundingBox();

        GetAnchoredChildren(Anchor.None)
            .ForEach(
                (child) => {
                    child.Size = ContentBox.Size;
                    child.ComputeLayout(ContentBox.Min);
                }
            );

        IsDirty = false;
        DoAfterCompute();
    }

    protected void ComputeBoundingBox()
    {
        BoundingBox = new(
            Position                            + Margin.TopLeft,
            Position + ComputedSize.ToVector2() - Margin.BottomRight
        );

        ContentBox = new(
            BoundingBox.Min + Padding.TopLeft,
            BoundingBox.Max - Padding.BottomRight
        );
    }

    private bool ShouldRevalidate()
    {
        if (IsDirty || ComputedSize.IsEmpty) return true;

        return _children.Any(child => child.ShouldRevalidate());
    }

    private void DoBeforeCompute()
    {
        OnBeforeCompute?.Invoke();

        BeforeCompute();
        _children.ForEach(child => child.DoBeforeCompute());
    }

    private void DoAfterCompute()
    {
        AfterCompute();
        _children.ForEach(child => child.DoAfterCompute());
    }

    private void CalculateAnchoredChildPositions(Anchor childAnchor)
    {
        _children.Sort((a, b) => a.SortIndex.CompareTo(b.SortIndex));

        List<Element> children = GetAnchoredChildren(childAnchor);
        if (children.Count == 0) return;

        int x = (int)Position.X
              + (childAnchor.IsLeft()
                    ? Padding.Left
                    : childAnchor.IsRight()
                        ? -Padding.Right
                        : 0),
            y = (int)Position.Y
              + (childAnchor.IsTop()
                    ? Padding.Top
                    : childAnchor.IsBottom()
                        ? -Padding.Bottom
                        : 0);

        if (childAnchor.IsCenter())
            x += (ComputedSize.Width / 2)
              - (Flow == Flow.Horizontal
                    ? GetTotalSizeOfChildren(children).Width
                    : GetMaxSizeOfChildren(children).Width)
              / 2;

        if (childAnchor.IsRight()) x += ComputedSize.Width;

        if (childAnchor.IsMiddle())
            y += (ComputedSize.Height / 2)
              - (Flow == Flow.Horizontal
                    ? GetMaxSizeOfChildren(children).Height
                    : GetTotalSizeOfChildren(children).Height)
              / 2;

        if (childAnchor.IsBottom()) y += ComputedSize.Height;

        var maxHeight = 0;

        if (childAnchor.IsMiddle()) {
            maxHeight = GetMaxSizeOfChildren(children).Height;
        }

        foreach (Element child in children) {
            var yOffset = 0;

            if (child.Anchor.IsMiddle() && child.ComputedSize.Height < maxHeight) {
                yOffset = (maxHeight - child.ComputedSize.Height) / 2;
            }

            child.ComputeLayout(new(x, y + yOffset));

            switch (Flow) {
                case Flow.Horizontal:
                    x = childAnchor.IsRight()
                        ? x - child.ComputedSize.Width
                        : x + child.ComputedSize.Width;

                    if (children.Last() != child) {
                        x += childAnchor.IsRight() ? -Gap : Gap;
                    }

                    break;
                case Flow.Vertical:
                    y = childAnchor.IsBottom()
                        ? y - child.ComputedSize.Height
                        : y + child.ComputedSize.Height;

                    if (children.Last() != child) {
                        y += childAnchor.IsBottom() ? -Gap : Gap;
                    }

                    break;
                case Flow.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown flow direction '{Flow}'.");
            }
        }
    }

    private Vector2 CalculatePosition(Vector2 rootPosition)
    {
        Size size = CalculateSize();

        var x = (int)rootPosition.X;
        var y = (int)rootPosition.Y;

        if (Anchor.IsCenter() && Parent == null) {
            x -= size.Width / 2;
        }

        if (Anchor.IsRight()) {
            x -= size.Width;
        }

        if (Anchor.IsMiddle() && Parent == null) {
            y -= size.Height / 2;
        }

        if (Anchor.IsBottom()) {
            y -= size.Height;
        }

        return new(x, y);
    }

    private List<Element> GetAnchoredChildren(Anchor a)
    {
        List<Element> result = [];

        foreach (Element t in _children) {
            if (t.IsVisible && t.Anchor == a) {
                result.Add(t);
            }
        }

        return result;
    }
}
