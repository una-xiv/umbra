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

namespace Umbra.Interface;

public partial class Element
{
    public Element? Parent;

    private readonly List<Element> _children = [];

    public IEnumerable<Element> Children => _children;

    public List<Element> Siblings => Parent?._children.Where(c => c.Id != Id && c.Anchor == Anchor).ToList() ?? [];

    /// <summary>
    /// Returns the previous sibling of the given element.
    /// </summary>
    /// <param name="child">The element to get the previous sibling of.</param>
    /// <returns>The previous sibling element or NULL if no such element exists.</returns>
    public Element? GetPrevSibling(Element child)
    {
        int index = _children.IndexOf(child);

        return index is -1 or 0 ? null : _children[index - 1];
    }

    /// <summary>
    /// Returns the next sibling of the given element.
    /// </summary>
    /// <param name="child">The element to get the next sibling of.</param>
    /// <returns>The next sibling element or NULL if no such element exists.</returns>
    public Element? GetNextSibling(Element child)
    {
        int index = _children.IndexOf(child);
        if (index == -1 || index == _children.Count - 1) return null;

        return _children[index + 1];
    }

    /// <summary>
    /// Adds the given child element to this element.
    /// </summary>
    /// <param name="child">The element to add to this element.</param>
    public void AddChild(Element child)
    {
        // Check if a child with the same ID already exists.
        if (_children.Any(x => x.Id == child.Id))
            throw new InvalidOperationException($"Element with ID '{child.Id}' already exists in '{Id}'.");

        child.Parent?.RemoveChild(child);

        _children.Add(child);
        child.Parent = this;
        IsDirty      = true;
    }

    /// <summary>
    /// Removes the given child element from this element.
    /// </summary>
    /// <param name="child">The element to remove from this element.</param>
    public void RemoveChild(Element child)
    {
        if (child.Parent != this) return;

        _children.Remove(child);
        child.Parent = null;
        IsDirty      = true;

        RemoveQueryCacheForChild(child);
    }
}
