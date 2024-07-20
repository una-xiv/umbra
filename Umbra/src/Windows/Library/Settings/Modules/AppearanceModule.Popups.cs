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
using Umbra.Widgets.System;
using Umbra.Windows.Components;

namespace Umbra.Windows.Settings.Modules;

internal partial class AppearanceModule
{
    private void CreatePopupAppearancePanel()
    {
        CheckboxNode shadowOption = new(
            "EnablePopupShadow",
            WidgetManager.EnableWidgetPopupShadow,
            I18N.Translate("Settings.AppearanceModule.PopupAppearance.EnableShadow.Name"),
            I18N.Translate("Settings.AppearanceModule.PopupAppearance.EnableShadow.Description")
        );

        CheckboxNode floatingOption = new(
            "EnforceFloatingPopups",
            WidgetManager.EnforceFloatingPopups,
            I18N.Translate("Settings.AppearanceModule.PopupAppearance.EnforceFloatingPopups.Name"),
            I18N.Translate("Settings.AppearanceModule.PopupAppearance.EnforceFloatingPopups.Description")
        );

        shadowOption.OnValueChanged += v => ConfigManager.Set("Toolbar.EnableWidgetPopupShadow", v);
        floatingOption.OnValueChanged += v => ConfigManager.Set("Toolbar.EnforceFloatingPopups", v);

        PopupPanel.ChildNodes.Add(shadowOption);
        PopupPanel.ChildNodes.Add(floatingOption);
    }
}
