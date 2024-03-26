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

using Dalamud.Interface;
using Umbra.Common;
using Umbra.Drawing;

namespace Umbra.Windows.Config;

internal sealed class ConfigVariableCheckbox : Element
{
    public ConfigVariableCheckbox(Cvar cvar) : base(
        id: cvar.Id.Replace(".", "_"),
        direction: Direction.Horizontal,
        anchor: Anchor.Top | Anchor.Left,
        fit: true,
        gap: 8,
        children: [
            new(
                id: "Checkbox",
                size: new(20, 20),
                nodes: [
                    new RectNode(
                        color: 0xFF1A1A1A,
                        borderColor: 0xFF3F3F3F,
                        rounding: 3,
                        borderSize: 2
                    ),
                    new RectNode(
                        id: "CheckBackground",
                        color: (bool)(cvar.Value ?? false) ? 0xFF45B9C0 : 0u,
                        rounding: 2,
                        margin: new(3, 3, 3, 3)
                    ),
                    new TextNode(
                        id: "CheckSymbol",
                        text: (bool)(cvar.Value ?? false) ? FontAwesomeIcon.Check.ToIconString() : "",
                        align: Align.MiddleCenter,
                        font: Font.FontAwesomeSmall,
                        color: 0xFF000000,
                        offset: new(0, -1)
                    )
                ]
            ),
            new(
                id: "Label",
                direction: Direction.Vertical,
                anchor: Anchor.Top | Anchor.Left,
                children: [
                    new(
                        id: "Name",
                        size: new(0, 16),
                        nodes: [
                            new TextNode(
                                text: cvar.Name!,
                                align: Align.TopLeft,
                                font: Font.Axis,
                                color: 0xFFC0C0C0,
                                outlineColor: 0xC0000000,
                                outlineSize: 1,
                                offset: new(0, -1)
                            )
                        ]
                    ),
                    new(
                        id: "Description",
                        direction: Direction.Vertical,
                        anchor: Anchor.Top | Anchor.Left,
                        fit: true,
                        padding: new(0, 8, 8, 1),
                        nodes: [
                            new WrappedTextNode(
                                text: cvar.Description ?? "",
                                font: Font.AxisSmall,
                                color: 0xFF808080
                            )
                        ]
                    )
                ]
            )
        ]
    )
    {
        OnBeforeCompute += () => {
            Get("Label.Description").IsVisible = cvar.Description != null;

            var isChecked = ConfigManager.Get<bool>(cvar.Id);
            Get("Checkbox").GetNode<RectNode>("CheckBackground").Color = isChecked ? 0xFF45B9C0 : 0u;

            Get("Checkbox").GetNode<TextNode>("CheckSymbol").Text =
                isChecked ? FontAwesomeIcon.Check.ToIconString() : "";
        };

        Get("Label.Name").OnMouseEnter += () => Get("Label.Name").GetNode<TextNode>().Color = 0xFFFFFFFF;
        Get("Label.Name").OnMouseLeave += () => Get("Label.Name").GetNode<TextNode>().Color = 0xFFC0C0C0;

        OnClick += () => {
            var isChecked = ConfigManager.Get<bool>(cvar.Id);
            ConfigManager.Set(cvar.Id, !isChecked);
        };
    }
}
