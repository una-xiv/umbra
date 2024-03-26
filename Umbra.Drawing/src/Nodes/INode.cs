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

using ImGuiNET;

namespace Umbra.Drawing;

public interface INode
{
    /// <summary>
    /// The id of the content. This should be a unique across all content in the same element.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Whether the content is visible or not.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Renders the content to the screen.
    /// </summary>
    /// <param name="drawList">The draw list to render to.</param>
    /// <param name="rect">
    /// The bounding box that is either returned by GetComputedSize or determined by the element
    /// this content is attached to if GetComputedSize returns null.
    /// </param>
    /// <param name="elementOpacity">The opacity to apply to the content.</param>
    public void Render(ImDrawListPtr drawList, Rect rect, float elementOpacity);

    /// <summary>
    /// Returns the computed size of the content. If this content does not have a size by itself,
    /// it's bounding box is determined by the element this content is attached to. For example, a
    /// text content has a size, since it can be computed, but a zero-sized rect will have a size
    /// determined by the element it is attached to instead.
    /// </summary>
    public Size? GetComputedSize();
}
