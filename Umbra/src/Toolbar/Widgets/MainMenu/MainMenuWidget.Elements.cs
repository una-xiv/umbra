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
using Umbra.Common;
using Umbra.Game;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.MainMenu;

internal sealed partial class MainMenuWidget
{
    private static int _buttonId;

    private readonly Dictionary<MainMenuItem, Element> _menuItemElements = [];

    public Element Element { get; } = new(
        id: "ToolbarWidget",
        anchor: Anchor.MiddleLeft,
        sortIndex: int.MinValue,
        flow: Flow.Horizontal,
        gap: 6,
        children: []
    );

    private void BuildCategoryButton(MainMenuCategory category)
    {
        var button = new Element(
            id: $"MainMenu_{_buttonId++}",
            text: category.Name,
            padding: new(0, 4),
            style: new() {
                Font         = Font.Axis,
                TextColor    = Theme.Color(ThemeColor.Text),
                TextAlign    = Anchor.MiddleCenter,
                TextOffset   = new(0, -1),
                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                OutlineWidth = 1,
            }
        );

        button.OnMouseEnter += () => button.Style.TextColor = Theme.Color(ThemeColor.TextLight);
        button.OnMouseLeave += () => button.Style.TextColor = Theme.Color(ThemeColor.Text);

        Element.AddChild(button);

        _popupContext.RegisterDropdownActivator(button, BuildCategoryDropdown(category));
    }

    private DropdownElement BuildCategoryDropdown(MainMenuCategory category)
    {
        var dropdown = new DropdownElement(
            id: category.Name.ToLower(),
            children: [
                new(
                    id: "Items",
                    flow: Flow.Vertical,
                    gap: 6,
                    padding: new(6, 2),
                    children: []
                )
            ]
        );

        category.Items.ForEach(
            item => {
                if (item.Type == MainMenuItemType.Separator) {
                    Element separator = new DropdownSeparatorElement { SortIndex = item.SortIndex };
                    _menuItemElements[item] = separator;
                    dropdown.Get("Items").AddChild(separator);
                    return;
                }

                dropdown.Get("Items").AddChild(CreateMainMenuElement(item));
            }
        );

        category.OnItemAdded += item => {
            if (item.Type == MainMenuItemType.Separator) {
                var separator = new DropdownSeparatorElement { SortIndex = item.SortIndex };
                _menuItemElements[item] = separator;
                dropdown.Get("Items").AddChild(separator);
                return;
            }

            dropdown.Get("Items").AddChild(CreateMainMenuElement(item));
        };

        category.OnItemRemoved += item => {
            if (_menuItemElements.Remove(item, out var element)) {
                dropdown.Get("Items").RemoveChild(element);
            }
        };

        return dropdown;
    }

    private DropdownButtonElement CreateMainMenuElement(MainMenuItem item)
    {
        var button = new DropdownButtonElement(
            id: $"MainMenu_{_buttonId++}",
            label: item.Name,
            icon: item.Icon,
            iconColor: item.IconColor ?? Theme.Color(ThemeColor.TextLight),
            keyBind: string.IsNullOrEmpty(item.Shortkey) ? null : item.Shortkey
        ) {
            Tag        = "MenuItem",
            SortIndex  = item.SortIndex,
            IsDisabled = item.IsDisabled
        };

        var iconStyle = button.Get("Icon").Style;

        iconStyle.ImageGrayscale  = 0.65f;
        iconStyle.ImageContrast   = 1.8f;
        iconStyle.ImageBrightness = 1.2f;
        iconStyle.ImageUVs        = new(0.15f, 0.15f, 0.85f, 0.85f);

        button.OnClick += () => {
            item.Invoke();
            _popupContext.Clear();
        };

        button.OnBeforeCompute += () => {
            button.IsDisabled = item.IsDisabled;
        };

        _menuItemElements[item] = button;

        return button;
    }
}
