/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using FFXIVClientStructs.FFXIV.Client.UI;
using Umbra.Drawing;

namespace Umbra.Windows.Config;

internal sealed class ConfigCategoryButton : Element
{
    public bool IsActive { get; set; }

    public ConfigCategoryButton(string id, string label) : base(
        id: id,
        direction: Direction.Horizontal,
        anchor: Anchor.Top | Anchor.Left,
        size: new(0, 26),
        fit: true,
        nodes: [
            new RectNode(
                color: 0xFF292929,
                borderColor: 0xFF151515,
                rounding: 6,
                borderSize: 1
            ),
            new RectNode(
                id: "ActiveBackground",
                color: 0u,
                rounding: 4,
                margin: new(3, 3, 3, 3)
            ),
            new RectNode(
                id: "Background",
                color: 0,
                borderColor: 0xFF3F3F3F,
                rounding: 5,
                borderSize: 1,
                margin: new(1, 1, 1, 1)
            ),
            new TextNode(
                text: label,
                align: Align.MiddleRight,
                font: Font.Axis,
                color: 0xFFC0C0C0,
                outlineColor: 0xC0000000,
                outlineSize: 1,
                margin: new(0, 8, 0, 8),
                offset: new(0, -1)
            )
        ]
    )
    {
        OnBeforeCompute += () => {
            GetNode<RectNode>("ActiveBackground").Color = IsActive ? 0x605EA1C2 : 0u;
            GetNode<TextNode>().Color                   = IsActive ? 0xFFFFFFFF : 0xFFC0C0C0;
        };

        OnMouseEnter += () => GetNode<RectNode>("Background").Color = 0x3083C5D1;
        OnMouseLeave += () => GetNode<RectNode>("Background").Color = 0u;

        OnClick += () => { UIModule.PlaySound(22); };
    }
}
