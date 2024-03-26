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

using System.Collections.Generic;
using ImGuiNET;

namespace Umbra.Drawing.Prefabs;

public sealed class DropdownElement : Element
{
    public readonly Element Content = new(
        id: "Content",
        anchor: Anchor.Top | Anchor.Left,
        direction: Direction.Vertical,
        padding: new(4)
    );

    public DropdownElement(string id = "", Anchor anchor = Anchor.Top | Anchor.Left, List<Element>? children = null, int gap = 0, List<INode>? nodes = null)
    {
        Id          = id;
        Anchor      = anchor;
        Direction   = Direction.Vertical;
        Content.Gap = gap;

        nodes?.ForEach(node => Content.AddNode(node));

        AddChild(Content);
        children?.ForEach(child => Content.AddChild(child));

        AddNode(new ShadowNode(
            id   : "Shadow",
            side : ShadowSide.All,
            size : 64,
            requiresFullscreenClipRect: true
        ));

        AddNode(new RectNode(
            id          : "Background",
            color       : 0xFF212021,
            borderColor : 0xFF3F3F3F,
            borderSize  : 1,
            rounding    : 8,
            flags       : ImDrawFlags.RoundCornersTop
        ));

        AddNode(new RectNode(
            id          : "Gradient",
            color       : 0,
            gradients   : new(bottomLeft: 0xFF2F2F2F, bottomRight: 0xFF2F2F2F),
            margin      : new(0, 1, 0, 1)
        ));

        OnBeforeCompute += () => {
            uint color1 = 0xFF2F2F2F;
            uint color2 = 0xFF212021;

            GetNode<RectNode>("Background").Color = color2;

            if (Anchor.HasFlag(Anchor.Top)) {
                GetNode<ShadowNode>().Side              = ShadowSide.Bottom | ShadowSide.Left | ShadowSide.Right;
                GetNode<ShadowNode>().Inset             = new(32, 8, 8, 8);
                GetNode<RectNode>("Gradient").Gradients = new(topLeft: color1, topRight: color1);
                GetNode<RectNode>("Background").Flags   = ImDrawFlags.RoundCornersBottom;
            } else if (Anchor.HasFlag(Anchor.Bottom)) {
                GetNode<ShadowNode>().Side              = ShadowSide.Top | ShadowSide.Left | ShadowSide.Right;
                GetNode<ShadowNode>().Inset             = new(8, 8, 32, 8);
                GetNode<RectNode>("Gradient").Gradients = new(bottomLeft: color1, bottomRight: color1);
                GetNode<RectNode>("Background").Flags   = ImDrawFlags.RoundCornersTop;
            }
        };
    }
}
