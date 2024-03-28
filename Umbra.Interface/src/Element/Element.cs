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

using System.Collections.Generic;

namespace Umbra.Interface;

public partial class Element
{
    /// <summary>
    /// A unique identifier for this element.
    /// </summary>
    public readonly string Id;

    private Flow    _flow;
    private Anchor  _anchor;
    private Size    _size;
    private Spacing _padding;
    private Spacing _margin;
    private string? _text;
    private int     _sortIndex;
    private bool    _stretch;
    private bool    _fit;
    private bool    _isVisible;

    public Element(
        string         id,
        Flow           flow      = Flow.Horizontal,
        Anchor         anchor    = Anchor.TopLeft,
        Size           size      = new(),
        Spacing        padding   = new(),
        Spacing        margin    = new(),
        Style?         style     = null,
        string?        text      = null,
        int            sortIndex = 0,
        bool           stretch   = false,
        bool           fit       = false,
        bool           isVisible = true,
        List<Element>? children  = null
    )
    {
        Id         = id;
        Style      = style ?? new();
        _flow      = flow;
        _anchor    = anchor;
        _size      = size;
        _padding   = padding;
        _margin    = margin;
        _text      = text;
        _sortIndex = sortIndex;
        _stretch   = stretch;
        _fit       = fit;
        _isVisible = isVisible;

        children?.ForEach(AddChild);
    }

    public int SortIndex {
        get => _sortIndex;
        set {
            if (_sortIndex == value) return;
            _sortIndex   = value;
            IsDirty      = true;
            ComputedSize = Size.Auto;
        }
    }

    public bool IsVisible {
        get => _isVisible;
        set {
            if (_isVisible == value) return;
            _isVisible   = value;
            IsDirty      = true;
            ComputedSize = Size.Auto;
        }
    }

    public string? Text {
        get => _text;
        set {
            if (_text == value) return;

            _text        = value;
            IsDirty      = true;
            ComputedSize = Size.Auto;
        }
    }
}
