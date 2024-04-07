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
using Umbra.Common;

namespace Umbra.Interface;

public partial class Element
{
    public event Action<Element>? OnRender;

    private readonly List<ImDrawListPtr> _drawLists = [];

    /// <summary>
    /// Renders the element and its children at the given position.
    /// </summary>
    /// <param name="drawList">The draw list to render to.</param>
    /// <param name="position">The screen coordinates.</param>
    public void Render(ImDrawListPtr drawList, Vector2? position)
    {
        if (!IsVisible) return;
        if (null != position) ComputeLayout(position.Value);

        _drawLists.Add(drawList);

        ComputeStyle();

        try {
            SetupInteractive(drawList);

            RenderShadow(drawList);
            RenderBackground(drawList);
            RenderImage(drawList);
            RenderBorders(drawList);
            RenderText(drawList);

            OnRender?.Invoke(this);

            if (IsInWindowDrawList(drawList)) BeginDraw(drawList);

            var childDrawList = _drawLists.Last();

            foreach (var child in Children) {
                if (child.IsVisible) child.Render(childDrawList, null);
            }

            if (IsInWindowDrawList(drawList)) EndDraw(drawList);

            Draw(drawList);

            EndInteractive();
        } catch (Exception e) {
            Logger.Warning($"Rendering of element '{FullyQualifiedName}' failed: {e.Message} - {e.StackTrace ?? ""}");
        }

        if (IsVisibleSince == 0) {
            IsVisibleSince = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        RenderDebugger();
        _drawLists.Remove(drawList);
    }

    /// <summary>
    /// Invoked after the base rendering has been completed and before any
    /// children are rendered. This allows for custom rendering of ImGUI
    /// frames. Note that this only works when rendering inside a window.
    /// </summary>
    protected virtual void BeginDraw(ImDrawListPtr drawList) { }

    /// <summary>
    /// Invoked after the base rendering has been completed and after all
    /// children have been rendered. This allows for closing any ImGUI frames
    /// that have been opened in the <see cref="BeginDraw"/> method.
    /// </summary>
    protected virtual void EndDraw(ImDrawListPtr drawList) { }

    /// <summary>
    /// Invoked after the base rendering has been completed. This allows for
    /// custom rendering of ImGUI components within this element. Note that
    /// this method is invoked AFTER the children have been rendered, meaning
    /// that custom elements are drawn ON TOP of the children. This is by
    /// design to allow for custom elements to "style" the appearance of the
    /// ImGUI components.
    /// </summary>
    protected virtual void Draw(ImDrawListPtr drawList) { }

    protected void PushDrawList(ImDrawListPtr drawList)
    {
        _drawLists.Add(drawList);
    }

    protected void PopDrawList()
    {
        _drawLists.RemoveAt(_drawLists.Count - 1);
    }
}
