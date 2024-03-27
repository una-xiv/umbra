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
    public Size Size {
        get => _size;
        set {
            if (_size == value) return;

            _size        = value;
            IsDirty      = true;
            ComputedSize = Size.Auto;
        }
    }

    public Spacing Padding {
        get => _padding;
        set {
            if (_padding == value) return;

            _padding     = value;
            IsDirty      = true;
            ComputedSize = Size.Auto;
        }
    }

    public Spacing Margin {
        get => _margin;
        set {
            if (_margin == value) return;

            _margin      = value;
            IsDirty      = true;
            ComputedSize = Size.Auto;
        }
    }

    // Circular reference check.
    private bool _isCalculatingSize;

    /// <summary>
    /// Calculates the <see cref="ComputedSize"/> of the element.
    /// </summary>
    private Size CalculateSize()
    {
        if (_isCalculatingSize) {
            // Recursive invocation may occur when a sibling has a dynamic size
            // based on the size of this element and our parent. Layout
            // calculation hasn't fully completed yet, so it's safe to just
            // return the current computed size.
            return ComputedSize;
        }

        if (!IsDirty && ComputedSize.IsFixed) return ComputedSize;

        if (Size.IsFixed) {
            return ComputedSize = new(
                Size.Width  + _margin.Horizontal,
                Size.Height + _margin.Vertical
            );
        }

        _isCalculatingSize = true;

        Size computedSize = Size.Max(CalculateOwnSize(), CalculateSizeBasedOnFlowAndChildren());
        int  width        = computedSize.Width + Margin.Horizontal;
        int  height       = computedSize.Height + Margin.Vertical;

        if (Fit) {
            if (Flow == Flow.Horizontal) {
                height = Math.Max(height, GetMaxSizeOfChildren(Siblings).Height);
            } else {
                width = Math.Max(width, GetMaxSizeOfChildren(Siblings).Width);
            }
        }

        if (Stretch && Parent != null) {
            switch (Flow) {
                case Flow.Horizontal when Parent.Size.Width > 0:
                    width = Math.Max(width, Parent.Size.Width - GetTotalSizeOfChildren(Siblings).Width);
                    break;
                case Flow.Vertical when Parent.Size.Height > 0:
                    height = Math.Max(height, Parent.Size.Height - GetTotalSizeOfChildren(Siblings).Height);
                    break;
                case Flow.None:
                default:
                    break;
            }
        }

        _isCalculatingSize = false;
        return ComputedSize = new(width, height);
    }

    /// <summary>
    /// Calculates the size of this element and its own contents.
    /// </summary>
    private Size CalculateOwnSize()
    {
        if (Text == null) return Size;
        if (Size.IsFixed) return Size;

        Font font = Style.GetFont(this);
        FontRepository.PushFont(font);

        Vector2 textSize = Size.ShouldSpanHorizontally
            ? ImGui.CalcTextSize(Text)
            : ImGui.CalcTextSize(Text, (float)Size.Width);

        FontRepository.PopFont(font);

        return new(
            Size.Width  == 0 ? (int)textSize.X + Padding.Horizontal : Size.Width,
            Size.Height == 0 ? (int)textSize.Y + Padding.Vertical : Size.Height
        );
    }

    /// <summary>
    /// Returns the max size of all given children.
    /// </summary>
    private static Size GetMaxSizeOfChildren(IReadOnlyCollection<Element> children)
    {
        if (children.Count == 0) return new();
        foreach (var child in children) child.CalculateSize();

        return new(
            children.Max(child => child.ComputedSize.Width),
            children.Max(child => child.ComputedSize.Height)
        );
    }

    /// <summary>
    /// Returns the total combined size of all given children.
    /// </summary>
    private static Size GetTotalSizeOfChildren(IReadOnlyCollection<Element> children)
    {
        if (children.Count == 0) return new();

        foreach (var child in children) child.CalculateSize();

        return new(
            children.Sum(child => child.ComputedSize.Width),
            children.Sum(child => child.ComputedSize.Height)
        );
    }

    /// <summary>
    /// Returns the size of the element based on the flow and children.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private Size CalculateSizeBasedOnFlowAndChildren()
    {
        List<Size> totalSizes = [
            GetTotalSizeOfChildren(GetAnchoredChildren(Anchor.TopLeft)),
            GetTotalSizeOfChildren(GetAnchoredChildren(Anchor.TopCenter)),
            GetTotalSizeOfChildren(GetAnchoredChildren(Anchor.TopRight)),
            GetTotalSizeOfChildren(GetAnchoredChildren(Anchor.MiddleLeft)),
            GetTotalSizeOfChildren(GetAnchoredChildren(Anchor.MiddleCenter)),
            GetTotalSizeOfChildren(GetAnchoredChildren(Anchor.MiddleRight)),
            GetTotalSizeOfChildren(GetAnchoredChildren(Anchor.BottomLeft)),
            GetTotalSizeOfChildren(GetAnchoredChildren(Anchor.BottomCenter)),
            GetTotalSizeOfChildren(GetAnchoredChildren(Anchor.BottomRight))
        ];

        List<Size> maxSizes = [
            GetMaxSizeOfChildren(GetAnchoredChildren(Anchor.TopLeft)),
            GetMaxSizeOfChildren(GetAnchoredChildren(Anchor.TopCenter)),
            GetMaxSizeOfChildren(GetAnchoredChildren(Anchor.TopRight)),
            GetMaxSizeOfChildren(GetAnchoredChildren(Anchor.MiddleLeft)),
            GetMaxSizeOfChildren(GetAnchoredChildren(Anchor.MiddleCenter)),
            GetMaxSizeOfChildren(GetAnchoredChildren(Anchor.MiddleRight)),
            GetMaxSizeOfChildren(GetAnchoredChildren(Anchor.BottomLeft)),
            GetMaxSizeOfChildren(GetAnchoredChildren(Anchor.BottomCenter)),
            GetMaxSizeOfChildren(GetAnchoredChildren(Anchor.BottomRight))
        ];

        return Flow switch {
            Flow.None       => maxSizes.Max(size => size),
            Flow.Horizontal => new(totalSizes.Sum(size => size.Width), maxSizes.Max(size => size.Height)),
            Flow.Vertical   => new(maxSizes.Max(size => size.Width), totalSizes.Sum(size => size.Height)),
            _               => throw new ArgumentOutOfRangeException()
        };
    }
}
