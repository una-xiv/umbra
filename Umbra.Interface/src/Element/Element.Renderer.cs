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
using System.Numerics;
using ImGuiNET;
using Umbra.Common;

namespace Umbra.Interface;

public partial class Element
{
    public event Action<Element>? OnRender;

    /// <summary>
    /// Renders the element and its children at the given position.
    /// </summary>
    /// <param name="drawList">The draw list to render to.</param>
    /// <param name="position">The screen coordinates.</param>
    public void Render(ImDrawListPtr drawList, Vector2? position)
    {
        if (null != position) ComputeLayout(position.Value);

        ComputeStyle();

        try {
            SetupInteractive(drawList);

            RenderShadow(drawList);
            RenderBackground(drawList);
            RenderImage(drawList);
            RenderBorders(drawList);
            RenderText(drawList);

            OnRender?.Invoke(this);

            foreach (var child in Children) {
                child.Render(drawList, null);
            }

            EndInteractive();
        } catch (Exception e) {
            Logger.Warning($"Rendering of element '{FullyQualifiedName}' failed: {e.Message}");
        }

        RenderDebugger();
    }
}
