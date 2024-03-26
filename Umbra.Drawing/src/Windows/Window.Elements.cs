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

using Dalamud.Game.Text;
using ImGuiNET;
using Umbra.Drawing.Prefabs;

namespace Umbra.Drawing;

public abstract partial class Window
{
    private readonly Element Backdrop = new(
        anchor   : Anchor.Top | Anchor.Left,
        direction: Direction.Horizontal,
        nodes    : [
            new RectNode(
                color       : ImGui.GetColorU32(ImGuiCol.FrameBg),
                rounding    : 7,
                flags       : ImDrawFlags.RoundCornersTop,
                borderColor : 0xFF101010,
                borderSize  : 1
            ),
            new ShadowNode(
                size  : 64,
                inset : new(4, 4, 4, 4),

                //Allow drawing outside the bounding box of the ImGui window.
                requiresFullscreenClipRect: true
            ),
        ]
    );

    private readonly Element _titlebar = new(
        anchor   : Anchor.Top | Anchor.Left,
        direction: Direction.Horizontal,
        size     : new(0, 32),
        nodes    : [
            new RectNode(
                id       : "Highlight",
                size     : new(0, 4),
                color    : ImGui.GetColorU32(ImGuiCol.TabHovered),
                rounding : 8,
                flags    : ImDrawFlags.RoundCornersTop,
                margin   : new(-1, 0, 0, 0)
            ),
            new RectNode(
                color       : 0,
                rounding    : 7,
                borderColor : 0xFF3F3F3F,
                borderSize  : 1,
                margin      : new(0, 1, 0, 0)
            ),
            new RectNode(
                size     : new(0, 28),
                margin   : new(4, 1, 0, 0),
                color    : ImGui.GetColorU32(ImGuiCol.FrameBg)
            ),
            new RectNode(
                margin   : new(4, 1, 16, 0),
                gradients: new(
                    topLeft     : ImGui.GetColorU32(ImGuiCol.FrameBg),
                    topRight    : ImGui.GetColorU32(ImGuiCol.FrameBg),
                    bottomLeft  : ImGui.GetColorU32(ImGuiCol.WindowBg),
                    bottomRight : ImGui.GetColorU32(ImGuiCol.WindowBg)
                )
            ),
            new RectNode(
                margin   : new(16, 1, 0, 0),
                gradients: new(
                    topLeft     : ImGui.GetColorU32(ImGuiCol.WindowBg),
                    topRight    : ImGui.GetColorU32(ImGuiCol.WindowBg),
                    bottomLeft  : ImGui.GetColorU32(ImGuiCol.FrameBg),
                    bottomRight : ImGui.GetColorU32(ImGuiCol.FrameBg)
                )
            ),
            new RectNode(
                margin   : new(29, 1, 0, 0),
                color    : 0xFF121212
            )
        ],
        children : [
            new(
                id        : "Title",
                size      : new(0, 32),
                fit       : true,
                anchor    : Anchor.Top | Anchor.Left,
                direction : Direction.Horizontal,
                nodes     : [
                    new TextNode(
                        text             : "Unnamed Window",
                        font             : Font.Axis,
                        align            : Align.MiddleLeft,
                        color            : 0xFFC0C0C0,
                        outlineColor     : 0xFF000000,
                        outlineSize : 1,
                        margin           : new(0, 8, 0, 8),
                        offset           : new(0, -2)
                    )
                ]
            ),
            new(
                id        : "Buttons",
                size      : new(0, 32),
                fit       : true,
                anchor    : Anchor.Top | Anchor.Right,
                direction : Direction.Horizontal,
                padding   : new(6, 7, 0, 0),
                children  : [
                    new ButtonElement(
                        id      : "Close",
                        text    : SeIconChar.Cross.ToIconString(),
                        isSmall : true,
                        isGhost : true
                    )
                ]
            )
        ]
    );

    private readonly Element _contents = new(
        anchor    : Anchor.Top | Anchor.Left,
        direction : Direction.Vertical,
        nodes     : [
            new RectNode(
                margin      : new(0, 32, 48, 1),
                color       : ImGui.GetColorU32(ImGuiCol.WindowBg),
                borderColor : ImGui.GetColorU32(ImGuiCol.FrameBg),
                borderSize  : 1
            ),
        ]
    );
}
