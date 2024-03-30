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
    /// <summary>
    /// A reference to the parent element of this element.
    /// </summary>
    public Element? Parent;

    /// <summary>
    /// An immutable list of all children of this element.
    /// </summary>
    public IEnumerable<Element> Children => _children;

    /// <summary>
    /// A list of all siblings of this element.
    /// </summary>
    public List<Element> Siblings => Parent?._children.Where(c => c.Id != Id && c.Anchor == Anchor).ToList() ?? [];

    private readonly List<Element> _children = [];

    /// <summary>
    /// Adds the given child element to this element.
    /// </summary>
    /// <param name="child">The element to add to this element.</param>
    public void AddChild(Element child)
    {
        // Check if a child with the same ID already exists.
        if (child.Id != "" && _children.Any(x => x.Id == child.Id))
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
