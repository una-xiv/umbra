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
using Dalamud.Interface;
using Umbra.Common;
using Umbra.Markers.System;

namespace Umbra.Widgets;

[ToolbarWidget("MarkerControl", "Widget.MarkerControl.Name", "Widget.MarkerControl.Description")]
internal class MarkerControlWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : IconToolbarWidget(info, guid, configValues)
{
    public override MenuPopup Popup { get; } = new() { CloseOnItemClick = false };

    private WorldMarkerFactoryRegistry Registry { get; } = Framework.Service<WorldMarkerFactoryRegistry>();

    protected override void Initialize()
    {
        SetIcon(FontAwesomeIcon.MapSigns);

        foreach (string id in Registry.GetFactoryIds()) {
            var factory = Registry.GetFactory(id);

            Popup.AddButton(
                id,
                factory.Name,
                onClick: () => { factory.SetConfigValue("Enabled", !factory.GetConfigValue<bool>("Enabled")); }
            );
        }
    }

    protected override void OnUpdate()
    {
        SetGhost(!GetConfigValue<bool>("Decorate"));
        SetIconYOffset(GetConfigValue<int>("IconYOffset"));

        foreach (string id in Registry.GetFactoryIds()) {
            var factory = Registry.GetFactory(id);
            var enabled = factory.GetConfigValue<bool>("Enabled");

            Popup.SetButtonIcon(id, enabled ? FontAwesomeIcon.Check : null);
        }
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.MarkerControl.Config.Decorate.Name"),
                I18N.Translate("Widget.MarkerControl.Config.Decorate.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "IconYOffset",
                I18N.Translate("Widget.MarkerControl.Config.IconYOffset.Name"),
                I18N.Translate("Widget.MarkerControl.Config.IconYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        ];
    }
}
