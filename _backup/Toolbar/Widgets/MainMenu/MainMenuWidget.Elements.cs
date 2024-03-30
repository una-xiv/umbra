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

using System.Collections.Generic;
using Dalamud.Game.Text;
using Umbra.Drawing;
using Umbra.Drawing.Prefabs;
using Umbra.Game;

namespace Umbra;

internal sealed partial class MainMenuWidget
{
    private static int _buttonId;

    private readonly Dictionary<MainMenuItem, Element> _menuItemElements = [];

    public Element Element { get; } = new(
        anchor: Anchor.Top | Anchor.Left,
        direction: Direction.Horizontal,
        size: new(0, Height),
        padding: new(top: 2),
        gap: 6,
        children: []
    );

    private void BuildCategoryButton(MainMenuCategory category)
    {
        var button = new ButtonElement(
            text: category.Name,
            isGhost: true
        );

        Element.AddChild(button);

        _dropdownContext.RegisterDropdownActivator(button, BuildCategoryDropdown(category));
    }

    private DropdownElement BuildCategoryDropdown(MainMenuCategory category)
    {
        var dropdown = new DropdownElement(
            id: category.Name.ToLower(),
            gap: 4,
            children: []
        );

        category.Items.ForEach(
            item => {
                if (item.Type == MainMenuItemType.Separator) {
                    Element separator = CreateSeparator(item);
                    _menuItemElements[item] = separator;
                    dropdown.Content.AddChild(separator);
                    return;
                }

                dropdown.Content.AddChild(CreateMainMenuElement(item));
            }
        );

        category.OnItemAdded += item => {
            if (item.Type == MainMenuItemType.Separator) {
                var separator = CreateSeparator(item);
                _menuItemElements[item] = separator;
                dropdown.Content.AddChild(separator);
                return;
            }

            dropdown.Content.AddChild(CreateMainMenuElement(item));
        };

        category.OnItemRemoved += item => {
            if (_menuItemElements.TryGetValue(item, out var element)) {
                _menuItemElements.Remove(item);
                dropdown.Content.RemoveChild(element);
            }
        };

        return dropdown;
    }

    private Element CreateMainMenuElement(MainMenuItem item)
    {
        int buttonHeight = 24;

        var button = new Element(
            id: $"MainMenu_{_buttonId++}",
            direction: Direction.Horizontal,
            sortIndex: item.SortIndex,
            size: new(0, buttonHeight),
            padding: new(0, 4),
            fit: true,
            gap: 4,
            nodes: [
                new RectNode(
                    id: "HoverBackground",
                    color: 0,
                    overflow: true,
                    rounding: 4,
                    margin: new(0, -4, 0, -4)
                )
            ],
            children: [
                new(
                    id: "Icon",
                    size: new(16, buttonHeight),
                    nodes: [
                        new TextNode(
                            text: item.IconChar?.ToIconString() ?? "",
                            color: item.IconColor               ?? 0xFFFFFFFF,
                            font: Font.Axis,
                            align: Align.MiddleCenter
                        )
                    ]
                ),
                new(
                    id: "Label",
                    size: new(0, buttonHeight),
                    nodes: [
                        new TextNode(
                            text: item.Name,
                            color: 0xFFC0C0C0,
                            font: Font.Axis,
                            align: Align.MiddleLeft,
                            outlineColor: 0xFF000000,
                            outlineSize: 1,
                            margin: new(0, 64, 0, 0)
                        )
                    ]
                ),
                new(
                    id: "HotKey",
                    anchor: Anchor.Middle | Anchor.Right,
                    size: new(0, buttonHeight),
                    nodes: [
                        new TextNode(
                            text: item.Shortkey != "" ? $"[{item.Shortkey}]" : "",
                            color: 0xFF808080,
                            font: Font.AxisSmall,
                            align: Align.MiddleRight,
                            outlineColor: 0xFF000000,
                            outlineSize: 1,
                            margin: new(0, 8, 0, 0)
                        )
                    ]
                )
            ]
        );

        button.OnMouseEnter += () => {
            button.GetNode<RectNode>("HoverBackground").Color = 0x405EA1C2;
            button.Get("Label").GetNode<TextNode>().Color     = 0xFFFFFFFF;
        };

        button.OnMouseLeave += () => {
            button.GetNode<RectNode>("HoverBackground").Color = 0;
            button.Get("Label").GetNode<TextNode>().Color     = 0xFFC0C0C0;
        };

        button.OnClick += () => {
            item.Invoke();
            _dropdownContext.Clear();
        };

        button.OnBeforeCompute += () => {
            button.Opacity    = item.IsDisabled ? 0.55f : 1;
            button.IsDisabled = item.IsDisabled;
        };

        _menuItemElements[item] = button;

        return button;
    }

    private static Element CreateSeparator(MainMenuItem item)
    {
        return new(
            sortIndex: item.SortIndex,
            size: new(0, 1),
            fit: true,
            nodes: [
                new RectNode(
                    color: 0xFF3F3F3F,
                    overflow: true
                ),
            ]
        );
    }
}
