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

using System;
using Umbra.Common;
using Umbra.Drawing;
using Umbra.Game;

namespace Umbra;

[Service]
internal sealed partial class MainMenuWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Height")] private static int Height { get; set; } = 32;

    [ConfigVariable(
        "Toolbar.Widget.MainMenu.Position",
        "Toolbar Settings",
        "Main menu position",
        "Determines the position of the main menu widget on the toolbar."
    )]
    private static WidgetPosition Position { get; set; } = WidgetPosition.Left;

    private readonly ToolbarDropdownContext _dropdownContext;

    public MainMenuWidget(IMainMenuRepository repository, ToolbarDropdownContext dropdownContext)
    {
        _dropdownContext = dropdownContext;
        repository.GetCategories().ForEach(BuildCategoryButton);
    }

    [OnDraw]
    public void OnDraw()
    {
        Element.Anchor = Position switch {
            WidgetPosition.Left  => Anchor.Top | Anchor.Left,
            WidgetPosition.Right => Anchor.Top | Anchor.Right,
            _                    => throw new ArgumentOutOfRangeException()
        };

        Element.Size = new(0, Height);
        Element.Children.ForEach(child => child.Size = new(0, Height - 5));
    }
}
