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

using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.MainMenu;

[Service]
internal sealed partial class WorldMarkerMenuWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.WorldMarkerMenu.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = true;

    public WorldMarkerMenuWidget(ToolbarPopupContext ctx)
    {
        ctx.RegisterDropdownActivator(Element, _dropdownElement);

        ConfigManager.GetVariablesFromCategory("EnabledMarkers").ForEach(cvar =>
            _dropdownElement.Get("Container").AddChild(CreateMarkerButton(cvar)));

        Element.OnMouseEnter += OnMouseEnter;
        Element.OnMouseLeave += OnMouseLeave;
    }

    public void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        Element.IsVisible = true;
    }

    public void OnUpdate()
    {
    }

    private void OnMouseEnter()
    {
        Element.Get<BorderElement>().Color     = 0xFF6A6A6A;
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.Background);
    }

    private void OnMouseLeave()
    {
        Element.Get<BorderElement>().Color     = Theme.Color(ThemeColor.Border);
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.BackgroundDark);
    }
}
