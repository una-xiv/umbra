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
    /// A tag that can be used to find this element in an element tree.
    /// </summary>
    public string? Tag;

    /// <summary>
    /// The fully qualified name of this element.
    /// </summary>
    public string FullyQualifiedName => Parent == null ? Id : $"{Parent.FullyQualifiedName}.{Id}";

    private readonly Dictionary<string, Element> _queryCache = [];

    /// <summary>
    /// Returns a list of all children of this element that have the given tag.
    /// </summary>
    public List<Element> FindByTag(string tag, bool recursive = false)
    {
        List<Element> taggedChildren = [];

        foreach (Element child in _children) {
            if (child.Tag == tag) {
                taggedChildren.Add(child);
            }

            if (recursive) taggedChildren.AddRange(child.FindByTag(tag, true));
        }

        return taggedChildren;
    }

    /// <summary>
    /// Finds a child of this element by its ID.
    /// </summary>
    /// <remarks>
    /// Searches for a child element with the given ID. If the ID contains a dot,
    /// the search is performed recursively on the child elements. For example,
    /// if a child of this element is named "Foo" and that child has a child named
    /// "Bar", you can find the "Bar" element by calling Find("Foo.Bar").
    /// </remarks>
    /// <param name="id">The element ID to search for.</param>
    /// <returns>The element with the given ID, or NULL if no such element exists.</returns>
    public Element? Find(string id)
    {
        if (id.Contains('.')) {
            string[] parts = id.Split('.');
            Element? root  = Find(parts[0]);

            return root?.Find(string.Join('.', parts.Skip(1)));
        }

        if (_queryCache.TryGetValue(id, out Element? cached)) return cached;

        Element? el = _children.FirstOrDefault(el => el.Id == id);

        if (el != null) {
            return _queryCache[id] = el;
        }

        return null;
    }

    /// <summary>
    /// Test whether a child with the given ID exists.
    /// </summary>
    /// <remarks>
    /// If the ID contains a dot, the search is performed recursively on the
    /// child elements. For example, if a child of this element is named "Foo"
    /// and that child has a child named "Bar", you can find the "Bar" element
    /// by calling Find("Foo.Bar").
    /// </remarks>
    /// <param name="id">The element ID to search for.</param>
    /// <returns>True if an element exists, false otherwise.</returns>
    public bool Has(string id)
    {
        return Find(id) != null;
    }

    /// <summary>
    /// Returns a child of this element by its ID. Throws an exception if no
    /// such element exists.
    /// </summary>
    /// <remarks>
    /// If the ID contains a dot, the search is performed recursively on the
    /// child elements. For example, if a child of this element is named "Foo"
    /// and that child has a child named "Bar", you can find the "Bar" element
    /// by calling Find("Foo.Bar").
    /// </remarks>
    /// <param name="id">The element ID to search for.</param>
    /// <returns>The element with the given ID.</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public Element Get(string id)
    {
        return Find(id)
         ?? throw new KeyNotFoundException($"Element '{id}' not found in '{FullyQualifiedName}'.");
    }

    /// <summary>
    /// Same as <see cref="Get(string)"/>, but casts the result to the given type.
    /// </summary>
    public T Get<T>(string id) where T : Element
    {
        return (T) Get(id);
    }

    /// <summary>
    /// Returns the first child of this element that is of the given type.
    /// </summary>
    /// <exception cref="KeyNotFoundException"></exception>
    public T Get<T>() where T : Element
    {
        foreach (Element child in _children) {
            if (child is T t) return t;
        }

        throw new KeyNotFoundException($"Element of type '{typeof(T).Name}' not found in '{FullyQualifiedName}'.");
    }

    /// <summary>
    /// Find the first element of the given type with the specified ID.
    /// </summary>
    public T? FindFirst<T>(string id) where T : Element
    {
        if (id.Contains('.')) {
            throw new ArgumentException("ID cannot contain dots when using Find. Use Get() instead.");
        }

        if (Id == id) return (T) this;

        return _children.Select(child => child.FindFirst<T>(id)).OfType<T>().FirstOrDefault();
    }

    private void RemoveQueryCacheForChild(Element element)
    {
        _queryCache
            .Keys
            .Where(key => key == element.Id)
            .ToList()
            .ForEach(k => _queryCache.Remove(k));
    }
}
