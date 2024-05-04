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

using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Interface;
using Element = Umbra.Interface.Element;

namespace Umbra.Toolbar.Widgets.MainMenu;

[Service]
internal sealed partial class MainMenuWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.MainMenu.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.MainMenu.ShowIcons", "ToolbarSettings", "MainMenuSettings")]
    private static bool ShowIcons { get; set; } = false;

    [ConfigVariable("Toolbar.Widget.MainMenu.ShowMainIcons", "ToolbarSettings", "MainMenuSettings")]
    private static bool ShowMainIcons { get; set; } = false;

    private readonly ToolbarPopupContext _popupContext;
    private readonly IMainMenuRepository _repository;

    private Anchor _currentAnchor = Anchor.MiddleLeft;

    public MainMenuWidget(IMainMenuRepository repository, ToolbarPopupContext popupContext)
    {
        _popupContext = popupContext;
        _repository   = repository;

        repository.GetCategories().ForEach(BuildCategoryButton);
    }

    public void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        Element.IsVisible = true;

        foreach (Element element in _menuItemElements.Values) {
            if (element is DropdownButtonElement { Icon: uint } btn) {
                btn.Get("Icon").Style.Opacity = ShowIcons ? 0.75f : 0;
            }
        }
    }

    public void OnUpdate()
    {
        if (_currentAnchor == Element.Anchor) {
            return;
        }

        Framework.DalamudFramework.Run(
            () => {
                _currentAnchor = Element.Anchor;
                Element.Clear();

                _repository.GetCategories().ForEach(BuildCategoryButton);
                var sortIndex = 0;

                foreach (Element btn in Element.Children) {
                    btn.Anchor    = Element.Anchor;
                    btn.SortIndex = btn.Anchor == Anchor.MiddleLeft ? sortIndex : -sortIndex;
                    sortIndex++;
                }
            }
        );
    }
}
